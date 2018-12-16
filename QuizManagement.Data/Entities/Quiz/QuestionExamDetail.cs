using QuizManagement.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuizManagement.Data.Entities.Quiz
{
    [Table("QuestionExamDetails")]
    public class QuestionExamDetail: DomainEntity<int>
    {
        public int QuestionId { get; set; }
        public int ExamId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }
    }
}
