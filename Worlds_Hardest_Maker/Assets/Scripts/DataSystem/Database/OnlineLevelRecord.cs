using System;
using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("published_levels")]
public class OnlineLevelRecord : BaseModel
{
    [PrimaryKey("id", shouldInsert: true)]
    public Guid Id { get; set; }

    [Column("code")]
    public string Code { get; set; }

    [Column("storage_path")]
    public string StoragePath { get; set; }

    [Column("created_at", nullValueHandling: JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; set; }
}