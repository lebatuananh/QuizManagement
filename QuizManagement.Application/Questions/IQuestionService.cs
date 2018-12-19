using System.Collections.Generic;
using QuizManagement.Application.Questions.ViewModel;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Questions
{
    public interface IQuestionService
    {
        void Create(QuestionViewModel questionViewModel);
        void Update(QuestionViewModel questionViewModel);
        PagedResult<QuestionViewModel> GetAllPaging(int? chapterId, int? subjectId,string keyword, int page, int pageSize);
        List<QuestionViewModel> GetAll();
        QuestionViewModel GetById(int id);
        void Delete(int id);
        void SaveChanges();
    }
}