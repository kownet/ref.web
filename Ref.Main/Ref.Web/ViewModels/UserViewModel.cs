using System.ComponentModel.DataAnnotations;

namespace Ref.Web.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}