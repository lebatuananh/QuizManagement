using System.Collections.Generic;
using QuizManagement.Application.Exams.ViewModel;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Exams
{
    public interface IExamService
    {
        void Create(ExamViewModel examViewModel);
        void Update(ExamViewModel examViewModel);
        PagedResult<ExamViewModel> GetAllPaging(int? subjectId, string keyword, int pageIndex, int pageSize);
        ExamViewModel GetById(int examId);
        QuestionExamDetailViewModel CreateDetail(QuestionExamDetailViewModel questionExamDetailViewModel);
        void DeleteDetail(int examId, int questionId);
        List<QuestionExamDetailViewModel> GetExamDetails(int examId);
        void SaveChanges();

    }
}