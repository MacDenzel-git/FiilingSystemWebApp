using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechArchDataHandler.General;

namespace TechArchFillingSystem.GeneralSettings.Authentication
{
    public class UserSignUpResource
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }
        public string Username { get; set; }
        public OutputHandler OutputHandler { get; set; }
    }
}
