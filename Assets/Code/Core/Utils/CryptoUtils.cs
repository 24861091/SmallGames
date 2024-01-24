using System.Security.Cryptography;
using System.Text;

namespace Code.Core.Utils
{
    public static class CryptoUtils
    {
        public static AesCryptoServiceProvider AesCSP = new AesCryptoServiceProvider();

        public static byte[] Key2 =
        {
            0x97, 0xd2, 0x95, 0x8e,
            0x47, 0x8f, 0x4f, 0x5c,
            0xd5, 0x36, 0xdf, 0xf7,
            0xfb, 0xaa, 0x5c, 0xef,
            0xe6, 0xa7, 0x13, 0x35,
            0x3d, 0x1b, 0xc4, 0x5b,
            0xd2, 0xca, 0xb7, 0x85,
            0xb3, 0xf4, 0x78, 0x1e
        };

        public static byte[] Iv2 =
        {
            0x02, 0x12, 0xd6, 0xca,
            0x3c, 0xc1, 0xb5, 0x66,
            0x46, 0xe9, 0xe9, 0x47,
            0xc9, 0xfe, 0xbb, 0xbc
        };

        public static string CreateMD5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("x2"));
            return sb.ToString();
        }

        public static string CreateMD5(byte[] inputBytes)
        {
            using var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("x2"));
            return sb.ToString();
        }

        public static byte[] Decrypt(byte[] encryptedBytes)
        {
            var decryptor = AesCSP.CreateDecryptor(Key2, Iv2);
            var outBlock = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return outBlock;
        }

        public static byte[] Encrypt(byte[] bytes)
        {
            var encryptor = AesCSP.CreateEncryptor(Key2, Iv2);
            var outBlock = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return outBlock;
        }
    }
}