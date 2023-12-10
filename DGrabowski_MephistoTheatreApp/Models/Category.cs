using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DGrabowski_MephistoTheatreApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        // navigational properties

        public List<Post> Posts { get; set; }
    }
}