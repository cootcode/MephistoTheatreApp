using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DGrabowski_MephistoTheatreApp.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Body { get; set; }
        public bool IsDraft { get; set; }
        public bool IsPublished { get; set; }
        public int? ParentCommentId { get; set; }


        // navigational properties
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

        [ForeignKey("ParentCommentId")]
        public virtual Comment ParentComment { get; set; }

        public virtual ICollection<SubComment> SubComments { get; set; }
    }
}