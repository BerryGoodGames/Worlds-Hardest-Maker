using System;
using System.IO;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Client = Supabase.Client;

[Table("levels")]
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
        string code = GenerateUniqueCode();
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

        await supabase.From<OnlineLevelRecord>().Insert(record);

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
    
    private static string GenerateUniqueCode()
    {
        string code;
        do
        {
            code = GenerateRandomCode();
        } while (DoesCodeExist(code));
        return code;
    }
    
    private static bool DoesCodeExist(string code)
    {
        // This is a placeholder for the actual implementation that checks if the code exists in the database.
        // You would typically call your database service here to check for the existence of the code.
        throw new NotImplementedException("Code existence check not implemented.");
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