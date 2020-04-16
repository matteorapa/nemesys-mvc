using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace mvc.ViewModels
{
    public class ValidateSearch
    {

        [Required(ErrorMessage = "Please type some keywords for search!")]
        [StringLength(60, MinimumLength = 1)]
        public string Search { get; set; }
    }
}
