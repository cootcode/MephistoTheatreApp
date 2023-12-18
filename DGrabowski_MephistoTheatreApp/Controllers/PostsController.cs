using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DGrabowski_MephistoTheatreApp.Models;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace DGrabowski_MephistoTheatreApp.Controllers
{
    public class PostsController : Controller
    {
        private MephistoTheatreDbContext db = new MephistoTheatreDbContext();

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
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                posts = posts.Where(p => p.Category.CategoryName == category);
            }

            switch (sortBy)
            {
                case "Date":
                    posts = posts.OrderByDescending(p => p.CreatedAt);
                    break;
                case "Author":
                    posts = posts.OrderBy(p => p.Staff.FirstName).ThenBy(p => p.Staff.LastName);
                    break;
                case "Popularity":
                    // Implement popularity sorting logic
                    break;
                default:
                    posts = posts.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            var pagedPosts = posts.ToPagedList(page ?? 1, pageSize);


            return View(pagedPosts);
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
                .SingleOrDefault(p => p.PostId == id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.StaffId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,Title,CreatedAt,Body,IsPublished,IsArchived,LastEditAt,IsDraft,StaffId,CategoryId")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
            ViewBag.StaffId = new SelectList(db.Users, "Id", "FirstName", post.StaffId);
            return View(post);
        }

        // GET: Posts/Edit/5
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
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,Title,CreatedAt,Body,IsPublished,IsArchived,LastEditAt,IsDraft,StaffId,CategoryId")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
            ViewBag.StaffId = new SelectList(db.Users, "Id", "FirstName", post.StaffId);
            return View(post);
        }

        // GET: Posts/Delete/5
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
