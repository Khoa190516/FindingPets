namespace FindingPets.Data.Models.UserModel
{
    public class UserProfileUpdateModel
    {
        public string FullName { get; set; } = null!;

        //public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }

    public class ProfileModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        //public string Token { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
