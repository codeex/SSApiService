using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSApiService.Models
{
    public class UserParam
    {
        public string Uri { get; set; }
        public string PostValue { get; set; }
    }

    public class UserToken
    {
        public string Result { get; set; }
        public string Ssid { get; set; }
    }
}
