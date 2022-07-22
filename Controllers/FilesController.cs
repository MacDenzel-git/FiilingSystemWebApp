
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechArch.DataHandler;
using TechArchFillingSystem.GeneralSettings;
using TechArchFillingSystem.Models;
using TFSDataAccessLayer.DTO;
using System.IO;
using TechArchDataHandler.General;
using System.Net.Http.Json;

namespace TechArchFillingSystem.Controllers
{
    public class FilesController : Controller
    {

        private readonly IConfiguration _configuration;
        static readonly string apiUrl = "FileDetail";

        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }

        public FilesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                GeneralViewModel generalViewModel = new GeneralViewModel();
                IEnumerable<FileDetailDTO> fileTypes = new List<FileDetailDTO>();
                var sessionDetails = await StaticDataHandler.GetSessionDetails();
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllFiles?departmentId={sessionDetails.DepartmentId}";
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await httpClient.GetAsync(requestUrl);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    fileTypes = JsonConvert.DeserializeObject<IEnumerable<FileDetailDTO>>(data);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                {
                    generalViewModel.FileDetails = fileTypes;
                    return View(generalViewModel);
                }
                generalViewModel.FileDetails = fileTypes;
                return View(generalViewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", ViewBag.Message = $" {ex.Message} Something went wrong, please contact Administrator ");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UploadFiles()
        {
            var sessionDetails = await StaticDataHandler.GetSessionDetails();

            var generalVM = new GeneralViewModel
            {
                FileTypeCategories = await StaticDataHandler.GetFileTypeCategories(BaseUrl, sessionDetails.DepartmentId),
                Months = StaticDataHandler.GetMonths(),
                Clients = await StaticDataHandler.GetClients(BaseUrl,sessionDetails.DepartmentId)
            };
            return View(generalVM);
        }


        [HttpPost]
        public async Task<IActionResult> UploadFiles(GeneralViewModel generalViewModel, ICollection<IFormFile> pdfDoc)
        {
            foreach (var file in pdfDoc)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        string extension = Path.GetExtension(file.FileName);
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        // act on the Base64 data
                        generalViewModel.FileDetail.UploadedFile = fileBytes;
                        generalViewModel.FileDetail.FileName = $"{generalViewModel.FileDetail.FileName}{extension}";
                    }
                }
            }


            OutputHandler result = new();
            var sessionDetails = await StaticDataHandler.GetSessionDetails();

            generalViewModel.FileDetail.DateCreated = DateTime.Now.AddHours(2);
            generalViewModel.FileDetail.CreatedBy = "SYSADMIN";
            generalViewModel.FileDetail.FileLocation = "unknown";
            generalViewModel.FileDetail.folderUrl = sessionDetails.FileLocationUrl;
            generalViewModel.FileDetail.DepartmentId = sessionDetails.DepartmentId;

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";

            using var httpClient = new HttpClient();
            HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync(requestUrl, generalViewModel.FileDetail);
    
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<OutputHandler>(data);

                return RedirectToAction("Index", "Files", ViewBag.Message = result.Message);
            }
            else
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<OutputHandler>(data);
                generalViewModel.FileTypeCategories = await StaticDataHandler.GetFileTypeCategories(BaseUrl,sessionDetails.DepartmentId);
                generalViewModel.Months = StaticDataHandler.GetMonths();
                generalViewModel.Clients = await StaticDataHandler.GetClients(BaseUrl, sessionDetails.DepartmentId);
                ViewBag.Message = result.Message;
                return View(generalViewModel);
            }
        }

        public async Task<FileResult> DownloadFile(int fileDetailId)
        {
            var requestUrl = $"{BaseUrl}{apiUrl}/GetFileDetail?fileDetailId={fileDetailId}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    FileDetailDTO file = JsonConvert.DeserializeObject<FileDetailDTO>(responseData);
                    return File(file.UploadedFile, "application/pdf", file.FileName);
                };
            };

            //return to download page or stay on page
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Preview(int fileId)
        {
            var generalViewModel = new GeneralViewModel();
            var requestUrl = $"{BaseUrl}{apiUrl}/GetFile?fileId={fileId}";

            using var httpClient = new HttpClient();
            HttpResponseMessage responseMessage = await httpClient.GetAsync(requestUrl);
           
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                generalViewModel.FileDetail = JsonConvert.DeserializeObject<FileDetailDTO>(data);
            }
          
            Response.ContentType = "Application/pdf";
            Response.ContentType = "\".pdf\", \"application/pdf\"";
           await Response.WriteAsync(generalViewModel.FileDetail.FileLocation);
            Response.Headers.Add("content-disposition", "attachment;filename=" + generalViewModel.FileDetail.FileName + ".pdf");

            return View();
        }
    }
}
