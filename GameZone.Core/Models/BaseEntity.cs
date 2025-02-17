using System.ComponentModel.DataAnnotations;

namespace GameZone.MVC.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
