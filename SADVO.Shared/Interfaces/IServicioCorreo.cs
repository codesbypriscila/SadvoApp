namespace SADVO.Shared.Interfaces
{
    public interface IServicioCorreo
    {
        Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml);
    }
}
