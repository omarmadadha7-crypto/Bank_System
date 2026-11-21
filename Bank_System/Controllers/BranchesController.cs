using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.DTOs.Branches;
using Business_Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank_System.Controllers
{
    // Any logged-in employee can read. Only Admin can change.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        // GET api/branches?isActive=true
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<BranchResponse>>> GetAll([FromQuery] bool? isActive, CancellationToken cancellationToken)
        {
            return Ok(await _branchService.GetAllAsync(isActive, cancellationToken));
        }

        // GET api/branches/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<BranchResponse>> GetById(long id, CancellationToken cancellationToken)
        {
            return Ok(await _branchService.GetByIdAsync(id, cancellationToken));
        }

        // POST api/branches
        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<BranchResponse>> Create(CreateBranchRequest request, CancellationToken cancellationToken)
        {
            var created = await _branchService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/branches/5
        [HttpPut("{id:long}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<BranchResponse>> Update(long id, UpdateBranchRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _branchService.UpdateAsync(id, request, cancellationToken));
        }

        // PATCH api/branches/5/activate
        [HttpPatch("{id:long}/activate")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<BranchResponse>> Activate(long id, CancellationToken cancellationToken)
        {
            return Ok(await _branchService.SetActiveAsync(id, true, cancellationToken));
        }

        // PATCH api/branches/5/deactivate
        [HttpPatch("{id:long}/deactivate")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<BranchResponse>> Deactivate(long id, CancellationToken cancellationToken)
        {
            return Ok(await _branchService.SetActiveAsync(id, false, cancellationToken));
        }

        // DELETE api/branches/5
        [HttpDelete("{id:long}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            await _branchService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
