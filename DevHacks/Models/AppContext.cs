using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DevHacks.Models
{
    public class AppContext : DbContext
    {
            public AppContext (): base ("DBConnectionString") {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext,DevHacks.Migrations.Configuration>("DBConnectionString"));
        }


        public DbSet<Category> Categories{ get; set; }
        public DbSet<Comment> Comments{ get; set; }
        public DbSet<Question> Questions{ get; set; }
        public DbSet<Article> Articles{ get; set; }


    }
}