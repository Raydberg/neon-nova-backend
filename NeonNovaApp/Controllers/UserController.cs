using Application.DTOs.AuthDTOs;
using Application.DTOs.UsersDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Intrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NeonNovaApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    // [Authorize(Policy = "isUser")]
    public class UserController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<Users> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserController(ICurrentUserService currentUserService, UserManager<Users> userManager,
            ApplicationDbContext context,IMapper mapper)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "isAdmin")]
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
            return usersDto;
        }

        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetCurrentUser()
        {
            var user = await _currentUserService.GetUser();
            if (user is null) return NotFound();
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
            });
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UserUpdateDto dto)
        {
            var user = await GetCurrentUser();
            if (user is null) return NotFound();
            // user.E
            // _userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}