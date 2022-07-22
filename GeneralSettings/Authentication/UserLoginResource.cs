using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechArchDataHandler.General;

namespace TechArchFillingSystem.GeneralSettings.Authentication
{
    public class UserLoginResource
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string FileLocationUrl { get; set; }
        public int DepartmentId { get; internal set; }
        public OutputHandler OutputHandler { get; set; }
    }
}
