using System.ComponentModel.DataAnnotations;

namespace HSG.Warehouse.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введіть дані")]
        public string? Login { get; set; }
        [Required(ErrorMessage = "Введіть дані")]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }        
    }
}
