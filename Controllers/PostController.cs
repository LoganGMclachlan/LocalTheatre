
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
    public class PostController : Controller
    {
        // creates a DBcontext instance
        private ApplicationDbContext context = new ApplicationDbContext();


        public ActionResult Blog()
        {
            // sends list of all posts to blog
            return View(context.Posts.Include(p => p.User).ToList());
        }


        public ActionResult FindInCatergory(string category)
        {
            // creates empty list for posts
            List<Post> posts = new List<Post>();

            // gets post only in given category
            // loops through posts in context
            foreach (Post post in context.Posts.Include(p => p.User).ToList())
            {
                // check if post category matches category being searched for and that the post is approved
                if (post.Category.ToString().Equals(category) && post.IsApproved)
                {
                    posts.Add(post);// adds post to list
                }
            }

            // alert message if there are no posts in the category
            if (posts.Count < 1) { TempData["AlertMessage"] = "There are no posts in " + category + " category."; }

            // returns posts to blog page
            return View("Blog", posts);
        }



        [HttpGet]
        [Authorize(Roles = "Staff")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public ActionResult Create(Post model)
        {
            // checks if model is valid
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();//gets the id of the currently logged in user
                model.User = context.Users.Find(userId);
                model.Id = userId;//assigning the foreign key

                // sets other info on post
                model.IsApproved = false;
                model.DatePosted = DateTime.Now;
                model.Comments = new List<Comment>();

                context.Posts.Add(model);// adds new post to db
                context.SaveChanges();// saves db changes

                TempData["AlertMessage"] = "Your post was created";// sets message to display in blog

                return RedirectToAction("Blog");
            }

            // sends back with model if something goes wrong
            return View(model);
        }



        // GET: Post/Edit/5
        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        public ActionResult Edit(int id)
        {
            // checks if id given exists
            if(id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // finds the post from id
            var post = context.Posts.Find(id);

            // checks if a post was found
            if (post == null)
            {
                return HttpNotFound();
            }


            // gives post to view
            return View(post);
        }

        // POST: Post/Edit/5
        [HttpPost]
        [Authorize(Roles = "Staff,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostID,Title,Category,Content")] Post post)
        {
            if(ModelState.IsValid)
            {
                post.DatePosted = DateTime.Now;// updates dateposted
                post.Id = User.Identity.GetUserId();// sets foriegn key
                post.IsApproved = false;

                // updates db
                context.Entry(post).State = EntityState.Modified;
                // saves changes to db
                context.SaveChanges();

                TempData["AlertMessage"] = "Your post was updated";// sets message to display in blog

                // redirects to blog page
                return RedirectToAction("Blog");
            }
            // if model isnt valid, return to edit page
            return View(post);
        }



        // POST: Get/Delete/5
        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        public ActionResult Delete(int id)
        {
            // checks if id given exists
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // finds the post from id
            var post = context.Posts.Find(id);

            // checks if a post was found
            if (post == null)
            {
                return HttpNotFound();
            }


            // gives post to view
            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Staff,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var post = context.Posts.Find(id);// gets post being deleted

                context.Posts.Remove(post);// deletes post from context

                context.SaveChanges();// updates db

                TempData["AlertMessage"] = "Your post was Deleted";// sets message to display in blog

                return RedirectToAction("Blog");// returns to blog page
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        // sets the approval of a post to false
        public ActionResult DisapprovePost(int id)
        {
            // gets post
            Post post = context.Posts.Find(id);
            post.IsApproved = false;// sets isapproved

            context.SaveChanges();// saves changes

            // returns to blog
            return RedirectToAction("Blog", "Post");
        }

        [Authorize(Roles = "Admin")]
        // sets the approval of a post to true
        public ActionResult ApprovePost(int id)
        {
            // gets post
            Post post = context.Posts.Find(id);
            post.IsApproved = true;// sets isapproved

            context.SaveChanges();// saves changes

            // returns to blog
            return RedirectToAction("Blog");
        }
    }
}
