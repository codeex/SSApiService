using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSApiService.Models
{
    public class WmsRtn
    {
        public int Code { get; set; }
        public string Msg { get; set; }

        public static WmsRtn Ok = new WmsRtn
        {
            Code = 0,
            Msg = string.Empty
        };
    }
}
