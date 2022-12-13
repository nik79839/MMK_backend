using Domain;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task CreateUser(User user);
        Task<List<User>> GetAllUsers();
        Task<User> Login(string login, string password);
    }
}