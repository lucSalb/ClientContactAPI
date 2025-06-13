using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace ClientContactAPI.Utils
{
    public static class Utils
    {
        public static string GenerateID(string[] input)
        {
            string ID = "";
            for(int i = 0; i < input.Length; i++)
            {
                ID += input[i].ToUpper();
            }

            ID += DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(ID);
            byte[] hash = sha256.ComputeHash(bytes);

            ID = Convert.ToBase64String(hash);

            return ID;
        }
    }
}
