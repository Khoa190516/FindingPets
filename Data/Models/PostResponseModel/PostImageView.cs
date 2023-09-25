namespace FindingPets.Data.Models.PostResponseModel
{
    public class PostImageView
    {
        public Guid Id { get; set; }

        public string? ImageBase64 { get; set; }

        public Guid PostId { get; set; }

    }
}
