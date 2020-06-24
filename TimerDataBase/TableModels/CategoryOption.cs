using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class CategoryOption
    {
        [Key]
        public int Identity { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
