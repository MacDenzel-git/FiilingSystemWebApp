using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TFSDataAccessLayer.DTO;

namespace TechArchFillingSystem.Models
{
    public class GeneralViewModel
    {
        //SingleObjects
        public FileDetailDTO FileDetail { get; set; }
        public IFormFile File { get; set; }
        public FileTypeDTO FileType { get; set; }  
        public SubsidiaryCompanyDTO SubsidiaryCompany { get; set; }  
        public FileTypeCategoryDTO FileTypeCategory { get; set; }
        public ClientDTO Client { get; set; }
        public DepartmentDTO Department { get; set; }

        //Lists
        public IEnumerable<FileTypeDTO> FileTypes { get; set; }
        public IEnumerable<FileDetailDTO> FileDetails { get; set; }
        public IEnumerable<FileTypeCategoryDTO> FileTypeCategories { get; set; }
        public IEnumerable<CompanyDTO> Companies { get; set; }
        public IEnumerable<SubsidiaryCompanyDTO> SubsidiaryCompanies { get; set; }
        public IEnumerable<ClientDTO> Clients { get; set; }
        public IEnumerable<DepartmentDTO> Departments { get; set; }
        public IEnumerable<MonthDTO> Months { get; set; }
    }
}
