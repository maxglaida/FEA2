using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFApp
{
    public interface ICryptoService
    {
        bool DoesKeyExists();
        void GenerateKeys();
        string ExportPublicKeyToString(Person user);
         void DeleteKeyFromContainer();
        byte[] EncryptFile(Stream file, Person person, string fileExtension, string fileName);

    }
}
