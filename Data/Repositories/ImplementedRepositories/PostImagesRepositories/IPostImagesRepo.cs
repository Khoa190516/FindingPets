using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.BaseRepositories;

namespace FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories
{
    public interface IPostImagesRepo : IBaseRepo<Postimage>
    {
        public Task<List<PostImageView>> GetPostImagesByPostID(Guid postId);  
        public Task RemoveImages(List<PostImageView> postImages);
    }
}
