using System;
using QuizManagement.Application.Chapters.ViewModel;
using QuizManagement.Application.Subjects.ViewModel;
using QuizManagement.Data.Enum;

namespace QuizManagement.Application.Questions.ViewModel
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string QuestionName { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Answer { get; set; }
        public int ScoreQuestion { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
        public int SubjectId { get; set; }
        public int ChapterId { get; set; }
        public SubjectViewModel SubjectViewModel { get; set; }
        public ChapterViewModel ChapterViewModel { get; set; }
    }
}