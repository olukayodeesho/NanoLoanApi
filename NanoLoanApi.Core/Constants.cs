using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoLoanApi.Core
{
    public class Constants
    {
        public static string SANDBOX_URL = "https://sandbox.interswitchng.com";
        public static string PRODUCTION_URL = "https://saturn.interswitchng.com";
        public static string DEVELOPMENT_URL = "https://qa.interswitchng.com";



        public static string LENDING_SERVICE_URL = "https://sandbox.interswitchng.com/lending-service/api/v1/";
        public static string LENDING_SERVICE_URLV2 = "https://sandbox.interswitchng.com/lending-service/api/v2/";
        public static string BEARER_AUTHORIZATION_REALM = "Bearer";

        public static string Contenttype = "Content-Type";
        public static string Cachecontrol = "cache-control";
        public static string Authorization = "Authorization";
        public static string ContentType = "application/x-www-form-urlencoded";

        public static string CLIENT_ID = "IKIA9614B82064D632E9B6418DF358A6A4AEA84D7218";
        public static string CLIENT_SECRET = "XCTiBtLy1G9chAnyg0z3BcaFK4cVpwDg/GTw2EmjTZ8=";


        public static String CARD_NAME = "default";
        public static String GET = "GET";
        public static String POST = "POST";
        public static String SECURE_HEADER = "4D";
        //public static String SECURE_FORMAT_VERSION = "11";
        public static String SECURE_FORMAT_VERSION = "12";
        public static String SECURE_MAC_VERSION = "05";
        public static String SECURE_FOOTER = "5A";
        public static String SIGNATURE_HEADER = "Signature";
        public static String SIGNATURE_METHOD_HEADER = "SignatureMethod";
        public static String INTERSWITCH_AUTH = "InterswitchAuth";
        public static String CHANNEL_CODE = "QTUSSD";
        


    }
}