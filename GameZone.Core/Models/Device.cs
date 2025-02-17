using GameZone.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GameZone.MVC.Models
{
    public class Device : BaseEntity, ISoftDeleteable
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Icon { get; set; } = string.Empty;

        public ICollection<GameDevice> GameDevices { get; set; } = new List<GameDevice>();

        public ICollection<Game> Games { get; set; } = new List<Game>();

        public bool IsDeleted { get; set; }

        public DateTime? DateOFDelete { get; set; }
    }
}
