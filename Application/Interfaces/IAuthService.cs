using Domain;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<List<User>> GetUsers();
    }
}