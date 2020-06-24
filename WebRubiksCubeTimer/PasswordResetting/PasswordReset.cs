using System;
using System.ComponentModel.DataAnnotations;

namespace WebRubiksCubeTimer.PasswordResetting
{
    public class PasswordReset
    {
        [Key]
        public long RequestId { get; set; }
        public string UserIdentity { get; set; }
        public string EmailAdddress { get; set; }
        public string ResetKey { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime KeyExpires { get; set; }
        public int Attempts { get; set; }
        public bool WasVerified { get; set; }
        public bool ResetVerified { get; set; }
    }
}
