using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DGrabowski_MephistoTheatreApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath {  get; set; }
        [NotMapped]
        [Display(Name = "Upload Image")]
        public HttpPostedFileBase ImageFile { get; set; }

        // navigational properties

        public List<Post> Posts { get; set; }
    }
}