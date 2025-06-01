using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace database_proj
{
    public static class UserSession
    {
        public static bool IsTraveler = false;
        public static bool IsTourOperator = false;
        public static bool IsAdmin = false;

        public static string CurrentEmail = "";
        public static string CurrentRole = "";
        public static int UserId = -1; // Added User ID field
        public static int OpId = -1;

        public static void Reset()
        {
            IsTraveler = false;
            IsTourOperator = false;
            IsAdmin = false;
            CurrentEmail = "";
            CurrentRole = "";
            UserId = -1;
            OpId = -1;
        }
    }
}