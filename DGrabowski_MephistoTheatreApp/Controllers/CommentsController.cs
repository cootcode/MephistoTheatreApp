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
        private MephistoTheatreDbContext db = new MephistoTheatreDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments
                .Include(c => c.SubComments)
                .Include(c => c.Post)
                .Include(c => c.User);
            return View(comments.ToList());
        }

        [HttpPost]
        public ActionResult PublishUnpublishComment(int commentId, string commentAction)
        {
            var comment = db.Comments.Find(commentId);

            if (comment != null)
            {
                // Toggle the IsPublished property
                comment.IsPublished = !comment.IsPublished;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // Comment not found
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult PublishUnpublishSubComment(int subCommentId, string commentAction)
        {
            var subComment = db.SubComments.Find(subCommentId);

            if (subComment != null)
            {
                // Toggle the IsPublished property for the sub-comment
                subComment.IsPublished = !subComment.IsPublished;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // Sub-comment not found
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

            ViewBag.ParentCommentId = new SelectList(db.Comments, "CommentId", "Body", comment.ParentCommentId);
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
            ViewBag.ParentCommentId = new SelectList(db.Comments, "CommentId", "Body", comment.ParentCommentId);
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
            ViewBag.ParentCommentId = new SelectList(db.Comments, "CommentId", "Body", comment.ParentCommentId);
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "Title", comment.PostId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
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
