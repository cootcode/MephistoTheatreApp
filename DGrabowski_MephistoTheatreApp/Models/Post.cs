using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DGrabowski_MephistoTheatreApp.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Body { get; set; }
        public bool IsPublished { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? LastEditAt { get; set; }
        public bool IsDraft { get; set; }

        // navigational properties
        [ForeignKey("Staff")]
        public string StaffId { get; set; }
        public Staff Staff { get; set; }

        public List<Comment> Comments { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}