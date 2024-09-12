using BlogProjectPrac7.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProjectPrac7.Models
{
    public class Post
    {
        public int Id { get; set; }



        public string? BlogUserId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long", MinimumLength = 2)]
        public string? Abstract { get; set; }

        [Required]
        public string? Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        public string? Slug { get; set; }


        public ReadyStatus ReadyStatus { get; set; }

        [Display(Name = "Post Image")]
        public byte[]? ImageData { get; set; }

        public string? ContentType { get; set; }

        [NotMapped]
        [Display(Name = "Post Image")]
        public IFormFile? Image { get; set; }


        //Navigational properties

        public virtual BlogUser? BlogUser { get; set; }
        //my child is:
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    }
}
