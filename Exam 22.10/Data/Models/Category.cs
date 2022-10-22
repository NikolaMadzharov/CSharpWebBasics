namespace Library.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }

        public List<Book> Books { get; set; } = new List<Book>();
    }
}
