using FindingPets.Data.Entities;
using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace FindingPets.Data.Repositories.ImplementedRepositories.PostRepositories
{
    public class PostRepo : BaseRepo<Post>, IPostRepo
    {
        public PostRepo(FindingPetsDbContext context) : base(context)
        {
        }

        public async Task<List<PostView>> GetPosts()
        {
            var query = from p in context.Posts
                        select new { p };

            List<PostView> postsView = await query.Select(x => new PostView()
            {
                Id = x.p.Id,
                Created = x.p.Created,
                Contact = x.p.Contact,
                Description = x.p.Description,
                IsBanned = x.p.IsBanned,
                IsClosed = x.p.IsClosed,
                OwnerId = x.p.OwnerId,
            }).ToListAsync();

            return postsView;
        }

        public async Task<List<PostView>> GetPostsByOwner(Guid ownerId)
        {
            var query = from p in context.Posts
                        where p.OwnerId == ownerId
                        select new { p };

            List<PostView> postsView = await query.Select(x => new PostView()
            {
                Id = x.p.Id,
                Created = x.p.Created,
                Contact = x.p.Contact,
                Description = x.p.Description,
                IsBanned = x.p.IsBanned,
                IsClosed = x.p.IsClosed,
                OwnerId = x.p.OwnerId,
            }).ToListAsync();

            return postsView;
        }

        public async Task<bool> UpdatePost(PostUpdateModel post)
        {
            var postEntity = await context.Posts.FindAsync(post.Id)
                ?? throw new Exception($"Post {post.Id} not found");

            postEntity.Contact = post.Contact;
            postEntity.Description = post.Description;
            await Update();
            return true;
        }

        public async Task<bool> UpdatePostStatus(Guid postId, bool isBanRequest)
        {
            var postEntity = await context.Posts.FindAsync(postId);
            if (postEntity == null) throw new Exception($"Post ID: {postId} not found in DB");

            switch (isBanRequest)
            {
                case true: //Change isBanned Status
                    {
                        if (postEntity.IsBanned == null || postEntity.IsBanned == false)
                        {
                            postEntity.IsBanned = true;
                        }
                        else
                        {
                            postEntity.IsBanned = false;
                        }
                        break;
                    }
                case false:  // Change is Closed Status
                    {
                        if (postEntity.IsClosed == null || postEntity.IsClosed == false)
                        {
                            postEntity.IsClosed = true;
                        }
                        else
                        {
                            postEntity.IsClosed = false;
                        }
                        break;
                    }
            }
            await Update();
            return true;

        }
    }
}
