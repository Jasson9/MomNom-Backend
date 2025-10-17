namespace MomNom_Backend.Model.Request
{
    public class LoginRequest
    {
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
