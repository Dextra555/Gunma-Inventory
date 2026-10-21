using Microsoft.EntityFrameworkCore;
using OBMS.WebAPI.Models;
using OBMS.WebAPI.Models.Domain;
using OBMS.WebAPI.Models.DTO;
using OBMS.WebAPI.Repositories.Interface;

namespace OBMS.WebAPI.Repositories.Implementation
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly OBMSDbContext _db;

        // Display-name lookup for document types
        private static readonly Dictionary<string, string> _docTypeDisplay = new()
        {
            ["PURCHASE_REQUISITION"] = "Purchase Requisition",
            ["PURCHASE_ORDER"]       = "Purchase Order",
            ["STOCK_ADJUSTMENT"]     = "Stock Adjustment",
            ["STOCK_TRANSFER"]       = "Stock Transfer",
            ["DAMAGE_LOSS"]          = "Stock Damage / Loss",
            ["ASSET_DISPOSAL"]       = "Asset Disposal"
        };

        public ApprovalRepository(OBMSDbContext db)
        {
            _db = db;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string DocTypeDisplay(string docType) =>
            _docTypeDisplay.TryGetValue(docType, out var display) ? display : docType;

        /// <summary>
        /// Returns the configured approver username for a given (requester, documentType) pair.
        /// Returns null when no active routing exists.
        /// </summary>
        public async Task<string?> GetApproverForUser(string requesterUser, string documentType)
        {
            var routing = await _db.ApprovalRoutings
                .FirstOrDefaultAsync(r =>
                    r.RequesterUser == requesterUser &&
                    r.DocumentType  == documentType  &&
                    r.IsActive);
            return routing?.ApproverUser;
        }

        // ── Pending Approvals ─────────────────────────────────────────────────

        public async Task<List<PendingApprovalDto>> GetPendingApprovals(
            string approverUser, bool isSuperAdmin)
        {
            var today = DateTime.Today;
            var results = new List<PendingApprovalDto>();

            // ── Purchase Requisitions ──────────────────────────────────────────
            var prs = await _db.PurchaseRequisitions
                .Where(p => p.Status == "Submitted" && p.IsActive)
                .Include(p => p.Details)
                .ToListAsync();

            foreach (var pr in prs)
            {
                if (!isSuperAdmin)
                {
                    var approver = await GetApproverForUser(pr.SubmittedBy ?? string.Empty, "PURCHASE_REQUISITION");
                    if (!string.Equals(approver, approverUser, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                results.Add(new PendingApprovalDto
                {
                    DocumentType        = "PURCHASE_REQUISITION",
                    DocumentTypeDisplay = DocTypeDisplay("PURCHASE_REQUISITION"),
                    DocumentID          = pr.ID,
                    DocumentNumber      = pr.PRNumber,
                    DocumentDate        = pr.PRDate,
                    RequestedBy         = pr.SubmittedBy ?? string.Empty,
                    SubmittedDate       = pr.SubmittedDate,
                    CurrentStatus       = pr.Status,
                    ItemCount           = pr.Details.Count,
                    PendingSinceDays    = pr.SubmittedDate.HasValue
                                           ? (today - pr.SubmittedDate.Value.Date).Days
                                           : 0
                });
            }

            // ── Purchase Orders ────────────────────────────────────────────────
            var pos = await _db.PurchaseOrders
                .Where(p => p.Status == "Submitted" && p.IsActive)
                .Include(p => p.Warehouse)
                .ToListAsync();

            foreach (var po in pos)
            {
                if (!isSuperAdmin)
                {
                    var approver = await GetApproverForUser(po.SubmittedBy ?? string.Empty, "PURCHASE_ORDER");
                    if (!string.Equals(approver, approverUser, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                results.Add(new PendingApprovalDto
                {
                    DocumentType        = "PURCHASE_ORDER",
                    DocumentTypeDisplay = DocTypeDisplay("PURCHASE_ORDER"),
                    DocumentID          = po.ID,
                    DocumentNumber      = po.PONumber,
                    DocumentDate        = po.PODate,
                    RequestedBy         = po.SubmittedBy ?? string.Empty,
                    SubmittedDate       = po.SubmittedDate,
                    CurrentStatus       = po.Status,
                    TotalAmount         = po.TotalAmount,
                    WarehouseName       = po.Warehouse?.Name,
                    PendingSinceDays    = po.SubmittedDate.HasValue
                                           ? (today - po.SubmittedDate.Value.Date).Days
                                           : 0
                });
            }

            // ── Stock Adjustments ──────────────────────────────────────────────
            var adjs = await _db.StockAdjustments
                .Where(a => a.Status == "Submitted" && a.IsActive)
                .Include(a => a.Warehouse)
                .ToListAsync();

            foreach (var adj in adjs)
            {
                if (!isSuperAdmin)
                {
                    var approver = await GetApproverForUser(adj.SubmittedBy ?? string.Empty, "STOCK_ADJUSTMENT");
                    if (!string.Equals(approver, approverUser, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                results.Add(new PendingApprovalDto
                {
                    DocumentType        = "STOCK_ADJUSTMENT",
                    DocumentTypeDisplay = DocTypeDisplay("STOCK_ADJUSTMENT"),
                    DocumentID          = adj.ID,
                    DocumentNumber      = adj.AdjustmentNo,
                    DocumentDate        = adj.AdjustmentDate,
                    RequestedBy         = adj.SubmittedBy ?? string.Empty,
                    SubmittedDate       = adj.SubmittedDate,
                    CurrentStatus       = adj.Status,
                    WarehouseName       = adj.Warehouse?.Name,
                    ItemCount           = 1,
                    PendingSinceDays    = adj.SubmittedDate.HasValue
                                           ? (today - adj.SubmittedDate.Value.Date).Days
                                           : 0
                });
            }

            // ── Asset Disposals ────────────────────────────────────────────────
            var disposals = await _db.AssetDisposals
                .Where(d => d.Status == "Submitted" && d.IsActive)
                .Include(d => d.Asset)
                .ToListAsync();

            foreach (var d in disposals)
            {
                if (!isSuperAdmin)
                {
                    var approver = await GetApproverForUser(d.SubmittedBy ?? string.Empty, "ASSET_DISPOSAL");
                    if (!string.Equals(approver, approverUser, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                results.Add(new PendingApprovalDto
                {
                    DocumentType        = "ASSET_DISPOSAL",
                    DocumentTypeDisplay = DocTypeDisplay("ASSET_DISPOSAL"),
                    DocumentID          = d.ID,
                    DocumentNumber      = d.DisposalNo,
                    DocumentDate        = d.DisposalDate,
                    RequestedBy         = d.SubmittedBy ?? string.Empty,
                    SubmittedDate       = d.SubmittedDate,
                    CurrentStatus       = d.Status,
                    TotalAmount         = d.DisposalValue,
                    PendingSinceDays    = d.SubmittedDate.HasValue
                                           ? (today - d.SubmittedDate.Value.Date).Days
                                           : 0
                });
            }

            return results.OrderByDescending(r => r.PendingSinceDays).ToList();
        }

        // ── Approval History ──────────────────────────────────────────────────

        public async Task<List<ApprovalHistoryDto>> GetApprovalHistory(
            string documentType, int documentID)
        {
            return await _db.ApprovalHistories
                .Where(h => h.DocumentType == documentType && h.DocumentID == documentID)
                .OrderByDescending(h => h.ActionDate)
                .Select(h => new ApprovalHistoryDto
                {
                    ID             = h.ID,
                    DocumentType   = h.DocumentType,
                    DocumentID     = h.DocumentID,
                    DocumentNumber = h.DocumentNumber,
                    Action         = h.Action,
                    ActionBy       = h.ActionBy,
                    ActionDate     = h.ActionDate,
                    Comments       = h.Comments,
                    PreviousStatus = h.PreviousStatus,
                    NewStatus      = h.NewStatus
                })
                .ToListAsync();
        }

        public async Task<List<ApprovalHistoryDto>> GetAllApprovalHistory(
            string? documentType, int? documentID,
            DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _db.ApprovalHistories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(documentType))
                query = query.Where(h => h.DocumentType == documentType);
            if (documentID.HasValue && documentID > 0)
                query = query.Where(h => h.DocumentID == documentID.Value);
            if (dateFrom.HasValue)
                query = query.Where(h => h.ActionDate >= dateFrom.Value);
            if (dateTo.HasValue)
                query = query.Where(h => h.ActionDate < dateTo.Value.AddDays(1));

            return await query
                .OrderByDescending(h => h.ActionDate)
                .Select(h => new ApprovalHistoryDto
                {
                    ID             = h.ID,
                    DocumentType   = h.DocumentType,
                    DocumentID     = h.DocumentID,
                    DocumentNumber = h.DocumentNumber,
                    Action         = h.Action,
                    ActionBy       = h.ActionBy,
                    ActionDate     = h.ActionDate,
                    Comments       = h.Comments,
                    PreviousStatus = h.PreviousStatus,
                    NewStatus      = h.NewStatus
                })
                .ToListAsync();
        }

        // ── Purchase Order workflow ───────────────────────────────────────────

        public async Task<(bool success, string message)> SubmitPO(int id, string submittedBy)
        {
            var po = await _db.PurchaseOrders.FindAsync(id);
            if (po == null || !po.IsActive)
                return (false, "Purchase order not found.");
            if (po.Status != "Draft")
                return (false, $"Purchase order is already {po.Status}. Only Draft POs can be submitted.");

            // Check if approval routing exists for submitter
            var approver = await GetApproverForUser(submittedBy, "PURCHASE_ORDER");
            var prevStatus = po.Status;

            if (string.IsNullOrEmpty(approver))
            {
                // No routing — auto-approve
                po.Status      = "Approved";
                po.ApprovedBy  = submittedBy;
                po.ApprovedDate = DateTime.Now;
            }
            else
            {
                po.Status        = "Submitted";
                po.SubmittedBy   = submittedBy;
                po.SubmittedDate = DateTime.Now;
            }

            po.LASTUPDATE    = DateTime.Now;
            po.LastUpdatedBy = submittedBy;

            _db.ApprovalHistories.Add(new ApprovalHistory
            {
                DocumentType   = "PURCHASE_ORDER",
                DocumentID     = po.ID,
                DocumentNumber = po.PONumber,
                Action         = "Submitted",
                ActionBy       = submittedBy,
                ActionDate     = DateTime.Now,
                PreviousStatus = prevStatus,
                NewStatus      = po.Status
            });

            await _db.SaveChangesAsync();
            return (true, string.IsNullOrEmpty(approver)
                ? "Purchase order submitted and auto-approved (no approver configured)."
                : "Purchase order submitted for approval.");
        }

        public async Task<(bool success, string message)> ApprovePOWithHistory(
            int id, string approverUser, bool isSuperAdmin, string? comments)
        {
            var po = await _db.PurchaseOrders.FindAsync(id);
            if (po == null || !po.IsActive)
                return (false, "Purchase order not found.");
            if (po.Status != "Submitted")
                return (false, $"Purchase order is not in Submitted status (current: {po.Status}).");

            // Validate approver authority
            if (!isSuperAdmin)
            {
                // Self-approval check
                if (string.Equals(po.SubmittedBy, approverUser, StringComparison.OrdinalIgnoreCase))
                    return (false, "You cannot approve your own purchase order.");

                var configuredApprover = await GetApproverForUser(po.SubmittedBy ?? string.Empty, "PURCHASE_ORDER");
                if (!string.Equals(configuredApprover, approverUser, StringComparison.OrdinalIgnoreCase))
                    return (false, "You are not the configured approver for this purchase order.");
            }

            var prevStatus  = po.Status;
            po.Status       = "Approved";
            po.ApprovedBy   = approverUser;
            po.ApprovedDate = DateTime.Now;
            po.LASTUPDATE   = DateTime.Now;
            po.LastUpdatedBy = approverUser;

            _db.ApprovalHistories.Add(new ApprovalHistory
            {
                DocumentType   = "PURCHASE_ORDER",
                DocumentID     = po.ID,
                DocumentNumber = po.PONumber,
                Action         = "Approved",
                ActionBy       = approverUser,
                ActionDate     = DateTime.Now,
                Comments       = comments,
                PreviousStatus = prevStatus,
                NewStatus      = "Approved"
            });

            await _db.SaveChangesAsync();
            return (true, "Purchase order approved successfully.");
        }

        public async Task<(bool success, string message)> RejectPO(
            int id, string rejectorUser, bool isSuperAdmin, string rejectReason)
        {
            var po = await _db.PurchaseOrders.FindAsync(id);
            if (po == null || !po.IsActive)
                return (false, "Purchase order not found.");
            if (po.Status != "Submitted")
                return (false, $"Purchase order is not in Submitted status (current: {po.Status}).");

            if (!isSuperAdmin)
            {
                if (string.Equals(po.SubmittedBy, rejectorUser, StringComparison.OrdinalIgnoreCase))
                    return (false, "You cannot reject your own purchase order.");

                var configuredApprover = await GetApproverForUser(po.SubmittedBy ?? string.Empty, "PURCHASE_ORDER");
                if (!string.Equals(configuredApprover, rejectorUser, StringComparison.OrdinalIgnoreCase))
                    return (false, "You are not the configured approver for this purchase order.");
            }

            var prevStatus    = po.Status;
            po.Status         = "Rejected";
            po.RejectReason   = rejectReason;
            po.LASTUPDATE     = DateTime.Now;
            po.LastUpdatedBy  = rejectorUser;

            _db.ApprovalHistories.Add(new ApprovalHistory
            {
                DocumentType   = "PURCHASE_ORDER",
                DocumentID     = po.ID,
                DocumentNumber = po.PONumber,
                Action         = "Rejected",
                ActionBy       = rejectorUser,
                ActionDate     = DateTime.Now,
                Comments       = rejectReason,
                PreviousStatus = prevStatus,
                NewStatus      = "Rejected"
            });

            await _db.SaveChangesAsync();
            return (true, "Purchase order rejected.");
        }

        // ── Approval Configuration ────────────────────────────────────────────

        public async Task<List<ApprovalConfigDto>> GetApprovalConfigurations()
        {
            return await _db.ApprovalConfigurations
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayName)
                .Select(c => new ApprovalConfigDto
                {
                    ID               = c.ID,
                    DocumentType     = c.DocumentType,
                    DisplayName      = c.DisplayName,
                    ApprovalRequired = c.ApprovalRequired,
                    ApproverRole     = c.ApproverRole,
                    Remarks          = c.Remarks
                })
                .ToListAsync();
        }

        public async Task<(bool success, string message)> UpdateApprovalConfiguration(
            int id, UpdateApprovalConfigDto dto)
        {
            var config = await _db.ApprovalConfigurations.FindAsync(id);
            if (config == null || !config.IsActive)
                return (false, "Approval configuration not found.");

            config.ApprovalRequired = dto.ApprovalRequired;
            config.ApproverRole     = dto.ApproverRole;
            config.Remarks          = dto.Remarks;
            config.LastUpdatedBy    = dto.LastUpdatedBy;
            config.LASTUPDATE       = DateTime.Now;

            await _db.SaveChangesAsync();
            return (true, "Approval configuration updated.");
        }

        // ── Approval Routing ──────────────────────────────────────────────────

        public async Task<List<ApprovalRoutingDto>> GetApprovalRoutings()
        {
            return await _db.ApprovalRoutings
                .OrderBy(r => r.DocumentType)
                .ThenBy(r => r.RequesterUser)
                .Select(r => new ApprovalRoutingDto
                {
                    ID                  = r.ID,
                    DocumentType        = r.DocumentType,
                    DocumentTypeDisplay = r.DocumentType, // mapped below
                    RequesterUser       = r.RequesterUser,
                    ApproverUser        = r.ApproverUser,
                    IsActive            = r.IsActive,
                    LastUpdatedBy       = r.LastUpdatedBy,
                    LASTUPDATE          = r.LASTUPDATE
                })
                .ToListAsync()
                .ContinueWith(t =>
                {
                    foreach (var row in t.Result)
                        row.DocumentTypeDisplay = DocTypeDisplay(row.DocumentType);
                    return t.Result;
                });
        }

        public async Task<ApprovalRoutingDto?> GetApprovalRoutingById(int id)
        {
            var r = await _db.ApprovalRoutings.FindAsync(id);
            if (r == null) return null;
            return new ApprovalRoutingDto
            {
                ID                  = r.ID,
                DocumentType        = r.DocumentType,
                DocumentTypeDisplay = DocTypeDisplay(r.DocumentType),
                RequesterUser       = r.RequesterUser,
                ApproverUser        = r.ApproverUser,
                IsActive            = r.IsActive,
                LastUpdatedBy       = r.LastUpdatedBy,
                LASTUPDATE          = r.LASTUPDATE
            };
        }

        public async Task<(bool success, string message)> SaveApprovalRouting(SaveApprovalRoutingDto dto)
        {
            // Validate: requester ≠ approver
            if (string.Equals(dto.RequesterUser, dto.ApproverUser, StringComparison.OrdinalIgnoreCase))
                return (false, "Requester and approver cannot be the same user.");

            if (dto.ID == 0)
            {
                // New row — check for duplicate active mapping
                bool duplicate = await _db.ApprovalRoutings.AnyAsync(r =>
                    r.DocumentType   == dto.DocumentType &&
                    r.RequesterUser  == dto.RequesterUser &&
                    r.IsActive &&
                    r.ID != dto.ID);

                if (duplicate)
                    return (false, $"An active approval routing already exists for {dto.RequesterUser} on {DocTypeDisplay(dto.DocumentType)}.");

                var row = new ApprovalRouting
                {
                    DocumentType  = dto.DocumentType,
                    RequesterUser = dto.RequesterUser,
                    ApproverUser  = dto.ApproverUser,
                    IsActive      = dto.IsActive,
                    LastUpdatedBy = dto.LastUpdatedBy,
                    LASTUPDATE    = DateTime.Now
                };
                _db.ApprovalRoutings.Add(row);
            }
            else
            {
                var row = await _db.ApprovalRoutings.FindAsync(dto.ID);
                if (row == null)
                    return (false, "Approval routing record not found.");

                // Duplicate check excludes self
                bool duplicate = await _db.ApprovalRoutings.AnyAsync(r =>
                    r.DocumentType  == dto.DocumentType &&
                    r.RequesterUser == dto.RequesterUser &&
                    r.IsActive &&
                    r.ID != dto.ID);

                if (duplicate)
                    return (false, $"An active approval routing already exists for {dto.RequesterUser} on {DocTypeDisplay(dto.DocumentType)}.");

                row.DocumentType  = dto.DocumentType;
                row.RequesterUser = dto.RequesterUser;
                row.ApproverUser  = dto.ApproverUser;
                row.IsActive      = dto.IsActive;
                row.LastUpdatedBy = dto.LastUpdatedBy;
                row.LASTUPDATE    = DateTime.Now;
            }

            await _db.SaveChangesAsync();
            return (true, dto.ID == 0 ? "Approval routing created." : "Approval routing updated.");
        }

        public async Task<(bool success, string message)> DeactivateApprovalRouting(int id, string updatedBy)
        {
            var row = await _db.ApprovalRoutings.FindAsync(id);
            if (row == null)
                return (false, "Approval routing record not found.");
            if (!row.IsActive)
                return (true, "Approval routing is already inactive.");

            row.IsActive      = false;
            row.LastUpdatedBy = updatedBy;
            row.LASTUPDATE    = DateTime.Now;

            await _db.SaveChangesAsync();
            return (true, "Approval routing deactivated.");
        }

        // ── Helper methods used by other repositories ─────────────────────────

        /// <summary>
        /// Returns true if approval is required for the given document type
        /// (per ApprovalConfiguration table). Defaults to true if no config row exists.
        /// </summary>
        public async Task<bool> IsApprovalRequired(string documentType)
        {
            var config = await _db.ApprovalConfigurations
                .FirstOrDefaultAsync(c => c.DocumentType == documentType && c.IsActive);
            return config?.ApprovalRequired ?? true;
        }

        /// <summary>
        /// Returns true when <paramref name="approverUser"/> is the configured approver
        /// for <paramref name="requesterUser"/> on <paramref name="documentType"/>.
        /// </summary>
        public async Task<bool> IsAuthorizedApprover(
            string approverUser, string requesterUser, string documentType)
        {
            var routing = await _db.ApprovalRoutings
                .FirstOrDefaultAsync(r =>
                    r.DocumentType  == documentType &&
                    r.RequesterUser == requesterUser &&
                    r.IsActive);

            if (routing == null) return false;
            return string.Equals(routing.ApproverUser, approverUser, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Appends one row to ApprovalHistory. Does NOT call SaveChangesAsync — the caller
        /// is responsible for saving the full unit of work.
        /// </summary>
        public async Task RecordHistory(
            string documentType, int documentID, string documentNumber,
            string action, string actionBy,
            string previousStatus, string newStatus, string? comments)
        {
            _db.ApprovalHistories.Add(new ApprovalHistory
            {
                DocumentType   = documentType,
                DocumentID     = documentID,
                DocumentNumber = documentNumber,
                Action         = action,
                ActionBy       = actionBy,
                ActionDate     = DateTime.Now,
                Comments       = comments,
                PreviousStatus = previousStatus,
                NewStatus      = newStatus
            });
            await _db.SaveChangesAsync();
        }
    }
}
