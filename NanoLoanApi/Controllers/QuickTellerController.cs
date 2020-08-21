using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NanoLoanApi.Core;
using NanoLoanApi.Core.CustomObjects;
using Newtonsoft.Json;
using NLog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NanoLoanApi.Controllers
{
   
    [Route("[controller]/[action]")]
    public class QuickTellerController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        [HttpGet]
        public IActionResult QueryTransaction(String requestreference)
        {
            logger.Info(" query txn request " + requestreference); 
            var result =  NanoLoanApi.Core.Core.QueryTransaction(requestreference);
            if (result["CODE"] == "200")
                return Ok(result["RESPONSE"]);
            else
               return NotFound(result["RESPONSE"]); 
        }
        [HttpGet]
        public IActionResult GetBillerPaymentItems()
        {
            logger.Info(" arriving GetBillerPaymentItems " );
            var result = Core.Core.GetBillerPaymentItems();
            if (result["CODE"] == "200")
                return Ok(result["RESPONSE"]);
            else
                return NotFound(result["RESPONSE"]);
        }

      

    [HttpPost]
    public IActionResult SendBillPaymentAdvice([FromBody]BillPaymentAdviceRequest request)
    {
            logger.Info("send bill payment advise" + JsonConvert.SerializeObject(request));
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var result = Core.Core.SendBillPaymentAdvice(request);
        if (result["CODE"] == "200")
            return Ok(result["RESPONSE"]);
        else
            return NotFound(result["RESPONSE"]);
    }

}
}
