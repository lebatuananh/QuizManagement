using QuizManagement.Data.Enum;
using QuizManagement.Data.Interfaces;
using QuizManagement.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizManagement.Data.Entities.Quiz
{
    [Table("Subjects")]
    public class Subject : DomainEntity<int>, ISwitchable, IDateTracking
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
    }
}