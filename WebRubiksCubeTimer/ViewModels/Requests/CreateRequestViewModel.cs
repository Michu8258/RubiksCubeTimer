using System.ComponentModel.DataAnnotations;
using TimerRequestsDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.Requests
{
    public class CreateRequestViewModel
    {
        public int TopicMaxLength { get; set; } = Request.TopicMaxLength;
        public int MessageMaxLength { get; set; } = Request.MessageMaxLength;

        [Required]
        [Display(Name = "Request topic")]
        [StringLength(Request.TopicMaxLength, MinimumLength = 10)]
        public string Topic { get; set; }

        [Required]
        [Display(Name = "Request message")]
        [StringLength(Request.MessageMaxLength, MinimumLength = 20)]
        public string Message { get; set; }

        [Required]
        [Display(Name = "Public request")]
        public bool PublicRequest { get; set; }
    }
}
