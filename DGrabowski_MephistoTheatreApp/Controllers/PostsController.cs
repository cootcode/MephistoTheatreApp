using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DGrabowski_MephistoTheatreApp.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using PagedList;

namespace DGrabowski_MephistoTheatreApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly MephistoTheatreDbContext db = new MephistoTheatreDbContext();

        public PostsController()
        {
        }

        // GET: Posts
        public ActionResult Index(int? page, string category, string sortBy)
        {
            int pageSize = 4;
            ViewBag.Category = category;
            ViewBag.SortBy = sortBy;

            var posts = db.Posts
                .Include(p => p.Category)
                .Include(p => p.Staff)
                .Include(p => p.Comments)
                .ToList();  // Materialize the query to avoid potential EF issues

            if (!string.IsNullOrEmpty(category))
            {
                posts = posts.Where(p => p.Category.CategoryName == category).ToList();
            }

            switch (sortBy)
            {
                case "Date":
                    posts = posts.OrderByDescending(p => p.CreatedAt).ToList();
                    break;
                case "Author":
                    posts = posts.OrderBy(p => p.Staff.FirstName).ThenBy(p => p.Staff.LastName).ToList();
                    break;
                case "Popularity":
                    posts = posts.OrderByDescending(p => p.CommentsCount).ToList();
                    break;
                default:
                    posts = posts.OrderByDescending(p => p.CreatedAt).ToList();
                    break;
            }

            var pagedPosts = posts.ToPagedList(page ?? 1, pageSize);

            return View(pagedPosts);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult ListOfAllPosts()
        {
            var allPosts = db.Posts
                .Include(p => p.Category)
                .Include(p => p.Staff)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(allPosts);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult DetailsOfPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Find(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        public ActionResult GetComments(int postId)
        {
            var comments = db.Comments
                .Include(c => c.User)
                .Include(c => c.SubComments)
                .Where(c => c.PostId == postId).ToList()
                .ToList();

            return PartialView("_Comments", comments);
        }

        [HttpPost]
        [Authorize] // Restrict to logged-in users
        public ActionResult SubmitComment(string commentText, int postId, int? parentCommentId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null || user.IsSuspended)
            {
                return Json(new { success = false, message = user == null ? "User not found." : "Your account is suspended. You cannot submit comments." });
            }

            var newComment = new Comment
            {
                TimeStamp = DateTime.Now,
                Body = commentText,
                IsDraft = false,
                IsPublished = false,
                UserId = userId,
                PostId = postId,
                ParentCommentId = parentCommentId
            };

            db.Comments.Add(newComment);
            db.SaveChanges();

            var comments = db.Comments
                .Include(c=> c.User)
                .Include(c=> c.SubComments)
                .Where(c => c.PostId == postId).ToList();
            var commentsHtml = RenderPartialViewToString("_Comments", comments);

            return Json(new { success = true, html = commentsHtml, message = "Comment submitted successfully!" });
        }

        [HttpPost]
        [Authorize] // Restrict to logged-in users
        public ActionResult SubmitReply(string replyText, int commentId, int postId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(u => u.Id == userId);
                var existingComment = db.Comments.Find(commentId);

                if (user == null || user.IsSuspended)
                {
                    return Json(new { success = false, message = user == null ? "User not found." : "Your account is suspended. You cannot submit comments." });
                }

                var newSubComment = new SubComment
                {
                    TimeStamp = DateTime.Now,
                    Body = replyText,
                    IsDraft = false,
                    IsPublished = false,
                    UserId = userId,
                    CommentId = commentId,
                    Comment = existingComment,
                };

                db.SubComments.Add(newSubComment);
                db.SaveChanges();

                var comments = db.Comments
                    .Include(c => c.User)
                    .Include(c => c.SubComments)
                    .Where(c => c.PostId == postId)
                    .ToList();

                var commentsHtml = RenderPartialViewToString("_Comments", comments);
                return Json(new { success = true, html = commentsHtml, message = "Comment submitted successfully!"  });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Error submitting reply." });
            }
        }


        // Helper method to render a partial view to a string
        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            }

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        // GET: Posts/Details/5
        [Route("Posts/Details/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Eagerly load related entities (Category, Staff)
            Post post = db.Posts
                .Include(p => p.Category)
                .Include(p => p.Staff)
                .Include(p => p.Comments)
                .Include(p => p.Comments.Select(c => c.SubComments))
                .SingleOrDefault(p => p.PostId == id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }
        [Authorize(Roles = "Staff,Admin")]
        // GET: Posts/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.StaffId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Admin")]
        public ActionResult Create([Bind(Include = "PostId,Title,CreatedAt,Body,IsPublished,IsArchived,LastEditAt,IsDraft,StaffId,CategoryId")] Post post)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userId = User.Identity.GetUserId();

                    post.StaffId = userId;

                    post.CreatedAt = DateTime.Now;
                    post.IsArchived = false;
                    post.IsDraft = false;

                    db.Posts.Add(post);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details or inspect them during debugging
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    // Log or inspect inner exception details
                    innerException = innerException.InnerException;
                }


                // handle unique constraint violations
                if (ex.InnerException is System.Data.SqlClient.SqlException sqlException &&
                    (sqlException.Number == 2601 || sqlException.Number == 2627))
                {
                    // Handle unique constraint violation
                    ModelState.AddModelError("Title", "A post with this title already exists.");
                }
                else
                {
                    // Rethrow the exception if it's not the type you are handling
                    throw;
                }
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
            ViewBag.StaffId = new SelectList(db.Users, "Id", "FirstName", post.StaffId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
            ViewBag.StaffId = new SelectList(db.Users, "Id", "FirstName", post.StaffId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,Title,CreatedAt,Body,IsPublished,IsArchived,LastEditAt,IsDraft,StaffId,CategoryId")] Post post)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Retrieve the current post from the database
                    var existingPost = db.Posts.Find(post.PostId);

                    if (existingPost == null)
                    {
                        return HttpNotFound();
                    }

                        // Update the properties of the existing post with the new values
                        existingPost.Title = post.Title;
                        existingPost.Body = post.Body;
                        existingPost.IsPublished = post.IsPublished;
                        existingPost.IsArchived = post.IsArchived;
                        existingPost.IsDraft = post.IsDraft;
                        existingPost.LastEditAt = DateTime.Now; // Update the LastEditAt timestamp

                        // Set the EntityState to Modified and save changes
                        db.Entry(existingPost).State = EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Index");
                }

                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
                ViewBag.StaffId = new SelectList(db.Users, "Id", "FirstName", post.StaffId);
                return View(post);
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or display an error message
                ModelState.AddModelError(string.Empty, "An error occurred while saving the changes.");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
            ViewBag.StaffId = new SelectList(db.Users, "Id", "FirstName", post.StaffId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Staff,Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Staff,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Search(SearchViewModel model)
        {
            var searchTerm = model.SearchTerm.ToLower(); // Convert to lowercase for case-insensitive search

            var results = db.Posts
                .Where(post =>
                    post.Title.ToLower().Contains(searchTerm) ||
                    post.Body.ToLower().Contains(searchTerm) ||
                    post.Category.CategoryName.ToLower().Contains(searchTerm)
                )
                .ToList();

            return View(results);
        }

        [HttpGet]
        public ActionResult FilterByCategory(string category, int? page)
        {
            int pageSize = 1000;

            var posts = db.Posts
                .Include(p => p.Category)
                .Include(p => p.Staff)
                .Include(p => p.Comments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                posts = posts.Where(p => p.Category.CategoryName == category)
                             .OrderByDescending(p => p.CreatedAt);
            }
            else
            {
                posts = posts.OrderByDescending(p => p.CreatedAt);
            }

            var pagedPosts = posts.ToPagedList(page ?? 1, pageSize);

            return PartialView("_FilteredPostsPartial", pagedPosts);
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

    public static class PostsExtensions
    {
        public static Post GetRandomPost(this IPagedList<Post> pagedPosts)
        {
            if (pagedPosts.Any())
            {
                return pagedPosts.OrderBy(p => Guid.NewGuid()).FirstOrDefault();
            }

            return null;
        }
    }
}
