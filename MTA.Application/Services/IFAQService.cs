using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for FAQ operations
/// </summary>
public interface IFAQService
{
    /// <summary>
    /// Get all FAQ categories with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of FAQ categories</returns>
    Task<PaginatedResult<FAQCategoryDto>> GetCategoriesAsync(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null);
    
    /// <summary>
    /// Get FAQ category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>FAQ category details</returns>
    Task<FAQCategoryDto?> GetCategoryByIdAsync(int id);
    
    /// <summary>
    /// Get FAQ category by title
    /// </summary>
    /// <param name="title">Category title</param>
    /// <returns>FAQ category details</returns>
    Task<FAQCategoryDto?> GetCategoryByTitleAsync(string title);
    
    /// <summary>
    /// Create new FAQ category
    /// </summary>
    /// <param name="categoryDto">Category data</param>
    /// <returns>Created category</returns>
    Task<FAQCategoryDto> CreateCategoryAsync(FAQCategoryDto categoryDto);
    
    /// <summary>
    /// Update existing FAQ category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="categoryDto">Updated category data</param>
    /// <returns>Updated category</returns>
    Task<FAQCategoryDto> UpdateCategoryAsync(int id, FAQCategoryDto categoryDto);
    
    /// <summary>
    /// Delete FAQ category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteCategoryAsync(int id);
    
    /// <summary>
    /// Get all questions in a category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of questions</returns>
    Task<PaginatedResult<QuestionDto>> GetQuestionsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10, bool? isActive = null);
    
    /// <summary>
    /// Get question by ID
    /// </summary>
    /// <param name="id">Question ID</param>
    /// <returns>Question details</returns>
    Task<QuestionDto?> GetQuestionByIdAsync(int id);
    
    /// <summary>
    /// Create new question
    /// </summary>
    /// <param name="questionDto">Question data</param>
    /// <returns>Created question</returns>
    Task<QuestionDto> CreateQuestionAsync(QuestionDto questionDto);
    
    /// <summary>
    /// Update existing question
    /// </summary>
    /// <param name="id">Question ID</param>
    /// <param name="questionDto">Updated question data</param>
    /// <returns>Updated question</returns>
    Task<QuestionDto> UpdateQuestionAsync(int id, QuestionDto questionDto);
    
    /// <summary>
    /// Delete question
    /// </summary>
    /// <param name="id">Question ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteQuestionAsync(int id);
    
    /// <summary>
    /// Search questions across all categories
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated search results</returns>
    Task<PaginatedResult<QuestionDto>> SearchQuestionsAsync(string searchTerm, int page = 1, int pageSize = 10);
    
    /// <summary>
    /// Get frequently asked questions
    /// </summary>
    /// <param name="limit">Maximum number of questions to return</param>
    /// <returns>List of frequently asked questions</returns>
    Task<IEnumerable<QuestionDto>> GetFrequentlyAskedQuestionsAsync(int limit = 10);
    
    /// <summary>
    /// Get FAQ statistics
    /// </summary>
    /// <returns>FAQ statistics</returns>
    Task<FAQStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get categories with question count
    /// </summary>
    /// <returns>List of categories with question counts</returns>
    Task<IEnumerable<FAQCategoryDto>> GetCategoriesWithQuestionCountAsync();
    
    /// <summary>
    /// Toggle category active status
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Updated category</returns>
    Task<FAQCategoryDto> ToggleCategoryStatusAsync(int id);
    
    /// <summary>
    /// Toggle question active status
    /// </summary>
    /// <param name="id">Question ID</param>
    /// <returns>Updated question</returns>
    Task<QuestionDto> ToggleQuestionStatusAsync(int id);
}

/// <summary>
/// FAQ statistics DTO
/// </summary>
public class FAQStatisticsDto
{
    public int TotalCategories { get; set; }
    public int ActiveCategories { get; set; }
    public int InactiveCategories { get; set; }
    public int TotalQuestions { get; set; }
    public int ActiveQuestions { get; set; }
    public int InactiveQuestions { get; set; }
    //public double AverageQuestionsPerCategory { get; set; }
    //public int CategoriesWithQuestions { get; set; }
    //public int CategoriesWithoutQuestions { get; set; }
    public Dictionary<int, int> QuestionsPerCategory { get; set; } = new();
    //public int QuestionsThisMonth { get; set; }
    //public int QuestionsLastMonth { get; set; }
}

