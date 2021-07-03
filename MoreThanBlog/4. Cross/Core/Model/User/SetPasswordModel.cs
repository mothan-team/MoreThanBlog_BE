namespace Core.Model.User
{
    public class SetPasswordModel
    {
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public string Otp { get; set; }

        public string Email { get; set; }
    }
}