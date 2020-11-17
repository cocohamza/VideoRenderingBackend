using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VideoRenderingBackend.Services
{
    public class DownloadService
    {
        public void DownloadImage(string url, string saveLocation)
        {
            
            try
            {
                using (WebClient client = new WebClient())
                    client.DownloadFile(url, saveLocation);   
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        public Image DownloadImage(string url)
        {

            try
            {
                using (WebClient client = new WebClient())
                {
                    Stream stream = client.OpenRead(url);
                    Bitmap bitmap; bitmap = new Bitmap(stream);

                    stream.Flush();
                    stream.Close();
                    client.Dispose();

                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
