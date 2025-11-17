using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FormSubmissionSystem.Infrastructure.Data;

namespace FormSubmissionSystem.Infrastructure.Data.Configurations;

public class FormSubmissionConfiguration : IEntityTypeConfiguration<FormSubmissionEntity>
{
    public void Configure(EntityTypeBuilder<FormSubmissionEntity> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired();

        builder.Property(s => s.FormType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Data)
            .IsRequired();

        builder.Property(s => s.SubmittedAt)
            .IsRequired();
    }
}

