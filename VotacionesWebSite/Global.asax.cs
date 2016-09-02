using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VotacionesWebSite.Models;

namespace VotacionesWebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Models.VotacionesContext, Migrations.Configuration>()); //Cada vez que ejecuto la aplicacion verifica si la base de datos cambio

            this.CheckSuperUser();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void CheckSuperUser()
        {
            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var db = new VotacionesContext();


            this.CheckRole("Admin", userContext);
            this.CheckRole("User", userContext);

            var user = db.Users.Where(p => p.UserName.ToLower().Equals("alexandromunera@gmail.com")).FirstOrDefault();

            if (user == null)
            {
                user = new User
                {
                    Address = "Cr 54 # 56",
                    FirstName = "Freud",
                    LastName = "Múnera González",
                    Phone = "4532258",
                    UserName = "alexandromunera@gmail.com"
                };

                db.Users.Add(user);
                db.SaveChanges();

            }

            var userASP = userManager.FindByName(user.UserName);

            if (userASP == null)
            {
                 userASP = new ApplicationUser
                {
                    UserName = user.UserName,
                    Email = user.UserName,
                    PhoneNumber = user.Phone
                };
                
                userManager.Create(userASP, "@Admin123");
            }

            userManager.AddToRole(userASP.Id, "Admin");



        }

        private void CheckRole(string roleName, ApplicationDbContext userContext)
        {
            //user managment
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(userContext));

            //Check to see if role exist if it doesn't create it
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }

        }
    }
}
