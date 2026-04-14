using Lab1.Entities;

namespace Lab1.Services
{
    public interface IDbService
    {
        IEnumerable<Author> GetAllAuthors();
        IEnumerable<Book> GetAuthorBooks(int id);
        void Init();
    }
}
