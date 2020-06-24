using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class CubeRating
    {
        [Key]
        public long Identity { get; set; }

        public Cube Cube { get; set; }

        [Required]
        public string UserIdentity { get; set; }

        [Required]
        public ushort RateValue { get; set; }

        [Required]
        public bool Rated { get; set; }
    }
}