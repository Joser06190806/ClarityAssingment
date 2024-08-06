using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailWebServicesApi.Models
{
    public class EmailServices
    {
        public string EmailRecipient { get; set; }
        public string EmailSender { get; set; } 
        public string EmailSubject { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;
    }
}