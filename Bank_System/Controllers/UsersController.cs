using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bank_System.Extensions;
using Business_Logic.Common;
using Business_Logic.DTOs.Users;
using Business_Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank_System.Controllers
{
    // Bank employees (people who can log in). Admin only.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/users
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _userService.GetAllAsync(cancellationToken));
        }

        // GET api/users/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<UserResponse>> GetById(long id, CancellationToken cancellationToken)
        {
            return Ok(await _userService.GetByIdAsync(id, cancellationToken));
        }

        // POST api/users
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var created = await _userService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/users/5
        [HttpPut("{id:long}")]
        public async Task<ActionResult<UserResponse>> Update(long id, UpdateUserRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _userService.UpdateAsync(id, request, cancellationToken));
        }

        // PATCH api/users/5/activate
        [HttpPatch("{id:long}/activate")]
        public async Task<ActionResult<UserResponse>> Activate(long id, CancellationToken cancellationToken)
        {
            return Ok(await _userService.SetActiveAsync(id, true, User.GetUserId(), cancellationToken));
        }

        // PATCH api/users/5/deactivate
        [HttpPatch("{id:long}/deactivate")]
        public async Task<ActionResult<UserResponse>> Deactivate(long id, CancellationToken cancellationToken)
        {
            return Ok(await _userService.SetActiveAsync(id, false, User.GetUserId(), cancellationToken));
        }

        // PATCH api/users/5/unlock
        [HttpPatch("{id:long}/unlock")]
        public async Task<ActionResult<UserResponse>> Unlock(long id, CancellationToken cancellationToken)
        {
            return Ok(await _userService.UnlockAsync(id, cancellationToken));
        }
    }
}
