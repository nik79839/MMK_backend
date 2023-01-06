using Application.Interfaces;
using Domain;
using Domain.InitialResult;
using Infrastructure.Services;
using Moq;
using RastrAdapter;

namespace UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        [Fact]
        public async Task GetUsers_ShouldGetUsers()
        {
            _userRepositoryMock
                .Setup(x => x.GetAllUsers())
                .Returns(() => Task.FromResult(new List<User>() { new User() { Name = "test", Id = 1, LastName = "test", Login = "nik",
                    Password = "test", Post = "test", SurName = "test"} }))
                .Verifiable();
            var service = new AuthService(_userRepositoryMock.Object);
            var result = service.GetUsers().Result;

            _userRepositoryMock.Verify(x => x.GetAllUsers(), Times.Once);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task Login_ShouldReturnUser()
        {
            _userRepositoryMock
                .Setup(x => x.Login("admin", "admin"))
                .Returns(() => Task.FromResult(new User()
                {
                    Name = "test",
                    Id = 1,
                    LastName = "test",
                    Login = "nik",
                    Password = "test",
                    Post = "test",
                    SurName = "test"
                }))
                .Verifiable();
            var service = new AuthService(_userRepositoryMock.Object);
            var result = service.Login("admin", "admin").Result;

            _userRepositoryMock.Verify(x => x.Login("admin", "admin"), Times.Once);
            Assert.IsType<User>(result);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnTrue()
        {
            var service = new AuthService(_userRepositoryMock.Object);
            var result = service.CreateUser(new User()
            {
                Name = "test",
                Id = 1,
                LastName = "test",
                Login = "nik",
                Password = "test",
                Post = "test",
                SurName = "test"
            });

            Assert.True(result.IsCompletedSuccessfully);
        }
    }
}
