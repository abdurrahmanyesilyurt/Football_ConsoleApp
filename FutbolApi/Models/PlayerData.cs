using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FutbolApi.Models
{
    public class PlayerData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Id's are set explicitly from API
        public int Id { get; set; }
        public int? TransferId { get; set; }
        public int? PlayerId { get; set; }
        public int? TeamId { get; set; }
        public int? PositionId { get; set; }
        public int? DetailedPositionId { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public bool? Captain { get; set; }
        public int? JerseyNumber { get; set; }
    }

    public class ApiResponse<T> 
    {
        public List<T> Data { get; set; }
        public object Subscription { get; set; }
        public object RateLimit { get; set; }
        public string Timezone { get; set; }
    }
}
