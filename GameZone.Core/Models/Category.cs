using GameZone.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GameZone.MVC.Models
{
    public class Category : BaseEntity, ISoftDeleteable
    {
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();

        public bool IsDeleted { get; set; }

        public DateTime? DateOFDelete { get; set; }
    }
}
