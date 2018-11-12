using QuizManagement.Data.Enum;
using QuizManagement.Data.Interfaces;
using QuizManagement.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuizManagement.Data.Entities.Quiz
{
    [Table("Exams")]
    public class Exam: DomainEntity<int>, ISwitchable, IDateTracking
    {
        public string ExamName { get; set; }
        public int Time { get; set; }
        public string Examiner { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }

    }
}
