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
using TFSDataAccessLayer.DTO;

namespace TechArchFillingSystem.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IConfiguration _configuration;
        static readonly string apiUrl = "Company";

        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }

        public CompanyController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<CompanyDTO> companies = new List<CompanyDTO>();
                var requestUrl = $"{BaseUrl}{apiUrl}/GetCompanies";

                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await httpClient.GetAsync(requestUrl);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    companies = JsonConvert.DeserializeObject<IEnumerable<CompanyDTO>>(data);
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompanyDTO CompanyDTO)
        {
            OutputHandler result = new();

            CompanyDTO.CreatedDate = DateTime.Now.AddHours(2);
            CompanyDTO.CreatedBy = "SYSADMIN";

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";

            using var httpClient = new HttpClient();
            HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync(requestUrl, CompanyDTO);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<OutputHandler>(data);
                return RedirectToAction("Index", "Company", ViewBag.Message = result.Message);
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
            var requestUrl = $"{BaseUrl}{apiUrl}/GetCompany?CompanyId={typeId}";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                var status = JsonConvert.DeserializeObject<CompanyDTO>(data);
                return View(status);
            }
            else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                return View();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(CompanyDTO CompanyDTO)
        {
            CompanyDTO.ModifiedDate = DateTime.UtcNow.AddHours(2);
            CompanyDTO.ModifiedBy = "SYSADMIN";

            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Update";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync(requestUrl, CompanyDTO);

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
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?companyId={id}";
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
