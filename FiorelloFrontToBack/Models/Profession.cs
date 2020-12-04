using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiorelloFrontToBack.Models
{
    public class Profession
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Expert> Experts { get; set; }
    }
}