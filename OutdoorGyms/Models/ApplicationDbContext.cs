using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OutdoorGyms.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Gym> Gyms { get; set; }

        public DbSet<GymStatus> GymStatuses { get; set; }

        public DbSet<County> Countys { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Picture> Pictures { get; set; }

        public DbSet<Sequence> Sequences { get; set; }
    }
}
