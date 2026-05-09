namespace GalleryMVC.Models
{
    public class UserManagementViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string CurrentRole { get; set; }
        public List<string> AllRoles { get; set; }
    }
}
