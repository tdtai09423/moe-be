using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.Admin.DTOs.AccountHolder;
using MOE_System.Application.Admin.Interfaces;
using MOE_System.Application.Common;

namespace MOE_System.API.Controllers.Admin
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
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<AccountHolderResponse>>> CreateAccountHolder(CreateAccountHolderRequest request)
        {
            var newAccountHolder = await _accountHolderService.AddAccountHolderAsync(request);
            return Success(newAccountHolder, "Account holder created successfully");
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<AccountHolderResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<AccountHolderResponse>>>> GetAccountHolders()
        {
            var accountHolders = await _accountHolderService.GetAccountHoldersAsync();
            return Success(accountHolders, "Account holders retrieved successfully");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AccountHolderDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<AccountHolderDetailResponse>>> GetAccountHolderDetail(string id)
        {
            var accountHolder = await _accountHolderService.GetAccountHolderDetailAsync(id);
            return Success(accountHolder, "Account holder details retrieved successfully");
        }
    }
}
