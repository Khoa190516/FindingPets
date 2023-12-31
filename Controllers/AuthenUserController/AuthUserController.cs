﻿using FindingPets.Business.JWT;
using FindingPets.Business.Services.AuthenUserServices;
using FindingPets.Business.Services.EmailServices;
using FindingPets.Data.Commons; 
using FindingPets.Data.Entities;
using FindingPets.Data.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using System.Net;

namespace FindingPets.Controllers.AuthenUserController
{
    [Route("api/account")]
    [ApiController]
    [Authorize]
    public class AuthUserController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IAuthenUserService _authenUserService;
        private readonly ILogger<AuthUserController> _logger;
        private readonly DecodeToken decodeToken;

        public AuthUserController(IEmailService emailService, IAuthenUserService authenUserService, ILogger<AuthUserController> logger)
        {
            _emailService = emailService;
            _authenUserService = authenUserService;
            _logger = logger;
            decodeToken = new();
        }

        [HttpPost("send-email-verification-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendEmailVerificationCode([FromBody] UserEmailModel emailModel)
        {
            try
            {
                _logger.LogInformation(message: $"Begin send email OTP at {DateTime.Now}");
                var OTP = await _emailService.SendOTP(emailModel.Email);

                return Ok(OTP);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            try
            {
                UserTokenModel? userLoggedIn;

                if (model == null) throw new ArgumentException("User Null");

                _logger.LogInformation(message: $"Begin call login with email service at {DateTime.Now}");
                userLoggedIn = await _authenUserService.LoginWithEmail(model);

                if (userLoggedIn != null)
                {
                    var OTP = string.Empty;

                    if(model.IsSendOTP != null && model.IsSendOTP == true)
                    {
                        OTP = await _emailService.SendOTP(model.Email);
                    }

                    UserLoginReponseModel result = new()
                    {
                        OTP = OTP,
                        Token = userLoggedIn.Token
                    };

                    return Ok(result);
                }

                return StatusCode(404, "Email Not Found");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (RecordNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("profile")]
        [Authorize(Roles ="admin,customer")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                Guid ownerId = decodeToken.DecodeID(token, Commons.JWTClaimID);

                var profile = await _authenUserService.GetProfile(ownerId);
                return Ok(profile);

            }
            catch(RecordNotFoundException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest("Get Profile Failed " + ex.Message);
            }
        }

        [HttpPut("update-profile")]
        [Authorize(Roles ="admin,customer")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateModel model)
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                Guid ownerId = decodeToken.DecodeID(token, Commons.JWTClaimID);

                _logger.LogInformation(message: $"Start Updating Profile ID: {ownerId}");
                var isUpdated = await _authenUserService.UpdateProfille(model, ownerId);

                return Ok(isUpdated);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] UserLoginModel signUpModel)
        {
            try
            {
                var isCreated = await _authenUserService.CreatAccount(signUpModel.Email);
                return isCreated ? Ok(isCreated) : BadRequest(isCreated);
            }
            catch(ResourceAlreadyExistsException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
