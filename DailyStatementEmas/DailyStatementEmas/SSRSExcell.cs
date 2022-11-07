using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DailyStatementEmas
{
    public class SSRSExcell
    {
        public static string getReportSSRSExcel(string reportname, string param, string filename, string pathreport)
        {
            try
            {
                string url = "http://10.12.5.60/ReportServerEOD?/" + pathreport + "/" + reportname + "&rs:Command=Render&rs:Format=EXCELOPENXML&rc:OutputFormat=XLS" + param;


                System.Net.HttpWebRequest Req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                Req.Timeout = 36000000;
                Req.Credentials = new NetworkCredential("Administrator", "Jakarta01");
                Req.Method = "GET";

                string path = AppDomain.CurrentDomain.BaseDirectory + "report\\" + filename + ".xlsx";

                System.Net.WebResponse objResponse = Req.GetResponse();
                
                System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                System.IO.Stream stream = objResponse.GetResponseStream();

                byte[] buf = new byte[1024];
                int len = stream.Read(buf, 0, 1024);
                while (len > 0)
                {
                    fs.Write(buf, 0, len);
                    len = stream.Read(buf, 0, 1024);
                }
                stream.Close();
                fs.Close();
                return path;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
