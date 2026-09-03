using System;
using System.IO;
using System.Threading.Tasks;
using Client = Supabase.Client;

// NOTE: [Table("levels")] belongs on OnlineLevelRecord, not here — see below.
public class OnlineLevelService
{
    private readonly Client supabase;

    public OnlineLevelService(Client supabase)
    {
        this.supabase = supabase;
    }

    public async Task EnsureSignedIn()
    {
        if (supabase.Auth.CurrentSession != null) return;
        await supabase.Auth.SignInAnonymously();
    }

    /// <summary>Uploads the level file at localPath, creates a DB record, returns the join code.</summary>
    public async Task<string> UploadLevel(string localPath, string name, string description)
    {
        await EnsureSignedIn();

        string userId = supabase.Auth.CurrentUser!.Id;
        string code = await GenerateUniqueCode();
        string storagePath = $"{code}.whm";

        byte[] fileBytes = await File.ReadAllBytesAsync(localPath);

        await supabase.Storage
            .From("levels")
            .Upload(fileBytes, storagePath);

        OnlineLevelRecord record = new()
        {
            Name = name,
            Description = description,
            AuthorId = Guid.Parse(userId),
            FilePath = storagePath,
            Code = code,
            Published = true,
        };

        try
        {
            await supabase.From<OnlineLevelRecord>().Insert(record);
        }
        catch (Exception e)
        {
            // Roll back the storage upload if the DB insert fails (e.g. unique constraint race),
            // otherwise we leak an orphaned file with no record pointing to it.
            try { await supabase.Storage.From("levels").Remove(new System.Collections.Generic.List<string> { storagePath }); }
            catch { /* best-effort cleanup, ignore secondary failure */ }

            throw new Exception($"Failed to publish level: {e.Message}", e);
        }

        return code;
    }

    /// <summary>Looks up a level by its code, downloads it, saves it locally, returns the local path.</summary>
    public async Task<string> DownloadLevel(string code)
    {
        OnlineLevelRecord response = await supabase.From<OnlineLevelRecord>()
            .Where(r => r.Code == code)
            .Single();

        if (response == null) throw new($"No level found with code {code}");

        byte[] fileBytes = await supabase.Storage
            .From("levels")
            .Download(response.FilePath, (Supabase.Storage.TransformOptions)null);

        string localPath = Path.Combine(SaveSystem.LevelSavePath, $"{response.Name}.lvl");
        await File.WriteAllBytesAsync(localPath, fileBytes);

        // fire-and-forget, don't block the download on this
        _ = supabase.Rpc("increment_level_downloads", new { p_code = code });

        return localPath;
    }

    private async Task<string> GenerateUniqueCode()
    {
        const int MAX_ATTEMPTS = 10;

        for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
        {
            string code = GenerateRandomCode();
            if (!await DoesCodeExist(code)) return code;
        }

        // Astronomically unlikely with a 6-char/33-symbol alphabet, but fail loudly rather than
        // silently handing out a colliding code.
        throw new Exception($"Could not generate a unique level code after {MAX_ATTEMPTS} attempts");
    }

    private async Task<bool> DoesCodeExist(string code)
    {
        OnlineLevelRecord existing = await supabase.From<OnlineLevelRecord>()
            .Where(r => r.Code == code)
            .Single();

        return existing != null;
    }

    private static string GenerateRandomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no ambiguous chars
        Random rng = new();
        char[] buffer = new char[6];
        for (int i = 0; i < buffer.Length; i++) buffer[i] = chars[rng.Next(chars.Length)];
        return new(buffer);
    }
}