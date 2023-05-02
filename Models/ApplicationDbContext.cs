using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace Local_Theatre.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        // dbsets to add data to
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }

        public ApplicationDbContext()
            : base("LocalTheatreConnection", throwIfV1Schema: false)
        {
            // calls db initialiser
            Database.SetInitializer(new DatabaseInitialiser());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    // creates initial data and roles for db
    public class DatabaseInitialiser : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            // check if any users exist in the context already
            if (!context.Users.Any())
            {
                // populates roles table

                // creates role manger with role store
                var roleManger = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                // adding member role
                if (!roleManger.RoleExists("Member"))// checks if member already exists
                {
                    // creates role then adds it to db
                    var member = new IdentityRole("Member");
                    var roleResult = roleManger.Create(member);
                }

                // adding staff role
                if (!roleManger.RoleExists("Staff"))// checks if staff already exists
                {
                    // creates role then adds it to db
                    var staff = new IdentityRole("Staff");
                    var roleResult = roleManger.Create(staff);
                }


                // adding admin role
                if (!roleManger.RoleExists("Admin"))// checks if admin already exists
                {
                    // creates role then adds it to db
                    var admin = new IdentityRole("Admin");
                    var roleResult = roleManger.Create(admin);
                }

                // adding suspended role
                if (!roleManger.RoleExists("Suspended"))// checks if suspended already exists
                {
                    // creates role then adds it to db
                    var suspended = new IdentityRole("Suspended");
                    var roleResult = roleManger.Create(suspended);
                }

                // ---------------------------------------------------------------------------------------------

                // creates post and comment list
                List<Post> posts = new List<Post>();
                List<Comment> comments = new List<Comment>();

                // checks if there are any comments existing
                if (!context.Comments.Any())
                {
                    // create a comment for the post
                    var comment1 = new Comment
                    {
                        Content = "You speak the truth",
                        DatePosted = DateTime.Now,
                        IsApproved = true
                    };
                    // adds comment to list
                    comments.Add(comment1);

                    // create a comment for the post
                    var comment2 = new Comment
                    {
                        Content = "This is an absolte masterpiece thank you for spreading his green holy light. #Blessed",
                        DatePosted = DateTime.Now,
                        IsApproved = true
                    };
                    // adds comment to list
                    comments.Add(comment2);
                }

                // checks if there are any posts existing
                if (!context.Posts.Any())
                {

                    // creates an initial post
                    var post1 = new Post
                    {
                        Title = "Shrek 5 Review",
                        Category = Categories.Review,
                        Content = "11/10. It has been 200 years sinse we were last blessed with a shrek movie but finally HE has returned to grace " +
                        "us with his presance once more. Shrek  makes you relay FEEL like shrek. This is a must watch.",
                        DatePosted = DateTime.Now,
                        IsApproved = true,
                        Comments = comments
                    };
                    posts.Add(post1);// adds to posts list

                    // creates an initial post
                    var post2 = new Post
                    {
                        Title = " I HAVE A BURGER!",
                        Category = Categories.Announcment,
                        Content = "THIS IS VERY IMPORTANT AND EVERYONE MUST KNOW THAT I INDEED HAVE A NICE, CHEESY AND BURGERY BOIGAR! That is all have a good day.",
                        DatePosted = DateTime.Now,
                        IsApproved = true,
                        Comments = new List<Comment>()
                    };
                    posts.Add(post2);// adds to posts list
                }


                // adding a staff and admin

                // creates user manager
                var userManager = new UserManager<User>(new UserStore<User>(context));


                // adding staff
                string username = "JaneDoe";
                string password = "123ABC";

                // checks user not already added
                if (userManager.FindByName(username) == null)
                {
                    // creates member
                    User staff = new User
                    {
                        UserName = username,
                        Email = "JaneDoe@email.com",
                        EmailConfirmed = true,
                        DateRegistered = DateTime.Now,
                        Posts = posts,
                        CurrentRole = "Staff"
                    };

                    // adds user to db
                    var userResult = userManager.Create(staff, password);

                    // assigns it staff role
                    if (userResult.Succeeded)
                    {
                        userManager.AddToRole(staff.Id, "Staff");
                    }
                }
                

                // adding admin
                username = "BigMan";
                password = "Admin1";

                // checks user not already added
                if (userManager.FindByName(username) == null)
                {
                    // creates member
                    var admin = new User
                    {
                        UserName = username,
                        Email = "BigMan@email.com",
                        EmailConfirmed = true,
                        DateRegistered = DateTime.Now,
                        CurrentRole = "Admin"
                    };

                    // adds user to db
                    var userResult = userManager.Create(admin, password);

                    // assigns it admin role
                    if (userResult.Succeeded)
                    {
                        userManager.AddToRole(admin.Id, "Admin");
                    }
                }

                // adding member
                username = "JohnBurosa";
                password = "Password";

                // checks user not already added
                if (userManager.FindByName(username) == null)
                {
                    // creates member
                    var member = new User
                    {
                        UserName = username,
                        Email = "JohnWurosa@email.com",
                        EmailConfirmed = true,
                        DateRegistered = DateTime.Now,
                        Comments = comments,
                        CurrentRole = "Member"
                    };

                    // adds user to db
                    var userResult = userManager.Create(member, password);

                    // assigns it admin role
                    if (userResult.Succeeded)
                    {
                        userManager.AddToRole(member.Id, "Member");
                    }
                }

                // seeds db and saves changes
                base.Seed(context);
                context.SaveChanges();
            }
        }
    }
}