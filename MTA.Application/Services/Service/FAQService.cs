using AutoMapper;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for FAQ operations
/// </summary>
public class FAQService : IFAQService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FAQService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<FAQCategoryDto>> GetCategoriesAsync(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<FAQCategory>().GetQueryable()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Title.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(ct);
        var categories = await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var categoryDtos = _mapper.Map<IEnumerable<FAQCategoryDto>>(categories);

        return new PaginatedResult<FAQCategoryDto>
        {
            Data = categoryDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<FAQCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await _unitOfWork.Repository<FAQCategory>().GetByIdAsync(id, ct);
        return _mapper.Map<FAQCategoryDto>(category);
    }


    public async Task<FAQCategoryDto?> GetCategoryByTitleAsync(string title, CancellationToken ct = default)
    {
        var category = await _unitOfWork.Repository<FAQCategory>().GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Title == title, ct);
        return _mapper.Map<FAQCategoryDto>(category);
    }

    public async Task<FAQCategoryDto> CreateCategoryAsync(CreateFAQCategoryDto categoryDto, CancellationToken ct = default)
    {
        var category = _mapper.Map<FAQCategory>(categoryDto);

        await _unitOfWork.Repository<FAQCategory>().AddAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<FAQCategoryDto>(category);
    }

    public async Task<FAQCategoryDto> UpdateCategoryAsync(int id, FAQCategoryDto categoryDto, CancellationToken ct = default)
    {
        var existingCategory = await _unitOfWork.Repository<FAQCategory>().GetByIdAsync(id, ct);
        if (existingCategory == null)
            throw new ArgumentException($"FAQ category with ID {id} not found");

        _mapper.Map(categoryDto, existingCategory);

        await _unitOfWork.Repository<FAQCategory>().UpdateAsync(existingCategory, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<FAQCategoryDto>(existingCategory);
    }

    public async Task<bool> DeleteCategoryAsync(int id, CancellationToken ct = default)
    {
        var category = await _unitOfWork.Repository<FAQCategory>().GetByIdAsync(id, ct);
        if (category == null)
            return false;

        // Check if category has questions
        var hasQuestions = await _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .AnyAsync(q => q.CategoryId == id, ct);

        if (hasQuestions)
            throw new InvalidOperationException("Cannot delete category with existing questions");

        await _unitOfWork.Repository<FAQCategory>().DeleteAsync(category.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    public async Task<PaginatedResult<QuestionDto>> GetQuestionsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10, bool? isActive = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .AsNoTracking()
            .Include(q => q.Category)
            .Where(q => q.CategoryId == categoryId);

        if (isActive.HasValue)
        {
            query = query.Where(q => q.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(ct);
        var questions = await query
            .OrderBy(q => q.Category.SortOrder)
            .ThenBy(q => q.QuestionText)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var questionDtos = questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            QuestionText = q.QuestionText,
            AnswerText = q.AnswerText,
            IsActive = q.IsActive,
            CategoryId = q.CategoryId,
            CategoryTitle = q.Category?.Title,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt
        });

        return new PaginatedResult<QuestionDto>
        {
            Data = questionDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<QuestionDto?> GetQuestionByIdAsync(int id, CancellationToken ct = default)
    {
        var question = await _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .AsNoTracking()
            .Include(q => q.Category)
            .FirstOrDefaultAsync(q => q.Id == id, ct);

        if (question == null)
            return null;

        return new QuestionDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            AnswerText = question.AnswerText,
            IsActive = question.IsActive,
            CategoryId = question.CategoryId,
            CategoryTitle = question.Category?.Title,
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt
        };
    }

    public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto questionDto, CancellationToken ct = default)
    {
        var question = new QuestionFAQ
        {
            QuestionText = questionDto.QuestionText,
            AnswerText = questionDto.AnswerText,
            IsActive = questionDto.IsActive,
            CategoryId = questionDto.CategoryId
        };

        await _unitOfWork.Repository<QuestionFAQ>().AddAsync(question, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var result = await GetQuestionByIdAsync(question.Id, ct);
        if (result == null)
        {
            throw new InvalidOperationException("Failed to retrieve created question.");
        }

        return result;
    }

    public async Task<QuestionDto> UpdateQuestionAsync(int id, UpdateQuestionDto questionDto, CancellationToken ct = default)
    {
        var existingQuestion = await _unitOfWork.Repository<QuestionFAQ>().GetByIdAsync(id, ct);
        if (existingQuestion == null)
            throw new ArgumentException($"Question with ID {id} not found");

        existingQuestion.QuestionText = questionDto.QuestionText;
        existingQuestion.AnswerText = questionDto.AnswerText;
        existingQuestion.IsActive = questionDto.IsActive;

        await _unitOfWork.Repository<QuestionFAQ>().UpdateAsync(existingQuestion, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetQuestionByIdAsync(id, ct) ?? _mapper.Map<QuestionDto>(existingQuestion);
    }

    public async Task<bool> DeleteQuestionAsync(int id, CancellationToken ct = default)
    {
        var question = await _unitOfWork.Repository<QuestionFAQ>().GetByIdAsync(id, ct);
        if (question == null)
            return false;

        await _unitOfWork.Repository<QuestionFAQ>().DeleteAsync(question.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    public async Task<PaginatedResult<QuestionDto>> SearchQuestionsAsync(string searchTerm, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .AsNoTracking()
            .Include(q => q.Category)
            .Where(q => q.QuestionText.Contains(searchTerm) || q.AnswerText.Contains(searchTerm));

        var totalCount = await query.CountAsync(ct);
        var questions = await query
            .OrderBy(q => q.Category.SortOrder)
            .ThenBy(q => q.QuestionText)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var questionDtos = questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            QuestionText = q.QuestionText,
            AnswerText = q.AnswerText,
            IsActive = q.IsActive,
            CategoryId = q.CategoryId,
            CategoryTitle = q.Category?.Title,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt
        });

        return new PaginatedResult<QuestionDto>
        {
            Data = questionDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<IEnumerable<QuestionDto>> GetFrequentlyAskedQuestionsAsync(int limit = 10, CancellationToken ct = default)
    {
        var questions = await _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .AsNoTracking()
            .Include(q => q.Category)
            .Where(q => q.IsActive)
            .OrderBy(q => q.Category.SortOrder)
            .ThenBy(q => q.QuestionText)
            .Take(limit)
            .ToListAsync(ct);

        return questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            QuestionText = q.QuestionText,
            AnswerText = q.AnswerText,
            IsActive = q.IsActive,
            CategoryId = q.CategoryId,
            CategoryTitle = q.Category?.Title,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt
        });
    }

    public async Task<FAQStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var categories = await _unitOfWork.Repository<FAQCategory>().GetQueryable()
            .AsNoTracking()
            .ToListAsync(ct);
        var questions = await _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .AsNoTracking()
            .ToListAsync(ct);

        var questionsPerCategory = questions
            .GroupBy(q => q.CategoryId)
            .ToDictionary(g => g.Key, g => g.Count());

        return new FAQStatisticsDto
        {
            TotalCategories = categories.Count,
            ActiveCategories = categories.Count(c => c.IsActive),
            InactiveCategories = categories.Count(c => !c.IsActive),
            TotalQuestions = questions.Count,
            ActiveQuestions = questions.Count(q => q.IsActive),
            InactiveQuestions = questions.Count(q => !q.IsActive),
            //AverageQuestionsPerCategory = categories.Count > 0 ? (double)questions.Count / categories.Count : 0,
            //CategoriesWithQuestions = questionsPerCategory.Count,
            //CategoriesWithoutQuestions = categories.Count - questionsPerCategory.Count,
            QuestionsPerCategory = questionsPerCategory
        };
    }

    public async Task<IEnumerable<FAQCategoryDto>> GetCategoriesWithQuestionCountAsync(CancellationToken ct = default)
    {
        var categories = await _unitOfWork.Repository<FAQCategory>().GetQueryable()
            .AsNoTracking()
            .Include(c => c.Questions)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Title)
            .ToListAsync(ct);

        return categories.Select(c => new FAQCategoryDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            SortOrder = c.SortOrder,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }

    public async Task<FAQCategoryDto> ToggleCategoryStatusAsync(int id, CancellationToken ct = default)
    {
        var category = await _unitOfWork.Repository<FAQCategory>().GetByIdAsync(id, ct);
        if (category == null)
            throw new ArgumentException($"FAQ category with ID {id} not found");

        category.IsActive = !category.IsActive;

        await _unitOfWork.Repository<FAQCategory>().UpdateAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<FAQCategoryDto>(category);
    }

    public async Task<QuestionDto> ToggleQuestionStatusAsync(int id, CancellationToken ct = default)
    {
        var question = await _unitOfWork.Repository<QuestionFAQ>().GetByIdAsync(id, ct);
        if (question == null)
            throw new ArgumentException($"Question with ID {id} not found");

        question.IsActive = !question.IsActive;

        await _unitOfWork.Repository<QuestionFAQ>().UpdateAsync(question, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var result = await GetQuestionByIdAsync(id, ct);
        if (result == null)
            throw new InvalidOperationException($"Question with ID {id} not found after update");

        return result;
    }

}
