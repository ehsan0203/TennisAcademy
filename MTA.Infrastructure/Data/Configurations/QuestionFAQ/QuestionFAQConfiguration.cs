using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class QuestionFAQConfiguration : IEntityTypeConfiguration<QuestionFAQ>
{
    public void Configure(EntityTypeBuilder<QuestionFAQ> builder)
    {
        builder.HasKey(question => question.Id);

        builder.Property(question => question.QuestionText)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(question => question.AnswerText)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(question => question.IsActive)
            .IsRequired();

        builder.HasOne(question => question.Category)
            .WithMany(category => category.Questions)
            .HasForeignKey(question => question.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
