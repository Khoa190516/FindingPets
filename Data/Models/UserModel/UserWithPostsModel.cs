using FindingPets.Data.Models.PostResponseModel;

namespace FindingPets.Data.Models.UserModel
{
    public class UserWithPostsModel : ProfileModel
    {
        public List<PostView> Posts { get; set; } = new();
    }
}
