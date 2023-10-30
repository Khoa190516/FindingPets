using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostRepositories;
using Microsoft.IdentityModel.Tokens;
using FindingPets.Data.UnitOfWork;
using FindingPets.Data.Commons;

namespace FindingPets.Business.Services.PostServices
{
    public class PostService : IPostService
    {
        private readonly IPostRepo _postRepo;
        private readonly IPostImagesRepo _postImagesRepo;
        private readonly IAuthenUserRepo _authenUserRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PostService> _logger;

        public PostService(IUnitOfWork unitOfWork, IPostRepo postRepo, IPostImagesRepo postImagesRepo, ILogger<PostService> logger, IAuthenUserRepo authenUserRepo)
        {
            _unitOfWork = unitOfWork;
            _postRepo = postRepo;
            _postImagesRepo = postImagesRepo;
            _logger = logger;
            _authenUserRepo = authenUserRepo;
        }

        public async Task<bool> ChangePostStatus(Guid postId, Guid userId)
        {
            var account = await _authenUserRepo.FindByID(userId) ?? throw new Exception($"User ID: {userId} not found");
            switch (account.Userrole)
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
                    Title = newPost.Title,
                    Contact = newPost.Contact,
                    Description = newPost.Description,
                    Created = DateTime.Now,
                    Isbanned = false,
                    Isclosed = false,
                    Ownerid = ownerId,
                };

                // Insert post to DB
                _logger.LogInformation(message: $"Begin insert Post to DB with ID: {postEntity.Id} at {DateTime.Now}");
                _unitOfWork.BeginTransaction();
                var account = await _unitOfWork.AuthenUserRepo.FindByID(ownerId) ?? 
                    throw new RecordNotFoundException($"User ID: {ownerId} Not Found");
                _unitOfWork.PostRepo.Add(postEntity); 

                // Insert Images
                foreach (var newImage in newPost.PostImages)
                {
                    Postimage imageEntity = new()
                    {
                        Id = Guid.NewGuid(),
                        Imagebase64 = newImage.ImageBase64,
                        Postid = postEntity.Id
                    };
                    // Insert images to DB
                    _logger.LogInformation(message: $"Begin insert Image to DB with ID: {imageEntity.Id} at {DateTime.Now}");
                    _unitOfWork.PostImagesRepo.Add(imageEntity);
                }
                var effectedRows = _unitOfWork.CommitTransaction();
                _unitOfWork.Dispose();
                return effectedRows > 0;
            }
            catch(Exception ex)
            {
                _logger.LogInformation(message: $"Error when inserting post to DB: {ex.Message} at {DateTime.Now}");
                _unitOfWork.RollbackTransaction();
                _unitOfWork.Dispose();
                throw;
            }
        }

        public async Task<bool> DeletePost(PostDeleteModel post)
        {
            var postImages = await _postImagesRepo.GetPostImagesByPostID(post.Id) ??
                throw new RecordNotFoundException($"Post Images of Post ID: {post.Id} Not Found");

            try
            {
                if (postImages != null)
                {
                    _unitOfWork.BeginTransaction();
                    await _unitOfWork.PostImagesRepo.FindByID(post.Id);

                    var postEntity = await _unitOfWork.PostRepo.FindByID(post.Id) ??
                        throw new RecordNotFoundException($"Post ID: {post.Id} Not Found");

                    _unitOfWork.PostRepo.Remove(postEntity);
                    var effectedRows = _unitOfWork.CommitTransaction();
                    _unitOfWork.Dispose();
                    return effectedRows > 0;
                }
                return false;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<List<PostView>> GetAllPosts()
        {
            List<PostView> posts = await _postRepo.GetPosts();

            foreach (PostView postView in posts)
            {
                List<PostImageView> images = await _postImagesRepo.GetPostImagesByPostID(postView.Id);
                postView.PostImages.AddRange(images);
                DateTime createdDate = postView.Created ?? DateTime.Now; 
                postView.CreatedString = createdDate.ToString("MM/dd/yyyy hh:mm tt");
            }

            return posts;

        }

        public async Task<bool> UpdatePost(PostUpdateModel newPost)
        {
            try
            {
                //Update post content
                _logger.LogInformation(message: $"Update Post ID: {newPost.Id} to DB");

                _unitOfWork.BeginTransaction();
                await _unitOfWork.PostRepo.UpdatePost(newPost);

                //Get old images 
                var oldPostImages = await _unitOfWork.PostImagesRepo.GetPostImagesByPostID(newPost.Id);
                //Remove all old images
                if (!oldPostImages.IsNullOrEmpty())
                {
                    _logger.LogInformation(message: $"Remove old all postImages PostID: {newPost.Id} from DB");
                    await _unitOfWork.PostImagesRepo.RemoveImages(oldPostImages);
                }
                //Add new images
                foreach (var image in newPost.postImages)
                {
                    Postimage newImage = new()
                    {
                        Id = Guid.NewGuid(),
                        Imagebase64 = image.ImageBase64,
                        Postid = newPost.Id
                    };
                    _logger.LogInformation(message: $"Insert new postImage ID: {newImage.Id} to DB");
                    _unitOfWork.PostImagesRepo.Add(newImage);
                }
                var effectedRows = _unitOfWork.CommitTransaction();
                _unitOfWork.Dispose();
                return effectedRows > 0;
            }
            catch(Exception ex)
            {
                _logger.LogError(message: $"ERROR: Update Post ID: {newPost.Id} to DB. \n Error: {ex.Message}");
                _unitOfWork.RollbackTransaction();
                _unitOfWork.Dispose();
                throw new Exception($"Update Post {newPost.Id} Failed with error: {ex.Message}");
            }        
        }
    }
}
