using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using RestSharp.Portable;
using RestSharp.Portable.Deserializers;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using NanoLoanApi.Core.CustomObjects;
using NLog;
//using Payment;

namespace NanoLoanApi.Core
{


    public class Core
    {

        public string clientId;
        public string clientSecret;
        public string myAccessToken;
        public string environment;
        public string authData;
        public static string SANDBOX = "SANDBOX";
        public static string PRODUCTION = "PRODUCTION";
        public static string DEV = "DEVELOPMENT";
        public static string HTTP_CODE = "CODE";
        public static string HTTP_RESPONSE = "RESPONSE";
        public string LENDING_SERVICE_URL = "https://sandbox.interswitchng.com/lending-service/api/v1/";
        public string LENDING_SERVICE_URLV2 = "https://sandbox.interswitchng.com/lending-service/api/v2/";
        public string BEARER_AUTHORIZATION_REALM = "Bearer";
        private string currentUserAgent;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public CookieContainer Cookies { get; private set; }

        public Core()
        {

        }
        public Core(String clientId, String clientSecret, String environment = null)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.environment = environment;

        }

        //Controller facing actions : Quickteller
        #region Quickteller
        public static Dictionary<string, string> QueryTransaction(string requestRef)
        {
            var interswitch = new Core(Constants.CLIENT_ID, Constants.CLIENT_SECRET, Constants.SANDBOX_ENV);
            String resourceUrl = "/api/v2/quickteller/transactions?requestreference=" + requestRef;//api/v2/quickteller/transactions?requestreference=14561489752590
            string method = "GET";
            Dictionary<string, string> aditionalHeaders = new Dictionary<string, string>();
            aditionalHeaders.Add("TerminalId", Constants.TerminalID);
            var result = interswitch.SendWithAccessTokenQuickteller(resourceUrl, method, null, aditionalHeaders, null);
            return result; 

        }
        public static Dictionary<string, string> GetBillerPaymentItems()
        {
            var interswitch = new Core(Constants.CLIENT_ID, Constants.CLIENT_SECRET, Constants.SANDBOX_ENV);
            String resourceUrl = "/api/v2/quickteller/billers/14267/paymentitems";

            string method = "GET";
            Dictionary<string, string> aditionalHeaders = new Dictionary<string, string>();
            aditionalHeaders.Add("TerminalId", Constants.TerminalID);
            var result = interswitch.SendWithAccessTokenQuickteller(resourceUrl, method, null, aditionalHeaders, null);
            Console.WriteLine("  >>>>result   :    " + result);
            return result;
        }

        public static Dictionary<string, string> SendBillPaymentAdvice(BillPaymentAdviceRequest request)
        {

           
            var interswitch = new Core(Constants.CLIENT_ID, Constants.CLIENT_SECRET, Constants.SANDBOX_ENV);


            String resourceUrl = "/api/v2/quickteller/payments/advices";


            JObject json = new JObject();
            json.Add("TerminalId", Constants.TerminalID);
            // json.Add("paymentCode", "051426701");
            //json.Add("paymentCode", "10401");
            //json.Add("customerId", "0000000001");
            //json.Add("customerMobile", "2348056731576");
            //json.Add("customerEmail", "iswtester2@yahoo.com");
            //json.Add("amount", "1460000");
            json.Add("paymentCode", request.paymentCode);
            json.Add("customerId", request.customerId);
            json.Add("customerMobile", request.customerMobileNo);
            json.Add("customerEmail", request.customerEmail);
            json.Add("amount", request.amount);
            json.Add("requestReference", "1453" + ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds)).ToString());

            string method = "POST";
            Dictionary<string, string> aditionalHeaders = new Dictionary<string, string>();
            aditionalHeaders.Add("TerminalId", Constants.TerminalID);
            var result = interswitch.SendWithAccessTokenQuickteller(resourceUrl, method, json, aditionalHeaders, null);
            //Console.WriteLine("here is the result CODE  :    " + result["CODE"]);
            //Console.WriteLine("here is the result RESPONSE :    " + result["RESPONSE"]);
            return result;

        }
        #endregion


        #region NanoLoan

        public static Dictionary<string, string> GetProviders()
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            try
            {
                var interswitch = new Core();
                string accessToken = interswitch.GetClientAccessToken(Constants.CLIENT_ID, Constants.CLIENT_SECRET).Result.access_token;
                String resourceUrl = Constants.LENDING_SERVICE_URL + "offers/providers" + "channelCode=" + Constants.CHANNEL_CODE; // interswitch.LENDING_SERVICE_URL + "offers?customerId=" + customerId + "&channelCode=" + channelCode + "&providerCode=" + providerCode + "&amount=" + amount;
                string authorization = $"{Constants.BEARER_AUTHORIZATION_REALM} {accessToken}";
                response = interswitch.SendILS(resourceUrl, authorization);
            }catch(Exception e) { logger.Debug(e); }
            return response; 

        }

        public static Dictionary<string, string> GetPaymentMethod(PaymentMethodRequest req)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            try
            {
                var interswitch = new Core();
                string accessToken = interswitch.GetClientAccessToken(Constants.CLIENT_ID, Constants.CLIENT_SECRET).Result.access_token;
                String resourceUrl = Constants.LENDING_SERVICE_URL + "users/" + req.customerId + "/payment-methods?channelCode=" + Constants.CHANNEL_CODE;
                string authorization = $"{Constants.BEARER_AUTHORIZATION_REALM} {accessToken}";
                response = interswitch.SendILS(resourceUrl, authorization);
            }
            catch (Exception e) { logger.Debug(e); }
            return response;

        }
        public static Dictionary<string, string> GetOffer(OfferRequest req)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            try
            {
                var interswitch = new Core();
                string accessToken = interswitch.GetClientAccessToken(Constants.CLIENT_ID, Constants.CLIENT_SECRET).Result.access_token;
                String resourceUrl = Constants.LENDING_SERVICE_URL + "offers?customerId=" + req.customerId + "&channelCode=" + Constants.CHANNEL_CODE + "&providerCode=" + req.ProviderCode + "&amount=" + req.Amount ;// interswitch.LENDING_SERVICE_URL + "offers?customerId=" + customerId + "&channelCode=" + channelCode + "&providerCode=" + providerCode + "&amount=" + amount;

                string authorization = $"{Constants.BEARER_AUTHORIZATION_REALM} {accessToken}";
                response = interswitch.SendILS(resourceUrl, authorization);
            }
            catch (Exception e) { logger.Debug(e); }
            return response;

        }

        public static Dictionary<string, string> acceptOffers( AcceptOfferRequest acceptOfferRequest)
        {
            var interswitch = new Core();
            var response = new Dictionary<string, string>();
            try
            {
                String resourceUrl = Constants.LENDING_SERVICE_URL + "offers/" + acceptOfferRequest.offerId + "/accept";
                Console.WriteLine("Accept Url ===============================" + resourceUrl);
                string accessToken = interswitch.GetClientAccessToken(Constants.CLIENT_ID, Constants.CLIENT_SECRET).Result.access_token;
                String authData = interswitch.GetAuthData(acceptOfferRequest.pan, acceptOfferRequest.expiryDate, acceptOfferRequest.cvv, acceptOfferRequest.pin);
                //This is for the authoirization
                string authorization = $"{Constants.BEARER_AUTHORIZATION_REALM} {accessToken}";

                JObject json = new JObject();
                json.Add("customerId", acceptOfferRequest.customerId);
                json.Add("channelCode", Constants.CHANNEL_CODE);
                json.Add("providerCode", acceptOfferRequest.providerCode);

                JObject debitMethod = new JObject();
                debitMethod.Add("authData", authData);

                json.Add("debitMethod", debitMethod);

                JObject creditMethod = new JObject();
                creditMethod.Add("accountNumber", acceptOfferRequest.accountNumber);
                creditMethod.Add("bankCode", acceptOfferRequest.bankCode);

                json.Add("creditMethod", creditMethod);
                String jsonData = json.ToString();

                Console.WriteLine(jsonData);

                // interswitch.SendILSPost(json, resourceUrl, authorization);
                response = interswitch.SendILSPost2(json, resourceUrl, authorization);
            }
            catch (Exception e) { logger.Debug(e); }
            return response;
        }


        #endregion
        public String getPassportUrl(String env)
        {
            if (env == null)
            {
                // return Constants.SANDBOX_URL;//default to sandbox

                return "https://sandbox.interswitchng.com";
            }
            if (env.Equals(PRODUCTION, StringComparison.OrdinalIgnoreCase))
            {
                return Constants.PRODUCTION_URL;
            }
            else if (env.Equals(SANDBOX, StringComparison.OrdinalIgnoreCase))
            {
                // return Constants.SANDBOX_URL;
                return "https://sandbox.interswitchng.com";
            }
            else if (env.Equals(DEV, StringComparison.OrdinalIgnoreCase))
            {
                return "https://qa.interswitchng.com";
            }
            else
            {
                return null;
            }
        }


        public String getToken()
        {
            Token accessToken = GetClientAccessToken(this.clientId, this.clientSecret).Result;
            Console.WriteLine("This is the accessToken =====" + accessToken);
            return accessToken.access_token;
        }

        /*
        public long getTimeStamp()
        {
            Config config = new Config();
            return config.GetTimeStamp();
        }
        public String getSignature()
        {
            Config config = new Config();
            return config.GetSignature();
        }
        public String getNonce()
        {
            Config config = new Config();
            return config.GetNonce();
        }
        */

        public virtual async Task<Token> GetClientAccessToken(String ClientId, String ClientSecret)
        {
            string url = getPassportUrl(environment);

            url = String.Concat(url, "/passport/oauth/token");
            Console.WriteLine("This is the Url =====" + url);


            RestClient client = new RestClient(url);
            client.IgnoreResponseStatusCode = true;

            var request = new RestRequest(url, HttpMethod.Post);
            request.AddHeader(Constants.Contenttype, Constants.ContentType);
            request.AddHeader(Constants.Authorization, setAuthorization(ClientId, ClientSecret));
            request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
            request.AddParameter("Scope", "profile", ParameterType.GetOrPost);

            JsonDeserializer deserial = new JsonDeserializer();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = await client.Execute(request);

            HttpStatusCode httpStatusCode = response.StatusCode;
            int numericStatusCode = (int)httpStatusCode;
            Token passportResponse = new Token();
            // Console.WriteLine(passportResponse);


            if (numericStatusCode == 200)
            {
                passportResponse = deserial.Deserialize<Token>(response);
                passportResponse.setAccessToken(passportResponse.access_token);
            }
            else if (response.ContentType == "text/html" || (numericStatusCode == 401 || numericStatusCode == 404 || numericStatusCode == 502 || numericStatusCode == 504))
            {
                passportResponse.ErrorCode = numericStatusCode.ToString();
                passportResponse.ErrorMessage = response.StatusDescription;
            }
            else
            {
                var errorResponse = deserial.Deserialize<ErrorResponse>(response);
                passportResponse.ErrorCode = errorResponse.error.code;
                passportResponse.ErrorMessage = errorResponse.error.message;
            }
            Console.WriteLine("=====================================" + passportResponse);
            return passportResponse;
        }


        //public Dictionary<string, string> Send(String uri, String httpMethod, object data = null, Dictionary<string, string> headers = null, String signedParameters = null)
        //{
        //    Dictionary<string, string> dictionary = new Dictionary<string, string>();
        //    try
        //    {
        //        Token token = GetClientAccessToken(Constants.CLIENT_ID, Constants.CLIENT_SECRET).Result;
        //        var accessToken = token.access_token;
        //        return SendWithAccessToken(uri, httpMethod, accessToken, data, headers, signedParameters);

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>" +ex.StackTrace.ToString());
        //    }
        //    return dictionary;
        //}

        public Dictionary<string, string> SendILS(string resourceUrl, string authorization)
        {
            Dictionary<string, string> responseMap = new Dictionary<string, string>();

            //this should be used when on interswitch network
            //Proxy proxy = new Proxy(Proxy.Type.HTTP, new InetSocketAddress("172.16.10.20",8080));
            //  HttpURLConnection con = (HttpURLConnection) obj.openConnection(proxy);

            //This should be used when not on interswitch network-----For HttpURLConnection
            //HttpMethod httpMethodObj = (httpMethod == null || httpMethod.Equals("")) ? HttpMethod.Get : new HttpMethod(httpMethod);
            IRestResponse response = null;

            RestClient con = new RestClient(resourceUrl);
            con.IgnoreResponseStatusCode = true;

            RestRequest request = new RestRequest(resourceUrl, HttpMethod.Get);
            request.AddHeader("Authorization", authorization);
            response = con.Execute(request).Result;


            int responseCode = (int)response.StatusCode;

            responseMap.Add(HTTP_CODE, responseCode.ToString());
            responseMap.Add(HTTP_RESPONSE, System.Text.Encoding.UTF8.GetString(response.RawBytes));

            return responseMap;
        }

        public Dictionary<string, string> SendILSPost2(JObject json, string resourceUrl, string authorization)
        {
            Dictionary<string, string> responseMap = new Dictionary<string, string>();
            try
            {
                
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorization);//
                    //client.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType, "application/json");
                    var postData = JsonConvert.SerializeObject(json);
                    var data = new StringContent(postData, Encoding.UTF8, "application/json");

                    var httpResponseMsg = client.PostAsync(resourceUrl, data).Result;

                    int responseCode = (int)httpResponseMsg.StatusCode;

                    responseMap.Add(HTTP_CODE, responseCode.ToString());
                    responseMap.Add(HTTP_RESPONSE, httpResponseMsg.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {

            }
            return responseMap;
        }

        public void SendILSPost(JObject json, string resourceUrl, string authorization)
        {
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.Authorization, authorization);
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            var postData = JsonConvert.SerializeObject(json);

            //var response = client.UploadString(resourceUrl, postData);
            //var response = client.UploadStringAsync(resourceUrl, postData);
            /*Task.Factory.StartNew(() =>
            {
                string returnVal = client.UploadString(resourceUrl, postData);
                Console.WriteLine(returnVal);
            });
            */
            //Console.WriteLine(response.ToString());

        }


        public Dictionary<string, string> SendWithAccessToken(String uri, String httpMethod, object data = null, Dictionary<string, string> headers = null, String signedParameters = null)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                string url = getUrl(environment);
                url = String.Concat(url, uri);

                Console.WriteLine("===============================Url from  SendwithAccessToken " + url);

                RestClient client = new RestClient(url);
                client.IgnoreResponseStatusCode = true;
                IRestResponse response = null;
                Config authConfig = new Config(httpMethod, url, Constants.CLIENT_ID, Constants.CLIENT_SECRET);

                HttpMethod httpMethodObj = (httpMethod == null || httpMethod.Equals("")) ? HttpMethod.Get : new HttpMethod(httpMethod);

                var paymentRequests = new RestRequest(url, httpMethodObj);
                paymentRequests.AddHeader(Constants.Contenttype, "application/json");
                paymentRequests.AddHeader("Signature", authConfig.Signature);
                paymentRequests.AddHeader("SignatureMethod", "SHA1");
                paymentRequests.AddHeader("Timestamp", authConfig.TimeStamp);
                paymentRequests.AddHeader("Nonce", authConfig.Nonce);
                paymentRequests.AddHeader("Authorization", authConfig.GetInterswitchAuth(Constants.CLIENT_ID));
                if (headers != null && headers.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> entry in headers)
                    {
                        paymentRequests.AddHeader(entry.Key, entry.Value);
                    }
                }

                if (data != null)
                {
                    paymentRequests.AddJsonBody(data);
                }

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //JsonDeserializer deserial = new JsonDeserializer();
                //try
                //{
                response = client.Execute(paymentRequests).Result;

                /*
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());
                    throw ex;
                }
                */

                HttpStatusCode httpStatusCode = response.StatusCode;
                int numericStatusCode = (int)httpStatusCode;
                Dictionary<string, string> responseObject = new Dictionary<string, string>();
                responseObject.Add(HTTP_CODE, numericStatusCode.ToString());
                responseObject.Add(HTTP_RESPONSE, System.Text.Encoding.UTF8.GetString(response.RawBytes));

                return responseObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>" + ex.StackTrace.ToString());
            }
            return dictionary;
        }

        public Dictionary<string, string> SendWithAccessTokenQuickteller(String uri, String httpMethod, object data = null, Dictionary<string, string> headers = null, String signedParameters = null)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                string url = getUrl(environment);
                url = String.Concat(url, uri);

                Console.WriteLine("===============================Url from  SendwithAccessToken " + url);

                RestClient client = new RestClient(url);
                client.IgnoreResponseStatusCode = true;
                IRestResponse response = null;
                Config authConfig = new Config(httpMethod, url, Constants.CLIENT_ID, Constants.CLIENT_SECRET);

                HttpMethod httpMethodObj = (httpMethod == null || httpMethod.Equals("")) ? HttpMethod.Get : new HttpMethod(httpMethod);

                var paymentRequests = new RestRequest(url, httpMethodObj);
                paymentRequests.AddHeader(Constants.Contenttype, "application/json");
                paymentRequests.AddHeader("Signature", authConfig.Signature);
                paymentRequests.AddHeader("SignatureMethod", "SHA1");
                paymentRequests.AddHeader("Timestamp", authConfig.TimeStamp);
                paymentRequests.AddHeader("Nonce", authConfig.Nonce);
                paymentRequests.AddHeader("Authorization", authConfig.Authorization);
                if (headers != null && headers.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> entry in headers)
                    {
                        paymentRequests.AddHeader(entry.Key, entry.Value);
                    }
                }

                if (data != null)
                {
                    paymentRequests.AddJsonBody(data);
                }
                Console.WriteLine("  paymentRequests =====" + JsonConvert.SerializeObject(paymentRequests));

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                
                response = client.Execute(paymentRequests).Result;
               

                HttpStatusCode httpStatusCode = response.StatusCode;
                int numericStatusCode = (int)httpStatusCode;
                Dictionary<string, string> responseObject = new Dictionary<string, string>();
                responseObject.Add(HTTP_CODE, numericStatusCode.ToString());
                responseObject.Add(HTTP_RESPONSE, System.Text.Encoding.UTF8.GetString(response.RawBytes));

                return responseObject;
            }
            catch (Exception ex)
            {
                logger.Debug(">>>>>>>>>>>>>>>>>>>>" + ex.StackTrace.ToString());
            }
            return dictionary;
        }
        public Dictionary<string, string> SendWithAccessTokenPost(String uri, String httpMethod, object data, Dictionary<string, string> headers = null, String signedParameters = null)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                string url = getUrl(environment);
                url = String.Concat(url, uri);

                Console.WriteLine("===============================Url from  SendwithAccessToken " + url);

                RestClient client = new RestClient(url);
                client.IgnoreResponseStatusCode = true;
                IRestResponse response = null;
                Config authConfig = new Config(httpMethod, url, Constants.CLIENT_ID, Constants.CLIENT_SECRET);

                HttpMethod httpMethodObj = (httpMethod == null || httpMethod.Equals("")) ? HttpMethod.Get : new HttpMethod(httpMethod);

                var requests = new RestRequest(url, httpMethodObj);
                requests.AddHeader(Constants.Contenttype, "application/json");
                requests.AddHeader("Signature", authConfig.Signature);
                requests.AddHeader("SignatureMethod", "SHA1");
                requests.AddHeader("Timestamp", authConfig.TimeStamp);
                requests.AddHeader("Nonce", authConfig.Nonce);
                requests.AddHeader("Authorization", authConfig.GetInterswitchAuth(Constants.CLIENT_ID));

                if (headers != null && headers.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> entry in headers)
                    {
                        requests.AddHeader(entry.Key, entry.Value);
                    }
                }

                if (data != null)
                {
                    requests.AddJsonBody(data);

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //JsonDeserializer deserial = new JsonDeserializer();
                    response = client.Execute(requests).Result;

                    HttpStatusCode httpStatusCode = response.StatusCode;
                    int numericStatusCode = (int)httpStatusCode;
                    Dictionary<string, string> responseObject = new Dictionary<string, string>();
                    responseObject.Add(HTTP_CODE, numericStatusCode.ToString());
                    responseObject.Add(HTTP_RESPONSE, System.Text.Encoding.UTF8.GetString(response.RawBytes));

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>" + ex.StackTrace.ToString());
            }
            return dictionary;
        }


        public String GetAuthData(string pan, string expiryDate, string cvv, string pin, string mod = null, string pubExpo = null)
        {
            authData = SecurityUtils.GetAuthData(pan, pin, expiryDate, cvv, mod, pubExpo);
            return authData;
        }

        public Dictionary<string, string> GetSecureData(string pan, string expDate, string cvv, string pin, string amt = null, string msisdn = null, string ttid = null)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            Dictionary<string, string> pinData = new Dictionary<string, string>();

            options.Add("pan", pan);
            options.Add("ttId", ttid);
            options.Add("amount", amt);
            options.Add("mobile", msisdn);

            pinData.Add("pin", pin);
            pinData.Add("cvv", cvv);
            pinData.Add("expiry", expDate);

            Dictionary<string, string> secure = SecurityUtils.generateSecureData(options, pinData);

            return secure;
        }




        private static String setAuthorization(String clientId, String clientSecret)
        {
            try
            {
                String Auth;
                byte[] bytes;
                bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", clientId, clientSecret));
                Auth = Convert.ToBase64String(bytes);
                return String.Concat("Basic ", " " + Auth);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to encode parameters, Please contact connect@interswitchng.com. for assistance.", e);
            }
        }

        private static String getUrl(String environment)
        {
            string url = Constants.SANDBOX_URL;
            if (PRODUCTION.Equals(environment, StringComparison.OrdinalIgnoreCase))
            {
                url = Constants.PRODUCTION_URL;
            }
            else if (DEV.Equals(environment, StringComparison.OrdinalIgnoreCase))
            {
                url = Constants.DEVELOPMENT_URL;
            }
            return url;
        }

    }

    public class Token
    {

        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
        public string requestor_id { get; set; }
        public string merchant_code { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string payable_id { get; set; }
        public string payment_code { get; set; }
        public string jti { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public void setAccessToken(string token)
        {
            this.access_token = token;
        }

        public static implicit operator Task<object>(Token v)
        {
            throw new NotImplementedException();
        }
    }
    public class Error1
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Error2
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class ErrorResponse
    {
        public List<Error1> errors { get; set; }
        public Error2 error { get; set; }
        public string transactionRef { get; set; }
    }
}
