using System;
namespace NanoLoanApi.Core
{
    public class PostData
    {
        public string customerId { get; set; }
        public string providerCode { get; set; }
        public string channelCode { get; set; }
        public object debitMethod { get; set; }
        public object creditMethod { get; set; }
    }
}
