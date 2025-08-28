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

    public async Task<PaginatedResult<FAQCategoryDto>> GetCategoriesAsync(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null)
    {
        var query = _unitOfWork.Repository<FAQCategory>().GetQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Title.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();
        var categories = await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var categoryDtos = _mapper.Map<IEnumerable<FAQCategoryDto>>(categories);
        
        return new PaginatedResult<FAQCategoryDto>
        {
            Data = categoryDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<FAQCategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Repository<FAQCategory>().GetByIdAsync(id);
        return _mapper.Map<FAQCategoryDto>(category);
    }


    public async Task<FAQCategoryDto?> GetCategoryByTitleAsync(string title)
    {
        var category = await _unitOfWork.Repository<FAQCategory>().GetQueryable()
            .FirstOrDefaultAsync(c => c.Title == title);
        return _mapper.Map<FAQCategoryDto>(category);
    }

    public async Task<FAQCategoryDto> CreateCategoryAsync(FAQCategoryDto categoryDto)
    {
        var category = _mapper.Map<FAQCategory>(categoryDto);
        category.CreatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Repository<FAQCategory>().AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<FAQCategoryDto>(category);
    }

    public async Task<FAQCategoryDto> UpdateCategoryAsync(int id, FAQCategoryDto categoryDto)
    {
        var existingCategory = await _unitOfWork.Repository<FAQCategory>().GetByIdAsync(id);
        if (existingCategory == null)
            throw new ArgumentException($"FAQ category with ID {id} not found");

        _mapper.Map(categoryDto, existingCategory);
        existingCategory.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Repository<FAQCategory>().UpdateAsync(existingCategory);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<FAQCategoryDto>(existingCategory);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Repository<FAQCategory>().GetByIdAsync(id);
        if (category == null)
            return false;

        // Check if category has questions
        var hasQuestions = await _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .AnyAsync(q => q.CategoryId == id);
        
        if (hasQuestions)
            throw new InvalidOperationException("Cannot delete category with existing questions");

        _unitOfWork.Repository<FAQCategory>().DeleteAsync(category.Id);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<PaginatedResult<QuestionDto>> GetQuestionsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10, bool? isActive = null)
    {
        var query = _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .Include(q => q.Category)
            .Where(q => q.CategoryId == categoryId);

        if (isActive.HasValue)
        {
            query = query.Where(q => q.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();
        var questions = await query
            .OrderBy(q => q.Category.SortOrder)
            .ThenBy(q => q.QuestionText)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

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

    public async Task<QuestionDto?> GetQuestionByIdAsync(int id)
    {
        var question = await _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .Include(q => q.Category)
            .FirstOrDefaultAsync(q => q.Id == id);

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

    public async Task<QuestionDto> CreateQuestionAsync(QuestionDto questionDto)
    {
        var question = _mapper.Map<QuestionFAQ>(questionDto);
        question.CreatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Repository<QuestionFAQ>().AddAsync(question);
        await _unitOfWork.SaveChangesAsync();
        
        return await GetQuestionByIdAsync(question.Id) ?? questionDto;
    }

    public async Task<QuestionDto> UpdateQuestionAsync(int id, QuestionDto questionDto)
    {
        var existingQuestion = await _unitOfWork.Repository<QuestionFAQ>().GetByIdAsync(id);
        if (existingQuestion == null)
            throw new ArgumentException($"Question with ID {id} not found");

        _mapper.Map(questionDto, existingQuestion);
        existingQuestion.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Repository<QuestionFAQ>().UpdateAsync(existingQuestion);
        await _unitOfWork.SaveChangesAsync();
        
        return await GetQuestionByIdAsync(id) ?? questionDto;
    }

    public async Task<bool> DeleteQuestionAsync(int id)
    {
        var question = await _unitOfWork.Repository<QuestionFAQ>().GetByIdAsync(id);
        if (question == null)
            return false;

        _unitOfWork.Repository<QuestionFAQ>().DeleteAsync(question.Id);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<PaginatedResult<QuestionDto>> SearchQuestionsAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        var query = _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .Include(q => q.Category)
            .Where(q => q.QuestionText.Contains(searchTerm) || q.AnswerText.Contains(searchTerm));

        var totalCount = await query.CountAsync();
        var questions = await query
            .OrderBy(q => q.Category.SortOrder)
            .ThenBy(q => q.QuestionText)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

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

    public async Task<IEnumerable<QuestionDto>> GetFrequentlyAskedQuestionsAsync(int limit = 10)
    {
        var questions = await _unitOfWork.Repository<QuestionFAQ>().GetQueryable()
            .Include(q => q.Category)
            .Where(q => q.IsActive)
            .OrderBy(q => q.Category.SortOrder)
            .ThenBy(q => q.QuestionText)
            .Take(limit)
            .ToListAsync();

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

    public async Task<FAQStatisticsDto> GetStatisticsAsync()
    {
        var categories = await _unitOfWork.Repository<FAQCategory>().GetQueryable().ToListAsync();
        var questions = await _unitOfWork.Repository<QuestionFAQ>().GetQueryable().ToListAsync();

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

    public async Task<IEnumerable<FAQCategoryDto>> GetCategoriesWithQuestionCountAsync()
    {
        var categories = await _unitOfWork.Repository<FAQCategory>().GetQueryable()
            .Include(c => c.Questions)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Title)
            .ToListAsync();

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

    public async Task<FAQCategoryDto> ToggleCategoryStatusAsync(int id)
    {
        var category = await _unitOfWork.Repository<FAQCategory>().GetByIdAsync(id);
        if (category == null)
            throw new ArgumentException($"FAQ category with ID {id} not found");

        category.IsActive = !category.IsActive;
        category.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Repository<FAQCategory>().UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<FAQCategoryDto>(category);
    }

    public async Task<QuestionDto> ToggleQuestionStatusAsync(int id)
    {
        var question = await _unitOfWork.Repository<QuestionFAQ>().GetByIdAsync(id);
        if (question == null)
            throw new ArgumentException($"Question with ID {id} not found");

        question.IsActive = !question.IsActive;
        question.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<QuestionFAQ>().UpdateAsync(question);
        await _unitOfWork.SaveChangesAsync();

        var result = await GetQuestionByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Question with ID {id} not found after update");

        return result;
    }

}

