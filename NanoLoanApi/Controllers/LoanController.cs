using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NanoLoanApi.Core.CustomObjects;
using Newtonsoft.Json;
using NLog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NanoLoanApi.Controllers
{
    [Route("[controller]/[action]")]
    public class LoanController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

       

        [HttpGet]
        public IActionResult GetLoanProviders()
        {
            var result = Core.Core.GetProviders();
            if (result["CODE"] == "200")
                return Ok(result["RESPONSE"]);
            else
                return NotFound(result["RESPONSE"]);
        }

        [HttpPost]
        public IActionResult GetPaymentMethod([FromBody] PaymentMethodRequest request)
        {
            logger.Info("get paymentn mtd" + JsonConvert.SerializeObject(request));
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var result = Core.Core.GetPaymentMethod(request);
            if (result["CODE"] == "200")
                return Ok(result["RESPONSE"]);
            else
                return NotFound(result["RESPONSE"]);
        }


        [HttpPost]
        public IActionResult GetAvailableOffers([FromBody] OfferRequest request)
        {
            logger.Info("get offrs" + JsonConvert.SerializeObject(request));
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var result = Core.Core.GetOffer(request);
            if (result["CODE"] == "200")
                return Ok(result["RESPONSE"]);
            else
                return NotFound(result["RESPONSE"]);
        }

        [HttpPost]
        public IActionResult GetAllAvailableOffers([FromBody] OfferRequest request)
        {
            logger.Info("get all offrs" + JsonConvert.SerializeObject(request));
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var result = Core.Core.GetAllOffer(request);
            if (result["CODE"] == "200")
                return Ok(result["RESPONSE"]);
            else
                return NotFound(result["RESPONSE"]);
        }
        [HttpPost]
        public IActionResult AcceptOffer([FromBody] AcceptOfferRequest request)
        {
            logger.Info("accept offer" + JsonConvert.SerializeObject(request));
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var result = Core.Core.acceptOffers(request);
            if (result["CODE"] == "200")
                return Ok(result["RESPONSE"]);
            else
                return NotFound(result["RESPONSE"]);
        }
    }
}
