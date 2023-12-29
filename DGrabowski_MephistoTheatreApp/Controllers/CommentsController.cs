using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DGrabowski_MephistoTheatreApp.Models;

namespace DGrabowski_MephistoTheatreApp.Controllers
{
    public class CommentsController : Controller
    {
        private readonly MephistoTheatreDbContext db = new MephistoTheatreDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments
                .Include(c => c.SubComments)
                .Include(c => c.Post)
                .Include(c => c.User)
                .ToList();

            return View(comments);
        }


        [HttpPost]
        public ActionResult PublishUnpublishComment(int commentId)
        {
            var comment = db.Comments.Find(commentId);

            if (comment != null)
            {
                // Toggle the IsPublished property
                comment.IsPublished = !comment.IsPublished;

                // Update the subcomments' IsPublished property
                foreach (var subComment in comment.SubComments)
                {
                    subComment.IsPublished = comment.IsPublished;
                }

                // Save the changes to the database
                db.SaveChanges();

                // Redirect to the Index action or any other appropriate action
                return RedirectToAction("Index");
            }
            else
            {
                // Comment not found
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult PublishUnpublishSubComment(int subCommentId)
        {
            var subComment = db.SubComments.Find(subCommentId);

            if (subComment != null)
            {
                // Toggle the IsPublished property
                subComment.IsPublished = !subComment.IsPublished;

                // Save the changes to the database
                db.SaveChanges();

                // Return the Index view with the updated data
                return RedirectToAction("Index");
            }
            else
            {
                // SubComment not found
                return HttpNotFound();
            }
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.ParentCommentId = new SelectList(db.Comments, "CommentId", "Body");
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommentId,TimeStamp,Body,IsDraft,IsPublished,ParentCommentId,UserId,PostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParentCommentId = new SelectList(db.Comments, "CommentId", "Body");
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "Title", comment.PostId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentCommentId = new SelectList(db.Comments, "CommentId", "Body");
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "Title", comment.PostId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentId,TimeStamp,Body,IsDraft,IsPublished,ParentCommentId,UserId,PostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentCommentId = new SelectList(db.Comments, "CommentId", "Body");
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "Title", comment.PostId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // POST: Comments/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int commentId)
        {
            Comment comment = db.Comments.Find(commentId);

            if (comment != null)
            {
                // Remove the comment from the database
                db.Comments.Remove(comment);

                // Save the changes to the database
                db.SaveChanges();

                // Redirect to the Index action or any other appropriate action
                return RedirectToAction("Index");
            }
            else
            {
                // Comment not found
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSubComment(int subCommentId)
        {
            SubComment subComment = db.SubComments.Find(subCommentId);

            if (subComment != null)
            {
                // Remove the subcomment from the database
                db.SubComments.Remove(subComment);

                // Save the changes to the database
                db.SaveChanges();

                // Redirect to the Index action or any other appropriate action
                return RedirectToAction("Index");
            }
            else
            {
                // SubComment not found
                return HttpNotFound();
            }
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
