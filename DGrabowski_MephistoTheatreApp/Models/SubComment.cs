using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DGrabowski_MephistoTheatreApp.Models
{
    public class SubComment
    {
        public int SubCommentId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Body { get; set; }
        public bool IsDraft { get; set; }
        public bool IsPublished { get; set; }

        // Navigational properties
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Comment")]
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }

    }
}