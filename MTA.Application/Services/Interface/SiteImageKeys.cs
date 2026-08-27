namespace MTA.Application.Services.Interface;

public static class SiteImageKeys
{
    public const string Logo = "Logo";
    public const string HomeHeroBanner = "HomeHeroBanner";
    public const string TrainingPlansBanner = "TrainingPlansBanner";
    public const string HomeCoachingCardImage = "HomeCoachingCardImage";
    public const string HomeCoursesCardImage = "HomeCoursesCardImage";
    public const string CoursesIntroVideo = "CoursesIntroVideo";
    public const string TrainingPlansIntroVideo = "TrainingPlansIntroVideo";
    public const string CoursesBanner = "CoursesBanner";

    public static readonly string[] All =
    {
        Logo, HomeHeroBanner, TrainingPlansBanner, HomeCoachingCardImage, HomeCoursesCardImage,
        CoursesIntroVideo, TrainingPlansIntroVideo, CoursesBanner
    };
}

public static class SiteTextKeys
{
    public const string HomeHeroHeadline = "HomeHeroHeadline";
    public const string HomeHeroSubtext = "HomeHeroSubtext";
    public const string HomeCoachingCardTitle = "HomeCoachingCardTitle";
    public const string HomeCoursesCardTitle = "HomeCoursesCardTitle";
    public const string CoursesBannerTitle = "CoursesBannerTitle";
    public const string CoursesBannerSubtitle = "CoursesBannerSubtitle";
    public const string TrainingPlansBannerTitle = "TrainingPlansBannerTitle";
    public const string TrainingPlansBannerSubtitle = "TrainingPlansBannerSubtitle";

    public static readonly Dictionary<string, string> DefaultsByKey = new()
    {
        [HomeHeroHeadline] = "who we are",
        [HomeHeroSubtext] =
            "40+ years of coaching\n" +
            "Grand Slam officiating\n" +
            "A tennis family trusted by hundreds\n" +
            "We’ve taken everything we’ve learned — from elite courts to global tournaments — and built a system that delivers real results, from wherever you are.No travel. No club fees. Just professional coaching, tailored to your game.",
        [HomeCoachingCardTitle] = "Online Tennis Couching",
        [HomeCoursesCardTitle] = "Online Tennis Courses",
        [CoursesBannerTitle] = "Master Every Shot - One Course at a Time",
        [CoursesBannerSubtitle] = "Explore structured training packages designed to improve your tennis game with expert guidance.",
        [TrainingPlansBannerTitle] = "Master Every Shot - One Course at a Time",
        [TrainingPlansBannerSubtitle] = "Explore structured training packages designed to improve your tennis game with expert guidance.",
    };

    public static readonly string[] All = DefaultsByKey.Keys.ToArray();
}
