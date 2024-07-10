namespace NotificacaoService.Services
{
    public interface IEmailService
    {
        Task<bool> SendMail(string mailTO, string body, string subject);
    }
}
