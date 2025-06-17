using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using DnsClient;

namespace ClientContactAPI
{
    public static class Utils
    {
        public static string HashGenerator(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));

                byte[] hexBytes = Encoding.ASCII.GetBytes(builder.ToString());

                return Convert.ToBase64String(hexBytes);
            }
        }
        public static async Task<bool> ValidateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if(addr.Address != email) return false;

                var domain = email.Split('@').Last();
                var lookup = new LookupClient();

                var result = await lookup.QueryAsync(domain, QueryType.MX);
                return result.Answers.MxRecords().Any();
            }
            catch
            {
                return false;
            }
        }
        public static string PrettyTextDisplay(string name)
        {
            string result = "";
            string[] text = name.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < text.Length; i++) 
            {
                result += Char.ToUpper(text[i][0]) + text[i].Substring(1).ToLower() + " ";
            }
            result = result.Remove(result.Length - 1, 1);
            return result;
        }
    }
}
