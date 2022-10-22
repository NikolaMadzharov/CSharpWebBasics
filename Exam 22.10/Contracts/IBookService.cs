using Library.Data.Models;
using Library.Models.Movies;
using System.Threading.Tasks;

namespace Library.Contracts
{
    public interface IBookService
    {
        Task AddBookAsync(AddBookViewModel model);

        Task<IEnumerable<Category>> GetCategoriesAsync();

        Task<IEnumerable<BookViewModel>> GetAllAsync();

        Task AddBookToCollectionAsync(int bookId, string userId);

        Task<IEnumerable<BookViewModel>> GetFavouritesBooks(string userId);

        Task RemoveBookFromCollectionAsync(int movieId, string userId);
    }
}
