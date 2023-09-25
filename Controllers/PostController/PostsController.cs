using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindingPets.Data.Entities;
using FindingPets.Business.Services.PostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata;
using FindingPets.Business.Services.ImageServices;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Business.JWT;
using FindingPets.Data.Commons;

namespace FindingPets.Controllers.PostController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IImageService _imageService;
        private readonly ILogger<PostsController> _logger;
        private readonly DecodeToken _decodeToken;

        public PostsController(IPostService postService, IImageService imageService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _imageService = imageService;
            _logger = logger;
            _decodeToken = new();
        }

        // GET: api/Posts
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _postService.GetAllPosts();
            if (posts == null)
            {
                return NotFound();
            }
            return Ok(posts);
        }

        [HttpPost("insert")]
        [Authorize(Roles ="admin, customer")]
        public async Task<ActionResult<bool>> InsertPost(PostCreateModel newPost)
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                Guid onwerId = _decodeToken.DecodeID(token, Commons.JWTClaimID);
                _logger.LogInformation(message: $"Begin Call InsertPost Service at {DateTime.Now}");
                await _postService.CreatePost(newPost, onwerId);
                return Ok(true);
            }catch(Exception ex)
            {
                _logger.LogError(message: $"Insert Post Error with Exception: {ex.Message} at {DateTime.Now}");
                return BadRequest(false);
            }
        }

        [HttpPut("update")]
        [Authorize(Roles ="admin,customer")]
        public async Task<IActionResult> UpdatePost([FromBody] PostUpdateModel post)
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                Guid onwerId = _decodeToken.DecodeID(token, Commons.JWTClaimID);

                _logger.LogInformation(message: $"Start Updating Post ID: {post.Id}");
                var isUpdated = await _postService.UpdatePost(post);

                return isUpdated ==true ? Ok(isUpdated) : BadRequest(isUpdated);
            }
            catch(Exception ex)
            {
                _logger.LogError(message: $"Update Post ID: {post.Id} Error with Exception: {ex.Message} at {DateTime.Now}");
                return BadRequest(false);
            }
        }

        [HttpPut("update-status")]
        [Authorize(Roles = "admin,customer")]
        public async Task<IActionResult> UpdatePostStatus([FromBody] PostUpdateStatusModel post)
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                Guid ownerId = _decodeToken.DecodeID(token, Commons.JWTClaimID);

                _logger.LogInformation(message: $"Start Updating Post ID: {post.Id}");
                var isUpdated = await _postService.ChangePostStatus(post.Id, ownerId);

                return isUpdated == true ? Ok(isUpdated) : BadRequest(isUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Update Post ID: {post.Id} Error with Exception: {ex.Message} at {DateTime.Now}");
                return BadRequest(false);
            }
        }

        [HttpPost("upload-images")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadAnFile(List<IFormFile> files)
        {
            try
            {
                var imagesBase64 = await _imageService.ConvertImagesToBase64(files);

                return Ok(imagesBase64);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
