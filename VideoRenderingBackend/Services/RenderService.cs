using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoRenderingBackend.Models;
using System.Drawing.Imaging;
using VideoRenderingBackend.Helpers;
using System.Drawing;
using System.Diagnostics;

namespace VideoRenderingBackend.Services
{
    public class RenderService
    {
        /// <summary>
        /// Always end the path with a / extension
        /// </summary>

        private string mainPath = "C:/VideoRendering/";

        private string templatesLocation = "Templates/";
        private string afterEffectsProjectExtension = "aeTemplate01.aep";

        private string saveLocation = "StoreData/";
        private string inputLocation = "Input/";
        private string imageFolderLocation = "Images/";

        private string scriptName = "autoRenderSctipt.jsx";

        private static string aeCommand = "afterfx -r {0}";




        public void GenerateScriptData(VideoImport videoImportData)
        {
            try
            {

                var scriptHead = File.ReadAllText(mainPath + "Scripts/scriptHead.txt");
                var scriptBody = File.ReadAllText(mainPath + "Scripts/scriptBody.txt");

                string titles = JsonConvert.SerializeObject(videoImportData.Titles);
                string textDescriptions = JsonConvert.SerializeObject(videoImportData.Descriptions);
                string oldPrices = JsonConvert.SerializeObject(videoImportData.OldPrices);
                string newPrices = JsonConvert.SerializeObject(videoImportData.NewPrices);


                string filledHeader = string.Format(scriptHead,
                        mainPath + afterEffectsProjectExtension,
                        videoImportData.HeadText,
                        videoImportData.TailText,
                        titles,
                        textDescriptions,
                        oldPrices,
                        newPrices
                    );

                string filledScript = filledHeader + scriptBody;

                string scriptPath = SaveScript(videoImportData.storeId.ToString(), filledScript);
                CopyTemplate(videoImportData.storeId.ToString());

                foreach (var imgStr in videoImportData.Images)
                {
                    SaveImageToLocalFolder(videoImportData.storeId.ToString(), imgStr);
                }


                StartAfterEffectsProcess(scriptPath);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CopyTemplate(string storeFolder)
        {
            string folderPath = Path.Combine(mainPath, saveLocation, storeFolder, inputLocation);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string sourceFileName = Path.Combine(mainPath, templatesLocation, afterEffectsProjectExtension);
            string destinationFileName = Path.Combine(folderPath, afterEffectsProjectExtension);

            if (File.Exists(destinationFileName))
            {
                File.Delete(destinationFileName);
            }
            File.Copy(sourceFileName, destinationFileName);
        }
        private bool SaveImageToLocalFolder(string storeFolder, NameUrlStructure imgStr)
        {
            try
            {
                DownloadService downloadService = new DownloadService();
                string folderPath = Path.Combine(mainPath, saveLocation, storeFolder, inputLocation, imageFolderLocation);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, imgStr.name);

                Image myImage = downloadService.DownloadImage(imgStr.url);

                byte[] imageScaledBytes = ImageHelper.GetScaledCenteredJpgImageByte(myImage, 600, 600);

                saveImage(imageScaledBytes, imgStr.name, folderPath);



                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string SaveScript(string subFolderName, string filledScript)
        {
            try
            {
                string folderPath = Path.Combine(mainPath, saveLocation, subFolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, scriptName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                CreateAndWriteFile(filledScript, filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void saveImage(byte[] imageData, string filename, string path)
        {
            if (imageData != null)
            {
                var byteImage = ImageHelper.GetScaledCenteredJpgImageByte(imageData, 600, 600);

                using (Image image = Image.FromStream(new MemoryStream(byteImage)))
                {
                    image.Save(path + filename, ImageFormat.Png);  // Or jpg
                }
            }
        }
        private static void CreateAndWriteFile(string filledScript, string filePath)
        {
            var fs = File.Create(filePath);
            fs.Close();
            File.WriteAllText(filePath, filledScript);
        }

        private static string StartAfterEffectsProcess(string scriptPath)
        {
            try
            {

                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardError = true;

                // Start the process
                Process proc =  Process.Start(psi);
                StreamReader strm = proc.StandardError;
                // Attach the output for reading
                StreamReader sOut = proc.StandardOutput;

                // Attach the in for writing
                StreamWriter sIn = proc.StandardInput;

                string completeAeCommand =  string.Format(aeCommand, scriptPath);
                sIn.WriteLine(completeAeCommand);
                strm.Close();

                sIn.WriteLine("EXIT");

                proc.Close();

                // Read the sOut to a string.
                string results = sOut.ReadToEnd().Trim();

                // Close the io Streams;
                sIn.Close();
                sOut.Close();

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetRenderedVideo()
        {
            throw new NotImplementedException();
        }
    }
}
