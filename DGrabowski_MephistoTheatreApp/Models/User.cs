using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace DGrabowski_MephistoTheatreApp.Models
{
    // You can add profile data for the user by adding more properties to your User class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public abstract class User : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }

        [Display(Name = "Post Code")]
        public string PostCode { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public bool IsSuspended { get; set; }

        //needing the UserManager to get the user's current role
        private UserManager userManager;


        [NotMapped]
        public string CurrentRole
        {
            get
            {
                var userManager = HttpContext.Current.GetOwinContext().GetUserManager<UserManager>();

                // Check if the user has any roles before calling Single
                var roles = userManager.GetRoles(Id);
                if (roles.Any())
                {
                    // If the user has roles, return the first role (or adjust as needed)
                    return roles.Single();
                }

                // Handle the case where the user has no roles
                return "Member";
            }
            set 
            {
                var userManager = HttpContext.Current.GetOwinContext().GetUserManager<UserManager>();
                userManager.RemoveFromRoles(Id, userManager.GetRoles(Id).ToArray());
                userManager.AddToRole(Id, value);
            }
        }

        //Navigation properties
        public List<Comment> Comments { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}