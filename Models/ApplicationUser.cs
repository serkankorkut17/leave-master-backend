using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDB.Bson;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;


namespace leave_master_backend.Models
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }

        public List<KeyValuePair<int, int>> UsedLeaveDaysPerYear { get; set; } = new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(DateTime.Now.Year, 0) };

        public void AddUsedLeaveDays(int year, int days)
        {
            var index = UsedLeaveDaysPerYear.FindIndex(kvp => kvp.Key == year);
            if (index >= 0)
            {
                var kvp = UsedLeaveDaysPerYear[index];
                UsedLeaveDaysPerYear[index] = new KeyValuePair<int, int>(year, kvp.Value + days);
            }
            else
            {
                UsedLeaveDaysPerYear.Add(new KeyValuePair<int, int>(year, days));
            }
        }
    }

    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole<Guid>
    {

    }
}