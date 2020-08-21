using System;
using System.ComponentModel.DataAnnotations;

namespace NanoLoanApi.Core.CustomObjects
{
    public class AcceptOfferRequest :  Base
    {
        [Required]
        public string pan { get; set; }
        [Required]
        public string expiryDate { get; set; }
        [Required]
        public string cvv { get; set; }
        [Required]
        public string pin { get; set; }
        [Required]
        public string offerId { get; set; }
        [Required]
        public string providerCode { get; set; }
        [Required]
        public string identifier { get; set; }
        [Required]
        public string accountNumber { get; set; }
        [Required]
        public string bankCode { get; set; }

    }
}
