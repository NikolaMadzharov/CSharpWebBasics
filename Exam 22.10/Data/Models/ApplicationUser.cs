namespace Library.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser:IdentityUser
    {

        public List<ApplicationUserBook> ApplicationUserBooks { get; set; } = new List<ApplicationUserBook>();
    }
}
