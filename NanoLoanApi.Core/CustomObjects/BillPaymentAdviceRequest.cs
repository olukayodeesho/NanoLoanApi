using System;
using System.ComponentModel.DataAnnotations;

namespace NanoLoanApi.Core.CustomObjects
{
    public class BillPaymentAdviceRequest : Base
    {
        [Required]
        public string paymentCode { get; set; }
        
        [Required]
        public string customerMobileNo { get; set; }
        [Required]
        public string customerEmail  { get; set; }
        [Required]
        public decimal amount { get; set; }
        public string requestReference { get; set; }
    }
}
