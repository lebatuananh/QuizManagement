using System.Collections.Generic;
using QuizManagement.Application.Subjects.ViewModel;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Subjects
{
    public interface ISubjectService
    {
        void Add(SubjectViewModel subjectVm);

        void Update(SubjectViewModel subjectVm);

        void Delete(int id);

        List<SubjectViewModel> GetAll();

        PagedResult<SubjectViewModel> GetAllPaging(string keyword, int page, int pageSize);

        SubjectViewModel GetById(int id);

        void SaveChanges();
    }
}