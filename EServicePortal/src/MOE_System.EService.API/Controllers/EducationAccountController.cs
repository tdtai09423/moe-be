using Microsoft.AspNetCore.Mvc;
using MOE_System.EService.Application.Common;
using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;


namespace MOE_System.EService.API.Controllers
{
    [Route("api/v1/education-accounts")]
    public class EducationAccountController : BaseApiController
    {
        private readonly IEducationAccountService _educationAccountService;

        public EducationAccountController(IEducationAccountService educationAccountService)
        {
            _educationAccountService = educationAccountService;
        }

        [HttpGet("{accountId}/balance")]
        public async Task<ActionResult<ApiResponse<BalanceResponse>>> GetAccountBalance([FromRoute] string accountId)
        {
            var balanceResponse = await _educationAccountService.GetBalanceAsync(accountId);

            return Success(balanceResponse);
        }

        [HttpGet("{accountId}/outstanding-fees")]
        public async Task<ActionResult<ApiResponse<OutstandingFeeResponse>>> GetAccountOutstandingFee([FromRoute] string accountId)
        {
            var outstandingFeeResponse = await _educationAccountService.GetOutstandingFeeAsync(accountId);

            return Success(outstandingFeeResponse);
        }
    }
}
