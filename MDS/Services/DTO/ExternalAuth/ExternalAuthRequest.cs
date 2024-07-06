namespace MDS.Services.DTO.ExternalAuth
{
    public class ExternalAuthRequest
    {
        public string? Provider { get; set; }
        public string? IdToken { get; set; }
    }
}
