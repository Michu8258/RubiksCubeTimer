namespace WebRubiksCubeTimer.Models.Pagination
{
    public class FilterModel
    {
        public int PageNumber { get; set; } = 1;
        public string Filter { get; set; } = string.Empty;
    }
}
