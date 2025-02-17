using GameZone.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GameZone.MVC.Models
{
    public class Game : BaseEntity, ISoftDeleteable
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2500)]
        public string Description { get; set; } = string.Empty;

        public string Cover { get; set; } = default!;

        public int CategoryId { get; set; }

        public Category category { get; set; } = default!;

        public ICollection<GameDevice> GameDevices { get; set; } = new List<GameDevice>();

        public ICollection<Device> Devices { get; set; } = new List<Device>();

        public bool IsDeleted { get; set; }

        public DateTime? DateOFDelete { get; set; }
    }
}
