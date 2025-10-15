namespace FGS_BE.Repo.Settings;
public class JwtSettings
{
    public static readonly string Section = "Authentication:Schemes:Bearer";

    public string SerectKey { get; set; } = default!;
    public string SerectRefreshKey { get; set; } = default!;
    public int TokenExpire { get; set; }
    public int RefreshTokenExpire { get; set; }
    public string ValidIssuer { get; set; } = default!;
    public string[] ValidAudiences { get; set; } = default!;

}