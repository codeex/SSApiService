using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SSApiService.Models;

namespace SSApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WmsFileController : ControllerBase
    {
        private IHostingEnvironment _host;
        public WmsFileController(IHostingEnvironment host)
        {
            this._host = host;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<WmsFileInfo> Get(string id)
        {
            var dir = this._host.ContentRootPath;
            dir = Path.Combine(dir, "wms", id);
            var files = Directory.GetFiles(dir);
            if(files.Length <= 0)
            {
                return null;
            }

            string s = string.Empty;
            using (var f = new StreamReader(files[0], Encoding.UTF8))
            {
                s = f.ReadLine();
            }

            return new WmsFileInfo {
                FileContent = s,
                FileId = (new FileInfo(files[0])).Name
            };

        }

        // POST api/values
        [HttpPost]
        public WmsRtn Post([FromBody] WmsFileInfo value)
        {
            if(string.IsNullOrEmpty(value?.FileId) || string.IsNullOrEmpty(value?.FileContent))
            {
                return new WmsRtn
                {
                    Code = 500,
                    Msg = "信息内容为空"
                };
            }

            var dir = this._host.ContentRootPath;
            dir = Path.Combine(dir, "wms",value.FileId);
            //if (Directory.Exists(dir))
            //{
            //    return new WmsRtn
            //    {
            //        Code = 501,
            //        Msg = "文件已经处理过了"
            //    };
            //}
            try
            {
                Directory.CreateDirectory(dir);
                var str = Guid.NewGuid().ToString("N");
                using(var f = new StreamWriter(Path.Combine(dir, str), false,Encoding.UTF8))
                {
                    f.WriteLine(value.FileContent);
                }
                return WmsRtn.Ok;
            }
            catch(Exception ex)
            {
                return new WmsRtn
                {
                    Code = 502,
                    Msg = ex.Message
                };
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5/id
        [HttpDelete("{id}/{fileId}")]
        public void Delete(string id,string fileId)
        {
            try
            {
                var dir = this._host.ContentRootPath;
                dir = Path.Combine(dir, "wms", id);
                var f = Path.Combine(dir, fileId);
                if (!System.IO.File.Exists(f))
                {
                    return;
                }
                //move 
                var dir2 = Path.Combine(dir, "del");
                if (!Directory.Exists(dir2))
                {
                    Directory.CreateDirectory(dir2);
                }
                var f2 = Path.Combine(dir2, fileId);
                if (System.IO.File.Exists(f2))
                {
                    f2 += DateTime.Now.ToString("_HHmmssffff");
                }
                Directory.Move(f, f2);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return;
            }
        }
    }
}
