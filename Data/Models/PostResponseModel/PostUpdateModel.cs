namespace FindingPets.Data.Models.PostResponseModel
{
    public class PostUpdateModel
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Contact { get; set; }

        public List<PostImageUpdateModel> postImages { get; set; } = new();
    }

    public class PostImageUpdateModel
    {
        public Guid Id { get; set; }
        public string? ImageBase64 { get; set;}
    }

    public class PostUpdateStatusModel
    {
        public Guid Id { get; set; }
    }
}
