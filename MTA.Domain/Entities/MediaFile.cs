using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents a file in the system
/// </summary>
public class MediaFile : BaseEntity
{
    public required string Title { get; set; }
    public required string Url { get; set; }
    public long FileSize { get; set; }
    public string FileExtension { get; set; }

    public int TypeId { get; set; }
    [ForeignKey("TypeId")]
    public virtual Lookup Type{ get; set; }

    public int? PlacementId { get; set; }
    [ForeignKey("PlacementId")]
    public virtual Lookup Placement { get; set; }
}

