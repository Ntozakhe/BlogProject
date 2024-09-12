namespace BlogProjectPrac7.Models.ViewModel
{
    public class PostDetailViewModel
    {
        public Post? Post { get; set; }
        public List<String> Tags { get; set; } = new List<String>();
    }
}
