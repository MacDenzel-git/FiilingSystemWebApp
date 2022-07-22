using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechArchFillingSystem.GeneralSettings.Authentication
{
    public class SessionDetailsDTO
    {
        public string Token { get; set; }
        public string Username { get; set; }

        public string FileLocationUrl { get; set; }
        public bool IsSet { get; set; }
        public int DepartmentId { get; set; }
    }
}
