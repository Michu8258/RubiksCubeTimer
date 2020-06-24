using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class CubesCollection
    {
        [Key]
        public long Identity { get; set; }

        [Required]
        public string UserIdentity { get; set; }

        public virtual Cube Cube { get; set; }
    }
}
