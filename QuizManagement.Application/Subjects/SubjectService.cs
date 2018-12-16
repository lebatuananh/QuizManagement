using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using QuizManagement.Application.Subjects.ViewModel;
using QuizManagement.Data.Entities.Quiz;
using QuizManagement.Data.Enum;
using QuizManagement.Infrastructure.Interfaces;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Subjects
{
    public class SubjectService:ISubjectService
    {
        private readonly IRepository<Subject, int> _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IRepository<Subject, int> subjectRepository, IUnitOfWork unitOfWork)
        {
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
        }
        
        public void Add(SubjectViewModel subjectVm)
        {
            var subject = Mapper.Map<SubjectViewModel, Subject>(subjectVm);
            _subjectRepository.Add(subject);
        }

        public void Update(SubjectViewModel subjectVm)
        {
            var subject = Mapper.Map<SubjectViewModel, Subject>(subjectVm);
            _subjectRepository.Update(subject);
        }

        public void Delete(int id)
        {
            _subjectRepository.Remove(id);
        }

        public List<SubjectViewModel> GetAll()
        {
            return _subjectRepository.FindAll().ProjectTo<SubjectViewModel>().ToList();
        }

        public PagedResult<SubjectViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = _subjectRepository.FindAll(x => x.Status == Status.Active);
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));

            int totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var paginationSet = new PagedResult<SubjectViewModel>()
            {
                Results = data.ProjectTo<SubjectViewModel>().ToList(),
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public SubjectViewModel GetById(int id)
        {
            return Mapper.Map<Subject, SubjectViewModel>(_subjectRepository.FindById(id));
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}