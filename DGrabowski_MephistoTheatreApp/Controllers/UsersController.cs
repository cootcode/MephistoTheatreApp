using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DGrabowski_MephistoTheatreApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace DGrabowski_MephistoTheatreApp.Controllers
{
    public class UsersController : Controller
    {
        private MephistoTheatreDbContext db = new MephistoTheatreDbContext();

        private readonly UserManager<User> _userManager;

        // GET: Users
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Member member = db.Users.OfType<Member>().FirstOrDefault(u => u.Id == id);

            if (member == null)
            {
                return HttpNotFound();
            }

            return View(member);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Street,City,PostCode,RegisteredAt,IsSuspended,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] Member member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(member);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);

                // Optionally, you can redirect to an error page or display a user-friendly message
                ModelState.AddModelError(string.Empty, "An error occurred while saving changes.");
                return View(member);
            }
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Retrieve the User from your data source based on the id
            User user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Check if the user being deleted is in the "Admin" role
            var userManager = new UserManager<User>(new UserStore<User>(db));
            var userRoles = userManager.GetRoles(user.Id);

            if (userRoles.Contains("Admin"))
            {
                // If the user being deleted is an admin, set a message and redirect
                TempData["ErrorMessage"] = "You cannot delete an admin user.";
                return RedirectToAction("Index"); // Redirect to the user index or any other page
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);

            // Delete associated comments
            var commentsToDelete = db.Comments.Where(c => c.UserId == id);
            db.Comments.RemoveRange(commentsToDelete);

            // Delete associated posts where the user is a staff member
            var postsToDelete = db.Posts.Where(p => p.StaffId == id);
            db.Posts.RemoveRange(postsToDelete);

            // Delete the user
            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult PromoteToStaff(string id)
        {

            var user = db.Users.Find(id);

            if (user != null && user.CurrentRole == "Member")
            {
                // Promote the user to "Staff"
                user.CurrentRole = "Staff";

                db.SaveChanges();
            }

            // Redirect or return a view accordingly
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult DemoteToMember(string id)
        {

            var user = db.Users.Find(id);

            if (user != null && user.CurrentRole == "Staff")
            {
                // Demote the user to "Member"
                user.CurrentRole = "Member";

                db.SaveChanges();
            }

            // Redirect or return a view accordingly
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult SuspendUser(string id)
        {
            var user = db.Users.Find(id);

            if (user != null)
            {
                // Toggle the IsSuspended attribute
                user.IsSuspended = !user.IsSuspended;

                db.SaveChanges();
            }

            // Redirect or return a view accordingly
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult ActivateUser(string id)
        {
            var user = db.Users.Find(id);

            if (user != null)
            {
                // Toggle the IsSuspended attribute
                user.IsSuspended = !user.IsSuspended;

                db.SaveChanges();
            }

            // Redirect or return a view accordingly
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
