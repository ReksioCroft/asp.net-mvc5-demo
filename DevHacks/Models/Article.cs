using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevHacks.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }
        [Required]
        public string ArticleName { get; set; }
        [Required]
        public string ArticleContent { get; set; }
        [Required][DataType(DataType.DateTime)]
        public DateTime ArticleDate { get; set; }

        public float Rating { get; set; }
        public int NumVotes { get; set; }
        
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public IEnumerable<SelectListItem> Categ { get; set; }

    }
}