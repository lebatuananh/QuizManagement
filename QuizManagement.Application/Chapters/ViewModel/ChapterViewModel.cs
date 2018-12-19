using System;
using QuizManagement.Data.Enum;

namespace QuizManagement.Application.Chapters.ViewModel
{
    public class ChapterViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
    }
}