using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public  class ApiLinksConfigurations
    {
        public string ApiName { get; set; }

        public string BaseUrl { get; set; }

        public IEnumerable<ApiLink> Links { get; set; }

        public class ApiLink
        {
            public string Key
            {
                get; set;
            }

            public string Path
            {
                get; set;
            }

            /// <summary>
            /// The required permission to access the resource
            public string Permission
            {
                get; set;
            }
        }
    }
}
