using BlogProjectPrac7.Data;
using BlogProjectPrac7.Models;
using BlogProjectPrac7.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogProjectPrac7.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPostToCategoryAsync(int categoryId, int postId)
        {
            try
            {
                if (!await IsPostInCategory(categoryId, postId))
                {
                    Post? post = await _context.Posts.FindAsync(postId);
                    Category? category = await _context.Categories.FindAsync(categoryId);

                    if (category != null && post != null)
                    {
                        category.Posts.Add(post);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoryListAsync()
        {
            List<Category> categories = new List<Category>();

            try
            {
                categories = await _context.Categories.OrderBy(c => c.Name)
                                                      .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return categories;
        }

        public async Task<ICollection<Category>> GetPostCategoriesAsync(int postId)
        {
            try
            {
                Post? post = await _context.Posts.Include(c => c.Categories)
                                                          .FirstOrDefaultAsync(c => c.Id == postId);
                return post!.Categories;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<int>> GetPostCategoryIdsAsync(int postId)
        {
            var post = await _context.Posts.Include(p => p.Categories)
                                           .FirstOrDefaultAsync(p => p.Id == postId);
            List<int> categoryIds = post!.Categories.Select(c => c.Id).ToList();
            return categoryIds;
        }

        public async Task<bool> IsPostInCategory(int categoryId, int postId)
        {
            Post? post = await _context.Posts.FindAsync(postId);

            return await _context.Categories
                                            .Include(c => c.Posts)
                                            .Where(c => c.Id == postId && c.Posts.Contains(post))
                                            .AnyAsync();
        }

        public async Task RemovePostFromCategoryAsync(int categoryId, int postId)
        {
            try
            {
                if (await IsPostInCategory(categoryId, postId))
                {
                    Post post = await _context.Posts.FindAsync(postId);
                    Category category = await _context.Categories.FindAsync(categoryId);

                    if (post != null && category != null)
                    {
                        category.Posts.Remove(post);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<Post> SearchForPosts(string searchString)
        {
            throw new NotImplementedException();
        }
    }
}
