using System;
using System.Linq;
using FTPServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace FTPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string key) // kiểm tra máy đã đăng kí license hay chưa?
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
                    if (ret2 == null) // license bị hết hạn
                    {
                        status.Message = "The License expired.";
                        responseLicense.LicenceKeyHis = ret;
                    }
                    else // license còn hạn
                    {
                        ret2.CurrentDate = DateTime.Now;
                        context.Update(ret2);
                        context.SaveChanges();
                        responseLicense.LicenceKeyHis = ret2;
                    }
                    responseLicense.Status = status;

                }
            }
            catch
            {
                status.StatusCode = "400";
                responseLicense.Status = status;
            }
            return Ok(responseLicense);
        }

        [HttpPost]
        public IActionResult Post([FromBody]PmlicenceKeyHis pmlicenceKeyHis) // đăng kí license
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
                            pmlicenceKeyHis.CurrentDate = now;
                            pmlicenceKeyHis.ExpiredDate = now.AddDays(pmLicensekey.DayOfUse);
                            context.Add(pmlicenceKeyHis);
                            context.SaveChanges();
                            response.StatusCode = "200";
                        }
                        else
                        {
                            response.StatusCode = "0"; // Key này đã đăng kí full rồi. không cho đăng kí nữa.
                            response.Message = cnt.ToString();
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