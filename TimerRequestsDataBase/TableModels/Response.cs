using System;
using System.ComponentModel.DataAnnotations;

namespace TimerRequestsDataBase.TableModels
{
    public class Response
    {
        [Key]
        public long Identity { get; set; }

        public Request Request { get; set; }

        [Required]
        public string UserIdentity { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public DateTime ResponseTime { get; set; }

        [Required]
        [StringLength(Request.MessageMaxLength)]
        public string Message { get; set; }
    }
}
