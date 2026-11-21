using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.DTOs.Roles;
using Business_Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank_System.Controllers
{
    // Read only: roles are seeded (Admin, Manager, Teller)
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.Admin)]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET api/roles
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<RoleResponse>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _roleService.GetAllAsync(cancellationToken));
        }

        // GET api/roles/1
        [HttpGet("{id:long}")]
        public async Task<ActionResult<RoleResponse>> GetById(long id, CancellationToken cancellationToken)
        {
            return Ok(await _roleService.GetByIdAsync(id, cancellationToken));
        }
    }
}
