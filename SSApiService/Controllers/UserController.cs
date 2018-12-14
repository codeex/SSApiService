using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSApiService.Models;
using System.Text;
using System.IO;

namespace SSApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public UserToken Post([FromBody]UserParam user)
        {
            var result = new UserToken();

            if(string.IsNullOrEmpty(user?.Uri) || string.IsNullOrEmpty(user?.PostValue))
            {
                result.Result = "传递的参数为空";
                return result;
            }

            try
            {
                using (HttpClient http = new HttpClient())
                {
                    var ss = Encoding.UTF8.GetBytes(user.PostValue);
                    using (Stream dataStream = new MemoryStream(ss ?? new byte[0]))
                    {
                        using (HttpContent content = new StreamContent(dataStream))
                        {
                            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                            var task = http.PostAsync(user.Uri, content);
                            var c = task.Result;
                            if (c != null && c.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                using (c)
                                {
                                    result.Result = c.Content.ReadAsStringAsync().Result;
                                }
                                var cookie = c.Headers.GetValues("Set-Cookie");
                                result.Ssid = string.Join(";", cookie.ToList());
                            }
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                result.Result = ex.Message;
            }

            if (!string.IsNullOrEmpty(result.Ssid))
            {
                var nStart = result.Ssid.IndexOf("SSID=")+ "SSID=".Length;
                var nEnd = result.Ssid.IndexOf(";", nStart);
                result.Ssid = result.Ssid.Substring(nStart, nEnd-nStart);
            }

            return result;

        }
    }
}