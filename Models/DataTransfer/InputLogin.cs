using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OOL_API.Models.DataTransfer
{
    public class InputLogin
    {
        [Required]
        [JsonProperty("login")]
        public string Login { get; set; }

        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}