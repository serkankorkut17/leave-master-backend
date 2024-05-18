using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using leave_master_backend.Models;


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
            modelBuilder.Entity<User>().ToCollection("Users");
        }

        public DbSet<User> Users { get; set; }
    }
}