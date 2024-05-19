using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using leave_master_backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace leave_master_backend.Context
{
    public class MongoDBContext : IdentityDbContext<AppUser>
    {
        public MongoDBContext(DbContextOptions<MongoDBContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<User>().ToCollection("Users");
            modelBuilder.Entity<LeaveRequest>().ToCollection("LeaveRequests");
        }

        // public DbSet<User> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
    }
}