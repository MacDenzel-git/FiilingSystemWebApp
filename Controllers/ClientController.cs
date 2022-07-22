using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechArch.DataHandler.GeneralSettings;
using TechArchDataHandler.General;
using TechArchFillingSystem.GeneralSettings;
using TechArchFillingSystem.Models;
using TFSDataAccessLayer.DTO;

namespace TechArchFillingSystem.Controllers
{
    public class ClientController : Controller
    {
        private readonly IConfiguration _configuration;
        static readonly string apiUrl = "Client";

        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }

        public ClientController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<ClientDTO> companies = new List<ClientDTO>();
                var sessionDetails = await StaticDataHandler.GetSessionDetails();
                var requestUrl = $"{BaseUrl}{apiUrl}/GetClients?departmentId={sessionDetails.DepartmentId}";

                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await httpClient.GetAsync(requestUrl);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    companies = JsonConvert.DeserializeObject<IEnumerable<ClientDTO>>(data);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                {
                    return View();
                }
                return View(companies);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", ViewBag.Message = $" {ex.Message} Something went wrong, please contact Administrator ");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var generalVM = new GeneralViewModel
            {
                Departments = await StaticDataHandler.GetDepartments(BaseUrl)
            };
            return View(generalVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GeneralViewModel generalViewModel)
        {
            OutputHandler result = new();

            generalViewModel.Client.DateCreated = DateTime.Now.AddHours(2);
            generalViewModel.Client.CreatedBy = "SYSADMIN";
             var requestUrl = $"{BaseUrl}{apiUrl}/Create";

            using var httpClient = new HttpClient();
            HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync(requestUrl, generalViewModel.Client);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<OutputHandler>(data);
                return RedirectToAction("Index", "Client", ViewBag.Message = result.Message);
            }
            else
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<OutputHandler>(data);
                return View(ViewBag.Message = result.Message);
            }
        }

        public async Task<IActionResult> Update(int typeId)
        {
            var requestUrl = $"{BaseUrl}{apiUrl}/GetClient?ClientId={typeId}";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                var status = JsonConvert.DeserializeObject<ClientDTO>(data);
                return View(status);
            }
            else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                return View();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(GeneralViewModel generalViewModel)
        {
            generalViewModel.Client.ModifiedDate = DateTime.UtcNow.AddHours(2);
            generalViewModel.Client.ModifiedBy = "SYSADMIN";

            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Update";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync(requestUrl, generalViewModel.Client);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                resultHandler = JsonConvert.DeserializeObject<OutputHandler>(data);
            }
            else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                return View(ViewBag.Message = resultHandler.Message);
            }
            return RedirectToAction("Index", new { message = resultHandler.Message });
        }

        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?clientId={id}";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.DeleteAsync(requestUrl);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                resultHandler = JsonConvert.DeserializeObject<OutputHandler>(data);
                return RedirectToAction("Index");
            }
            else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                resultHandler = JsonConvert.DeserializeObject<OutputHandler>(data);
                return RedirectToAction("Index", new { message = resultHandler.Message });
            }
            return RedirectToAction("Index");
        }
    }
}
