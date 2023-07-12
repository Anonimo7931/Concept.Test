using Domain;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace HIKVISION.App
{
    public static class HttpMethod
    {
        public static HttpWebRequest GetHttpRequest(string url, string method, AuthData authData)
        {
            HttpWebRequest requ = null;
            Uri myUri;
            try
            {
                myUri = new Uri(url);
            }
            catch (Exception ex)
            {
                return requ;
            }

            NetworkCredential myNetworkCredential = new NetworkCredential(authData.User, authData.Pass);
            CredentialCache myCredentialCache = new CredentialCache();
            myCredentialCache.Add(myUri, "Digest", myNetworkCredential);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.PreAuthenticate = true;
            request.Credentials = myCredentialCache;
            request.Method = method;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Cache-Control", "max-age=0");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            return request;
        }

        public static string SendXML(string url, string objectRequest, AuthData authData, string method = "POST")
        {
            HttpWebRequest request = GetHttpRequest(url, method, authData);
            byte[] bytes;
            bytes = Encoding.ASCII.GetBytes(objectRequest);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                return responseStr;
            }

            return response.StatusCode.ToString();
        }
    }

    public static class HttpSendMultipartFormData
    {
        public static async Task<string> SendMultipartFormData(HttpWebRequest httpWebRequest, string xmlData, string fileName, string filePath)
        {
            var requestContent = new MultipartFormDataContent();

            byte[] bytes;
            //string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            //httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            ////////////////////////////////////////////////////////////////////////////
            bytes = Encoding.ASCII.GetBytes(xmlData);

            MemoryStream memoryStream = new MemoryStream(bytes);

            requestContent.Add(new StreamContent(memoryStream), "FaceAppendData");

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            requestContent.Add(new StreamContent(fileStream), "importImage", fileName);

            bytes = await requestContent.ReadAsByteArrayAsync();

            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            return "OK";
        }

        public static string UploadFilesToServer(HttpWebRequest httpWebRequest, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            string responseString = string.Empty;

            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.BeginGetRequestStream((result) =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                    using (Stream requestStream = request.EndGetRequestStream(result))
                    {
                        WriteMultipartForm(requestStream, boundary, data, fileName, fileContentType, fileData);
                    }
                    request.BeginGetResponse(a =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(a);
                            var responseStream = response.GetResponseStream();
                            using (var sr = new StreamReader(responseStream))
                            {
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    responseString = streamReader.ReadToEnd();
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }, null);
                }
                catch (Exception ex)
                {
                    responseString = ex.Message;
                }
            }, httpWebRequest);

            return responseString;
        }
                
        private static void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            /// the form data, properly formatted
            string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName, fileContentType));
            /// Write the file data to the stream.
            WriteToStream(s, fileData);
            WriteToStream(s, trailer);
        }
                
        private static void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        private static void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }
    }

    public static class HttpGetImageDataFromUrl
    {
        public static string GetBase64ImageFromUrl(string url, AuthData authData)
        {
            Uri myUri = new Uri(url);
            WebClient wc = new WebClient();
            NetworkCredential nc = new NetworkCredential(authData.User, authData.Pass);
            CredentialCache cc = new CredentialCache();
            cc.Add(myUri, "Digest", nc);
            wc.Credentials = cc;
            wc.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            wc.Headers.Add("Accept-Encoding", "gzip, deflate");
            wc.Headers.Add("Cache-Control", "max-age=0");
            wc.Headers.Add("Upgrade-Insecure-Requests", "1");

            byte[] bytes;
            string base64Image = string.Empty;

            try
            {
                bytes = wc.DownloadData(url);
                base64Image = Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }

            return base64Image;
        }

        public static byte[] GetBytesImageFromUrl(string url, AuthData authData)
        {
            Uri myUri = new Uri(url);
            WebClient wc = new WebClient();
            NetworkCredential nc = new NetworkCredential(authData.User, authData.Pass);
            CredentialCache cc = new CredentialCache();
            cc.Add(myUri, "Digest", nc);
            wc.Credentials = cc;
            wc.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            wc.Headers.Add("Accept-Encoding", "gzip, deflate");
            wc.Headers.Add("Cache-Control", "max-age=0");
            wc.Headers.Add("Upgrade-Insecure-Requests", "1");

            byte[] bytes;

            try
            {
                bytes = wc.DownloadData(url);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }

            return bytes;
        }
        
        public static byte[] GetBytesImageFromPath(string filePath)
        {
            byte[] fileBytes = null;

            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            BinaryReader binaryReader = new BinaryReader(file);

            long bytesNumber = new FileInfo(filePath).Length;

            fileBytes = binaryReader.ReadBytes((int)bytesNumber);

            return fileBytes;
        }
    }
}
