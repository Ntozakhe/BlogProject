using BlogProjectPrac7.Data;
using BlogProjectPrac7.Enums;
using BlogProjectPrac7.Models;
using BlogProjectPrac7.Services;
using BlogProjectPrac7.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BlogProjectPrac7.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;
        private readonly IBlogService _blogService;

        public PostsController(ApplicationDbContext context, ISlugService slugService, IImageService imageService, UserManager<BlogUser> userManager, BlogSearchService blogSearchService, IBlogService blogService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
            _blogService = blogService;
        }

        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;
            var pageNumber = page ?? 1;
            var pageSize = 5;

            var posts = _blogSearchService.Search(searchTerm);
            ViewData["HeaderImage"] = Url.Content("~/img/home-bg.jpg");
            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }



        // GET: Posts
        [Authorize(Roles = nameof(BlogRole.Administrator))]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.BlogUser);
            ViewData["HeaderImage"] = Url.Content("~/img/home-bg.jpg");
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            ViewData["Title"] = "Post Details Page";

            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }

            var dataVM = new Models.ViewModel.PostDetailViewModel()
            {
                Post = post,
                Tags = _context.Tags
                                 .Select(t => t.Text.ToLower())
                                 .Distinct().ToList()
            };

            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData!, post.ContentType!);
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;
            return View(dataVM);
        }

        // GET: Posts/Create
        [Authorize(Roles = nameof(BlogRole.Administrator))]
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryList"] = new MultiSelectList(await _blogService.GetCategoryListAsync(), "Id", "Name");
            ViewData["HeaderImage"] = Url.Content("~/img/home-bg.jpg");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = nameof(BlogRole.Administrator))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Abstract,Content,ReadyStatus,Image")] Post post, List<int> CategoryList, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                post.ImageData = await _imageService.EncodeImageAsync(post.Image!);
                post.ContentType = _imageService.ContentType(post.Image!);



                //Create the slug and determine if its unque.
                var slug = _slugService.UrlFriendly(post.Title!);
                if (!_slugService.isUnique(slug))
                {
                    //Add a Model state error and return the user back to the create view.
                    ModelState.AddModelError("Title", "The Title you provided cannot be used as it results in a duplicate!");
                    ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();

                foreach (int categoryId in CategoryList)
                {
                    await _blogService.AddPostToCategoryAsync(categoryId, post.Id);
                }

                //How do i loop over the incoming list of strings
                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = nameof(BlogRole.Administrator))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData!, post.ContentType!);
            ViewData["MainText"] = "Edit in progress...";
            ViewData["CategoryList"] = new MultiSelectList(await _blogService.GetCategoryListAsync(), "Id", "Name", await _blogService.GetPostCategoryIdsAsync(post.Id));
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = nameof(BlogRole.Administrator))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Created,Title,Abstract,Content,ReadyStatus")] Post post, IFormFile? newImage, List<string> tagValues, List<int> CategoryList)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);
                    originalPost!.Updated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    originalPost.Title = post.Title;
                    originalPost.Abstract = post.Abstract;
                    originalPost.Content = post.Content;
                    originalPost.ReadyStatus = post.ReadyStatus;

                    var newSlug = _slugService.UrlFriendly(post.Title!);
                    if (newSlug != originalPost.Slug)
                    {
                        if (_slugService.isUnique(newSlug))
                        {
                            originalPost.Title = post.Title;
                            originalPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "The Title you provided cannot be used as it results in a duplicate!");
                            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
                            return View(post);
                        }
                    }


                    if (newImage != null)
                    {
                        originalPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        originalPost.ContentType = _imageService.ContentType(newImage);
                    }

                    //Remove all Tags previously associated with this post
                    _context.Tags.RemoveRange(originalPost.Tags);

                    //Add the new tags from the Edit form
                    foreach (var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = originalPost.BlogUserId,
                            Text = tagText
                        });
                    }

                    await _context.SaveChangesAsync();
                    //remove the previously selected category
                    List<Category> oldCategories = (await _blogService.GetPostCategoriesAsync(post.Id)).ToList();
                    foreach (var category in oldCategories)
                    {
                        await _blogService.RemovePostFromCategoryAsync(category.Id, post.Id);
                    }
                    //readd the category
                    foreach (int categoryId in CategoryList)
                    {
                        await _blogService.AddPostToCategoryAsync(categoryId, post.Id);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }


        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
