using OBMS.WebAPI.Models.DTO;

namespace OBMS.WebAPI.Repositories.Interface
{
    public interface IApprovalRepository
    {
        // ── Pending Approvals ──────────────────────────────────────────────────
        Task<List<PendingApprovalDto>> GetPendingApprovals(string approverUser, bool isSuperAdmin);

        // ── Approval History ───────────────────────────────────────────────────
        Task<List<ApprovalHistoryDto>> GetApprovalHistory(string documentType, int documentID);
        Task<List<ApprovalHistoryDto>> GetAllApprovalHistory(
            string? documentType, int? documentID,
            DateTime? dateFrom, DateTime? dateTo);

        // ── Purchase Order workflow ────────────────────────────────────────────
        Task<(bool success, string message)> SubmitPO(int id, string submittedBy);
        Task<(bool success, string message)> ApprovePOWithHistory(int id, string approverUser, bool isSuperAdmin, string? comments);
        Task<(bool success, string message)> RejectPO(int id, string rejectorUser, bool isSuperAdmin, string rejectReason);

        // ── Approval Configuration ─────────────────────────────────────────────
        Task<List<ApprovalConfigDto>> GetApprovalConfigurations();
        Task<(bool success, string message)> UpdateApprovalConfiguration(int id, UpdateApprovalConfigDto dto);

        // ── Approval Routing ───────────────────────────────────────────────────
        Task<string?> GetApproverForUser(string requesterUser, string documentType);
        Task<List<ApprovalRoutingDto>> GetApprovalRoutings();
        Task<ApprovalRoutingDto?> GetApprovalRoutingById(int id);
        Task<(bool success, string message)> SaveApprovalRouting(SaveApprovalRoutingDto dto);
        Task<(bool success, string message)> DeactivateApprovalRouting(int id, string updatedBy);

        // ── Helper methods used by other repositories ──────────────────────────
        Task<bool> IsApprovalRequired(string documentType);
        Task<bool> IsAuthorizedApprover(string approverUser, string requesterUser, string documentType);
        Task RecordHistory(string documentType, int documentID, string documentNumber,
            string action, string actionBy, string previousStatus, string newStatus, string? comments);
    }
}
