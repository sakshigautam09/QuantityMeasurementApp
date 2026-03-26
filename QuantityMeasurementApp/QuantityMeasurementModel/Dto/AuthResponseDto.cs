namespace QuantityMeasurementModel.Dto
{
    /// <summary>Auth operation response (tokens + user summary).</summary>
    public class AuthResponseDto
    {
        public string   AccessToken  { get; set; } = string.Empty;
        public string   RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt    { get; set; }
        public string   Username     { get; set; } = string.Empty;
        public string   Role         { get; set; } = string.Empty;
        public string   Message      { get; set; } = string.Empty;
    }
}
