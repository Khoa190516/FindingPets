namespace FindingPets.Data.Commons
{
    public static class Commons
    {
        // JWT Token
        public static readonly string JWTClaimID = "Id";
        public static readonly string JWTClaimName = "Fullname";
        public static readonly string JWTClaimEmail = "Email";
        public static readonly string JWTClaimRoleID = "RoleId";

        //User Roles
        public static readonly int ADMIN = 0;
        public static readonly int CUSTOMER = 1;
    }
}
