using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.DTOs.Customers;
using Business_Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.AllStaff)]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET api/customers?search=ahmad&branchId=1&isActive=true&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResult<CustomerResponse>>> Search(
            [FromQuery] string? search,
            [FromQuery] long? branchId,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            return Ok(await _customerService.SearchAsync(search, branchId, isActive, page, pageSize, cancellationToken));
        }

        // GET api/customers/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<CustomerResponse>> GetById(long id, CancellationToken cancellationToken)
        {
            return Ok(await _customerService.GetByIdAsync(id, cancellationToken));
        }

        // POST api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            var created = await _customerService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/customers/5
        [HttpPut("{id:long}")]
        public async Task<ActionResult<CustomerResponse>> Update(long id, UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _customerService.UpdateAsync(id, request, cancellationToken));
        }

        // PATCH api/customers/5/activate
        [HttpPatch("{id:long}/activate")]
        [Authorize(Roles = RoleNames.AdminOrManager)]
        public async Task<ActionResult<CustomerResponse>> Activate(long id, CancellationToken cancellationToken)
        {
            return Ok(await _customerService.SetActiveAsync(id, true, cancellationToken));
        }

        // PATCH api/customers/5/deactivate
        [HttpPatch("{id:long}/deactivate")]
        [Authorize(Roles = RoleNames.AdminOrManager)]
        public async Task<ActionResult<CustomerResponse>> Deactivate(long id, CancellationToken cancellationToken)
        {
            return Ok(await _customerService.SetActiveAsync(id, false, cancellationToken));
        }
    }
}
