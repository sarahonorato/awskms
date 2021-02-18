using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Amazon;
using Amazon.Runtime;
using KMSDemo.AwsClients;

namespace KMSDemo
{
    class Program
    {
        private static readonly RegionEndpoint Region = RegionEndpoint.USWest2;

        static void Main(string[] args)
        {
            Console.WriteLine($"Encrypting Order Number:");

            string orderNumber = Console.ReadLine();

            //New Encrypt
            var encryptionKey = GetKeyByAlias("test_key_sara");
            var encrypted = EncryptedText(orderNumber, encryptionKey);
            Console.WriteLine($" Encrypted OrderNo {orderNumber} with KMS as {encrypted}");
            Console.WriteLine("------------------------------------");

            //New Decrypt
            var decrypted = DecryptedText(encrypted);
            Console.WriteLine($" Decrypted Order Number {encrypted} with KMS as {decrypted}");
            Console.WriteLine("------------------------------------");

            //End
            Console.WriteLine("------------------------------------");
            Console.WriteLine("TADAAA");
            Console.ReadKey();
        }

        /// <summary>
        /// Get the key that we want to use in the encryption from KMS
        /// </summary>
        /// <param name="alias">The "key name"</param>
        /// <returns>The key id</returns>
        private static string GetKeyByAlias(string alias)
        {
            try
            {
                var credentials =
                    new BasicAWSCredentials("acessKey", "secretKey");

                var kmsClient = new AwsKeyManagementServiceClient(Region, credentials);
                var key = kmsClient.GetKeyAsync(alias);
                return key.Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Encrypted data using a AWS Key
        /// </summary>
        /// <param name="textToEncrypt">The data to encrypt</param>
        /// <param name="keyId">The key id</param>
        /// <returns></returns>
        private static string EncryptedText(string textToEncrypt, string keyId)
        {
            if (string.IsNullOrEmpty(textToEncrypt))
            {
                return string.Empty;
            }

            try
            {
                var credentials = new BasicAWSCredentials("acessKey", "secretKey");
                var kmsClient = new AwsKeyManagementServiceClient(Region, credentials);
                return kmsClient.Encrypt(textToEncrypt, keyId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Decrypt data previously encrypted by a AWS Key
        /// </summary>
        /// <param name="encryptedText">The encrypted text</param>
        /// <returns>The  decrypted text</returns>
        private static string DecryptedText(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return string.Empty;
            }

            try
            {
                var credentials = new BasicAWSCredentials("acessKey", "secretKey");
                var kmsClient = new AwsKeyManagementServiceClient(Region, credentials);
                return kmsClient.Decrypt(encryptedText);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
