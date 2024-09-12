using System.ComponentModel.DataAnnotations;

namespace BlogProjectPrac7.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} charactors")]
        public string? Name { get; set; }


        //my child is:
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}
