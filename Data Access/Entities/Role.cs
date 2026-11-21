using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Entities
{
    [Table("Roles")]
    [Index(nameof(Name), IsUnique = true)]
    public class Role
    {
        public long Id { get; set; }

        [Required, MaxLength(20)]
        public string Name { get; set; } = string.Empty;

        // Navigation property: all users that have this role
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
