using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DevHacks.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Article> Articles { get; set; }

    }
}