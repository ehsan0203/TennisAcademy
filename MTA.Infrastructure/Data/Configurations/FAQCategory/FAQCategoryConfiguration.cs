using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class FAQCategoryConfiguration : IEntityTypeConfiguration<FAQCategory>
{
    public void Configure(EntityTypeBuilder<FAQCategory> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(category => category.Description)
            .HasMaxLength(1000);

        builder.Property(category => category.SortOrder)
            .IsRequired();

        builder.Property(category => category.IsActive)
            .IsRequired();

        builder.HasMany(category => category.Questions)
            .WithOne(question => question.Category)
            .HasForeignKey(question => question.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
