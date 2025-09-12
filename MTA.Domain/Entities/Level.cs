namespace MTA.Domain.Entities;

public class Level : BaseEntity
{

    public required string Title { get; set; }
    
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<UserProfile> Profiles { get; set; } = new List<UserProfile>();
}
