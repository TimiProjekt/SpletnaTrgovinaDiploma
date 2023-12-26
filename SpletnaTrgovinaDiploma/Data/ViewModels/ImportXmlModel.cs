using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ImportXmlModel
    {
        [Required(ErrorMessage = "Please select file")]
        public IFormFile File { get; set; }

        [Display(Name = "Import new or update existing")]
        public bool IsUpdateExisting { get; set; }
    }
}
