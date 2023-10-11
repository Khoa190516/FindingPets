using FindingPets.Data.Entities;

namespace FindingPets.Data.Models.PostResponseModel
{
    public class PostView
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime? Created { get; set; }

        public string? Contact { get; set; }

        public bool? IsBanned { get; set; }

        public bool? IsClosed { get; set; }

        public Guid OwnerId { get; set; }

        public string? CreatedString { get; set; }

        public List<PostImageView> PostImages { get; set; } = new List<PostImageView>();
    }


    public class PostsView
    {
        public List<PostView> PostsListView { get; set; } = new List<PostView>();
    }
}
