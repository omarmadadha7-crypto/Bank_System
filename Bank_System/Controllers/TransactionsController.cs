using System.Threading;
using System.Threading.Tasks;
using Bank_System.Extensions;
using Business_Logic.Common;
using Business_Logic.DTOs.Transactions;
using Business_Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.AllStaff)]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // POST api/transactions/deposit
        [HttpPost("deposit")]
        public async Task<ActionResult<TransactionResponse>> Deposit(DepositRequest request, CancellationToken cancellationToken)
        {
            var result = await _transactionService.DepositAsync(request, User.GetUserId(), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // POST api/transactions/withdraw
        [HttpPost("withdraw")]
        public async Task<ActionResult<TransactionResponse>> Withdraw(WithdrawRequest request, CancellationToken cancellationToken)
        {
            var result = await _transactionService.WithdrawAsync(request, User.GetUserId(), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // POST api/transactions/transfer
        [HttpPost("transfer")]
        public async Task<ActionResult<TransactionResponse>> Transfer(TransferRequest request, CancellationToken cancellationToken)
        {
            var result = await _transactionService.TransferAsync(request, User.GetUserId(), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // GET api/transactions/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<TransactionResponse>> GetById(long id, CancellationToken cancellationToken)
        {
            return Ok(await _transactionService.GetByIdAsync(id, cancellationToken));
        }

        // GET api/transactions/by-reference/1234567890123
        [HttpGet("by-reference/{referenceNo:long}")]
        public async Task<ActionResult<TransactionResponse>> GetByReference(long referenceNo, CancellationToken cancellationToken)
        {
            return Ok(await _transactionService.GetByReferenceAsync(referenceNo, cancellationToken));
        }

        // GET api/transactions/account/5?from=2026-01-01&to=2026-12-31&type=Transfer&status=Success&page=1&pageSize=20
        [HttpGet("account/{accountId:long}")]
        public async Task<ActionResult<PagedResult<TransactionResponse>>> GetByAccount(
            long accountId, [FromQuery] TransactionFilter filter, CancellationToken cancellationToken)
        {
            return Ok(await _transactionService.GetByAccountAsync(accountId, filter, cancellationToken));
        }
    }
}
