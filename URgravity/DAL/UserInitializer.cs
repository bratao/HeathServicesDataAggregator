using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using URgravity.Models;

namespace URgravity.DAL
{
    public class UserInitializer : DropCreateDatabaseIfModelChanges<UserContext>
    {
        protected override void Seed(UserContext context)
        {
            //nop
        }
    }
}