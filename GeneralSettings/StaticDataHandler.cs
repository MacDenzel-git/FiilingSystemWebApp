using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechArchFillingSystem.GeneralSettings.Authentication;
using TechArchFillingSystem.Models;
using TFSDataAccessLayer.DTO;

namespace TechArchFillingSystem.GeneralSettings
{
    public class StaticDataHandler
    {
        public static async Task<FileTypeDTO> GetFileType(string baseUrl, int id)
        {
            var requestUrl = $"{baseUrl}FileType/GetFileType?={id}";
            FileTypeDTO fileType = new();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    fileType = JsonConvert.DeserializeObject<FileTypeDTO>(data);
                }
            }
            return fileType;
        }

        public static async Task<FileTypeCategoryDTO> GetFileTypeCategory(string baseUrl, int id)
        {
            var requestUrl = $"{baseUrl}FileTypeCategory/GetFileTypeCategory?={id}";
            FileTypeCategoryDTO fileTypeCategory = new();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    fileTypeCategory = JsonConvert.DeserializeObject<FileTypeCategoryDTO>(data);
                }
            }
            return fileTypeCategory;
        }


        
        public static async Task<IEnumerable<FileTypeDTO>> GetFileTypes(string baseUrl, int department)
        {
            var requestUrl = $"{baseUrl}FileType/GetFileTypes?departmentId={department}";
            IEnumerable<FileTypeDTO> fileTypes = new List<FileTypeDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    fileTypes = JsonConvert.DeserializeObject<IEnumerable<FileTypeDTO>>(data);
                }
            }
            return fileTypes;
        }

        public static async Task<IEnumerable<FileTypeCategoryDTO>> GetFileTypeCategories(string baseUrl, int departmentId)
        {
            var requestUrl = $"{baseUrl}FileTypeCategory/GetFileTypeCategories?departmentId={departmentId}";
            IEnumerable<FileTypeCategoryDTO> fileTypeCategories = new List<FileTypeCategoryDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    fileTypeCategories = JsonConvert.DeserializeObject<IEnumerable<FileTypeCategoryDTO>>(data);
                }
            }
            return fileTypeCategories;
        }

        public static async Task<IEnumerable<ClientDTO>> GetClients(string baseUrl, int departmentId)
        {
            var requestUrl = $"{baseUrl}Client/GetClients?departmentId={departmentId}";
            IEnumerable<ClientDTO> clients = new List<ClientDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    clients = JsonConvert.DeserializeObject<IEnumerable<ClientDTO>>(data);
                }
            }
            return clients;
        }  
        
        public static async Task<IEnumerable<CompanyDTO>> GetCompanies(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Company/GetCompanies";
            IEnumerable<CompanyDTO> companies = new List<CompanyDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    companies = JsonConvert.DeserializeObject<IEnumerable<CompanyDTO>>(data);
                }
            }
            return companies;
        }

        //Consider filtering by Company
        public static async Task<IEnumerable<SubsidiaryCompanyDTO>> GetSubsidiaryCompanies(string baseUrl)
        {
            var requestUrl = $"{baseUrl}SubsidiaryCompany/GetSubsidiaryCompanies";
            IEnumerable<SubsidiaryCompanyDTO> subCompanies = new List<SubsidiaryCompanyDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    subCompanies = JsonConvert.DeserializeObject<IEnumerable<SubsidiaryCompanyDTO>>(data);
                }
            }
            return subCompanies;
        }

        //Consider filtering with logged in company
        public static async Task<IEnumerable<DepartmentDTO>> GetDepartments(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Department/GetDepartments";
            IEnumerable<DepartmentDTO> companies = new List<DepartmentDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    companies = JsonConvert.DeserializeObject<IEnumerable<DepartmentDTO>>(data);
                }
            }
            return companies;
        }

        public static async Task<SessionDetailsDTO> GetSessionDetails()
        {
            SessionDetailsDTO sessionDetails = new SessionDetailsDTO
            {
                Username = TechArchDataHandler.SessionHandler.SessionHandlerAppContext.Current.Session.GetString("username"),
                FileLocationUrl = TechArchDataHandler.SessionHandler.SessionHandlerAppContext.Current.Session.GetString("URL"),
                DepartmentId = (int)TechArchDataHandler.SessionHandler.SessionHandlerAppContext.Current.Session.GetInt32("Identification"),
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

        public static IEnumerable<MonthDTO> GetMonths()
        {
            IEnumerable<MonthDTO> list = new List<MonthDTO>()
            {
                new MonthDTO { Id = 1, Name = "January",},
                new MonthDTO { Id = 2, Name = "February" },
                new MonthDTO { Id = 3, Name = "March" }
            };
            return list;
        }
    }
}
