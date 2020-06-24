using System;
using System.ComponentModel.DataAnnotations;

namespace TimerRequestsDataBase.TableModels
{
    public class Request
    {
        public const int TopicMaxLength = 100;
        public const int MessageMaxLength = 500;

        [Key]
        public long Identity { get; set; }

        [Required]
        public string UserIdentity { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        [StringLength(TopicMaxLength)]
        public string Topic { get; set; }

        [Required]
        [StringLength(MessageMaxLength)]
        public string Message { get; set; }

        [Required]
        public bool NewChangesByUser { get; set; }

        [Required]
        public bool NewChangesByAdmin { get; set; }

        [Required]
        public bool CaseClosed { get; set; }

        [Required]
        public bool PrivateRequest { get; set; }

        [Required]
        public int RepliesAmount { get; set; }
    }
}
