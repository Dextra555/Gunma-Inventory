namespace OBMS.WebAPI.Models.DTO
{
    /// <summary>
    /// DTO representing a single row in the uniform/equipment item list
    /// returned by GetUniformItemRows (used in Payroll module).
    /// </summary>
    public class EmployeeItemIssueListDto
    {
        public int ID { get; set; }           // EmployeeItemIssue.ID (0 if not yet issued)
        public int AdvanceID { get; set; }    // FK to SalaryAdvance
        public int ItemID { get; set; }       // FK to ItemMaster
        public string? Name { get; set; }     // ItemMaster.Name
        public decimal Price { get; set; }    // ItemMaster.SellPrice
        public int Quantity { get; set; }     // EmployeeItemIssue.Quantity (0 if not yet issued)
    }
}
