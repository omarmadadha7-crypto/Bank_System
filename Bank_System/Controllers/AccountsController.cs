using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bank_System.Extensions;
using Business_Logic.Common;
using Business_Logic.DTOs.Accounts;
using Business_Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.AllStaff)]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST api/accounts   (OpenedByUserId = the logged-in user)
        [HttpPost]
        public async Task<ActionResult<AccountResponse>> Open(OpenAccountRequest request, CancellationToken cancellationToken)
        {
            var account = await _accountService.OpenAsync(request, User.GetUserId(), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        // GET api/accounts/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<AccountResponse>> GetById(long id, CancellationToken cancellationToken)
        {
            return Ok(await _accountService.GetByIdAsync(id, cancellationToken));
        }

        // GET api/accounts/by-number/0011234567890123
        [HttpGet("by-number/{accountNumber}")]
        public async Task<ActionResult<AccountResponse>> GetByNumber(string accountNumber, CancellationToken cancellationToken)
        {
            return Ok(await _accountService.GetByNumberAsync(accountNumber, cancellationToken));
        }

        // GET api/accounts/customer/7
        [HttpGet("customer/{customerId:long}")]
        public async Task<ActionResult<IReadOnlyList<AccountResponse>>> GetByCustomer(long customerId, CancellationToken cancellationToken)
        {
            return Ok(await _accountService.GetByCustomerAsync(customerId, cancellationToken));
        }

        // GET api/accounts/5/balance
        [HttpGet("{id:long}/balance")]
        public async Task<ActionResult<AccountBalanceResponse>> GetBalance(long id, CancellationToken cancellationToken)
        {
            return Ok(await _accountService.GetBalanceAsync(id, cancellationToken));
        }

        // PATCH api/accounts/5/freeze
        [HttpPatch("{id:long}/freeze")]
        [Authorize(Roles = RoleNames.AdminOrManager)]
        public async Task<ActionResult<AccountResponse>> Freeze(long id, CancellationToken cancellationToken)
        {
            return Ok(await _accountService.FreezeAsync(id, cancellationToken));
        }

        // PATCH api/accounts/5/unfreeze
        [HttpPatch("{id:long}/unfreeze")]
        [Authorize(Roles = RoleNames.AdminOrManager)]
        public async Task<ActionResult<AccountResponse>> Unfreeze(long id, CancellationToken cancellationToken)
        {
            return Ok(await _accountService.UnfreezeAsync(id, cancellationToken));
        }

        // PATCH api/accounts/5/close   (balance must be zero)
        [HttpPatch("{id:long}/close")]
        [Authorize(Roles = RoleNames.AdminOrManager)]
        public async Task<ActionResult<AccountResponse>> Close(long id, CancellationToken cancellationToken)
        {
            return Ok(await _accountService.CloseAsync(id, cancellationToken));
        }

        // POST api/accounts/5/change-pin
        [HttpPost("{id:long}/change-pin")]
        public async Task<IActionResult> ChangePin(long id, ChangePinRequest request, CancellationToken cancellationToken)
        {
            await _accountService.ChangePinAsync(id, request, cancellationToken);
            return NoContent();
        }
    }
}
