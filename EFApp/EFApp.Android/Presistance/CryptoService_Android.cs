using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;
using System.IO;
using EFApp.Droid;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;
using System.Net;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(CryptoService_Android))]
namespace EFApp.Droid
{
    class CryptoService_Android : ICryptoService
    {
        CspParameters cspp = new CspParameters();
        RSACryptoServiceProvider rsa;

        // Path variables for source, encryption, and
        // decryption folders. Must end with a backslash.

        // Key container name for
        // private/public key value pair.
        const string keyName = "Key01";

        public void GenerateKeys()
        {
            // Stores a key pair in the key container.
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

        }

        public string ExportPublicKeyToString(Person user)
        {
            user.public_key = rsa.ToXmlString(false);
            user.password = "temp";

            string json = JsonConvert.SerializeObject(user);



            return json;

        }

        public bool DoesKeyExists()
        {
            var cspParams = new CspParameters
            {
                Flags = CspProviderFlags.UseExistingKey,
                KeyContainerName = "Key01"
            };

            try
            {
                var provider = new RSACryptoServiceProvider(cspParams);
            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }

        public void DeleteKeyFromContainer()
        {
            // Create the CspParameters object and set the key container   
            // name used to store the RSA key pair.  
            cspp.KeyContainerName = "Key01";

            // Create a new instance of RSACryptoServiceProvider that accesses  
            // the key container.  
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspp);

            // Delete the key entry in the container.  
            rsa.PersistKeyInCsp = false;

            // Call Clear to release resources and delete the key from the container.  
            rsa.Clear();

            //delete key also from server!! IMPORTANT
        }

        public byte[] EncryptFile(Stream file, Person person, string fileExtension, string fileName)
        {

            // Create instance of Rijndael for
            // symetric encryption of the data.
            RijndaelManaged rjndl = new RijndaelManaged();
            rjndl.KeySize = 256;
            rjndl.BlockSize = 256;
            rjndl.Mode = CipherMode.CBC;
            ICryptoTransform transform = rjndl.CreateEncryptor();

            // Use RSACryptoServiceProvider to
            // enrypt the Rijndael key.
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(person.public_key);
            byte[] keyEncrypted = RSA.Encrypt(rjndl.Key, false);

            // Create byte arrays to contain
            // the length values of the key and IV.
            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            int lKey = keyEncrypted.Length;
            LenK = BitConverter.GetBytes(lKey);
            int lIV = rjndl.IV.Length;
            LenIV = BitConverter.GetBytes(lIV);

            Stream outFs = new MemoryStream();
            outFs.Write(LenK, 0, 4);
            outFs.Write(LenIV, 0, 4);
            outFs.Write(keyEncrypted, 0, lKey);
            outFs.Write(rjndl.IV, 0, lIV);

            // Now write the cipher text using
            // a CryptoStream for encrypting.
            CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write);


            // By encrypting a chunk at
            // a time, you can save memory
            // and accommodate large files.
            int count = 0;
            int offset = 0;

            // blockSizeBytes can be any arbitrary size.
            int blockSizeBytes = rjndl.BlockSize / 8;
            byte[] data = new byte[blockSizeBytes];
            int bytesRead = 0;
            
            do
            {
                count = file.Read(data, 0, blockSizeBytes);
                offset += count;
                outStreamEncrypted.Write(data, 0, count);
                bytesRead += blockSizeBytes;
            }
            while (count > 0);
            outStreamEncrypted.FlushFinalBlock();


            var byteArr = ReadToEnd(outFs);

            outStreamEncrypted.Dispose();
            outFs.Dispose();
            file.Dispose();

            return byteArr;

        }

        private static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

    }
}