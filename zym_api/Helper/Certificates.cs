using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;

namespace zym_api.Helper
{
    public class Certificates {

        //// 生成32个十六进制字符
        public static string GenerateRandomHexString(int length)
        {
            // 确保生成的长度是偶数（每两个十六进制字符表示一个字节）
            if (length % 2 != 0)
            {
                throw new ArgumentException("Length must be even.");
            }

            byte[] data = new byte[length / 2]; // 每个字节转换为两个十六进制字符
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(data);
            }

            // 将字节数组转换为十六进制字符串
            return BitConverter.ToString(data).Replace("-", string.Empty).ToUpper();
        }
        //加密
        public static string Sign(string message)
        {
            //Log.WriteLog("是否可读到apiclient_key.pem");
            // 读取私钥C:\cer\cert\1688412838_20240922_cert
           string privateKey = File.ReadAllText(@"C:\cer\cert\1688412838_20240922_cert\apiclient_key.pem");
          //string privateKey = File.ReadAllText(@"D:\23\1688412838_20240922_cert\apiclient_key.pem");
            //   Log.WriteLog("可读到apiclient_key.pem");
            using (var reader = new StringReader(privateKey))
            {
                Log.WriteLog("StringReader apiclient_key.pem");
                var pemReader = new PemReader(reader);
                var keyParameter = pemReader.ReadObject() as AsymmetricKeyParameter;

                if (keyParameter == null)
                {
                    throw new InvalidOperationException("私钥无效");
                }

                using (var rsa = RSA.Create())
                {
                    rsa.ImportParameters(DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyParameter));

                    // 将消息转换为字节数组
                    byte[] data = Encoding.UTF8.GetBytes(message);

                    // 使用 SHA256 和 PKCS#1 填充模式对数据进行签名
                    byte[] signedData = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                    // 返回 Base64 编码的签名结果
                    return Convert.ToBase64String(signedData);
                }
            }
        }

    }

}