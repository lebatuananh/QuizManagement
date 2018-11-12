using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizManagement.Data.Entities.System;
using QuizManagement.DataEF.Extensions;

namespace QuizManagement.DataEF.Configurarions
{
    public class FunctionConfiguration : DbEntityConfiguration<Function>
    {
        public override void Configure(EntityTypeBuilder<Function> entity)
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).IsRequired()
                .HasMaxLength(128).IsUnicode(false);
            // etc.
        }
    }
}