namespace Core.Model.User
{
    public class AddUserModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}