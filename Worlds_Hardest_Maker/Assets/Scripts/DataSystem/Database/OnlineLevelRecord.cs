using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("levels")]
public class OnlineLevelRecord : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("code")]
    public string Code { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [Column("file_path")]
    public string FilePath { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("published")]
    public bool Published { get; set; }

    [Column("downloads")]
    public int Downloads { get; set; }
}