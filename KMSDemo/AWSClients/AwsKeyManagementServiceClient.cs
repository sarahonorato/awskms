using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Amazon.Runtime;

namespace KMSDemo.AwsClients
{
    public class AwsKeyManagementServiceClient
    {
        private readonly RegionEndpoint _region;
        private readonly BasicAWSCredentials _credentials;

        public AwsKeyManagementServiceClient(RegionEndpoint region, BasicAWSCredentials credentials)
        {
            _region = region;
            _credentials = credentials;
        }

        public async Task<string> GetKeyAsync(string keyName)
        {
            try
            {
                var kmsClient = new AmazonKeyManagementServiceClient(_credentials, _region);

                var response = await kmsClient.ListAliasesAsync(new ListAliasesRequest
                {
                    Limit = 1000
                });

                var keyList = response.Aliases;

                var foundAlias = keyList.FirstOrDefault(r => r.AliasName == "alias/" + keyName);
                if (foundAlias != null)
                {
                    return foundAlias.TargetKeyId;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return string.Empty;
        }

        public string Encrypt(string textToEncrypt, string keyId)
        {
            try
            {
                var kmsClient = new AmazonKeyManagementServiceClient(_credentials, _region);
                var encryptRequest = new EncryptRequest
                {
                    KeyId = keyId
                };

                var textBytes = Encoding.UTF8.GetBytes(textToEncrypt);
                encryptRequest.Plaintext = new System.IO.MemoryStream(textBytes, 0, textBytes.Length);
                var response = kmsClient.EncryptAsync(encryptRequest);
                return Convert.ToBase64String(response.Result.CiphertextBlob.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string Decrypt(string encryptedText)
        {
            try
            {
                var fromBase64Bytes = Convert.FromBase64String(encryptedText);
                var kmsClient = new AmazonKeyManagementServiceClient(_credentials, _region);
                var decryptRequest = new DecryptRequest
                {
                    CiphertextBlob = new System.IO.MemoryStream(fromBase64Bytes, 0, fromBase64Bytes.Length)
                };
                var response = kmsClient.DecryptAsync(decryptRequest);
                if (response.Result == null)
                {
                    return string.Empty;
                }
                return Encoding.UTF8.GetString(response.Result.Plaintext.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
