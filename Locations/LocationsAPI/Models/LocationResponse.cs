using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace LocationsAPI.Models
{
    public class LocationResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public String Result { get; set; }
        [ForeignKey("LocationRequest")]
        public int ReqId { get; set; }

    }
}
