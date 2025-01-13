namespace BusinessLocatorApp.Services
{
    public interface IContactUsService
    {
        Task SendContactUsMessageAsync(string name, string email, string message);
    }
}
