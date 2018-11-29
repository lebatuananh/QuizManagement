using AutoMapper;
using QuizManagement.Application.Chapters.ViewModel;
using QuizManagement.Data.Entities.Quiz;

namespace QuizManagement.Application.AutoMappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ChapterViewModel, Chapter>().ConstructUsing(c => new Chapter());
        }
    }
}