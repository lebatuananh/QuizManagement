using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using QuizManagement.Application.Chapters.ViewModel;
using QuizManagement.Data.Entities.Quiz;
using QuizManagement.Infrastructure.Interfaces;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Chapters
{
    public class ChapterService : IChapterService
    {
        private readonly IRepository<Chapter, int> _chapterRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChapterService(IRepository<Chapter, int> chapterRepository, IUnitOfWork unitOfWork)
        {
            _chapterRepository = chapterRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(ChapterViewModel chapterVm)
        {
            var chapter = Mapper.Map<ChapterViewModel, Chapter>(chapterVm);
            _chapterRepository.Add(chapter);
        }

        public void Update(ChapterViewModel chapterVm)
        {
            var chapter = Mapper.Map<ChapterViewModel, Chapter>(chapterVm);
            _chapterRepository.Update(chapter);
        }

        public void Delete(int id)
        {
            _chapterRepository.Remove(id);
        }

        public List<ChapterViewModel> GetAll()
        {
            return _chapterRepository.FindAll().ProjectTo<ChapterViewModel>().ToList();
        }

        public PagedResult<ChapterViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = _chapterRepository.FindAll();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));

            int totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var paginationSet = new PagedResult<ChapterViewModel>()
            {
                Results = data.ProjectTo<ChapterViewModel>().ToList(),
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public ChapterViewModel GetById(int id)
        {
            return Mapper.Map<Chapter, ChapterViewModel>(_chapterRepository.FindById(id));
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}