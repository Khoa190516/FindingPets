using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.BaseRepositories;

namespace FindingPets.Data.Repositories.ImplementedRepositories.PostRepositories
{
    public interface IPostRepo : IBaseRepo<Post>
    {
        public Task<List<PostView>> GetPosts();
        public Task<List<PostView>> GetPostsByOwner(Guid ownerId);
        public Task<bool> UpdatePost(PostUpdateModel post);
        public Task<bool> UpdatePostStatus(Guid postId, bool isBannedRequest);
    }
}
