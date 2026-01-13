using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.Interfaces;
using MOE_System.Application.Common;
using MOE_System.Application.DTOs.AccountHolder.Request;
using MOE_System.Application.DTOs.AccountHolder.Response;

namespace MOE_System.API.Controllers
{
    [Route("api/v1/admin/account-holders")]
    public class AccountHolderController : BaseApiController
    {
        private readonly IAccountHolderService _accountHolderService;
        
        public AccountHolderController(IAccountHolderService accountHolderService)
        {
            _accountHolderService = accountHolderService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AccountHolderResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<AccountHolderResponse>>> CreateAccountHolder(CreateAccountHolderRequest request)
        {
            var newAccountHolder = await _accountHolderService.AddAccountHolderAsync(request);
            return Success(newAccountHolder, "Account holder created successfully");
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedList<AccountHolderResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedList<AccountHolderResponse>>>> GetAccountHolders(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20)
        {
            var accountHolders = await _accountHolderService.GetAccountHoldersAsync(pageNumber, pageSize);
            return Paginated(accountHolders.Items, pageNumber, pageSize, accountHolders.TotalCount, "Account holders retrieved successfully");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AccountHolderDetailResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<AccountHolderDetailResponse>>> GetAccountHolderDetail(string id)
        {
            var accountHolder = await _accountHolderService.GetAccountHolderDetailAsync(id);
            return Success(accountHolder, "Account holder details retrieved successfully");
        }
    }
}
