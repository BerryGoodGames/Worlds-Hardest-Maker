using System;
using Supabase.Postgrest.Models;

public class OnlineLevelRecord : BaseModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Guid AuthorId { get; set; }

    public string FilePath { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool Published { get; set; }

    public int Downloads { get; set; }
}