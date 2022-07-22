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
    public class FileCategoryController : Controller
    {
        private readonly IConfiguration _configuration;
        static readonly string apiUrl = "FileTypeCategory";

        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public FileCategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<FileTypeCategoryDTO> fileTypeCategories = new List<FileTypeCategoryDTO>();
                var sessionDetails = await StaticDataHandler.GetSessionDetails();
                var requestUrl = $"{BaseUrl}{apiUrl}/GetFileTypeCategories?departmentId={sessionDetails.DepartmentId}";
 
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await httpClient.GetAsync(requestUrl);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    fileTypeCategories = JsonConvert.DeserializeObject<IEnumerable<FileTypeCategoryDTO>>(data);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                {
                    return View();
                }


                return View(fileTypeCategories);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", ViewBag.Message = $" {ex.Message} Something went wrong, please contact Administrator ");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var sessionDetails = await StaticDataHandler.GetSessionDetails();
            GeneralViewModel generalViewModel = new()
            {
                FileTypes = await StaticDataHandler.GetFileTypes(BaseUrl, sessionDetails.DepartmentId),
                Departments = await StaticDataHandler.GetDepartments(BaseUrl),
            };
            return View(generalViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GeneralViewModel generalViewModel)
        {
            OutputHandler result = new();
            var sessionDetails = await StaticDataHandler.GetSessionDetails();
            generalViewModel.FileTypeCategory.folderUrl = sessionDetails.FileLocationUrl;
            generalViewModel.FileTypeCategory.DateCreated = DateTime.Now.AddHours(2);
            generalViewModel.FileTypeCategory.DateModified = DateTime.Now.AddHours(2);
            generalViewModel.FileTypeCategory.CreatedBy = "SYSADMIN";

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";

            using var httpClient = new HttpClient();
            HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync(requestUrl, generalViewModel.FileTypeCategory);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<OutputHandler>(data);

                return RedirectToAction("Index", "FileCategory", ViewBag.Message = result.Message);
            }
            else
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<OutputHandler>(data);
                //var sessionDetails = await StaticDataHandler.GetSessionDetails();

                generalViewModel.FileTypes = await StaticDataHandler.GetFileTypes(BaseUrl,sessionDetails.DepartmentId);
                
                ViewBag.Message = result.Message;
                return View(generalViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int categoryId)
        {
            var sessionDetails = await StaticDataHandler.GetSessionDetails();

            GeneralViewModel generalViewModel = new()
            {
                FileTypes = await StaticDataHandler.GetFileTypes(BaseUrl,sessionDetails.DepartmentId)
            };

            var requestUrl = $"{BaseUrl}{apiUrl}/GetFileTypeCategory?fileTypeCategoryId={categoryId}";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                generalViewModel.FileTypeCategory = JsonConvert.DeserializeObject<FileTypeCategoryDTO>(data);

                return View(generalViewModel);
            }
            else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                return View();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(FileTypeCategoryDTO fileTypeCategoryDTO)
        {
            fileTypeCategoryDTO.DateModified = DateTime.UtcNow.AddHours(2);
            fileTypeCategoryDTO.ModifiedBy = "SYSADMIN";

            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Update";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync(requestUrl, fileTypeCategoryDTO);

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
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?fileTypeCategoryId={id}";
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
