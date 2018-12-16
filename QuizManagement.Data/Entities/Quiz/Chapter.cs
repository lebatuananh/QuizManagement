using QuizManagement.Data.Enum;
using QuizManagement.Data.Interfaces;
using QuizManagement.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuizManagement.Data.Entities.Quiz
{
    [Table("Chapters")]
    public class Chapter : DomainEntity<int>, ISwitchable, IDateTracking
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
    }
}
