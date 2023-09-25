namespace FindingPets.Business.JWT
{
    public class UserTokenModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
    }
}
