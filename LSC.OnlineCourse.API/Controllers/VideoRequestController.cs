using LSC.OnlineCourse.API.common;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace LSC.OnlineCourse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoRequestController : ControllerBase
    {

        private readonly IVideoRequestService _videoRequestService;
       private readonly IUserClaims userClaims;

        public VideoRequestController(IVideoRequestService videoRequestService, IUserClaims userClaims)
        {
            _videoRequestService = videoRequestService;
            this.userClaims = userClaims;
        }

        [HttpGet]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<IEnumerable<VideoRequestModel>>> GetAll()
        {
            List<VideoRequestModel> videoRequests;

            var userRole = userClaims.GetUserRoles();

            if (userRole.Contains("Admin"))
            {
                videoRequests = await _videoRequestService.GetAllAsync();
            }
            else
            {
              var  videoRequest=await _videoRequestService.GetByUserIdAsync(userClaims.GetUserId());
                videoRequests = videoRequest.ToList();
            }

            return Ok(videoRequests);
        }

        [HttpGet("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<VideoRequestModel>> GetById(int id)
        {

           var videoRequest = await _videoRequestService.GetByIdAsync(id);

            if (videoRequest == null) { 
                return NotFound();
            
            }
            return Ok(videoRequest);

        }


        [HttpGet("user/{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<IEnumerable<VideoRequestModel>>> GetByUserId(int id)
        {

            var videoRequest = await _videoRequestService.GetByUserIdAsync(id);

            if (videoRequest == null)
            {
                return NotFound();

            }
            return Ok(videoRequest);

        }

        [HttpDelete]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<ActionResult> Delete(int id)
        {
            await _videoRequestService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<ActionResult<VideoRequestModel>> Create(VideoRequestModel model)
        {
            var createdVideoRequest = await _videoRequestService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = model.VideoRequestId }, createdVideoRequest);
        }


        [HttpPut]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<ActionResult<VideoRequestModel>> Update(int id, VideoRequestModel model)
        {
            var updateVideoRequest = await _videoRequestService.UpdateAsync(id,model);
            if (updateVideoRequest == null)
            {
                return NotFound();
            }

            return Ok();
        }

    }
}
