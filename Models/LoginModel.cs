
namespace robot_controller_api.Models
{
    public partial class LoginModel
    {
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
    }
}
