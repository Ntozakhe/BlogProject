using System.ComponentModel.DataAnnotations;

namespace BlogProjectPrac7.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string? BlogUserId { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long", MinimumLength = 2)]
        public string? Text { get; set; }

        //Navigation Properits
        //My parents are:
        public virtual Post? Post { get; set; }
        public virtual BlogUser? BlogUser { get; set; }
    }
}
