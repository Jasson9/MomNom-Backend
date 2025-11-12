namespace MomNom_Backend.Model.Request
{
    public class ForgotPasswordRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
