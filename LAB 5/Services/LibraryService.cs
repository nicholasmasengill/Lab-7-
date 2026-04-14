using Data;
using LAB_5.Services;

namespace LAB_5.Services
{
    public interface ILibraryService
    {
        List<Book> GetBooks();
        List<User> GetUsers();
        Dictionary<int, List<Book>> GetBorrowedBooks();
        void AddBook(Book book);
        void EditBook(Book book);
        void DeleteBook(int id);
        void AddUser(User user);
        void EditUser(User user);
        void DeleteUser(int id);
        void BorrowBook(int bookId, int userId);
        void ReturnBook(int userId, int bookIndex);
    }

    public class LibraryService : ILibraryService
    {
        private List<Book> books = new List<Book>();
        private List<User> users = new List<User>();
        private Dictionary<int, List<Book>> borrowedBooks = new Dictionary<int, List<Book>>();

        private readonly string booksPath;
        private readonly string usersPath;

        public LibraryService(string? basePath = null)
        {
            var root = basePath ?? AppDomain.CurrentDomain.BaseDirectory;
            booksPath = Path.Combine(root, "Data", "Books.csv");
            usersPath = Path.Combine(root, "Data", "Users.csv");
            ReadBooks();
            ReadUsers();
        }

        private void ReadBooks()
        {
            try
            {
                if (!File.Exists(booksPath)) return;
                foreach (var line in File.ReadLines(booksPath))
                {
                    var fields = line.Split(',');
                    if (fields.Length >= 4)
                    {
                        books.Add(new Book
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Title = fields[1].Trim(),
                            Author = fields[2].Trim(),
                            ISBN = fields[3].Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading books: {ex.Message}");
            }
        }

        private void ReadUsers()
        {
            try
            {
                if (!File.Exists(usersPath)) return;
                foreach (var line in File.ReadLines(usersPath))
                {
                    var fields = line.Split(',');
                    if (fields.Length >= 3)
                    {
                        users.Add(new User
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Name = fields[1].Trim(),
                            Email = fields[2].Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading users: {ex.Message}");
            }
        }

        private void WriteBooks()
        {
            try
            {
                if (!File.Exists(booksPath)) return;
                var lines = books.Select(b => $"{b.Id},{b.Title},{b.Author},{b.ISBN}");
                File.WriteAllLines(booksPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing books: {ex.Message}");
            }
        }

        private void WriteUsers()
        {
            try
            {
                if (!File.Exists(usersPath)) return;
                var lines = users.Select(u => $"{u.Id},{u.Name},{u.Email}");
                File.WriteAllLines(usersPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing users: {ex.Message}");
            }
        }

        public List<Book> GetBooks() => books;
        public List<User> GetUsers() => users;
        public Dictionary<int, List<Book>> GetBorrowedBooks() => borrowedBooks;

        public void AddBook(Book book)
        {
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            books.Add(book);
            WriteBooks();
        }

        public void EditBook(Book book)
        {
            var existing = books.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            {
                existing.Title = book.Title;
                existing.Author = book.Author;
                existing.ISBN = book.ISBN;
                WriteBooks();
            }
        }

        public void DeleteBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                books.Remove(book);
                WriteBooks();
            }
        }

        public void AddUser(User user)
        {
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            WriteUsers();
        }

        public void EditUser(User user)
        {
            var existing = users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                existing.Name = user.Name;
                existing.Email = user.Email;
                WriteUsers();
            }
        }

        public void DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                users.Remove(user);
                WriteUsers();
            }
        }

        public void BorrowBook(int bookId, int userId)
        {
            var book = books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                if (!borrowedBooks.ContainsKey(userId))
                    borrowedBooks[userId] = new List<Book>();
                borrowedBooks[userId].Add(book);
                books.Remove(book);
            }
        }

        public void ReturnBook(int userId, int bookIndex)
        {
            if (borrowedBooks.ContainsKey(userId) && bookIndex < borrowedBooks[userId].Count)
            {
                var book = borrowedBooks[userId][bookIndex];
                borrowedBooks[userId].RemoveAt(bookIndex);
                books.Add(book);
            }
        }
    }
}