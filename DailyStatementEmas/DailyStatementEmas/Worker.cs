using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static DailyStatementEmas.SSRSExcell;
using static DailyStatementEmas.SendEmail;
using System.IO;
using RestSharp;
using Microsoft.Extensions.Configuration;

namespace DailyStatementEmas
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var jam = DateTime.Now.Hour;
                if (jam == 9)
                {
                    try
                    {
                        Console.WriteLine("start");
                        monitoringServices("DOP_DailyStatementEmas", "I", "Send email daily statement emas");
                        var context = new Data();
                        var vendorcode = context.app_VendorOutlet.Where(a => a.OutletState == 1).Select(b => new { b.OutletCounterPartCode, b.OutletName }).ToList();
                        foreach (var item in vendorcode)
                        {
                            List<string> to_email = new List<string>();
                            List<string> cc_email = new List<string>();
                            List<path> path_list = new List<path>();

                            //string filename = "Daily Statement Emas Off " + item.OutletName + " 29 Okt 2022.xlsx";
                            //string path = getReportSSRSExcel("RptDailyStatement", "&datestart=2022-10-29&dateend=2022-10-29&vendorcode=" + item.OutletCounterPartCode, filename, "EmassOff");

                            string filename = "Daily Statement Emas Off " + item.OutletName + " " + DateTime.Now.AddDays(-1).ToString("dd MMMM yyyy") + ".xlsx";
                            string path = getReportSSRSExcel("RptDailyStatement", "&datestart=" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "&dateend=" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "&vendorcode=" + item.OutletCounterPartCode, filename, "EmassOff");

                            var data_file = new path();
                            data_file.pathname = path;
                            data_file.filename = filename;
                            path_list.Add(data_file);

                            if (item.OutletCounterPartCode == "150")
                            {
                                var email_pg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ParameterEmail")["email_pg"].Split(",");
                                to_email.Add(email_pg[0]);
                                for (int i = 1; i < email_pg.Length; i++)
                                {
                                    cc_email.Add(email_pg[i]);
                                }
                            }
                            else if (item.OutletCounterPartCode == "152")
                            {
                                var email_se = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ParameterEmail")["email_se"].Split(",");
                                to_email.Add(email_se[0]);
                                for (int i = 1; i < email_se.Length; i++)
                                {
                                    cc_email.Add(email_se[i]);
                                }
                            }
                            string file = AppDomain.CurrentDomain.BaseDirectory + "\\index.html";
                            string text = System.IO.File.ReadAllText(file);
                            text = text.Replace("#name#", item.OutletName);
                            text = text.Replace("#tanggal#", DateTime.Now.AddDays(-1).ToString("dd MMMM yyyy"));
                            //text = text.Replace("#tanggal#", "29 oktober 2022");
                            //sendEmail(to_email, cc_email, path_list, "Daily Statement Emas Off Tanggal 29 Oktober 2022", text);
                            sendEmail(to_email, cc_email, path_list, "Daily Statement Emas Off Tanggal " + DateTime.Now.AddDays(-1).ToString("dd MMMM yyyy"), text);


                        }
                    }
                    catch (Exception ex)
                    {
                        monitoringServices("DOP_DailyStatementEmas", "E", "Send email daily statement emas eror : " + ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(3600000, stoppingToken);
            }
        }
        private static string monitoringServices(string servicename, string status, string desc)
        {
            string jsonString = "{" +
                                "\"name\" : \"" + servicename + "\"," +
                                "\"logstatus\": \"" + status + "\"," +
                                "\"logdesc\":\"" + desc + "\"," +
                                "}";
            var client = new RestClient("https://apiservicekbi.azurewebsites.net/api/ServiceStatus");

            RestRequest requestWa = new RestRequest("https://apiservicekbi.azurewebsites.net/api/ServiceStatus", Method.Post);
            requestWa.Timeout = -1;
            requestWa.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            requestWa.AddParameter("data", jsonString);
            var responseWa = client.ExecutePostAsync(requestWa);
            return (responseWa.Result.Content);
        }

    }
}
