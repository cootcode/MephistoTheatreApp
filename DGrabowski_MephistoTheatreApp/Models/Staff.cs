using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DGrabowski_MephistoTheatreApp.Models
{
    public class Staff : User
    {
        public DateTime EmploymentStartDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }

        // navigational properties

        public List<Post> Posts { get; set; }
    }
}