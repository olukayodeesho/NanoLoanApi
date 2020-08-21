using System;
using System.ComponentModel.DataAnnotations;

namespace NanoLoanApi.Core.CustomObjects
{
    public class Base
    {
        [Required]
        public string customerId { get; set; }
    }
}
