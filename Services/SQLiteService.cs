using Lab1.Entities;
using SQLite;

namespace Lab1.Services
{
    public class SQLiteService : IDbService
    {
        private SQLiteConnection _db;
        private readonly string _dbPath = Path.Combine(FileSystem.AppDataDirectory, "DataBase.db");

        public IEnumerable<Author> GetAllAuthors() => _db.Table<Author>().ToList();

        public IEnumerable<Book> GetAuthorBooks(int id) =>
            _db.Table<Book>().Where(b => b.AuthorId == id).ToList();

        public void Init()
        {
            _db = new SQLiteConnection(_dbPath);
            
            _db.CreateTable<Author>();
            _db.CreateTable<Book>();

            if (!_db.Table<Author>().Any())
            {
            
                List<Author> authors =
                [
                    new() { Name = "Иван Мележ" },
                    new() { Name = "Ян Борщевский" },
                    new() { Name = "Владимир Короткевич" },
                    new() { Name = "Кондрат Крапива" },
                    new() { Name = "Змитрок Бядуля" },
                    new() { Name = "Янка Купала" },
                    new() { Name = "Якуб Колас" }
                ];

                foreach (var author in authors)
                {
                    _db.Insert(author);
                }

                
                List<Book> books =
                [
                    // Иван Мележ 
                    new() { Title = "Люди на болоте", CountPages = 512, AuthorId = authors[0].Id },
                    new() { Title = "Дыхание грозы", CountPages = 704, AuthorId = authors[0].Id },
                    new() { Title = "Минское направление", CountPages = 759, AuthorId = authors[0].Id },
                    
                    // Ян Борщевский 
                    new() { Title = "Шляхтич Завальня", CountPages = 352, AuthorId = authors[1].Id },
                    
                    // Владимир Короткевич 
                    new() { Title = "Дикая охота короля Стаха", CountPages = 280, AuthorId = authors[2].Id },
                    new() { Title = "Колосья под серпом твоим", CountPages = 735, AuthorId = authors[2].Id },
                    new() { Title = "Земля под белыми крыльями", CountPages = 192, AuthorId = authors[2].Id },
                    
                    // Кондрат Крапива
                    new() { Title = "Кто смеётся последним", CountPages = 188, AuthorId = authors[3].Id },
                    new() { Title = "Дипломированный баран", CountPages = 136, AuthorId = authors[3].Id },
                    new() { Title = "Мой сосед", CountPages = 146, AuthorId = authors[3].Id },
                    
                    // Змитрок Бядуля 
                    new() { Title = "Бондарь", CountPages = 12, AuthorId = authors[4].Id },
                    new() { Title = "Соловей", CountPages = 157, AuthorId = authors[4].Id },
                    new() { Title = "Счастье не в золоте", CountPages = 326, AuthorId = authors[4].Id },
                    
                    // Янка Купала 
                    new() { Title = "Могила льва", CountPages = 79, AuthorId = authors[5].Id },
                    new() { Title = "Жалейка", CountPages = 192, AuthorId = authors[5].Id },
                    new() { Title = "Бондаровна", CountPages = 5, AuthorId = authors[5].Id },
                    new() { Title = "Курган", CountPages = 64, AuthorId = authors[5].Id },
                    new() { Title = "Наследство", CountPages = 72, AuthorId = authors[5].Id },
                    
                    // Якуб Колас 
                    new() { Title = "Новая земля", CountPages = 398, AuthorId = authors[6].Id },
                    new() { Title = "На росстанях", CountPages = 688, AuthorId = authors[6].Id },
                    new() { Title = "Симон-музыкант", CountPages = 304, AuthorId = authors[6].Id },
                    new() { Title = "Трясина", CountPages = 232, AuthorId = authors[6].Id }
                ];

    
                foreach (var book in books)
                {
                    _db.Insert(book);
                }
            }
        }
    }
}