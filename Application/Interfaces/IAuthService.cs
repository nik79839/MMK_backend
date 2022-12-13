using Domain;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task CreateUser(User user);
        Task<List<User>> GetUsers();
        Task<User> Login(string login, string password);
    }
}