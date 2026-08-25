using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IFAQService
{
    Task<PaginatedResult<FAQCategoryDto>> GetCategoriesAsync(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, CancellationToken ct = default);
    Task<FAQCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken ct = default);
    Task<FAQCategoryDto?> GetCategoryByTitleAsync(string title, CancellationToken ct = default);
    Task<FAQCategoryDto> CreateCategoryAsync(CreateFAQCategoryDto categoryDto, CancellationToken ct = default);
    Task<FAQCategoryDto> UpdateCategoryAsync(int id, FAQCategoryDto categoryDto, CancellationToken ct = default);
    Task<bool> DeleteCategoryAsync(int id, CancellationToken ct = default);
    Task<FAQCategoryDto> ToggleCategoryStatusAsync(int id, CancellationToken ct = default);

    Task<PaginatedResult<QuestionDto>> GetQuestionsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10, bool? isActive = null, CancellationToken ct = default);
    Task<QuestionDto?> GetQuestionByIdAsync(int id, CancellationToken ct = default);
    Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto questionDto, CancellationToken ct = default);
    Task<QuestionDto> UpdateQuestionAsync(int id, UpdateQuestionDto questionDto, CancellationToken ct = default);
    Task<bool> DeleteQuestionAsync(int id, CancellationToken ct = default);
    Task<QuestionDto> ToggleQuestionStatusAsync(int id, CancellationToken ct = default);
    Task<PaginatedResult<QuestionDto>> SearchQuestionsAsync(string searchTerm, int page = 1, int pageSize = 10, CancellationToken ct = default);
    Task<IEnumerable<QuestionDto>> GetFrequentlyAskedQuestionsAsync(int limit = 10, CancellationToken ct = default);
    Task<FAQStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<IEnumerable<FAQCategoryDto>> GetCategoriesWithQuestionCountAsync(CancellationToken ct = default);
}

public class FAQStatisticsDto
{
    public int TotalCategories { get; set; }
    public int ActiveCategories { get; set; }
    public int InactiveCategories { get; set; }
    public int TotalQuestions { get; set; }
    public int ActiveQuestions { get; set; }
    public int InactiveQuestions { get; set; }
    public Dictionary<int, int> QuestionsPerCategory { get; set; } = new();
}
