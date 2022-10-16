using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistance.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int SurName { get; set; }
        public int LastName { get; set; }
        public string Post { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

    }
}
