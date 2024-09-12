using BlogProjectPrac7.Models;

namespace BlogProjectPrac7.Services.Interfaces
{
    public interface IBlogService
    {
        Task AddPostToCategoryAsync(int categoryId, int postId);
        //when we save a post, we want to add a category it belongs to.
        Task<bool> IsPostInCategory(int categoryId, int postId);
        //we'll use this one during the edit phase, when we present the list and
        Task<IEnumerable<Category>> GetCategoryListAsync();
        //This will return a list of Categories that will bind to the dropdown
        Task<ICollection<int>> GetPostCategoryIdsAsync(int postId);
        //for the given post, return all the category ids
        Task<ICollection<Category>> GetPostCategoriesAsync(int postId);
        //This will return a collection of categories
        Task RemovePostFromCategoryAsync(int categoryId, int postId);
        IEnumerable<Post> SearchForPosts(string searchString);
    }
}
