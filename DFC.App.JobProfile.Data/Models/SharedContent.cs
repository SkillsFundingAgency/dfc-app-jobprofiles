using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.Data.Models
{
    public class SharedContent
    {
        public string ContentIds { get; set; }

        public string UrlExtension { get; } = "/content/getcontent/api/execute/html/";
    }
}
