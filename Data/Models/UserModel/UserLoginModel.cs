﻿namespace FindingPets.Data.Models.UserModel
{
    public class UserLoginModel
    {
        public string Email { get; set; } = string.Empty;
    }

    public class UserLoginReponseModel
    {
        public string OTP { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
