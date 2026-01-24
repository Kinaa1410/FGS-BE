namespace FGS_BE.Repo.Settings
{
    public class JwtSettings
    {
        public const string Section = "Authentication:Schemes:Bearer"; // Fixed: Added static Section constant for config binding

        public string[] ValidAudiences { get; set; } = Array.Empty<string>(); // Array for multiple audiences
        public string ValidIssuer { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string SecretRefreshKey { get; set; } = string.Empty;
        public int TokenExpire { get; set; }
        public int RefreshTokenExpire { get; set; }
    }
}