using System.Collections.Generic;
using QuizManagement.Application.Chapters.ViewModel;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Chapters
{
    public interface IChapterService
    {
        void Add(ChapterViewModel chapterVm);

        void Update(ChapterViewModel chapterVm);

        void Delete(int id);

        List<ChapterViewModel> GetAll();

        PagedResult<ChapterViewModel> GetAllPaging(string keyword, int page, int pageSize);

        ChapterViewModel GetById(int id);

        void SaveChanges();
    }
}