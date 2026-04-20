using Data;
using LAB_5.Services;

namespace Lab_6
{
    public class UnitTest1
    {

        // BOOK TESTS 

        [Fact]
        public void AddBook_AddsBookToList()
        {
            // Arrange
            var service = new LibraryService();
            var book = new Book { Title = "Test Book", Author = "Test Author", ISBN = "1234567890" };

            // Act
            service.AddBook(book);

            // Assert
            Assert.DoesNotContain(service.GetBooks(), b => b.Title == "Test Book"); //wertyujhk
        }

        [Fact]
        public void AddBook_AssignsUniqueId()
        {
            // Arrange
            var service = new LibraryService();
            var book1 = new Book { Title = "Book 1", Author = "Author 1", ISBN = "111" };
            var book2 = new Book { Title = "Book 2", Author = "Author 2", ISBN = "222" };

            // Act
            service.AddBook(book1);
            service.AddBook(book2);

            // Assert
            Assert.NotEqual(book1.Id, book2.Id);
        }

        [Fact]
        public void EditBook_UpdatesBookDetails()
        {
            // Arrange
            var service = new LibraryService();
            var book = new Book { Title = "Old Title", Author = "Old Author", ISBN = "000" };
            service.AddBook(book);

            // Act
            book.Title = "New Title";
            book.Author = "New Author";
            service.EditBook(book);

            // Assert
            var updated = service.GetBooks().FirstOrDefault(b => b.Id == book.Id);
            Assert.Equal("New Title", updated?.Title);
            Assert.Equal("New Author", updated?.Author);
        }

        [Fact]
        public void EditBook_NonExistentBook_DoesNothing()
        {
            // Arrange
            var service = new LibraryService();
            var fakeBook = new Book { Id = 9999, Title = "Ghost", Author = "Nobody", ISBN = "000" };

            // Act
            service.EditBook(fakeBook);

            // Assert
            Assert.DoesNotContain(service.GetBooks(), b => b.Id == 9999);
        }

        [Fact]
        public void DeleteBook_RemovesBookFromList()
        {
            // Arrange
            var service = new LibraryService();
            var book = new Book { Title = "To Delete", Author = "Author", ISBN = "999" };
            service.AddBook(book);

            // Act
            service.DeleteBook(book.Id);

            // Assert
            Assert.DoesNotContain(service.GetBooks(), b => b.Id == book.Id);
        }

        [Fact]
        public void DeleteBook_NonExistentId_DoesNothing()
        {
            // Arrange
            var service = new LibraryService();
            int countBefore = service.GetBooks().Count;

            // Act
            service.DeleteBook(9999);

            // Assert
            Assert.Equal(countBefore, service.GetBooks().Count);
        }

        // USER TESTS 

        [Fact]
        public void AddUser_AddsUserToList()
        {
            // Arrange
            var service = new LibraryService();
            var user = new User { Name = "Test User", Email = "test@test.com" };

            // Act
            service.AddUser(user);

            // Assert
            Assert.Contains(service.GetUsers(), u => u.Name == "Test User");
        }

        [Fact]
        public void AddUser_AssignsUniqueId()
        {
            // Arrange
            var service = new LibraryService();
            var user1 = new User { Name = "User 1", Email = "user1@test.com" };
            var user2 = new User { Name = "User 2", Email = "user2@test.com" };

            // Act
            service.AddUser(user1);
            service.AddUser(user2);

            // Assert
            Assert.NotEqual(user1.Id, user2.Id);
        }

        [Fact]
        public void EditUser_UpdatesUserDetails()
        {
            // Arrange
            var service = new LibraryService();
            var user = new User { Name = "Old Name", Email = "old@test.com" };
            service.AddUser(user);

            // Act
            user.Name = "New Name";
            user.Email = "new@test.com";
            service.EditUser(user);

            // Assert
            var updated = service.GetUsers().FirstOrDefault(u => u.Id == user.Id);
            Assert.Equal("New Name", updated?.Name);
            Assert.Equal("new@test.com", updated?.Email);
        }

        [Fact]
        public void EditUser_NonExistentUser_DoesNothing()
        {
            // Arrange
            var service = new LibraryService();
            var fakeUser = new User { Id = 9999, Name = "Ghost", Email = "ghost@test.com" };

            // Act
            service.EditUser(fakeUser);

            // Assert
            Assert.DoesNotContain(service.GetUsers(), u => u.Id == 9999);
        }

        [Fact]
        public void DeleteUser_RemovesUserFromList()
        {
            // Arrange
            var service = new LibraryService();
            var user = new User { Name = "To Delete", Email = "delete@test.com" };
            service.AddUser(user);

            // Act
            service.DeleteUser(user.Id);

            // Assert
            Assert.DoesNotContain(service.GetUsers(), u => u.Id == user.Id);
        }

        [Fact]
        public void DeleteUser_NonExistentId_DoesNothing()
        {
            // Arrange
            var service = new LibraryService();
            int countBefore = service.GetUsers().Count;

            // Act
            service.DeleteUser(9999);

            // Assert
            Assert.Equal(countBefore, service.GetUsers().Count);
        }

        // ========== BORROW & RETURN TESTS ==========

        [Fact]
        public void BorrowBook_RemovesBookFromAvailable()
        {
            // Arrange
            var service = new LibraryService();
            var book = new Book { Title = "Borrow Me", Author = "Author", ISBN = "555" };
            var user = new User { Name = "Borrower", Email = "borrow@test.com" };
            service.AddBook(book);
            service.AddUser(user);

            // Act
            service.BorrowBook(book.Id, user.Id);

            // Assert
            Assert.DoesNotContain(service.GetBooks(), b => b.Id == book.Id);
        }

        [Fact]
        public void BorrowBook_AddsBookToBorrowedList()
        {
            // Arrange
            var service = new LibraryService();
            var book = new Book { Title = "Borrow Me", Author = "Author", ISBN = "555" };
            var user = new User { Name = "Borrower", Email = "borrow@test.com" };
            service.AddBook(book);
            service.AddUser(user);

            // Act
            service.BorrowBook(book.Id, user.Id);

            // Assert
            Assert.Contains(service.GetBorrowedBooks()[user.Id], b => b.Title == "Borrow Me");
        }

        [Fact]
        public void ReturnBook_AddsBookBackToAvailable()
        {
            // Arrange
            var service = new LibraryService();
            var book = new Book { Title = "Return Me", Author = "Author", ISBN = "777" };
            var user = new User { Name = "Returner", Email = "return@test.com" };
            service.AddBook(book);
            service.AddUser(user);
            service.BorrowBook(book.Id, user.Id);

            // Act
            service.ReturnBook(user.Id, 0);

            // Assert
            Assert.Contains(service.GetBooks(), b => b.Title == "Return Me");
        }

        [Fact]
        public void ReturnBook_RemovesBookFromBorrowedList()
        {
            // Arrange
            var service = new LibraryService();
            var book = new Book { Title = "Return Me", Author = "Author", ISBN = "777" };
            var user = new User { Name = "Returner", Email = "return@test.com" };
            service.AddBook(book);
            service.AddUser(user);
            service.BorrowBook(book.Id, user.Id);

            // Act
            service.ReturnBook(user.Id, 0);

            // Assert
            Assert.Empty(service.GetBorrowedBooks()[user.Id]);
        }

        [Fact]
        public void BorrowBook_NonExistentBook_DoesNothing()
        {
            // Arrange
            var service = new LibraryService();
            var user = new User { Name = "Test", Email = "test@test.com" };
            service.AddUser(user);
            int countBefore = service.GetBooks().Count;

            // Act
            service.BorrowBook(9999, user.Id);

            // Assert
            Assert.Equal(countBefore, service.GetBooks().Count);
        }
    }
}