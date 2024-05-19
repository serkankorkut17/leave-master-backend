using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace leave_master_backend.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
        public required string Role { get; set; } = string.Empty;
        // start date
        public required DateTime StartDate { get; set; }

        // Used leave days for each year (stored as a list of key-value pairs for MongoDB compatibility)
        // default value => Key: Year now, Value: 0
        [BsonElement("UsedLeaveDaysPerYear")]
        public List<KeyValuePair<int, int>> UsedLeaveDaysPerYear { get; set; } = new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(DateTime.Now.Year, 0) };

        // Method to add used leave days for a specific year
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

        // Method to get used leave days for a specific year
        public int GetUsedLeaveDays(int year)
        {
            var kvp = UsedLeaveDaysPerYear.FirstOrDefault(kvp => kvp.Key == year);
            return kvp.Equals(default(KeyValuePair<int, int>)) ? 0 : kvp.Value;
        }
    }
}