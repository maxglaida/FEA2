using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EFApp
{
    class SendRecieveService
    {
        public static async Task<bool> SendPOST(string json, Person user)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://46.101.218.123/user");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
            {
                streamWriter.Write(json);
            }

            var httpResponse = await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
            return await CheckUserExistsAsync(user);

        }

        public static async Task<bool> CheckUserExistsAsync(Person user)
        {
            //build the uri
            string uri = "http://46.101.218.123/user?email=" + user.email;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            


            var httpResponse = await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                //string is null or empty = no such user
                if (string.IsNullOrEmpty(result)) return false;
            }
                //there is such a user
                return true;
            
            
        }

        public static async Task<string> SearchForUserOnServer(string email)
        {
            //build the uri
            string uri = "http://46.101.218.123/user?email=" + email;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";


            var httpResponse = await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                //string is null or empty = no such user
                return result;
            }
                     
        }

        public static async Task<bool> SendEncryptedFileTo(byte[] encryptedFile, Person user, string fileName)
        {
            //POST /file?sender=t@t.com&receiver=alex2@test.com&file_name=test.enc
            //build the uri
            string fn = fileName.Remove(fileName.LastIndexOf('.'));

            string uri = "http://46.101.218.123/file?sender=t@t.com&receiver=" + user.email + "&file_name=" + fn + ".enc";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/octet-stream";
            httpWebRequest.Method = "POST";

            var networkStream = await httpWebRequest.GetRequestStreamAsync();
            

                await networkStream.WriteAsync(encryptedFile, 0, encryptedFile.Length);

            
            var httpResponse = await httpWebRequest.GetResponseAsync();
            networkStream.Dispose();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //    //string is null or empty = no such user
            //    if (string.IsNullOrEmpty(result)) return false;
            //}
            //there is such a user
            return true;


        }

       

    }
}
