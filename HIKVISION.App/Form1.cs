using Domain;
using Domain.Models.FCSearch;
using Domain.Models.FDSearch;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Domain.Models.ResponseStatusDelete;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HIKVISION.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void getPersonMarks(object sender, EventArgs e)
        {

            var authData = new AuthData
            {
                Host = "192.168.16.81",
                APIUrl = "http://192.168.16.81/",
                User = "admin",
                Pass = "webmaster-123"
            };

            string urlMethod = "http://192.168.16.81/ISAPI/Intelligent/FDLib/FCSearch";
            string objectRequest = "" +
                "<?xml version=\"1.0\" " +
                "encoding=\"utf-8\"?>" +
                    "<FCSearchDescription>" +
                        "<searchID>587ff6c1-1f28-457a-85f9-89cc1004ae0d</searchID>" +
                        "<snapStartTime>2022-11-27T00:00:00Z</snapStartTime>" +
                        "<snapEndTime>2022-12-05T23:59:59Z</snapEndTime>" +
                        "<maxResults>500</maxResults>" +
                        "<searchResultPosition>1</searchResultPosition>" +
                        "<ChannelList>" +
                            "<Channel>" +
                                "<channelID>1</channelID>" +
                            "</Channel>" +
                        "</ChannelList>" +
                   "</FCSearchDescription>";

            var response = HttpMethod.SendXML(urlMethod, objectRequest, authData);

            FCSearchResult fcSearchResult = new FCSearchResult();

            XmlSerializer xmlSerializer = new XmlSerializer(fcSearchResult.GetType());

            StringReader stringReader = new StringReader(response);

            XmlTextReader xmlReader = new XmlTextReader(stringReader);

            fcSearchResult = (FCSearchResult)xmlSerializer.Deserialize(xmlReader);

            xmlReader.Close();
            stringReader.Close();

            string idPerson = IdPerson.Text;
            Domain.Models.FCSearch.MatchElement matchElement = null;

            if (fcSearchResult != null && fcSearchResult.MatchList != null)
                matchElement = fcSearchResult.MatchList.Where(m => m.FaceMatchInfoList[0].CertificateNumber.ToString() == idPerson).FirstOrDefault();

            if (matchElement == null)
            {
                MessageBox.Show("Persona no encontrada");
                return;
            }

            string imageBase64 = HttpGetImageDataFromUrl.GetBase64ImageFromUrl(matchElement.FaceMatchInfoList[0].PicURL, authData);

            var picture = Convert.FromBase64String(imageBase64);

            Image image = Image.FromStream(new MemoryStream(picture));

            pictureBox1.Image = image;

        }

        private void getPerson(object sender, EventArgs e)
        {
            var authData = new AuthData
            {
                Host = "192.168.16.81",
                APIUrl = "http://192.168.16.81/",
                User = "admin",
                Pass = "webmaster-123"
            };

            string urlMethod = "http://192.168.16.81/ISAPI/Intelligent/FDLib/FDSearch";
            string objectRequest = "<?xml version: \"1.0\" encoding =\"utf -8\" ?><FDSearchDescription><FDID>1</FDID><startTime>1970-01-01</startTime><endTime>2023-07-10</endTime><searchID>C3E2ADCD-F43F-4F9A-BB29-71B7E26101A9</searchID><maxResults>50</maxResults><searchResultPosition>1</searchResultPosition></FDSearchDescription>";
            var response = HttpMethod.SendXML(urlMethod, objectRequest, authData);

            FDSearchResult fdSearchResult = new FDSearchResult();

            XmlSerializer xmlSerializer = new XmlSerializer(fdSearchResult.GetType());

            StringReader stringReader = new StringReader(response);

            XmlTextReader xmlReader = new XmlTextReader(stringReader);

            fdSearchResult = (FDSearchResult)xmlSerializer.Deserialize(xmlReader);

            xmlReader.Close();
            stringReader.Close();

            string idPerson = IdPerson.Text;
            Domain.Models.FDSearch.MatchElement matchElement = null;

            if (fdSearchResult != null && fdSearchResult.MatchList != null)
                matchElement = fdSearchResult.MatchList.Where(m => m.CertificateNumber.ToString() == idPerson).FirstOrDefault();

            if (matchElement == null){
                MessageBox.Show("Persona no encontrada");
                return;
            }

            string imageBase64 = HttpGetImageDataFromUrl.GetBase64ImageFromUrl(matchElement.PicURL, authData);

            var picture = Convert.FromBase64String(imageBase64);

            Image image = Image.FromStream(new MemoryStream(picture));

            pictureBox1.Image = image;

        }

        private void removePerson(object sender, EventArgs e)
        {
            var authData = new AuthData
            {
                Host = "192.168.16.81",
                APIUrl = "http://192.168.16.81/",
                User = "admin",
                Pass = "webmaster-123"
            };

            string urlMethod = "http://192.168.16.81/ISAPI/Intelligent/FDLib/FDSearch";
            string objectRequest = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<FDSearchDescription>" +
                    "<FDID>2</FDID>" +
                    "<startTime>1970-01-01</startTime>" +
                    "<endTime>2022-12-07</endTime>" +
                    "<searchID>CA148DEC-4240-0001-A321-DA7EFC381661</searchID>" +
                    "<maxResults>50</maxResults>" +
                    "<searchResultPosition>1</searchResultPosition>" +
                "</FDSearchDescription>";
            var response = HttpMethod.SendXML(urlMethod, objectRequest, authData);

            FDSearchResult fdSearchResult = new FDSearchResult();

            XmlSerializer xmlSerializer = new XmlSerializer(fdSearchResult.GetType());

            StringReader stringReader = new StringReader(response);

            XmlTextReader xmlReader = new XmlTextReader(stringReader);

            fdSearchResult = (FDSearchResult)xmlSerializer.Deserialize(xmlReader);

            xmlReader.Close();
            stringReader.Close();

            string idPerson = IdPerson.Text;
            string PID = "";

            Domain.Models.FDSearch.MatchElement matchElement = null;

            if (fdSearchResult != null && fdSearchResult.MatchList != null)
                matchElement = fdSearchResult.MatchList.Where(m => m.CertificateNumber.ToString() == idPerson).FirstOrDefault();

            if (matchElement == null)
            {
                MessageBox.Show("Persona no encontrada");
                return;
            }

            PID = matchElement.PID;

            if (!string.IsNullOrEmpty(PID)) {
                urlMethod = "http://192.168.16.81/ISAPI/Intelligent/FDLib/2/picture/" + PID;
                objectRequest = "";
                response = HttpMethod.SendXML(urlMethod, objectRequest, authData, method: "DELETE");
            }
            ResponseStatus responseStatus = new ResponseStatus();

            xmlSerializer = new XmlSerializer(responseStatus.GetType());

            stringReader = new StringReader(response);

            xmlReader = new XmlTextReader(stringReader);

            responseStatus = (ResponseStatus)xmlSerializer.Deserialize(xmlReader);

            xmlReader.Close();
            stringReader.Close();

            MessageBox.Show(response, "Respuesta");
        }

        private async void syncPerson(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                var authData = new AuthData
                {
                    Host = "192.168.16.81",
                    APIUrl = "http://192.168.16.81/",
                    User = "admin",
                    Pass = "webmaster-123"
                };

                string urlMethod = "http://192.168.16.81/ISAPI/Intelligent/FDLib/2/picture";
                //string objectRequest = "<?xml version='1.0' encoding='UTF-8'?><FaceAppendData><name>Camilo</name><bornTime>2000-01-01</bornTime><sex>male</sex><certificateType>officerID</certificateType><certificateNumber>123456789</certificateNumber></FaceAppendData>";
                string objectRequest = 
                    @"<PictureUploadData>
                        <FDID>2</FDID>
                        <FaceAppendData>
	                        <name>Camilo</name>
	                        <bornTime>2000-01-01</bornTime>
	                        <sex>male</sex>
	                        <certificateType>officerID</certificateType>
	                        <certificateNumber>123456789</certificateNumber>
                        </FaceAppendData>
                    </PictureUploadData>";

                var fileBytes = HttpGetImageDataFromUrl.GetBytesImageFromPath(openFileDialog1.FileName);

                var responseThree = await HttpClientMultipartRequest(urlMethod, openFileDialog1.FileName, fileBytes, openFileDialog1.SafeFileName, objectRequest, authData);
            }           
        }

        #region Reybert

        //private async Task<string> HttpClientMultipartRequest(string urlMethod, byte[] fileBytes, string fileName, string xmlData, AuthData authData)
        //{
        //    HttpClient httpClient = null;
        //    HttpResponseMessage response = null;

        //    try
        //    {
        //        CredentialCache credentialCache = new CredentialCache();
        //        credentialCache.Add(new Uri(urlMethod), "Digest", new NetworkCredential(authData.User, authData.Pass));

        //        httpClient = new HttpClient(new HttpClientHandler { Credentials = credentialCache });

        //        MultipartFormDataContent form = new MultipartFormDataContent();

        //        string formDataBoundary = "---------------------------2156852217421";

        //        byte[] bytesXml = Encoding.ASCII.GetBytes(xmlData);

        //        MemoryStream memoryStream = new MemoryStream(bytesXml);

        //        form.Add(new StreamContent(memoryStream), "PictureUploadData");

        //        if (fileBytes != null)
        //            form.Add(new StreamContent(
        //                          BuildMultipartStream("importImage", fileName, fileBytes, formDataBoundary)));


        //        var httprequestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, urlMethod);
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html, application/xhtml+xml, */* ");
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        //        httprequestMessage.Content = form;
        //        httprequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
        //        httprequestMessage.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue(
        //        "boundary",
        //        formDataBoundary));

        //        response = await httpClient.SendAsync(httprequestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        //        response.EnsureSuccessStatusCode();
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return response.StatusCode.ToString();
        //}

        //private static Stream BuildMultipartStream(string name, string fileName, byte[] fileBytes, string boundary)
        //{
        //    // Create multipart/form-data headers.
        //    byte[] firstBytes = Encoding.UTF8.GetBytes(String.Format(
        //        "--{0}\r\n" +
        //        "Content-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\n" +
        //        "\r\n",
        //        boundary,
        //        name,
        //        fileName));

        //    byte[] lastBytes = Encoding.UTF8.GetBytes(String.Format(
        //        "\r\n" +
        //        "--{0}--\r\n",
        //        boundary));

        //    int contentLength = firstBytes.Length + fileBytes.Length + lastBytes.Length;
        //    byte[] contentBytes = new byte[contentLength];


        //    // Join the 3 arrays into 1.
        //    Array.Copy(
        //        firstBytes,
        //        0,
        //        contentBytes,
        //        0,
        //        firstBytes.Length);
        //    Array.Copy(
        //        fileBytes,
        //        0,
        //        contentBytes,
        //        firstBytes.Length,
        //        fileBytes.Length);
        //    Array.Copy(
        //        lastBytes,
        //        0,
        //        contentBytes,
        //        firstBytes.Length + fileBytes.Length,
        //        lastBytes.Length);

        //    return new MemoryStream(contentBytes);
        //}

        //private string HttpClientRequest(string urlMethod, string fileUrl, string xmlData, AuthData authData)
        //{
        //    CredentialCache credentialCache = new CredentialCache();
        //    credentialCache.Add(new Uri(urlMethod), "Digest", new NetworkCredential(authData.User, authData.Pass));

        //    using (var httpClient = new HttpClient(new HttpClientHandler { Credentials = credentialCache }))
        //    {
        //        httpClient.DefaultRequestHeaders.Add("X-Requested-By", "AM-Request");

        //        MultipartFormDataContent mpform = new MultipartFormDataContent();

        //        byte[] bytesXml = Encoding.ASCII.GetBytes(xmlData);

        //        MemoryStream memoryStream = new MemoryStream(bytesXml);

        //        mpform.Add(new StreamContent(memoryStream), "PictureUploadData");

        //        FileInfo arquivoInfo = new FileInfo(fileUrl);
        //        string Name = arquivoInfo.FullName;

        //        JsonSerializerSettings jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        //        var json = JsonConvert.SerializeObject(jsonSettings);

        //        using (FileStream fs = File.OpenRead(Name))
        //        {
        //            using (var streamContent = new StreamContent(fs))
        //            {
        //                var fileContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);

        //                mpform.Add(fileContent, "importImage", Path.GetFileName(Name));
        //                var response = httpClient.PostAsync(urlMethod, mpform).Result;

        //                return response.StatusCode.ToString();
        //            }
        //        }
        //    }
        //}

        #endregion


        #region Camilo
        private async Task<string> GetPeople(string urlMethod, string filePath, byte[] fileBytes, string fileName, string xmlData, AuthData authData)
        {
            HttpClient httpClient = null;
            HttpResponseMessage response = null;

            try
            {
                CredentialCache credentialCache = new CredentialCache();
                credentialCache.Add(new Uri(urlMethod), "Digest", new NetworkCredential(authData.User, authData.Pass));

                httpClient = new HttpClient(new HttpClientHandler { Credentials = credentialCache });



                //MultipartFormDataContent form = new MultipartFormDataContent();

                ////string formDataBoundary = "---------------------------2156852217421";

                //byte[] bytesXml = Encoding.ASCII.GetBytes(xmlData);

                //MemoryStream memoryStream = new MemoryStream(bytesXml);

                //form.Add(new StreamContent(memoryStream), "\"PictureUploadData\"");

                //FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                ////if (fileBytes != null)
                ////    form.Add(new StreamContent(
                ////                  BuildMultipartStream(fileBytes)), "importImage",fileName);

                ////form.Add(new StreamContent(fileStream), "importImage", fileName);   
                //form.Add(new StreamContent(fileStream));

                //form.ToList()[1].Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "\"importImage\"", FileName = $"\"{fileName}\"" };
                //form.ToList()[1].Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                //var httprequestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, urlMethod);
                //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
                //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                //httprequestMessage.Content = form;
                ////httprequestMessage.Content.Headers.ContentType = form.Headers.ContentType;
                //response = await httpClient.SendAsync(httprequestMessage, HttpCompletionOption.ResponseHeadersRead);

                string objectRequest = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FDSearchDescription><FDID>2</FDID><startTime>1970-01-01</startTime><endTime>2022-12-07</endTime><searchID>CA148DEC-4240-0001-A321-DA7EFC381661</searchID><maxResults>50</maxResults><searchResultPosition>1</searchResultPosition></FDSearchDescription>";

                urlMethod = "http://192.168.16.64/ISAPI/Intelligent/FDLib/FDSearch";
                MultipartFormDataContent form = new MultipartFormDataContent();
                byte[] bytesXml = Encoding.ASCII.GetBytes(objectRequest);
                MemoryStream memoryStream = new MemoryStream(bytesXml);
                form.Add(new StreamContent(memoryStream), "\"PictureUploadData\"");
                response = await httpClient.PostAsync(urlMethod, form);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }

            return response.StatusCode.ToString();
        }
        private async Task<string> HttpClientMultipartRequest(string urlMethod, string filePath, byte[] fileBytes, string fileName, string xmlData, AuthData authData)
        {
            HttpClient httpClient = null;
            HttpResponseMessage response = null;

            try
            {
                CredentialCache credentialCache = new CredentialCache();
                credentialCache.Add(new Uri(urlMethod), "Digest", new NetworkCredential(authData.User, authData.Pass));

                httpClient = new HttpClient(new HttpClientHandler { Credentials = credentialCache });

                MultipartFormDataContent form = new MultipartFormDataContent();

                //string formDataBoundary = "---------------------------2156852217421";

                byte[] bytesXml = Encoding.ASCII.GetBytes(xmlData);

                MemoryStream memoryStream = new MemoryStream(bytesXml);

                form.Add(new StreamContent(memoryStream), "\"PictureUploadData\"");

                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                //if (fileBytes != null)
                //    form.Add(new StreamContent(
                //                  BuildMultipartStream(fileBytes)), "importImage",fileName);

                //form.Add(new StreamContent(fileStream), "importImage", fileName);   
                form.Add(new StreamContent(fileStream));

                form.ToList()[1].Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "\"importImage\"", FileName = $"\"{fileName}\"" };
                form.ToList()[1].Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                var httprequestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, urlMethod);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                httprequestMessage.Content = form;
                //httprequestMessage.Content.Headers.ContentType = form.Headers.ContentType;
                response = await httpClient.SendAsync(httprequestMessage, HttpCompletionOption.ResponseHeadersRead);
                //response = await httpClient.PostAsync(urlMethod, form);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }

            return response.StatusCode.ToString();
        }

        private static Stream BuildMultipartStream( byte[] fileBytes)
        {
            // Create multipart/form-data headers.
            
            int contentLength =  fileBytes.Length ;
            byte[] contentBytes = new byte[contentLength];

            // Join the 3 arrays into 1.
           
            Array.Copy(
                fileBytes,
                0,
                contentBytes,
                0,
                fileBytes.Length);

            return new MemoryStream(contentBytes);
        }

        private string HttpClientRequest(string urlMethod, string fileUrl, string xmlData, AuthData authData)
        {
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new Uri(urlMethod), "Digest", new NetworkCredential(authData.User, authData.Pass));

            using (var httpClient = new HttpClient(new HttpClientHandler { Credentials = credentialCache }))
            {
                httpClient.DefaultRequestHeaders.Add("X-Requested-By", "AM-Request");

                MultipartFormDataContent mpform = new MultipartFormDataContent();

                byte[] bytesXml = Encoding.ASCII.GetBytes(xmlData);

                MemoryStream memoryStream = new MemoryStream(bytesXml);

                mpform.Add(new StreamContent(memoryStream), "PictureUploadData");

                FileInfo arquivoInfo = new FileInfo(fileUrl);
                string Name = arquivoInfo.FullName;

                JsonSerializerSettings jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                var json = JsonConvert.SerializeObject(jsonSettings);

                using (FileStream fs = File.OpenRead(Name))
                {
                    using (var streamContent = new StreamContent(fs))
                    {
                        var fileContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);

                        mpform.Add(fileContent, "importImage", Path.GetFileName(Name));
                        var response = httpClient.PostAsync(urlMethod, mpform).Result;

                        return response.StatusCode.ToString();
                    }
                }
            }
        }

        #endregion
    }
}
