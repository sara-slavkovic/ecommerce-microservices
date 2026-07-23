using Microsoft.AspNetCore.Mvc;
using SharedKernel.Web.Filters;
using Swashbuckle.AspNetCore.Annotations;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register a new user")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetUserSnapshot), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var user = await _userService.LoginAsync(dto);
            return Ok(user);
        }

        [HttpGet("{id:guid}/snapshot")]
        [InternalApiKey]
        [SwaggerOperation(Summary = "Get user snapshot for inter-service validation")]
        public async Task<IActionResult> GetUserSnapshot(Guid id)
        {
            var snapshot = await _userService.GetUserSnapshotByIdAsync(id);
            if (snapshot == null) return NotFound();
            return Ok(snapshot);
        }
    }
}
