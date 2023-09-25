namespace FindingPets.Data.Models.PostResponseModel
{
    public class PostCreateModel
    {
        public string? Description { get; set; }

        public string? Contact { get; set; }

        public List<PostImageCreateModel> PostImages { get; set; } = new List<PostImageCreateModel>();
    }

    public class PostImageCreateModel
    {
        public string ImageBase64 { get; set; } = string.Empty;
    }
}
