using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class CategoryOptionsSet
    {
        [Key]
        public int Identity { get; set; }

        public virtual Category Category { get; set; }

        public virtual CategoryOption Option { get; set; }
    }
}
