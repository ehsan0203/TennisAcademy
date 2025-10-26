namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Question (FAQ) entity
/// </summary>
public class QuestionDto : BaseDto
{
    /// <summary>
    /// The question text
    /// </summary>
    public required string QuestionText { get; set; }
    
    /// <summary>
    /// The answer text
    /// </summary>
    public required string AnswerText { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryTitle { get; set; }
}

public class CreateQuestionDto
{
    /// <summary>
    /// The question text
    /// </summary>
    public required string QuestionText { get; set; }

    /// <summary>
    /// The answer text
    /// </summary>
    public required string AnswerText { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryTitle { get; set; }
}

public class UpdateQuestionDto
{
    /// <summary>
    /// The question text
    /// </summary>
    public required string QuestionText { get; set; }

    /// <summary>
    /// The answer text
    /// </summary>
    public required string AnswerText { get; set; }

    /// <summary>
    /// Indicates whether the question is active
    /// </summary>
    public bool IsActive { get; set; }
}

