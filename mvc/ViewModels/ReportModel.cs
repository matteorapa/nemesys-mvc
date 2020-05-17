using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mvc.Models;

namespace mvc.ViewModels
{
    public class EditReport
    {
        
        [Required(ErrorMessage = "Please inform of the location of hazard in the report.")]
        [StringLength(60, MinimumLength = 1)]
        public string HazardLocation { get; set; }
        
        [Required]
        public DateTime HazardDate { get; set; }
        
        [Required(ErrorMessage = "Choose type of hazard observed.")]
        public string HazardType { get; set; }
        
        [Required(ErrorMessage = "Please describe the hazard observed in some detail.")]
        [StringLength(300, MinimumLength = 1)]
        public string HazardDescription { get; set; }

        public string ImageUrl { get; set; }
        [Display(Name = "Featured Image")]
        //Custom DataAnnotation - max 10MB allowed (refer to MaxFileSizeAttribute.cs)
        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is {0} bytes")]
        [AllowExtensions(Extensions = "png,jpg", ErrorMessage = "Please select only supported Files .png | .jpg")]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Please select the location of the hazard by clicking on the map to create a marker.")]
        public double LatitudeMarker { get; set; }

        [Required(ErrorMessage = "Please select the location of the hazard by clicking on the map to create a marker.")]
        public double LongitudeMarker { get; set; }

    }

    public class ValidateSearch
    {

        [Required(ErrorMessage = "Please type some keywords for search!")]
        [StringLength(60, MinimumLength = 1)]
        public string Search { get; set; }
    }

    public class ReportRegisterViewModel
    {
        public int TotalReports { get; set; }
        public IEnumerable<Report> Reports { get; set; }
    }

    public class HallFameModel
    {
        public int TotalReporters { get; set; }
        public IEnumerable<ApplicationUser> Reporters { get; set; }

    }
}
