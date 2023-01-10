namespace Application.DTOs.Requests
{
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string LastName { get; set; }
        public string Post { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
