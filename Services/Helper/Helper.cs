using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace HelperFuncoes{
    public class Helper : IHelper
    {
        public string retornaUsuario(string token)
        {
            token = token.Replace("Bearer ", "");
            var jsontoken = new JwtSecurityTokenHandler();
            var tokenresultado = jsontoken.ReadToken(token);
            var tokenS = jsontoken.ReadToken(token) as JwtSecurityToken;
            dynamic tokendecodedjson = JsonConvert.SerializeObject(tokenS);
            dynamic tokendecodedobjeto  = JsonConvert.DeserializeObject<dynamic>(tokendecodedjson);
            string usuario =  tokendecodedobjeto["Payload"]["usuario"];
            return usuario;
        }

        public string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return (res.ToString()).ToUpper();
        }
    }
}