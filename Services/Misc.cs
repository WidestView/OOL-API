using System.Text.RegularExpressions;

namespace OOL_API.Services
{
    public static class Misc
    {
        public static string StripCpf(string cpf)
        {
            return Regex.Replace(cpf, "[.-]", "");
        }
    }
}
