using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FTPServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterLicenseController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody]PmlicenceKeyHis pmlicenceKeyHis)
        {
            ResponseStatus response = new ResponseStatus();

            try
            {
                using (var context = new PMLicenceDevContext())
                {
                    var pmLicensekey = context.PmlicenceKey.FirstOrDefault(x => x.PublicKey == pmlicenceKeyHis.PublicKey);
                    if (pmLicensekey == null)
                        response.StatusCode = "404"; // Key không tồn tại
                    else
                    {
                        int cnt = context.PmlicenceKeyHis.Count(x => x.PublicKey == pmLicensekey.PublicKey);
                        if (cnt < pmLicensekey.LimitActived)
                        {
                            DateTime now = DateTime.Now;
                            pmlicenceKeyHis.ActivedDate = now;
                            pmlicenceKeyHis.ExpiredDate = now.AddDays(pmLicensekey.DayOfUse);
                            context.Add(pmlicenceKeyHis);
                            context.SaveChanges();
                            response.StatusCode = "200";
                        }
                        else
                        {
                            response.StatusCode = "0"; // Key này đã đăng kí full rồi. không cho đăng kí nữa.
                            response.Message = "5"; 
                        }

                    }

                }
            }
            catch
            {
                response.StatusCode = "400";
            }
            return Ok(response);
        }
    }
}