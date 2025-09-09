using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for MediaFile entity
/// Optimized for Swagger and frontend consumption
/// </summary>
public class MediaFileDto : BaseDto
{
    public required string Title { get; set; }

    // Url، FileSize و FileExtension حالا optional شدند
    public string? Url { get; set; }
    public long? FileSize { get; set; }
    public string? FileExtension { get; set; }
    

    public int TypeId { get; set; }
    public string MediaType { get; set; }
    public int? PlacementId { get; set; }
    public string? PlacementName { get; set; }
    public int? LessonId { get; set; }
    public string? LessonTitle { get; set; }
    public int? MessageId { get; set; }
}

public class MediaFileUploadDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string MediaType { get; set; }

    public int? PlacementId { get; set; }
    public string? PlacementName { get; set; }
    public int? LessonId { get; set; }
    public string? LessonTitle { get; set; }
    public int? MessageId { get; set; }
}


public class UpdateMediaFileTypePlacementDto
{
    public int TypeId { get; set; }
    public int? PlacementId { get; set; }
}
    /// <summary>
    /// Validator for creating/updating MediaFileDto
    /// </summary>
    public class MediaFileDtoValidator : AbstractValidator<MediaFileDto>
{
    public MediaFileDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("URL is required")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("URL must be a valid absolute URL");

        RuleFor(x => x.TypeId)
            .GreaterThan(0)
            .WithMessage("TypeId must be greater than 0");

        RuleFor(x => x.PlacementId)
            .GreaterThan(0)
            .When(x => x.PlacementId.HasValue)
            .WithMessage("PlacementId must be greater than 0 if provided");

        RuleFor(x => x.FileSize)
            .GreaterThanOrEqualTo(0)
            .WithMessage("FileSize cannot be negative");

        RuleFor(x => x.FileExtension)
            .MaximumLength(10)
            .When(x => !string.IsNullOrEmpty(x.FileExtension))
            .WithMessage("FileExtension must not exceed 10 characters");
    }
}

/// <summary>
/// Validator for updating Type and Placement of a MediaFile
/// </summary>
public class UpdateMediaFileTypePlacementDtoValidator : AbstractValidator<UpdateMediaFileTypePlacementDto>
{
    public UpdateMediaFileTypePlacementDtoValidator()
    {
        RuleFor(x => x.TypeId)
            .GreaterThan(0)
            .WithMessage("TypeId must be greater than 0");

        // Example: if TypeId = 5, PlacementId is required
        RuleFor(x => x.PlacementId)
            .NotNull()
            .When(x => x.TypeId == 5)
            .WithMessage("PlacementId is required when TypeId is 5");

        RuleFor(x => x.PlacementId)
            .GreaterThan(0)
            .When(x => x.PlacementId.HasValue)
            .WithMessage("PlacementId must be greater than 0 if provided");
    }
}

