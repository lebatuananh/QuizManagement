using QuizManagement.Data.Enum;
using QuizManagement.Data.Interfaces;
using QuizManagement.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuizManagement.Data.Entities.Quiz
{
    [Table("Questions")]
    public class Question: DomainEntity<int>, ISwitchable, IDateTracking
    {
        public string QuestionName { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Answer { get; set; }
        public int ScoreQuestion { get; set; }
        public int SubjectChapterDetailId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
        [ForeignKey("SubjectChapterDetailId")]
        public virtual SubjectChapterDetail SubjectChapterDetail { get; set; }
    }
}
