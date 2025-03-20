using System.Text.Json.Serialization;

namespace ThAmCo.CheapestProduct.Dtos
{
    public class TokenDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}