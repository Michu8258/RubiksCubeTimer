using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimerDataBase.TableModels
{
    public class Category
    {
        [Key]
        public int Identity { get; set; }

        [NotMapped]
        public IEnumerable<CategoryOption> OptionsSet { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public TimeSpan ShortestAcceptableResult { get; set; }
    }
}
