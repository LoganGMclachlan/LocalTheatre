using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Local_Theatre.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Net;

namespace Local_Theatre.Controllers
{
    public class CommentController : Controller
    {
        // creates a DBcontext instance
        private ApplicationDbContext context = new ApplicationDbContext();


        [HttpGet]
        [Authorize(Roles = "Member")]
        public ActionResult NewComment(int postID)
        {
            Comment newComment = new Comment();
            newComment.PostID = postID;
            // goes to view
            return View(newComment);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public ActionResult NewComment(Comment model)
        {
            // checks if model is valid
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();//gets the id of the current logged in user
                model.User = context.Users.Find(userId);
                model.Id = userId;//assigning the first foreign key

                // sets other info on post
                model.IsApproved = false;
                model.DatePosted = DateTime.Now;

                model.Post = context.Posts.Find(model.PostID);

                context.Comments.Add(model);// adds new post to db
                context.SaveChanges();// saves db changes

                // returns to blog
                return RedirectToAction("Blog", "Post");
            }

            // sends back with model if something goes wrong
            return View(model);
        }



        [ChildActionOnly]
        public ActionResult ListComments(int id)// controller for partial view
        {
            // create comment list
            List<Comment> comments = new List<Comment>();

            // loops through all comments
            foreach (Comment comment in context.Comments.ToList())
            {
                // checks if comment post id matches parameter 
                if (comment.PostID == id)
                {
                    comment.User = context.Users.Find(comment.Id);
                    // adds comment to list if it is for post
                    comments.Add(comment);
                }
            }

            // returns comments for post
            return PartialView(comments);
        }



        [HttpGet]
        [Authorize(Roles = "Member,Admin")]
        public ActionResult Edit(int id, int postID)
        {
            // checks if id given exists
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // finds the comment from id
            var comment = context.Comments.Find(id);

            // checks if a comment was found
            if (comment == null)
            {
                return HttpNotFound();
            }

            // gives comment to view
            return View(comment);
        }

        [HttpPost]
        [Authorize(Roles = "Member,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentID,PostID,Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.DatePosted = DateTime.Now;// updates dateposted
                comment.Id = User.Identity.GetUserId();// sets foriegn key
                comment.IsApproved = false;

                // updates db
                context.Entry(comment).State = EntityState.Modified;
                // saves changes to db
                context.SaveChanges();

                TempData["AlertMessage"] = "Your comment was updated";// sets message to display in blog

                // redirects to blog page
                return RedirectToAction("Blog", "Post");
            }
            // if model isnt valid, return to edit page
            return View(comment);
        }



        [HttpGet]
        [Authorize(Roles = "Member,Admin")]
        public ActionResult Delete(int id)
        {
            // checks if id given exists
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // finds the comment from id
            var comment = context.Comments.Find(id);

            // checks if a comment was found
            if (comment == null)
            {
                return HttpNotFound();
            }


            // gives comment to view
            return View(comment);
        }
        
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Member,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var comment = context.Comments.Find(id);// gets comment being deleted

                context.Comments.Remove(comment);// deletes comment from context

                context.SaveChanges();// updates db

                TempData["AlertMessage"] = "Your comment was Deleted";// sets message to display in blog

                return RedirectToAction("Blog", "Post");// returns to blog page
            }
            catch
            {
                return View();
            }
        }

        
        [Authorize(Roles = "Admin")]
        // sets the approval of a comment to false
        public ActionResult DisapproveComment(int id)
        {
            // gets post
            Comment comment = context.Comments.Find(id);
            comment.IsApproved = false;// sets isapproved

            context.SaveChanges();// saves changes

            // returns to blog
            return RedirectToAction("Blog", "Post");
        }

        [Authorize(Roles = "Admin")]
        // sets the approval of a comment to true
        public ActionResult ApproveComment(int id)
        {
            // gets post
            Comment comment = context.Comments.Find(id);
            comment.IsApproved = true;// sets isapproved

            context.SaveChanges();// saves changes

            // returns to blog
            return RedirectToAction("Blog", "Post");
        }
    }
}