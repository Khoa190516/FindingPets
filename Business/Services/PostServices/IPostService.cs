using FindingPets.Data.Models.PostResponseModel;

namespace FindingPets.Business.Services.PostServices
{
    public interface IPostService
    {
        public Task<List<PostView>> GetAllPosts();
        public Task<bool> CreatePost(PostCreateModel newPost, Guid ownerId);
        public Task<bool> UpdatePost(PostUpdateModel newPost);
        public Task<bool> ChangePostStatus(Guid postId, Guid userId);
    }
}
