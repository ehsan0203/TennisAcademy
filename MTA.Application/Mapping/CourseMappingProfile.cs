using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;
using MTA.Application.DTOs.Course;

namespace MTA.Application.Mapping;

/// <summary>
/// Mapping profile for Course and Lesson entities
/// </summary>
public class CourseMappingProfile : BaseMappingProfile
{
	public CourseMappingProfile()
	{
		// Course mappings
		CreateMap<Course, CourseDto>()
			.ForMember(dest => dest.LevelTitle, opt => opt.MapFrom(src => src.Level.Title))
			.ForMember(dest => dest.StatusValue, opt => opt.MapFrom(src => src.Status.Value))
			.ForMember(dest => dest.LessonCount, opt => opt.MapFrom(src => src.Lessons.Count))
			.ForMember(dest => dest.PurchaseCount, opt => opt.MapFrom(src => src.UserCourseHistory.Count))
			.ForMember(dest => dest.ImageIcon, opt => opt.MapFrom(src => src.IconMediaFile != null ? src.IconMediaFile.Url : null))
			.ForMember(dest => dest.Poster, opt => opt.MapFrom(src => src.PosterMediaFile != null ? src.PosterMediaFile.Url : null))
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
			.ForMember(dest => dest.LevelId, opt => opt.MapFrom(src => src.LevelId))
			.ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId));

		// CreateCourseDto -> Course
		CreateMap<CreateCourseDto, Course>()
			.ForMember(dest => dest.Lessons, opt => opt.Ignore())
			.ForMember(dest => dest.UserCourseHistory, opt => opt.Ignore())
			.ForMember(dest => dest.Level, opt => opt.Ignore())
			.ForMember(dest => dest.Status, opt => opt.Ignore());

		// UpdateCourseDto -> Course (for partial updates)
		CreateMap<UpdateCourseDto, Course>()
			.ForMember(dest => dest.Lessons, opt => opt.Ignore())
			.ForMember(dest => dest.UserCourseHistory, opt => opt.Ignore())
			.ForMember(dest => dest.Level, opt => opt.Ignore())
			.ForMember(dest => dest.Status, opt => opt.Ignore());

		CreateMap<CourseDto, Course>()
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
			.ForMember(dest => dest.LevelId, opt => opt.MapFrom(src => src.LevelId))
			.ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
			.ForMember(dest => dest.PosterMediaFileId, opt => opt.Ignore()) 
			.ForMember(dest => dest.IconMediaFileId, opt => opt.Ignore())   
			.ForMember(dest => dest.Lessons, opt => opt.Ignore())
			.ForMember(dest => dest.UserCourseHistory, opt => opt.Ignore())
			.ForMember(dest => dest.Level, opt => opt.Ignore())
			.ForMember(dest => dest.Status, opt => opt.Ignore())
			.ForMember(dest => dest.PosterMediaFile, opt => opt.Ignore())
			.ForMember(dest => dest.IconMediaFile, opt => opt.Ignore());



		// Lesson mappings
		// Lesson -> LessonDto
		CreateMap<Lesson, LessonDto>()
			.ForMember(dest => dest.MediaFileId, opt => opt.MapFrom(src => src.MediaFileId))
			.ForMember(dest => dest.MediaFileUrl, opt => opt.MapFrom(src => src.MediaFile != null ? src.MediaFile.Url : null))
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.IsFree, opt => opt.MapFrom(src => src.IsFree))
			.ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId));
		// LessonDto -> Lesson
		CreateMap<LessonDto, Lesson>()
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.IsFree, opt => opt.MapFrom(src => src.IsFree))
			.ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
			.ForMember(dest => dest.Course, opt => opt.Ignore())
			.ForMember(dest => dest.MediaFileId, opt => opt.MapFrom(src => src.MediaFileId))
			.ForMember(dest => dest.MediaFile, opt => opt.Ignore());


		// Level -> LevelDto
		CreateMap<Level, LevelDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses.Count))
			.ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.Profiles.Count));

		// LevelDto -> Level
		CreateMap<LevelDto, Level>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.Courses, opt => opt.Ignore())
			.ForMember(dest => dest.Profiles, opt => opt.Ignore());



	}
}
