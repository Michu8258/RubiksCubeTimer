using System.Collections.Generic;

namespace WebRubiksCubeTimer.ViewModels.User
{
    public class CubeInfoViewModel
    {
        public bool ModelError { get; set; }
        public string ManufacturerCountry { get; set; }
        public long ManufacturerFoundationYear { get; set; }
        public long UsersUsingThisCube { get; set; }
        public IEnumerable<string> PermittedCategoryOptions { get; set; }
    }
}
