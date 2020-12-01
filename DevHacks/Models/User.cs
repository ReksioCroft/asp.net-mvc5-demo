using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DevHacks.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Phone{ get; set; }
        [Required]
        public string Email{ get; set; }
        public string Description{ get; set; }
        public string Profesie{ get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Article> Articles { get; set; }

    }
}