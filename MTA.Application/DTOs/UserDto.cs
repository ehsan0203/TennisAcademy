namespace MTA.Application.DTOs.User
{

    // ======================
    // DTO اصلی پروفایل
    // ======================
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Experience { get; set; }
        public int SkillLevelId { get; set; } = 1;
        public string? SkillLevelValue { get; set; } = "Beginner";
        public int AccountId { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool HealthCondition { get; set; }
        public string? HealthDescription { get; set; }
    }

    // ======================
    // DTO ایجاد پروفایل
    // ======================
    public class CreateUserProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Experience { get; set; }
        public int SkillLevelId { get; set; }
        public int AccountId { get; set; }
        public bool HealthCondition { get; set; }
        public string? HealthDescription { get; set; }
    }

    // ======================
    // DTO بروزرسانی پروفایل
    // ======================
    public class UpdateUserProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Experience { get; set; }
        public int? SkillLevelId { get; set; }
        public bool HealthCondition { get; set; }
        public string? HealthDescription { get; set; }
    }

    // ======================
    // Base DTO اکانت (برای جلوگیری از تکرار)
    // ======================
    public class AccountBaseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Image { get; set; }
        public int RoleId { get; set; }
        public string? RoleTitle { get; set; }
        public int StatusId { get; set; }
        public string? StatusValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // ======================
    // DTO اصلی اکانت با پروفایل
    // ======================
    public class AccountDto : AccountBaseDto
    {
        public UserProfileDto? UserProfile { get; set; }
    }

    // ======================
    // DTO ایجاد اکانت
    // ======================
    public class CreateAccountDto
    {
        public string Email { get; set; } 
        public string Password { get; set; } 
        public bool IsActive { get; set; } = true;
        public string? Image { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
    }

    // ======================
    // DTO بروزرسانی اکانت
    // ======================
    public class UpdateAccountDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; } // فقط ورودی
        public bool? IsActive { get; set; }
        public string? Image { get; set; }
        public int? RoleId { get; set; }
        public int? StatusId { get; set; }
    }

    // ======================
    // DTO جستجو و فیلتر
    // ======================
    public class UserSearchDto
    {
        public string? SearchTerm { get; set; } = null;
        public int? RoleId { get; set; } = null;
        public int? SkillLevelId { get; set; } = null;
        public bool? IsActive { get; set; } = null;
        public int? Experience { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ======================
    // نتایج Pagination
    // ======================
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }

    // ======================
    // DTO آمار پروفایل‌ها
    // ======================
    public class UserProfileStatisticsDto
    {
        public int TotalProfiles { get; set; }
        public int ProfilesWithExperience { get; set; }
        public double AverageExperience { get; set; }
    }
}
