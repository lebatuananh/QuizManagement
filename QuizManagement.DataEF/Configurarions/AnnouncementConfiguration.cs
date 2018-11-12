using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizManagement.Data.Entities.System;
using QuizManagement.DataEF.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizManagement.DataEF.Configurarions
{
    public class AnnouncementConfiguration : DbEntityConfiguration<Announcement>
    {
        public override void Configure(EntityTypeBuilder<Announcement> entity)
        {
            entity.HasKey(n => n.Id);
            entity.Property(c => c.Id).HasMaxLength(128).IsRequired();
            // etc.
        }
    }
}
