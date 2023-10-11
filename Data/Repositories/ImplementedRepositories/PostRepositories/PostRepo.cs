using FindingPets.Data.Models.PostResponseModel;
using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;

namespace FindingPets.Data.Repositories.ImplementedRepositories.PostRepositories
{
    public class PostRepo : BaseRepo<Post>, IPostRepo
    {
        public PostRepo(D8hclhg7mplh6sContext context) : base(context)
        {
        }

        public async Task<List<PostView>> GetPosts()
        {
            var query = from p in context.Posts
                        select new { p };

            List<PostView> postsView = await query.Select(x => new PostView()
            {
                Id = x.p.Id,
                Title = x.p.Title,
                Created = x.p.Created,
                Contact = x.p.Contact,
                Description = x.p.Description,
                IsBanned = x.p.Isbanned,
                IsClosed = x.p.Isclosed,
                OwnerId = x.p.Ownerid,
            }).ToListAsync();

            return postsView;
        }

        public async Task<List<PostView>> GetPostsByOwner(Guid ownerId)
        {
            var query = from p in context.Posts
                        where p.Ownerid == ownerId
                        select new { p };

            List<PostView> postsView = await query.Select(x => new PostView()
            {
                Id = x.p.Id,
                Created = x.p.Created,
                Contact = x.p.Contact,
                Description = x.p.Description,
                IsBanned = x.p.Isbanned,
                IsClosed = x.p.Isclosed,
                OwnerId = x.p.Ownerid,
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
                        if (postEntity.Isbanned == null || postEntity.Isbanned == false)
                        {
                            postEntity.Isbanned = true;
                        }
                        else
                        {
                            postEntity.Isbanned = false;
                        }
                        break;
                    }
                case false:  // Change is Closed Status
                    {
                        if (postEntity.Isclosed == null || postEntity.Isclosed == false)
                        {
                            postEntity.Isclosed = true;
                        }
                        else
                        {
                            postEntity.Isclosed = false;
                        }
                        break;
                    }
            }
            await Update();
            return true;

        }
    }
}
