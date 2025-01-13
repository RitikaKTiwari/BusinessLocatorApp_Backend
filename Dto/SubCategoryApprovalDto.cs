namespace BusinessLocatorApp.Dto
{
    public class SubCategoryApprovalDto
    {
        public int Id { get; set; }
        public string Status { get; set; } // "Approved" or "Rejected"
        public string AdminComments { get; set; }
    }
}
