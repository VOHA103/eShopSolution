using eShopSolution.ViewModels.System.Users;
using eShopSolution.Application.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.BackenbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;


        public UsersController(IUserService userService) {
            _userService = userService;
        }


        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromForm] LoginRequest requst)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var resultToken = await _userService.Authenticate(requst);
            if (string.IsNullOrEmpty(resultToken))
            {
                return BadRequest("Username and password is incorrect");

            }

            return Ok(new { token = resultToken });

        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
           
            var result = await _userService.Register(request);
            if (!result)
            {
                return BadRequest("Register is unsuccessful ");

            }

            return Ok();

        }
    }
}
