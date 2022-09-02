﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactBook.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContactBook.Controllers
{
    public class RoleController : Controller
    {

        ApplicationDbContext context;

        public RoleController()
        {
            context = new ApplicationDbContext();

        }

        [Authorize(Roles = "Admin")]
        // GET: Role
        public ActionResult Index()
        {
            var Roles = context.Roles.ToList();
            return View(Roles);
        }
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}