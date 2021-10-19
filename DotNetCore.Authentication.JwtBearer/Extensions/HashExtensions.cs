using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer
{
    public static class HashExtensions
    {
        public static string ToMd5(this string keyString)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                md5.ComputeHash(Encoding.UTF8.GetBytes(keyString));
                byte[] result = md5.Hash;
                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    strBuilder.Append(result[i].ToString("x2"));
                }
                return strBuilder.ToString();
            }
        }

        /// <summary>
        /// Creates a SHA256 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash</returns>
        public static string Sha256(this string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Creates a SHA256 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash.</returns>
        public static byte[] Sha256(this byte[] input)
        {
            if (input == null)
            {
                return null;
            }

            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(input);
            }
        }

        /// <summary>
        /// Creates a SHA512 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash</returns>
        public static string Sha512(this string input)
        {
            using (var sha = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

    }
}
