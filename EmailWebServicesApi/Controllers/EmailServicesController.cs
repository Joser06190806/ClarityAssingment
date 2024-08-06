using EmailWebServicesApi.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EmailServicesLib;

namespace EmailWebServicesApi.Controllers
{
    public class EmailServicesController : ApiController
    {
        // GET api/EmailServices
        public string Get()
        {
            return "Email Services API started....";
        }

        // POST api/EmailServices
        public async Task<string> Post([FromBody]EmailServices emailServices)
        {
            EmailServicesLib.Services services = new EmailServicesLib.Services();

            string result = await services.SendEmail(emailServices.EmailSender, emailServices.EmailRecipient, emailServices.EmailSubject, emailServices.EmailBody );
            return result;
        }
    }
}
