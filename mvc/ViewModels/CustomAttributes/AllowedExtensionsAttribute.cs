using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.ViewModels
{
    public class AllowExtensionsAttribute : ValidationAttribute
    {
        public string Extensions { get; set; } = "png,jpg,jpeg,gif";

        public override bool IsValid(object value)
        {
            // Initialization  
            IFormFile file = value as IFormFile;
            bool isValid = true;

            // Settings.  
            List<string> allowedExtensions = this.Extensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Verification.  
            if (file != null)
            {
                // Initialization.  
                var fileName = file.FileName;

                // Settings.  
                isValid = allowedExtensions.Any(y => fileName.EndsWith(y));
            }

            // Info  
            return isValid;
        }
    }
}
