using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_master_backend.Context
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = "LeaveMasterDB";
    }
}