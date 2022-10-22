using Library.Contracts;
using Library.Data;
using Library.Data.Models;
using Library.Models.Movies;
using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext context;

        public BookService(LibraryDbContext _context)
        {
            context = _context;
        }

        public async Task AddBookAsync(AddBookViewModel model)
        {
            var entity = new Book()
            {
                Title = model.Title,
                Author = model.Author,
                Description = model.Description,
                Rating = model.Rating,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,

            };

            await context.Books.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task AddBookToCollectionAsync(int bookId, string userId)
        {
            var user = await context.Users
              .Where(u => u.Id == userId)
              .Include(u => u.ApplicationUserBooks)
              .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            var book = await context.Books.FirstOrDefaultAsync(u => u.Id == bookId);

            if (book == null)
            {
                throw new ArgumentException("The book is invalid!");
            }

            if (!user.ApplicationUserBooks.Any(a => a.BookId == bookId))
            {
                user.ApplicationUserBooks.Add(new ApplicationUserBook
                {
                    BookId = book.Id,
                    ApplicationUserId = user.Id,
                    Book = book,
                    ApplicationUser = user
                });

                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BookViewModel>> GetAllAsync()
        {
            var entities = await context.Books
                .Include(m => m.Category)
                .ToListAsync();


            return entities
               .Select(b => new BookViewModel()
               {
                   Author = b.Author,
                   Category = b?.Category?.Name,
                   Id = b.Id,
                   ImageUrl = b.ImageUrl,
                   Rating = b.Rating,
                   Title = b.Title,
                   Description = b.Description
                   
               });
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<BookViewModel>> GetFavouritesBooks(string userId)
        {
            var user = await context.Users
              .Where(u => u.Id == userId)
              .Include(u => u.ApplicationUserBooks)
              .ThenInclude(um => um.Book)
              .ThenInclude(m => m.Category)
              .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            return user.ApplicationUserBooks
                .Select(b => new BookViewModel()
                {
                    Author = b.Book.Author,
                    Category = b.Book.Category?.Name,
                    Id = b.BookId,
                    ImageUrl = b.Book.ImageUrl,
                    Title = b.Book.Title,
                    Rating = b.Book.Rating,
                });
        }

        public async Task RemoveBookFromCollectionAsync(int bookid, string userId)
        {
            var user = await context.Users
               .Where(u => u.Id == userId)
               .Include(u => u.ApplicationUserBooks)
               .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            var movie = user.ApplicationUserBooks.FirstOrDefault(m => m.BookId == bookid);

            if (movie != null)
            {
                user.ApplicationUserBooks.Remove(movie);

                await context.SaveChangesAsync();
            }
        }
    }
}
