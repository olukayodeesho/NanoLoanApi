using System;
using System.ComponentModel.DataAnnotations;

namespace NanoLoanApi.Core.CustomObjects
{
    public class OfferRequest : Base
    {
        [Required]
        public string ProviderCode { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
