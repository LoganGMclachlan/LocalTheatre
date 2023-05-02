using Local_Theatre.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Local_Theatre.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : AccountController
    {

        // creates a DBcontext instance
        private ApplicationDbContext context = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminController() : base()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager)
        {

        }


        // GET: Admin
        [HttpGet]
        public ActionResult UserRoles()
        {
            // returns users as a list
            return View(context.Users.ToList());
        }

        [HttpGet]
        public async Task<ActionResult> ChangeRole(string id)
        {
            // checks id isnt null
            if(id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // admin cant change own role
            if(id == User.Identity.GetUserId())
            {
                return RedirectToAction("UserRoles", "Admin");
            }

            // gets user from id
            User user = await UserManager.FindByIdAsync(id);
            // gets old role
            string oldRole = (await UserManager.GetRolesAsync(id)).Single();

            // gets all roles from db
            var items = context.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == oldRole// sets defualt selected as old role
            }).ToList();

            // returns to view with view model containing data
            return View(new ChangeRoleViewModel
            {
                UserName = user.UserName,
                Roles = items,
                OldRole = oldRole
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirm(string id, [Bind(Include="Role")] ChangeRoleViewModel model)
        {
            // admin cant change own role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("UserRoles", "Admin");
            }

            if (ModelState.IsValid)
            {
                // gets user from id
                User user = await UserManager.FindByIdAsync(id);

                // gets users old role
                string oldRole = (await UserManager.GetRolesAsync(id)).Single();

                // checks if current role is same as old role
                if (oldRole == model.Role)
                {
                    // returns to list
                    return RedirectToAction("UserRoles", "Admin");
                }

                user.CurrentRole = model.Role;// sets users role attribute

                // remove user from old role list
                await UserManager.RemoveFromRoleAsync(id,oldRole);

                // adds user to current role list
                await UserManager.AddToRoleAsync(id, model.Role);

                // returns to list
                return RedirectToAction("UserRoles", "Admin");
            }

            // returns to view with same model
            return View(model);
        }

    }
}