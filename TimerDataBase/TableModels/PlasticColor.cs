using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class PlasticColor
    {
        [Key]
        public int Identity { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
