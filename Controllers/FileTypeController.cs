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
    public class FileTypeController : Controller
    {
        private readonly IConfiguration _configuration;
        static readonly string apiUrl = "FileType";

        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }

        public FileTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<FileTypeDTO> fileTypes = new List<FileTypeDTO>();
                var sessionDetails = await StaticDataHandler.GetSessionDetails();
                var requestUrl = $"{BaseUrl}{apiUrl}/GetFileTypes?departmentId={sessionDetails.DepartmentId}";

                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await httpClient.GetAsync(requestUrl);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    fileTypes = JsonConvert.DeserializeObject<IEnumerable<FileTypeDTO>>(data);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                {
                    return View();
                }
                return View(fileTypes);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", ViewBag.Message = $" {ex.Message} Something went wrong, please contact Administrator ");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Create()
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
            var sessionDetails = await StaticDataHandler.GetSessionDetails();
            generalViewModel.FileType.folderUrl = sessionDetails.FileLocationUrl;
            generalViewModel.FileType.DateCreated = DateTime.Now.AddHours(2);
            generalViewModel.FileType.CreatedBy = "SYSADMIN";

            OutputHandler result = new();

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using var httpClient = new HttpClient();
            HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync(requestUrl, generalViewModel.FileType);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<OutputHandler>(data);
                return RedirectToAction("Index", "FileType", ViewBag.Message = result.Message);
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
            var requestUrl = $"{BaseUrl}{apiUrl}/GetFileType?fileTypeId={typeId}";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                var status = JsonConvert.DeserializeObject<FileTypeDTO>(data);
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
            generalViewModel.FileType.DateModified = DateTime.UtcNow.AddHours(2);
            generalViewModel.FileType.ModifiedBy = "SYSADMIN";

            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Update";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync(requestUrl, generalViewModel.FileType);

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
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?fileTypeId={id}";
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
