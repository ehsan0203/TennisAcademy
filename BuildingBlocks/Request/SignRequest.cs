using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BuildingBlocks.Exceptions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BuildingBlocks.Request
{
    public class SignRequest
    {
        private readonly RSA _rsa;
        private readonly string _keyPath;
        public SignRequest(IConfiguration configuration)
        {
            try
            {
                _keyPath = configuration["Host:KeyPath"] ?? throw new NotSetConfigException("KeyPath is missing in appsettings.json");
                string privateKeyPath = _keyPath + $"private/private-{configuration["Host:ClientId"]}.pem";
                _rsa = RSA.Create();
                _rsa.ImportFromPem(File.ReadAllText(privateKeyPath).ToCharArray());

            }
            catch (Exception e)
            {
                Log.Error("Config App Error:{error}", e.Message);
                throw new ForbiddenAccessException("شما دسترسی ندارید.", 100);
            }
        }

        public string SignData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new NotFoundException("Data to sign cannot be null or empty.");
            }
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signature = _rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }

        public string EncryptDataByRACert(string password)
        {
            X509Certificate2 certificate = new X509Certificate2(_keyPath + "public/PKIRAEncryptCertificate.cer");
            RSA rsa = certificate.GetRSAPublicKey();
            byte[] data = Encoding.UTF8.GetBytes(password);
            byte[] encryptedPass = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptedPass);
        }
    }
}
