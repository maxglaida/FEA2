using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoServices
{
    class KeyPairGenerationService
    {

        CspParameters cspp = new CspParameters();
        RSACryptoServiceProvider rsa;

        // Path variables for source, encryption, and
        // decryption folders. Must end with a backslash.
        const string EncrFolder = @"c:\CryptoApp\Encrypt\";
        const string DecrFolder = @"c:\CryptoApp\Decrypt\";
        const string SrcFolder = @"c:\CryptoApp\docs\";

        // Public key file
        const string PubKeyFile = @"c:\CryptoApp\Encrypt\rsaPublicKey.txt";

        // Key container name for
        // private/public key value pair.
        const string keyName = "Key01";

        //check if key already exists

        //generate key pair

        //export public key

        //save public key in a file with the name of the person

        //delete keys


    }


}
