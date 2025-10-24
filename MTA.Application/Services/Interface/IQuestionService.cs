using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Question (FAQ) operations
/// </summary>
public interface IQuestionService
{
    /// <summary>
    /// Get all questions with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for question or answer</param>
    /// <returns>Paginated list of questions</returns>
    Task<PaginatedResult<QuestionDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null);
    
    /// <summary>
    /// Get question by ID
    /// </summary>
    /// <param name="id">Question ID</param>
    /// <returns>Question details</returns>
    Task<QuestionDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Search questions by text
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>List of matching questions</returns>
    Task<IEnumerable<QuestionDto>> SearchAsync(string searchTerm);
    
    /// <summary>
    /// Create new question
    /// </summary>
    /// <param name="questionDto">Question data</param>
    /// <returns>Created question</returns>
    Task<QuestionDto> CreateAsync(QuestionDto questionDto);
    
    /// <summary>
    /// Update existing question
    /// </summary>
    /// <param name="id">Question ID</param>
    /// <param name="questionDto">Updated question data</param>
    /// <returns>Updated question</returns>
    Task<QuestionDto> UpdateAsync(int id, QuestionDto questionDto);
    
    /// <summary>
    /// Delete question
    /// </summary>
    /// <param name="id">Question ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Get question statistics
    /// </summary>
    /// <returns>Question statistics</returns>
    Task<QuestionStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get questions by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of questions</returns>
    Task<IEnumerable<QuestionDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Bulk create questions
    /// </summary>
    /// <param name="questionDtos">List of question data</param>
    /// <returns>List of created questions</returns>
    Task<IEnumerable<QuestionDto>> BulkCreateAsync(IEnumerable<QuestionDto> questionDtos);
    
    /// <summary>
    /// Get random questions for FAQ page
    /// </summary>
    /// <param name="count">Number of questions to return</param>
    /// <returns>List of random questions</returns>
    Task<IEnumerable<QuestionDto>> GetRandomQuestionsAsync(int count = 10);
}

/// <summary>
/// Question statistics DTO
/// </summary>
public class QuestionStatisticsDto
{
    public int TotalQuestions { get; set; }
    public int QuestionsThisMonth { get; set; }
    public int QuestionsLastMonth { get; set; }
    public double AverageQuestionLength { get; set; } // in characters
    public double AverageAnswerLength { get; set; } // in characters
    public int QuestionsByLengthShort { get; set; } // 0-100 characters
    public int QuestionsByLengthMedium { get; set; } // 101-500 characters
    public int QuestionsByLengthLong { get; set; } // 500+ characters
}
