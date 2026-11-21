using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.DTOs.Currencies;
using Business_Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank_System.Controllers
{
    // Any logged-in employee can read. Only Admin can change.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        // GET api/currencies
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CurrencyResponse>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _currencyService.GetAllAsync(cancellationToken));
        }

        // GET api/currencies/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<CurrencyResponse>> GetById(long id, CancellationToken cancellationToken)
        {
            return Ok(await _currencyService.GetByIdAsync(id, cancellationToken));
        }

        // GET api/currencies/by-code/JOD
        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<CurrencyResponse>> GetByCode(string code, CancellationToken cancellationToken)
        {
            return Ok(await _currencyService.GetByCodeAsync(code, cancellationToken));
        }

        // POST api/currencies
        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<CurrencyResponse>> Create(CreateCurrencyRequest request, CancellationToken cancellationToken)
        {
            var created = await _currencyService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/currencies/5
        [HttpPut("{id:long}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<CurrencyResponse>> Update(long id, UpdateCurrencyRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _currencyService.UpdateAsync(id, request, cancellationToken));
        }

        // PATCH api/currencies/5/rate
        [HttpPatch("{id:long}/rate")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<CurrencyResponse>> UpdateRate(long id, UpdateRateRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _currencyService.UpdateRateAsync(id, request, cancellationToken));
        }

        // DELETE api/currencies/5
        [HttpDelete("{id:long}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            await _currencyService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
