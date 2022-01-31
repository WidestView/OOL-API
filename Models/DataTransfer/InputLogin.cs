using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OOL_API.Models.DataTransfer
{
    public class InputLogin
    {
        [Required(ErrorMessage = "O login é obrigatório")]
        [JsonProperty("login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
    }
}
