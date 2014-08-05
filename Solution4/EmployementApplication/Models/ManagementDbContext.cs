using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EmployementApplication.Models
{
    public class AppUser : IdentityUser
    {
      
    }
    public class ManagementDbContext :IdentityDbContext<AppUser>
    {
        public ManagementDbContext() : base("Management")
        {
         //Database.SetInitializer<ManagementDbContext>(null);
        }
     
        public IDbSet<Employees> Employs { get; set; }
        public IDbSet<Departments> Departments { get; set; }
       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
            
        }
       
    }
}