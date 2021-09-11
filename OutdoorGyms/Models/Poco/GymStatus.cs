
using System.ComponentModel.DataAnnotations;

namespace OutdoorGyms.Models
{
    public class GymStatus
    {
        [Key]
        public string StatusId { get; set; }

        public string StatusName { get; set; }
    }
}
