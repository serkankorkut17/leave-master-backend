using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using leave_master_backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace leave_master_backend.Context
{
    public class MongoDBContext : DbContext
    {
        public MongoDBContext(DbContextOptions<MongoDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<User>().ToCollection("Users");
            // List<IdentityRole> roles = new List<IdentityRole>
            // {
            //     new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
            //     new IdentityRole { Name = "User", NormalizedName = "USER" }
            // };
            // modelBuilder.Entity<IdentityRole>().ToCollection("Roles");

            // modelBuilder.Entity<LeaveRequest>(x => ).ToCollection("LeaveRequests");
            modelBuilder.Entity<LeaveRequest>().HasKey(lr => lr.Id);

            modelBuilder.Entity<LeaveRequest>().Property(lr => lr.UserName).IsRequired();

            modelBuilder.Entity<LeaveRequest>().Property(lr => lr.StartDate).IsRequired();

            modelBuilder.Entity<LeaveRequest>().Property(lr => lr.EndDate).IsRequired();

            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.LeaveDays)
                .IsRequired();

            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.Status)
                .IsRequired();

            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.Reason)
                .IsRequired();

            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.UpdatedAt)
                .IsRequired();

            modelBuilder.Entity<LeaveRequest>().ToCollection("LeaveRequests");
            modelBuilder.Entity<Leave>().ToCollection("Leaves");
        }

        // public DbSet<User> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Leave> Leaves { get; set; }
    }
}