using System;
using System.Linq;
using FTPServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace FTPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckHWKeyController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string key)
        {
            ResponseStatus status = new ResponseStatus();
            ResponseLicense responseLicense = new ResponseLicense();

            try
            {
                using (var context = new PMLicenceDevContext())
                {
                    var ret = context.PmlicenceKeyHis.FirstOrDefault(x => x.Hwkey == key);
                    if (ret == null) // máy chưa đăng ký license
                    {
                        status.StatusCode = "404";
                        responseLicense.Status = status;
                        return Ok(responseLicense);
                    }

                    var ret2 = context.PmlicenceKeyHis.FirstOrDefault(x => x.Hwkey == key && x.ExpiredDate >= DateTime.Now);
                    status.StatusCode = "200";
                    responseLicense.Status = status;
                    if (ret2 == null) // license bị hết hạn
                        responseLicense.LicenceKeyHis = ret;
                    else // license còn hạn
                        responseLicense.LicenceKeyHis = ret2;
                }
            }
            catch
            {
                status.StatusCode = "400";
                responseLicense.Status = status;
            }
            return Ok(responseLicense);
        }

    }
}