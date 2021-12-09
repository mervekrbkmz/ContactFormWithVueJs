using ContactFormWithVueJs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContactFormWithVueJs.Controllers
{
    public class FormController : Controller
    {
        private readonly ILogger<FormController> _logger;
        private IConfiguration _configuration;

        public FormController(ILogger<FormController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IActionResult FormContact()
        {
            return View();
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> FormContact([FromBody] JObject form)
        {
            if (form != null)
            {
                var adSoyad = form["adSoyad"].ToString();
                var telNo = form["telNo"].ToString();
                var ePosta = form["ePosta"].ToString();
                var meslekSecimi = form["meslekSecimi"].ToString();
                var adres = form["adres"].ToString();
                var aciklama = form["aciklama"].ToString();
                var dogumTarihi = DateTime.Parse(form["dogumTarihi"].ToString());

                try
                {

                    string email_address = _configuration.GetSection("EmailSettings").GetSection("email_address").Value;
                    string email_password = _configuration.GetSection("EmailSettings").GetSection("email_password").Value;
                    string email_address_to = _configuration.GetSection("EmailSettings").GetSection("email_address_to_form").Value;
                    string email_name = _configuration.GetSection("EmailSettings").GetSection("email_name").Value;
                    string email_title = _configuration.GetSection("EmailSettings").GetSection("email_title").Value;
                    string email_host = _configuration.GetSection("EmailSettings").GetSection("email_host").Value;
                    string email_port_without_ssl = _configuration.GetSection("EmailSettings").GetSection("email_port_without_ssl").Value;

                    MailMessage meailit = new MailMessage();
                    //meailit.To.Add(new MailAddress(email_address, email_name));
                    meailit.To.Add(new MailAddress(email_address_to, email_address_to));
                    meailit.From = new MailAddress(email_address, email_name);
                    meailit.Subject = email_title;

                    meailit.Body = "<h2 style=\"text-align:left\"><strong>İletişim Formu</strong></h2><br>" +
                    "<p><u><strong> Ad ve Soyad:</strong></u> " + adSoyad + "</p><br>" +
                    "<p><u><strong> Telefon :</strong></u> " + telNo + "</p><br>" +
                    "<p><u><strong> Mail Adresi:</strong></u> " + ePosta + "</p><br>" +
                    "<p><u><strong> Adres :</strong></u> &nbsp; " + adres + "</p><br>" +
                    "<p><u><strong> Meslek Seçimi :</strong></u> &nbsp; " + meslekSecimi + "</p><br>" +
                    "<p><u><strong> Açıklama :</strong></u> &nbsp; " + aciklama + "</p>"+
                    "<p><u><strong> Doğum Tarihi :</strong></u> &nbsp; " + dogumTarihi + "</p>";

                    meailit.IsBodyHtml = true;
                    meailit.BodyEncoding = System.Text.Encoding.UTF8;
                    SmtpClient client = new SmtpClient();

                    client.Host = email_host;
                    client.Port = Int32.Parse(email_port_without_ssl);
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(email_address, email_password);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;

                    client.SendMailAsync(meailit);
                    return Ok(new { isSuccess = true, message = "Başvurunuz başarıyla gönderildi." });

                }
                catch (Exception ex)
                {

                    return Ok(new {isSuccess=false, message="Bir hata oluştu! "+ex.Message });

                }


                return View();
            }
            else
            {
                return Ok(new { isSuccess = false, message = "Dosya Yükleme Başarısız! "  });
            }


        }
    }
}
