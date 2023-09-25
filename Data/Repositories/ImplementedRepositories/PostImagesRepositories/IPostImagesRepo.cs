using FindingPets.Data.Entities;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.Repositories.BaseRepositories;

namespace FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories
{
    public interface IPostImagesRepo : IBaseRepo<PostImage>
    {
        public Task<List<PostImageView>> GetPostImagesByPostID(Guid postId);  
        public Task<bool> RemoveImages(List<PostImageView> postImages);
    }
}
