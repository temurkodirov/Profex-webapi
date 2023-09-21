﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profex.Persistance.Dtos.Auth;
using Profex.Persistance.Validations.Dtos;
using Profex.Persistance.Validations.Dtos.Auth;
using Profex.Service.Interfaces.Auth;
using Profex.Service.Interfaces.Users;

namespace Profex.WebApi.Controllers.User.UserAuth
{
    [Route("api/user")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private readonly int maxPageSize = 30;  
        private readonly IUserService _service;
        private readonly IAuthService _authService;
        public AuthUserController(IUserService service, IAuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterDto registerDto)
        {
            var validator = new RegisterValidator();
            var result = validator.Validate(registerDto);
            if (result.IsValid)
            {
                var serviceResult = await _authService.RegisterAsync(registerDto);

                return Ok(new { serviceResult.Result, serviceResult.CachedMinutes });
            }
            else return BadRequest(result.Errors);
        }

        [HttpPost("register/send-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendCodeRegisterAsync(string phone)
        {
            var result = PhoneNumberValidator.IsValid(phone);
            if (result == false) return BadRequest("Phone number is invalid!");

            var serviceResult = await _authService.SendCodeForRegisterAsync(phone);
            return Ok(new { serviceResult.Result, serviceResult.CachedVerificationMinutes });
        }

        [HttpPost("register/verify")]
        public async Task<IActionResult> VerifyRegisterAsync([FromBody] VerifyRegisterDto verifyRegisterDto)
        {
            var serviceResult = await _authService.VerifyRegisterAsync(verifyRegisterDto.PhoneNumber, verifyRegisterDto.Code);

            return Ok(new { serviceResult.Result, serviceResult.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            var validator = new LoginValidator();
            var valResult = validator.Validate(loginDto);
            if (valResult.IsValid == false) return BadRequest(valResult.Errors);
            var serviceResult = await _authService.LoginAsync(loginDto);

            return Ok(new { serviceResult.Result, serviceResult.Token });
        }

    }
}