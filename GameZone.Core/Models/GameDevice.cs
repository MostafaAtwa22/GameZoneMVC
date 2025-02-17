using GameZone.Core.Interfaces;

namespace GameZone.MVC.Models
{
    public class GameDevice : ISoftDeleteable
    {
        public Game Game { get; set; } = default!;
        public int GameId { get; set; }

        public Device Device { get; set; } = default!;
        public int DeviceId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DateOFDelete { get; set; }
    }
}
