using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Helper
{
    public static class Helper
    {
        public static string GenerateUniqueCode(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            string base64String = Convert.ToBase64String(bytes);
            base64String = base64String.Replace("+", "").Replace("/", "").Replace("=", "");
            if (base64String.Length > 8)
            {
                return base64String.Substring(0, 8);
            }
            return base64String.PadRight(8, '0');
        }

        public static string ConvertToBase64(string body)
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            return Convert.ToBase64String(bodyBytes);
        }
    }
}
