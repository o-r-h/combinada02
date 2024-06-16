using System.Security.Cryptography;
using System.Text;

namespace Veritrade2017.Helpers
{
    public class Sha2
    {
        public static string getStringSHA(string cadena)
        {
            SHA512 sha2 = SHA512Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha2.ComputeHash(encoding.GetBytes(cadena));
            for (int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }
            return sb.ToString();
        }
    }
}