using System;
using System.Collections.Generic;
using QuizManagement.Data.Enum;

namespace QuizManagement.Application.Exams.ViewModel
{
    public class ExamViewModel
    {
        public int Id { get; set; }
        public string ExamName { get; set; }
        public int Time { get; set; }
        public string Examiner { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
        public List<QuestionExamDetailViewModel> QuestionExamDetailViewModels { get; set; }
    }
}