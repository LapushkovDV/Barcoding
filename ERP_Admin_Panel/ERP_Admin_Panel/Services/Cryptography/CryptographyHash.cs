using System.Security.Cryptography;
using System.Text;

namespace ERP_Admin_Panel.Services.Cryptography
{
    /// <summary>
    /// Статический класс криптографии (Хеш-функции)
    /// </summary>
    public static class CryptographyHash
    {
        /// <summary>
        /// Создание хеш строки, используя алгоритм SHA256
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Хеш, созданный алгоритмом SHA256</returns>
        public static string ComputeSHA256(string data)
        {
            using var sha256 = SHA256.Create();

            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

            return FormatterByteArray(bytes);
        }

        /// <summary>
        /// Перевод байтов в строку
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Строка, конвертированная из байтов</returns>
        private static string FormatterByteArray(byte[] data)
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append($"{data[i]:x2}");
            }

            return stringBuilder.ToString();
        }
    }
}
