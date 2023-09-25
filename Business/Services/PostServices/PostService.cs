using FindingPets.Data.Entities;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostRepositories;
using Microsoft.IdentityModel.Tokens;

namespace FindingPets.Business.Services.PostServices
{
    public class PostService : IPostService
    {
        private readonly IPostRepo _postRepo;
        private readonly IPostImagesRepo _postImagesRepo;
        private readonly IAuthenUserRepo _authenUserRepo;
        private readonly ILogger<PostService> _logger;

        public PostService(IPostRepo postRepo, IPostImagesRepo postImagesRepo, ILogger<PostService> logger, IAuthenUserRepo authenUserRepo)
        {
            _postRepo = postRepo;
            _postImagesRepo = postImagesRepo;
            _logger = logger;
            _authenUserRepo = authenUserRepo;
        }

        public async Task<bool> ChangePostStatus(Guid postId, Guid userId)
        {
            var account = await _authenUserRepo.FindByID(userId);
            if (account == null) throw new Exception($"User ID: {userId} not found");

            switch (account.UserRole)
            {
                case 0:
                    {
                        return await _postRepo.UpdatePostStatus(postId, true);
                    }
                case 1:
                    {
                        return await _postRepo.UpdatePostStatus(postId, false);
                    }
            }
            return false;
        }

        public async Task<bool> CreatePost(PostCreateModel newPost, Guid ownerId)
        {
            try
            {
                // Insert Post
                Post postEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Contact = newPost.Contact,
                    Description = newPost.Description,
                    Created = DateTime.Now,
                    IsBanned = false,
                    IsClosed = false,
                    OwnerId = ownerId,
                };

                // Insert post to DB
                _logger.LogInformation(message: $"Begin insert Post to DB with ID: {postEntity.Id} at {DateTime.Now}");
                await _postRepo.Insert(postEntity);

                // Insert Images
                foreach (var newImage in newPost.PostImages)
                {
                    PostImage imageEntity = new()
                    {
                        Id = Guid.NewGuid(),
                        ImageBase64 = newImage.ImageBase64,
                        PostId = postEntity.Id
                    };
                    // Insert images to DB
                    _logger.LogInformation(message: $"Begin insert Image to DB with ID: {imageEntity.Id} at {DateTime.Now}");
                    await _postImagesRepo.Insert(imageEntity);
                }
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogInformation(message: $"Error when inserting post to DB: {ex.Message} at {DateTime.Now}");
                return false;
            }
        }

        public async Task<List<PostView>> GetAllPosts()
        {
            List<PostView> posts = await _postRepo.GetPosts();

            foreach (PostView postView in posts)
            {
                List<PostImageView> images = await _postImagesRepo.GetPostImagesByPostID(postView.Id);
                postView.PostImages.AddRange(images);
            }

            return posts;

        }

        public async Task<bool> UpdatePost(PostUpdateModel newPost)
        {
            try
            {
                //Update post content
                _logger.LogInformation(message: $"Update Post ID: {newPost.Id} to DB");
                var isUpdated = await _postRepo.UpdatePost(newPost);
                if (!isUpdated) return false;

                //Get old images 
                var oldPostImages = await _postImagesRepo.GetPostImagesByPostID(newPost.Id);
                //Remove all old images
                if (!oldPostImages.IsNullOrEmpty())
                {
                    _logger.LogInformation(message: $"Remove old all postImages PostID: {newPost.Id} from DB");
                    await _postImagesRepo.RemoveImages(oldPostImages);
                }
                //Add new images
                foreach (var image in newPost.postImages)
                {
                    PostImage newImage = new()
                    {
                        Id = Guid.NewGuid(),
                        ImageBase64 = image.ImageBase64,
                        PostId = newPost.Id
                    };
                    _logger.LogInformation(message: $"Insert new postImage ID: {newImage.Id} to DB");
                    await _postImagesRepo.Insert(newImage);
                }

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(message: $"ERROR: Update Post ID: {newPost.Id} to DB. \n Error: {ex.Message}");
                throw new Exception($"Update Post {newPost.Id} Failed with error: {ex.Message}");
            }        
        }
    }
}
