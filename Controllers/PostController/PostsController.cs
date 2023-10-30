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
using MessagePack.Formatters;
using Microsoft.Data.SqlClient;
using FindingPets.Business.Services.AuthenUserServices;

namespace FindingPets.Controllers.PostController
{
    [Route("api/post")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IAuthenUserService _authenUserService;
        private readonly IPostService _postService;
        private readonly IImageService _imageService;
        private readonly ILogger<PostsController> _logger;
        private readonly DecodeToken _decodeToken;

        public PostsController(IAuthenUserService authenUserService, IPostService postService, IImageService imageService, ILogger<PostsController> logger)
        {
            _authenUserService = authenUserService;
            _postService = postService;
            _imageService = imageService;
            _logger = logger;
            _decodeToken = new();
        }

        // GET: api/post
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                var posts = await _postService.GetAllPosts();
                posts = posts.OrderByDescending(p => p.Created).ToList();
                return Ok(posts);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Posts
        [HttpGet("get-by-id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            try
            {
                var posts = await _postService.GetAllPosts();

                if (posts == null)
                {
                    return NotFound();
                }

                var post = posts.Where(p => p.Id.Equals(postId)).FirstOrDefault() ??
                    throw new RecordNotFoundException($"Post ID: {postId} Not Found"); ;

                return Ok(post);
            }
            catch (RecordNotFoundException ex){
                return StatusCode(404, ex.Message);
            }
            catch(SqlException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-user-with-posts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserWithPosts(string email)
        {
            try
            {
                var result = await _authenUserService.GetUserWithPost(email);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get-posts-by-user")]
        [Authorize(Roles = "admin,customer")]
        public async Task<IActionResult> GetPostsByUser()
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                Guid ownerId = _decodeToken.DecodeID(token, Commons.JWTClaimID);

                _logger.LogInformation(message: $"Start Getting Posts By User Token: {ownerId}");
                var profile = await _authenUserService.GetProfile(ownerId);
                if (profile != null)
                {
                    var result = await _authenUserService.GetUserWithPost(profile.Email);
                    if (result != null)
                    {
                        result.Posts = result.Posts.OrderByDescending(p => p.Created).ToList();
                        return Ok(result);
                    }
                }
                return StatusCode(StatusCodes.Status404NotFound);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("insert")]
        [Authorize(Roles ="admin, customer")]
        public async Task<IActionResult> InsertPost([FromBody] PostCreateModel newPost)
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                Guid onwerId = _decodeToken.DecodeID(token, Commons.JWTClaimID);
                _logger.LogInformation(message: $"Begin Call InsertPost Service at {DateTime.Now}");
                var isCreated = await _postService.CreatePost(newPost, onwerId);
                return Ok(isCreated);
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
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
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

        [HttpDelete("delete")]
        [Authorize(Roles = "admin, customer")]
        public async Task<IActionResult> DeletePost([FromBody] PostDeleteModel post)
        {
            try
            {
                var isDeleted = await _postService.DeletePost(post);
                return Ok(isDeleted);
            }
            catch(RecordNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
