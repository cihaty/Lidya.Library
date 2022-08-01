using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace Lidya.Toolkit
{
    public static class Crypto
    {
        private static string _saltkey = "GU)Q^cfWwU";
        public static string Encrypt(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.EncryptString(_saltkey, AlgorithmTypeHash.SHA256);
        }

        public static string Decrypt(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.DecryptString(_saltkey, AlgorithmTypeHash.SHA256);
        }

        private static string EncryptString(this string input, string saltkey, AlgorithmTypeHash type)
        {
            string result = string.Empty;

            switch (type)
            {
                case AlgorithmTypeHash.SHA256:
                    result = EncryptSha256(input, saltkey);
                    break;
                case AlgorithmTypeHash.MD5:
                    result = EncryptStringMd5(input, saltkey);
                    break;
                case AlgorithmTypeHash.MD5Simple:
                    result = EncryptStringMd5Simple(input);
                    break;
            }
            return result;
        }

        private static string DecryptString(this string input, string saltkey, AlgorithmTypeHash type)
        {
            string result = string.Empty;

            switch (type)
            {
                case AlgorithmTypeHash.SHA256:
                    result = DecryptSha256(input, saltkey);
                    break;
                case AlgorithmTypeHash.MD5:
                    result = DecryptStringMd5(input, saltkey);
                    break;
            }

            return result;
        }

        public static string EncryptStringMd5Simple(this string toEncrypt)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(toEncrypt));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }

            return hash.ToString().ToUpper();
        }

        private static string EncryptStringMd5(string toEncrypt, string key)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static string DecryptStringMd5(string cipherString, string key)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);


            //if hashing was used get the hash code with regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //release any resource held by the MD5CryptoServiceProvider

            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private static string EncryptSha256(string input, string saltkey)
        {
            byte[] keyArray;

            using (var sha256 = new SHA256Managed())
            {
                keyArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltkey));
                keyArray = keyArray.Take(24).ToArray();
            }

            using (var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            using (var transform = tdes.CreateEncryptor())
            {
                var toEncryptArray = Encoding.UTF8.GetBytes(input);
                var resultArray = transform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }

        private static string DecryptSha256(string input, string saltkey)
        {
            byte[] toEncryptArray = Convert.FromBase64String(input);

            byte[] keyArray;

            using (var sha256 = new SHA256Managed())
            {
                keyArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltkey));
                keyArray = keyArray.Take(24).ToArray();
            }

            using (var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            using (var transform = tdes.CreateDecryptor())
            {
                var resultArray = transform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                var result = Encoding.UTF8.GetString(resultArray);
                return result;
            }
        }

        public static string MaskEncryptedData(this string encryptedData, string saltkey, AlgorithmTypeHash type, int padLeft = 2, int padRight = 2)
        {
            string masked = "";
            if (!string.IsNullOrEmpty(encryptedData))
            {
                string decrypted = "";
                switch (type)
                {
                    case AlgorithmTypeHash.SHA256:
                        decrypted = DecryptSha256(encryptedData, saltkey);
                        break;
                    case AlgorithmTypeHash.MD5:
                        decrypted = DecryptStringMd5(encryptedData, saltkey);
                        break;
                }

                if (decrypted.Length < padLeft + padRight)
                {
                    padLeft = padRight = decrypted.Length / 2;
                }

                masked = $"{decrypted.Substring(0, padLeft).PadRight(decrypted.Length - padRight, '*')}{decrypted.Substring(decrypted.Length - padRight)}";
            }

            return masked;
        }
    }

    public enum AlgorithmTypeHash
    {
        SHA256,
        MD5,
        MD5Simple
    }
}
