using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profex.Application.Utils;
using Profex.Persistance.Dtos.Master1;
using Profex.Persistance.Dtos.PostRequest;
using Profex.Persistance.Validations.Dtos.Masters;
using Profex.Persistance.Validations.Dtos.PostRequest;
using Profex.Service.Interfaces.Master1;
using Profex.Service.Interfaces.PostRequests;

namespace Profex.WebApi.Controllers.Admin.Master
{
    [Route("api/admin/masters")]
    [ApiController]
    public class AdminMaster : ControllerBase
    {
        private readonly IMaster1Service _masterService;
        private readonly IPostRequestService _requestService;
        private readonly int maxPageSize = 20;
        public AdminMaster(IMaster1Service master1Service, IPostRequestService requestService)
        {
            _masterService = master1Service;
            _requestService = requestService;
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, [FromForm] Master1UpdateDto dto)
        {
            var updateValidator = new MasterUpdateValidator();
            var result = updateValidator.Validate(dto);
            if (result.IsValid) return Ok(await _masterService.UpdateAsync(id, dto));

            else return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
             => Ok(await _masterService.DeleteAsync(id));


        [HttpGet("{masterId}/posts/requested")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMasterRequestedAllPostsAsync(long masterId, [FromQuery] int page = 1)
        {
            return Ok(await _requestService.GetMasterRequestedAllPostsAsync(masterId, new PaginationParams(page, maxPageSize)));
        }

        [HttpDelete("posts/requested")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRequestAsync([FromForm] RequestDeleteDto dto)
        {

            var validator = new RequestDeleteValidator();
            var result = validator.Validate(dto);

            if (result.IsValid) return Ok(await _requestService.DeleteRequestAsync(dto.masterId, dto.postId, dto.userId));
            else return BadRequest(result.Errors);
        }
    }
}
