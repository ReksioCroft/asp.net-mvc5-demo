﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OpenDiscussionTavGeorge.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenDiscussionTavGeorge.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: User
        public ActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.Roles.FirstOrDefault().RoleId, user.UserName
                        select user;

            ViewBag.UsersList = users;
            return View();
        }        [OverrideAuthorization]        [Authorize]        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            string currentRole = user.Roles.FirstOrDefault().RoleId;
            var userRoleName = (from role in db.Roles
                                where role.Id == currentRole
                                select role.Name).First();
            ViewBag.roleName = userRoleName;
            return View(user);
        }   
        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
        [OverrideAuthorization]
        [Authorize]        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            return View(user);
        }
        [OverrideAuthorization]
        [HttpPut]
        [Authorize]
        public ActionResult Edit(string id, ApplicationUser newData)
        {

            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new
               RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new
               UserStore<ApplicationUser>(context));

                if (TryUpdateModel(user))
                {
                    if (User.Identity.GetUserId() == user.Id || User.IsInRole("Admin"))
                    {
                        user.UserName = newData.UserName;
                        user.Email = newData.Email;
                        user.PhoneNumber = newData.PhoneNumber;
                        user.Description = newData.Description;
                        if (User.IsInRole("Admin"))
                        {
                            var roles = from role in db.Roles select role;
                            foreach (var role in roles)
                            {
                                UserManager.RemoveFromRole(id, role.Name);
                            }
                            var selectedRole =
                            db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                            UserManager.AddToRole(id, selectedRole.Name);
                        }
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Show", "User", new { @id = id });
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                newData.Id = id;
                return View(newData);
            }

        }        [HttpDelete]
        public ActionResult Delete(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new
           UserStore<ApplicationUser>(context));
            var user = UserManager.Users.FirstOrDefault(u => u.Id == id);
            var questions = db.Questions.Where(a => a.UserId == id);
            foreach (var question in questions)
            {
                db.Questions.Remove(question);
            }
            var comments = db.Comments.Where(comm => comm.UserId == id);
            foreach (var comment in comments)
            {
                db.Comments.Remove(comment);
            }

            db.SaveChanges();
            UserManager.Delete(user);
            return RedirectToAction("Index");
        }
    }
}