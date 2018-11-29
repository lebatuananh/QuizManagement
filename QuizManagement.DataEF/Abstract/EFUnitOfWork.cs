using QuizManagement.DataEF.Connector;
using QuizManagement.Infrastructure.Interfaces;

namespace QuizManagement.DataEF.Abstract
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public EFUnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}