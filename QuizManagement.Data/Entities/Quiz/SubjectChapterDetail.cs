using QuizManagement.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuizManagement.Data.Entities.Quiz
{
    [Table("SubjectChapterDetails")]
    public class SubjectChapterDetail : DomainEntity<int>
    {
        public int SubjectId { get; set; }

        public int ChapterId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
        [ForeignKey("ChapterId")]
        public virtual Chapter Chapter { get; set; }

    }
}
