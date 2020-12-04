using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Aetna.Models
{
    public class AetnaContext: DbContext
    {
        public AetnaContext() : base("name=connStr")
        {

        }

        public DbSet<TeamMaintenance> TeamMaintenances { get; set; }
    }
}