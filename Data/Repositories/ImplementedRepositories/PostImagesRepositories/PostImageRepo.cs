using Microsoft.EntityFrameworkCore;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.Repositories.BaseRepositories;
using FindingPets.Data.PostgreEntities;

namespace FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories
{
    public class PostImageRepo : BaseRepo<Postimage>, IPostImagesRepo
    {
        public PostImageRepo(D8hclhg7mplh6sContext context) : base(context) 
        {
        }

        public async Task<List<PostImageView>> GetPostImagesByPostID(Guid postId)
        {
            var query = from p in context.Posts
                        join i in context.Postimages
                        on p.Id equals i.Postid
                        where p.Id == postId
                        select new {p, i};

            List<PostImageView> images = await query.Select(x => new PostImageView()
            {
                Id = x.i.Id,
                ImageBase64 = x.i.Imagebase64,
                PostId = x.p.Id
            }).ToListAsync();

            return images;
        }

        public async Task<bool> RemoveImages(List<PostImageView> postImages)
        {
            //Remove old images
            foreach(var image in postImages)
            {
                var imgEntity = await context.Postimages.FindAsync(image.Id);
                if (imgEntity != null)
                {
                    context.Postimages.Remove(imgEntity);
                }
                else
                {
                    throw new Exception($"Image {image.Id} not found");
                }
            }
            await Update();
            return true;
        }
    }
}
