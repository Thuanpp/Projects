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
        public IActionResult Get(string key, string  version) // kiểm tra máy đã đăng kí license hay chưa?
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
                        ret2.SoftwareVersion = version;
                        context.Update(ret2);
                        responseLicense.LicenceKeyHis = ret2;
                        context.SaveChanges();
                        var PublicKeyNavigation = context.PmlicenceKey.FirstOrDefault(x => x.PublicKey == ret2.PublicKey);
                        if (PublicKeyNavigation != null)
                        {
                            responseLicense.LicenceKeyHis.PublicKeyNavigation = new PmlicenceKey();
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.MaxVersion = PublicKeyNavigation.MaxVersion;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.CusEmail = PublicKeyNavigation.CusEmail;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.CusName = PublicKeyNavigation.CusName;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.CusPhone = PublicKeyNavigation.CusPhone;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.DayOfUse = PublicKeyNavigation.DayOfUse;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.LimitActived = PublicKeyNavigation.LimitActived;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.RootPath = PublicKeyNavigation.RootPath;
                        }

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
            ResponseStatus status = new ResponseStatus();
            ResponseLicense responseLicense = new ResponseLicense();

            try
            {
                using (var context = new PMLicenceDevContext())
                {
                    var pmLicensekey = context.PmlicenceKey.FirstOrDefault(x => x.PublicKey == pmlicenceKeyHis.PublicKey);
                    if (pmLicensekey == null)
                    {
                        status.StatusCode = "404"; // Key không tồn tại
                        responseLicense.Status = status;
                    }
                    else
                    {
                        var key = context.PmlicenceKeyHis.
                            FirstOrDefault(x => x.PublicKey == pmlicenceKeyHis.PublicKey
                                            & x.Hwkey == pmlicenceKeyHis.Hwkey);

                        // xoa Key het han
                        if(key != null)
                        {
                            context.Remove(key);
                            context.SaveChanges();
                        }


                        int cnt = context.PmlicenceKeyHis.Count(x => x.PublicKey == pmLicensekey.PublicKey);
                        if (cnt < pmLicensekey.LimitActived)
                        {
                            DateTime now = DateTime.Now;
                            pmlicenceKeyHis.ActivedDate = now;
                            pmlicenceKeyHis.CurrentDate = now;
                            pmlicenceKeyHis.ExpiredDate = now.AddDays(pmLicensekey.DayOfUse);
                            context.Add(pmlicenceKeyHis);
                            context.SaveChanges();
                            status.StatusCode = "200";
                            responseLicense.Status = status;
                            responseLicense.LicenceKeyHis = pmlicenceKeyHis;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation = new PmlicenceKey();
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.MaxVersion = pmLicensekey.MaxVersion;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.CusEmail = pmLicensekey.CusEmail;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.CusName = pmLicensekey.CusName;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.CusPhone = pmLicensekey.CusPhone;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.DayOfUse = pmLicensekey.DayOfUse;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.LimitActived = pmLicensekey.LimitActived;
                            responseLicense.LicenceKeyHis.PublicKeyNavigation.RootPath = pmLicensekey.RootPath;
                        }
                        else
                        {
                            status.StatusCode = "0"; // Key này đã đăng kí full rồi. không cho đăng kí nữa.
                            status.Message = cnt.ToString();
                            responseLicense.Status = status;
                        }

                    }

                }
            }
            catch(Exception ex)
            {
                status.StatusCode = "400";
                responseLicense.Status = status;
            }
            return Ok(responseLicense);
        }

    }
}