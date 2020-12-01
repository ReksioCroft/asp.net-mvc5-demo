using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DevHacks.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string CommentContent { get; set; }
        [Required][DataType(DataType.DateTime)]
        public DateTime CommentDate { get; set; }
       
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }


    }
}