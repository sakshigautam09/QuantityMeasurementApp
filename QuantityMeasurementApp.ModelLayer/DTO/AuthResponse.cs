namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>
    /// Response for Register and Login.
    /// Contains JWT token — use as: Authorization: Bearer {token}
    /// No session stored on server (pure stateless JWT).
    /// </summary>
    public class AuthResponse
    {
        public bool     Success   { get; set; }
        public string   Message   { get; set; } = "";
        public string   Token     { get; set; } = "";
        public string   Username  { get; set; } = "";
        public string   Email     { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }
}
