using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevHacks.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [Required]
        public string QuestionTitle{ get; set; }
        [Required]
        public string QuestionContent{ get; set; }
        [Required][DataType(DataType.DateTime)]
        public DateTime QuestionDate { get; set; }
    
      
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public IEnumerable<SelectListItem> Categ { get; set; }

    }
}