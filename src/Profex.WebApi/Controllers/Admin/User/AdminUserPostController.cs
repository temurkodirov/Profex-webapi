using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profex.Application.Utils;
using Profex.Persistance.Dtos.PostRequest;
using Profex.Persistance.Dtos.Posts;
using Profex.Persistance.Validations.Dtos.PostRequest;
using Profex.Persistance.Validations.Dtos.Posts;
using Profex.Service.Interfaces.PostImages;
using Profex.Service.Interfaces.PostRequests;
using Profex.Service.Interfaces.Posts;

namespace Profex.WebApi.Controllers.Admin.User
{
    [Route("api/admin")]
    [ApiController]
    public class AdminUserPostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IPostImagesService _postImageService;
        private readonly IPostRequestService _requestService;
        private readonly int maxPageSize = 20;
        public AdminUserPostController(IPostService Postservice,
                                        IPostImagesService postImageService,
                                        IPostRequestService requestService)
        {
            _postService = Postservice;
            _postImageService = postImageService;
            _requestService = requestService;
        }


        [HttpPut("users/posts/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, [FromForm] PostUpdateDto dto)
        {
            var validator = new PostUpdateValidator();
            var validationResult = validator.Validate(dto);
            if (validationResult.IsValid) return Ok(await _postService.UpdateAsync(id, dto));
            else return BadRequest(validationResult.Errors);
        }

        [HttpDelete("users/posts/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
            => Ok(await _postService.DeleteAsync(id));

        [HttpGet("users/{userId}/requested")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserAllPostWithRequestAsync(long userId, [FromQuery] int page = 1)
            => Ok(await _requestService.GetUserAllPostWithRequestAsync(userId, new PaginationParams(page, maxPageSize)));


        [HttpGet("users/{userId}/posts/{postId}/requested")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserPostWithRequestAsync(long postId, long userId)
            => Ok(await _requestService.GetUserPostWithRequestAsync(userId, postId));

        [HttpDelete("users/posts/images/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImageAsync(long id)
            => Ok(await _postImageService.DeleteAsync(id));


        [HttpDelete("users/posts/requested")]
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
