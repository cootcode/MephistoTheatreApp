using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using DGrabowski_MephistoTheatreApp.Models;


namespace DGrabowski_MephistoTheatreApp.Models
{
    public class MephistoTheatreDbContext : IdentityDbContext<User>
    {

        public MephistoTheatreDbContext()
            : base("MephistoTheatreConnectionString", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MephistoTheatreDbInitializer());
        }

        public static MephistoTheatreDbContext Create()
        {
            return new MephistoTheatreDbContext();
        }

        // DbSets for POCO classes
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
    }
}