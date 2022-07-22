using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechArchDataHandler.General;
using TechArchFillingSystem.GeneralSettings;
using TechArchFillingSystem.GeneralSettings.Authentication;

namespace TechArchFillingSystem.Controllers
{
    public class AccountController : Controller
    {
        private const string FileLocationURl = "URL";
        private const string DepartmentId = "Identification";
        private const string LoggedInUser = "username";
        public IActionResult Index()
        {
            return View();
        } 
        
        [HttpGet]
        public IActionResult Login()
        {
            var loginDetails = new UserLoginResource
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false}
           };
            HttpContext.Session.Clear();
            return View(loginDetails);
        }
        
        public async Task<IActionResult> Login(UserLoginResource loginDetails)
        {
            UserLoginResource userLogin = new UserLoginResource
            {
                OutputHandler = new OutputHandler { IsErrorOccured = true, Message = "Login was successful, System has experienced a technical fault, Contact TechArch" }
            };
            if (loginDetails.Email == "test@user.com" && loginDetails.Password =="test")
            {
                //LoginCall should return rootfolderURl
                string companyName = "Nico Holdings";
                string subsidiaryCompanyName = "Nico Technologies";
                string department = "Finance Team";
                loginDetails.DepartmentId = 4;

                loginDetails.FileLocationUrl = $"{companyName}\\{subsidiaryCompanyName}\\{department}";
                var sessionDetails =  ConfigureSession(loginDetails);
                if (sessionDetails.IsSet)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    userLogin.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = "Login was successful, System has experienced a technical fault, Contact TechArch" };
                    
                    return View(userLogin);
                }
            }

            userLogin.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = "Username or Password incorrect" };
            HttpContext.Session.Clear();
           
            return View(userLogin);
        }

        private SessionDetailsDTO ConfigureSession(UserLoginResource loginDetails)
        {
            HttpContext.Session.Clear();
            HttpContext.Session.SetString(LoggedInUser, loginDetails.Email);
            HttpContext.Session.SetString(FileLocationURl, loginDetails.FileLocationUrl);
            HttpContext.Session.SetInt32(DepartmentId, loginDetails.DepartmentId);

            SessionDetailsDTO sessionDetails = new SessionDetailsDTO
            {
                Username = TechArchDataHandler.SessionHandler.SessionHandlerAppContext.Current.Session.GetString(LoggedInUser),
                FileLocationUrl = TechArchDataHandler.SessionHandler.SessionHandlerAppContext.Current.Session.GetString(FileLocationURl),
                DepartmentId = (int)TechArchDataHandler.SessionHandler.SessionHandlerAppContext.Current.Session.GetInt32(DepartmentId),

            };

            if (string.IsNullOrEmpty(sessionDetails.FileLocationUrl))
            {
                sessionDetails.IsSet = false;
            }
            else
            {
                sessionDetails.IsSet = true;
            }
            return sessionDetails;
        }
    }
}
