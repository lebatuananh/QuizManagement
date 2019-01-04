﻿using QuizManagement.Data.Enum;
using System;
using System.Collections.Generic;

namespace QuizManagement.WebApplication.Areas.Admin.Models
{
    public class RandomExamModel
    {
            public int Id { get; set; }
            public string ExamName { get; set; }
            public int Time { get; set; }
            public string Examiner { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime DateModified { get; set; } 
            public int QuestionsNumber { get; set; }
            public int SubjectId { get; set; }
            public Status Status { get; set; }
    }
}