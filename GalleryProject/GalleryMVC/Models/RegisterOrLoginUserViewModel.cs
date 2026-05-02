using System.ComponentModel.DataAnnotations;

namespace GalleryMVC.Models
{
    public class RegisterOrLoginUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]        
        public string Password { get; set; }
    }
}
