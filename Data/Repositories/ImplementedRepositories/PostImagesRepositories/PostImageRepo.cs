using Microsoft.EntityFrameworkCore;
using FindingPets.Data.Entities;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.Repositories.BaseRepositories;

namespace FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories
{
    public class PostImageRepo : BaseRepo<PostImage>, IPostImagesRepo
    {
        public PostImageRepo(FindingPetsDbContext context) : base(context) 
        {
        }

        public async Task<List<PostImageView>> GetPostImagesByPostID(Guid postId)
        {
            var query = from p in context.Posts
                        join i in context.PostImages
                        on p.Id equals i.PostId
                        where p.Id == postId
                        select new {p, i};

            List<PostImageView> images = await query.Select(x => new PostImageView()
            {
                Id = x.i.Id,
                ImageBase64 = x.i.ImageBase64,
                PostId = x.p.Id
            }).ToListAsync();

            return images;
        }

        public async Task<bool> RemoveImages(List<PostImageView> postImages)
        {
            //Remove old images
            foreach(var image in postImages)
            {
                var imgEntity = await context.PostImages.FindAsync(image.Id);
                if (imgEntity != null)
                {
                    context.PostImages.Remove(imgEntity);
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
