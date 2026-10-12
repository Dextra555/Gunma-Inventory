# Inventory Module Migration Plan — FWG-Malaysia-New → Gunma---Poilt

## Project Constants

| Item | Value |
|---|---|
| Target C# Namespace | `OBMS.WebAPI` |
| Target DB | `OBMSUATNEW` on `DESKTOP-FHKDAA6` (Integrated Security) |
| Target API base URL | `http://localhost:5003/api/` |
| Target Report base URL | `http://localhost:58008/` |
| Target API port | 5003 |
| JWT key (target) | `obms-authentication-secret-key-32chars!!` |
| JWT key (source) | `obms-authentication` ← **do NOT copy; keep target key** |
| Source API path | `d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI` |
| Target API path | `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI` |
| Source UI path | `d:\Gunma\FWG-Malaysia-New\OBMSAPP-UI\angularapp\src\app` |
| Target UI path | `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app` |
| Source DB scripts | `d:\Gunma\FWG-Malaysia-New\Database` |
| Target DB scripts | `d:\Gunma\Gunma---Poilt\Database` |
| Source Reports | `d:\Gunma\FWG-Malaysia-New\UatReports-new\UatReports\Inventory` |
| Target Reports | `d:\Gunma\Gunma---Poilt\Gunma - Report\Inventory` |

---

## Gap Analysis

### A. Database / Schema Gaps

The target database (`OBMSUATNEW`) lacks these objects that exist in source.
All 27 numbered migration scripts from source need applying.

**New Tables (not in target at all):**
1. `BrandMaster` — (Code, Name, Description, IsActive)
2. `UOMMaster` — (Code, Name, ShortName, IsPrimary)
3. `WarehouseMaster` — (Code, Name, Address, BranchID, IsActive)
4. `OpeningStock` — (ReferenceNo, WarehouseID, ItemID, OpeningDate, OpeningQty, UnitCost, TotalValue, IsActive)
5. `StockLedger` — (WarehouseID, ItemID, TransactionType, TransactionDate, TransactionID, Qty, BalanceQty, UnitCost, CreatedBy, CreatedDate)
6. `PurchaseRequisition` + `PurchaseRequisitionDetail`
7. `PRProcessingTask`
8. `PurchaseOrder` + `PurchaseOrderDetail`
9. `GoodsReceiptNote` + `GoodsReceiptNoteDetail`
10. `PurchaseReturn` + `PurchaseReturnDetail`
11. `StockIssueDoc` + `StockIssueDocDetail` (different from existing `StockIssues`/`StockIssueDetail`)
12. `StockIssueReturn` + `StockIssueReturnDetail`
13. `StockTransfer` + `StockTransferDetail`
14. `StockAdjustment`
15. `StockDamageLoss`
16. `AssetAllocation`
17. `AssetTransfer`
18. `AssetReturn`
19. `AssetMaintenance`
20. `AssetDamage`
21. `AssetDisposal`
22. `AssetMovementHistory`
23. `AssetLocationLog`
24. `ApprovalHistory`
25. `ApprovalConfiguration`
26. `ApprovalRouting`
27. `AppNotification`
28. `StockRequest` + `StockRequestDetail`
29. `StockIssueUniformLoanLink`

**New Columns on Existing Target Tables:**
- `ItemMaster`: Code, BrandID, UOMID, ReorderLevel, ReorderQuantity, WarehouseID, IsActive
- `InventoryCategory`: ParentCategoryID
- `InventoryCategory`: IsActive column (for soft delete)
- `AssetMaster`: Code, SerialNo, Model, Manufacturer, WarrantyExpiry, CurrentLocation, Status, IsActive

**ScreenList entries to insert (idempotently):**
Brand Master, UOM Master, Warehouse Master, Opening Stock, Purchase Requisition, Purchase Order, Goods Receipt, Purchase Return, Stock Issue, Stock Issue Return, Stock Transfer, Stock Adjustment, Stock Damage/Loss, Asset Allocation, Asset Transfer, Asset Return, Asset Maintenance, Asset Damage, Asset Disposal, Asset Movement History, Asset Location, Branch Stock Request, Pending Approvals, Approval History, Notification

---

### B. Backend — Missing Controllers

| Controller | Status |
|---|---|
| `ApprovalController.cs` | **MISSING** from target — entire file |
| `StockRequestController.cs` | **MISSING** from target — entire file |
| `NotificationController.cs` | **MISSING** from target — entire file |

---

### C. Backend — Missing from InventoryController (target vs source diff)

The following endpoints exist in source `InventoryController.cs` but are absent from the target:

**Masters (Part 1):**
- `GetItemCode` / `GetRecipientCode` / `GetBrandCode` / `GetUOMCode` / `GetWarehouseCode`
- `SaveAndUpdateBrandMaster` / `GetBrandMasters` / `GetBrandByID` / `DeleteBrand`
- `SaveAndUpdateUOMMaster` / `GetUOMMasters` / `GetUOMByID` / `DeleteUOM`
- `SaveAndUpdateWarehouseMaster` / `GetWarehouseMasters` / `GetWarehouseByID` / `DeleteWarehouse`
- `GetItemsForStock` / `GetReportHQMaster` (source has richer join version)

**Opening Stock (Part 2):**
- `GetOpeningStockList` / `GetOpeningStockByID` / `GetOpeningStockCode`
- `SaveOpeningStock` / `DeleteOpeningStock`
- `GetStockLedger` / `GetCurrentStock`

**Purchase Flow (Part 3):**
- `GetPRList` / `GetPRByID` / `GetPRCode` / `SavePR` / `SubmitPR` / `ApprovePR` / `RejectPR` / `DeletePR`
- `GetApprovedPRsForPO` / `GetPRProcessingTasks` / `GetPRProcessingTaskByPR` / `ClaimPR`
- `GetPOList` / `GetPOByID` / `GetPOByPRID` / `GetPOCode` / `SavePO` / `ApprovePO` / `CancelPO` / `GetPOEditability`
- `GetGRNList` / `GetGRNByID` / `GetGRNByPOID` / `GetGRNCode` / `GetApprovedPOsForGRN` / `GetPOLinesForGRN`
- `SaveGRN` / `CheckGRNDependency` / `DeleteGRN`
- `GetPurchaseReturnList` / `GetPurchaseReturnByID` / `GetGRNsForReturn` / `GetGRNLinesForReturn` / `SavePurchaseReturn`

**Stock Operations (Part 4):**
- `GetStockIssueList` / `GetStockIssueByID` / `GetStockIssueCode` / `SaveStockIssue` / `DeleteStockIssue`
- `GetIssuableDocsForReturn` / `GetIssueDetailsForReturn`
- `GetStockIssueReturnList` / `GetStockIssueReturnByID` / `GetStockIssueReturnCode` / `SaveStockIssueReturn`
- `GetStockTransferList` / `GetStockTransferByID` / `GetStockTransferCode` / `SaveStockTransfer` / `DeleteStockTransfer`
- `GetStockAdjustmentList` / `GetStockAdjustmentByID` / `GetStockAdjustmentCode` / `SaveStockAdjustment` / `DeleteStockAdjustment`
- `SubmitSA` / `ApproveSA` / `RejectSA`
- `GetStockDamageLossList` / `GetStockDamageLossByID` / `GetStockDamageLossCode` / `SaveStockDamageLoss` / `DeleteStockDamageLoss`

**Employee/Holding (Part 5):**
- `GetEmployeeHoldingSummaryList` / `GetEmployeeHoldingDetail` / `GetAllEmployeeHoldings`
- `GetStockBalanceSummary` / `GetEmployeeIssueRegister`
- `GetEmployeesByWarehouse` / `GetPendingStockIssuesForEmployee` / `LinkStockIssueToUniformLoan`

**Asset Management (Part 6):**
- `GetAssetList` / `GetAssetByIDEnriched` / `SaveAsset` / `DeleteAsset`
- `GetAssetAllocationList` / `GetAssetAllocationByID` / `GetActiveAllocationsByAsset`
- `SaveAssetAllocation` / `CancelAssetAllocation`
- `GetAssetTransferList` / `GetAssetTransferByID` / `SaveAssetTransfer`
- `GetAssetReturnList` / `GetAssetReturnByID` / `SaveAssetReturn`
- `GetAssetMaintenanceList` / `GetAssetMaintenanceByID` / `SaveAssetMaintenance` / `CompleteAssetMaintenance`
- `GetAssetDamageList` / `GetAssetDamageByID` / `SaveAssetDamage`
- `GetAssetDisposalList` / `GetAssetDisposalByID` / `SaveAssetDisposal` / `SubmitAD` / `ApproveAD` / `RejectAD`
- `GetAssetMovementHistory` / `UpdateAssetLocation` / `GetAssetLocationLog` / `GetDefectLocationAssets`

---

### D. Backend — Missing Repository Interface Methods (IInventoryRepository)

Everything in the source `IInventoryRepository.cs` that is absent from target:

- `GetSuppliers(string? category = null)` — target has `GetSuppliers()` without category param
- `GetUtilityBillsMasterList(string userID)` — target has it without userID
- Brand/UOM/Warehouse CRUD and Delete methods (6 groups × 4 = 24 methods)
- All Part 2–6 and Part 9 methods (50+ methods — see interface file above)

---

### E. Backend — Missing Repository Implementation Methods (InventoryRepository)

All methods listed above for the interface must also be implemented in:
`d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Repositories\Implementation\InventoryRepository.cs`

The full source implementation is at:
`d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI\Repositories\Implementation\InventoryRepository.cs`

---

### F. Backend — Missing Repositories (entirely absent from target)

| Interface | Implementation |
|---|---|
| `IApprovalRepository.cs` | `ApprovalRepository.cs` |
| `INotificationRepository.cs` | `NotificationRepository.cs` |

---

### G. Backend — Missing Domain Model Classes

| File | Missing from Target |
|---|---|
| `BrandMaster.cs` | Yes |
| `UOMMaster.cs` | Yes |
| `WarehouseMaster.cs` | Yes |
| `OpeningStock.cs` | Yes |
| `StockLedger.cs` | Yes |
| `PurchaseRequisition.cs` | Yes |
| `PurchaseRequisitionDetail.cs` | Yes |
| `PRProcessingTask.cs` | Yes |
| `PurchaseOrder.cs` | Yes |
| `PurchaseOrderDetail.cs` | Yes |
| `GoodsReceiptNote.cs` | Yes |
| `GoodsReceiptNoteDetail.cs` | Yes |
| `PurchaseReturn.cs` | Yes |
| `PurchaseReturnDetail.cs` | Yes |
| `StockIssueDoc.cs` | Yes |
| `StockIssueDocDetail.cs` | Yes |
| `StockIssueReturn.cs` | Yes |
| `StockIssueReturnDetail.cs` | Yes |
| `StockTransfer.cs` | Yes |
| `StockTransferDetail.cs` | Yes |
| `StockAdjustment.cs` | Yes |
| `StockDamageLoss.cs` | Yes |
| `StockRequest.cs` | Yes |
| `StockRequestDetail.cs` | Yes |
| `StockIssueUniformLoanLink.cs` | Yes |
| `AppNotification.cs` | Yes |
| `ApprovalConfiguration.cs` | Yes |
| `ApprovalHistory.cs` | Yes |
| `ApprovalRouting.cs` | Yes |
| `AssetAllocation.cs` | Yes |
| `AssetDamage.cs` | Yes |
| `AssetDisposal.cs` | Yes |
| `AssetLocationLog.cs` | Yes |
| `AssetMaintenance.cs` | Yes |
| `AssetMovementHistory.cs` | Yes |
| `AssetReturn.cs` | Yes |
| `AssetTransfer.cs` | Yes |

---

### H. Backend — Missing DTO Classes

| File | Missing from Target |
|---|---|
| `ApprovalDto.cs` | Yes |
| `AssetAllocationRequestDto.cs` | Yes |
| `AssetDamageRequestDto.cs` | Yes |
| `AssetDisposalRequestDto.cs` | Yes |
| `AssetListDto.cs` | Yes |
| `AssetLocationDto.cs` | Yes |
| `AssetMaintenanceRequestDto.cs` | Yes |
| `AssetMovementHistoryDto.cs` | Yes |
| `AssetReturnRequestDto.cs` | Yes |
| `AssetTransferRequestDto.cs` | Yes |
| `BrandMasterRequestDto.cs` | Yes |
| `EmployeeForIssueDto.cs` | Yes |
| `EmployeeHoldingDto.cs` | Yes |
| `EmployeeIssueRegisterDto.cs` | Yes |
| `GoodsReceiptNoteDto.cs` | Yes |
| `GRNPaymentRowDto.cs` | Yes |
| `NotificationDto.cs` | Yes |
| `OpeningStockListDto.cs` | Yes |
| `OpeningStockRequestDto.cs` | Yes |
| `PendingStockIssueDto.cs` | Yes |
| `PurchaseOrderDto.cs` | Yes |
| `PurchaseRequisitionDto.cs` | Yes |
| `PurchaseReturnDto.cs` | Yes |
| `SAApprovalDto.cs` | Yes |
| `StockAdjustmentListDto.cs` | Yes |
| `StockAdjustmentRequestDto.cs` | Yes |
| `StockBalanceSummaryDto.cs` | Yes |
| `StockDamageLossListDto.cs` | Yes |
| `StockDamageLossRequestDto.cs` | Yes |
| `StockIssueDocListDto.cs` | Yes |
| `StockIssueDocRequestDto.cs` | Yes |
| `StockIssueReturnListDto.cs` | Yes |
| `StockIssueReturnRequestDto.cs` | Yes |
| `StockLedgerFilterDto.cs` | Yes |
| `StockRequestDto.cs` | Yes |
| `StockTransferListDto.cs` | Yes |
| `StockTransferRequestDto.cs` | Yes |
| `UOMMasterRequestDto.cs` | Yes |
| `WarehouseMasterRequestDto.cs` | Yes |

Also: `AssetMasterRequestDto.cs` in target is the OLD version (missing Code, SerialNo, Model, etc.) and needs to be replaced with the source version.

---

### I. Backend — Missing SignalR Hub

| File | Missing from Target |
|---|---|
| `Hubs\NotificationHub.cs` | Yes |

---

### J. Angular UI — Missing Modules (31 folders)

| Source Folder | Present in Target |
|---|---|
| `approval-history` | No |
| `approval-history-dialog` | No |
| `asset-allocation` | No |
| `asset-damage` | No |
| `asset-disposal` | No |
| `asset-location` | No |
| `asset-maintenance` | No |
| `asset-movement-history` | No |
| `asset-return` | No |
| `asset-transfer` | No |
| `branch-stock-request` | No |
| `brand-master` | No |
| `employee-issue-register` | No |
| `employee-item-holding` | No |
| `goods-receipt` | No |
| `inventory-timesheet` | No |
| `notification-list` | No |
| `opening-stock` | No |
| `pending-approvals` | No |
| `purchase-order` | No |
| `purchase-requisition` | No |
| `purchase-return` | No |
| `stock-adjustment` | No |
| `stock-balance-summary` | No |
| `stock-damage-loss` | No |
| `stock-issue` | No |
| `stock-issue-return` | No |
| `stock-ledger` | No |
| `stock-transfer` | No |
| `uom-master` | No |
| `warehouse-master` | No |

**Modules that exist but need merging/updating (source has richer versions):**
- `asset-master` — source has additional fields (Code, SerialNo, Status, etc.)
- `category-master` — source has ParentCategoryID hierarchy
- `item-master` — source has BrandID, UOMID, WarehouseID, ReorderLevel
- `material-issue` — source has newer StockIssueDoc flow

---

### K. Angular Service — Missing Methods (inventory.service.ts)

The target `inventory.service.ts` has ~25 methods covering basic masters + utilities. The source has 100+ methods covering all the above modules. All new service methods must be added (not replace existing ones).

Key missing service groups:
- Brand, UOM, Warehouse master CRUD
- Opening Stock CRUD + GetStockLedger + GetCurrentStock + GetItemsForStock
- PR/PO/GRN/Purchase Return full CRUD + submit/approve/reject flows
- Stock Issue, Issue Return, Stock Transfer, Adjustment, Damage/Loss
- Employee holding, Stock balance summary, Employee Issue Register
- Asset Allocation, Transfer, Return, Maintenance, Damage, Disposal, Location
- BSR CRUD + submit/approve/reject
- Approval service methods (pending approvals, approval history, approval config, routing)
- Notification service methods

---

### L. Angular Routing — Missing Routes (admin-layout-routing.module.ts)

31 modules × ~2 routes each = ~62 new route entries needed.

---

### M. Angular Sidebar — Missing Menu Entries (sidebar.component.html)

New sidebar groups / entries needed under Inventory:
- Brand Master, UOM Master, Warehouse Master
- Opening Stock, Stock Ledger
- Purchase Requisition, Purchase Order, Goods Receipt, Purchase Return
- Stock Issue, Stock Issue Return, Stock Transfer, Stock Adjustment, Stock Damage/Loss
- Employee Item Holding, Employee Issue Register, Stock Balance Summary
- Asset Allocation, Asset Transfer, Asset Return, Asset Maintenance, Asset Damage, Asset Disposal, Asset Location, Asset Movement History
- Branch Stock Request
- Pending Approvals, Approval History, Notification

---

### N. Crystal Reports — Missing Report Files

Source has 23 report files; target only has 6:

| File | In Target |
|---|---|
| `AssetAllocationReport.aspx` | No |
| `AssetDamageReport.aspx` | No |
| `AssetDisposalReport.aspx` | No |
| `AssetMaintenanceReport.aspx` | No |
| `AssetMasterReport.aspx` | No |
| `AssetReturnReport.aspx` | No |
| `AssetTransferReport.aspx` | No |
| `BSRReport.aspx` | No |
| `GRNReport.aspx` | No |
| `IssueReturnReport.aspx` | No |
| `POReport.aspx` | No |
| `PRReport.aspx` | No |
| `StockAdjustmentReport.aspx` | No |
| `StockIssueReport.aspx` | No |
| `StockLedger.rpt` | No |
| `StockLedgerReport.aspx` | No |
| `StockTransferReport.aspx` | No |

---

---

## Implementation Plan

---

### Phase 1 — Database Migration

**Goal:** Apply all inventory schema changes to `OBMSUATNEW` idempotently, without touching existing data.

**Strategy:** Copy all 27 numbered SQL scripts from source to target Database folder and execute them in order. Every script uses `IF NOT EXISTS` guards — they are safe to run on a database that already has the base tables.

- [ ] 1. Copy source DB scripts to target Database folder.
      Copy all 27 numbered `.sql` files (01 through 27) plus supplemental scripts (`ApprovalRoutingBugFix_Migration.sql`, `ApprovalRouting_Part7_UserWise_Migration.sql`, `Fix_PRList_SubmittedBy_Columns.sql`, `Fix_SupplierInvoiceView_ExcludeGRN.sql`) from `d:\Gunma\FWG-Malaysia-New\Database\` to `d:\Gunma\Gunma---Poilt\Database\`.
      Files: `d:\Gunma\Gunma---Poilt\Database\` (27+ new .sql files)
      Verify: Files present in target Database folder.

- [ ] 2. Execute DB migration scripts in order against `OBMSUATNEW`.
      Connect to `DESKTOP-FHKDAA6\OBMSUATNEW` with Integrated Security. Run each script from `01.Inventory_Master_Migration.sql` through `27.PRProcessingTask_Migration.sql` in numeric order, then the supplemental fix scripts. Each prints a completion message.
      Verify: Run `SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('BrandMaster','UOMMaster','WarehouseMaster','OpeningStock','StockLedger','PurchaseRequisition','PurchaseOrder','GoodsReceiptNote','PurchaseReturn','StockIssueDoc','StockIssueReturn','StockTransfer','StockAdjustment','StockDamageLoss','AssetAllocation','AssetTransfer','AssetReturn','AssetMaintenance','AssetDamage','AssetDisposal','AssetMovementHistory','AssetLocationLog','ApprovalHistory','ApprovalConfiguration','ApprovalRouting','AppNotification','StockRequest','StockIssueUniformLoanLink') ORDER BY TABLE_NAME` — should return 28 rows.

---

### Phase 2 — Backend (API)

**Dependency note:** Steps in this phase must be done in the order shown. Domain models before DTOs, DTOs before interfaces, interfaces before implementations, implementations before controllers, controllers before DI registration.

**Important rules for every file copy in this phase:**
- Change namespace to `OBMS.WebAPI` if source has a different namespace.
- Do NOT copy `Program.cs` from source — target has different JWT key and CORS settings.
- Do NOT copy `appsettings*.json` from source.
- Do NOT copy `OBMSDbContext.cs` wholesale — merge DbSet additions only (see step 2-C).

---

- [ ] 2-A. Copy missing Domain Model classes from source to target.
      Copy all 37 model files listed in Section G above from `d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI\Models\Domain\` to `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Models\Domain\`.
      No namespace changes needed (both use `OBMS.WebAPI.Models.Domain`).
      Files (37 .cs files, full list in Gap Analysis Section G):
      `BrandMaster.cs`, `UOMMaster.cs`, `WarehouseMaster.cs`, `OpeningStock.cs`, `StockLedger.cs`, `PurchaseRequisition.cs`, `PurchaseRequisitionDetail.cs`, `PRProcessingTask.cs`, `PurchaseOrder.cs`, `PurchaseOrderDetail.cs`, `GoodsReceiptNote.cs`, `GoodsReceiptNoteDetail.cs`, `PurchaseReturn.cs`, `PurchaseReturnDetail.cs`, `StockIssueDoc.cs`, `StockIssueDocDetail.cs`, `StockIssueReturn.cs`, `StockIssueReturnDetail.cs`, `StockTransfer.cs`, `StockTransferDetail.cs`, `StockAdjustment.cs`, `StockDamageLoss.cs`, `StockRequest.cs`, `StockRequestDetail.cs`, `StockIssueUniformLoanLink.cs`, `AppNotification.cs`, `ApprovalConfiguration.cs`, `ApprovalHistory.cs`, `ApprovalRouting.cs`, `AssetAllocation.cs`, `AssetDamage.cs`, `AssetDisposal.cs`, `AssetLocationLog.cs`, `AssetMaintenance.cs`, `AssetMovementHistory.cs`, `AssetReturn.cs`, `AssetTransfer.cs`
      Verify: All 37 files present in target Models\Domain folder. Build solution (`dotnet build "d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\OBMS.WebAPI.csproj"`) — 0 errors.

- [ ] 2-B. Copy missing DTO classes from source to target.
      Copy all 39 DTO files listed in Gap Analysis Section H above from `d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI\Models\DTO\` to `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Models\DTO\`.
      Also replace the existing `AssetMasterRequestDto.cs` in target with the source version (it is an older, smaller DTO in target).
      Full list: `ApprovalDto.cs`, `AssetAllocationRequestDto.cs`, `AssetDamageRequestDto.cs`, `AssetDisposalRequestDto.cs`, `AssetListDto.cs`, `AssetLocationDto.cs`, `AssetMaintenanceRequestDto.cs`, `AssetMovementHistoryDto.cs`, `AssetReturnRequestDto.cs`, `AssetTransferRequestDto.cs`, `BrandMasterRequestDto.cs`, `EmployeeForIssueDto.cs`, `EmployeeHoldingDto.cs`, `EmployeeIssueRegisterDto.cs`, `GoodsReceiptNoteDto.cs`, `GRNPaymentRowDto.cs`, `NotificationDto.cs`, `OpeningStockListDto.cs`, `OpeningStockRequestDto.cs`, `PendingStockIssueDto.cs`, `PurchaseOrderDto.cs`, `PurchaseRequisitionDto.cs`, `PurchaseReturnDto.cs`, `SAApprovalDto.cs`, `StockAdjustmentListDto.cs`, `StockAdjustmentRequestDto.cs`, `StockBalanceSummaryDto.cs`, `StockDamageLossListDto.cs`, `StockDamageLossRequestDto.cs`, `StockIssueDocListDto.cs`, `StockIssueDocRequestDto.cs`, `StockIssueReturnListDto.cs`, `StockIssueReturnRequestDto.cs`, `StockLedgerFilterDto.cs`, `StockRequestDto.cs`, `StockTransferListDto.cs`, `StockTransferRequestDto.cs`, `UOMMasterRequestDto.cs`, `WarehouseMasterRequestDto.cs`
      Verify: Build solution — 0 errors.

- [ ] 2-C. Add missing DbSet entries to target OBMSDbContext.cs.
      Edit `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Models\OBMSDbContext.cs`.
      Add these DbSet declarations (copy from source OBMSDbContext.cs — these are fully absent from target):
      ```
      public virtual DbSet<BrandMaster> BrandMasters { get; set; }
      public virtual DbSet<UOMMaster> UOMMasters { get; set; }
      public virtual DbSet<WarehouseMaster> WarehouseMasters { get; set; }
      public DbSet<OpeningStock> OpeningStocks { get; set; }
      public DbSet<StockLedger> StockLedgers { get; set; }
      public DbSet<PurchaseRequisition> PurchaseRequisitions { get; set; }
      public DbSet<PurchaseRequisitionDetail> PurchaseRequisitionDetails { get; set; }
      public DbSet<PRProcessingTask> PRProcessingTasks { get; set; }
      public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
      public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
      public DbSet<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
      public DbSet<GoodsReceiptNoteDetail> GoodsReceiptNoteDetails { get; set; }
      public DbSet<PurchaseReturn> PurchaseReturns { get; set; }
      public DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }
      public DbSet<AssetAllocation> AssetAllocations { get; set; }
      public DbSet<AssetTransfer> AssetTransfers { get; set; }
      public DbSet<AssetReturn> AssetReturns { get; set; }
      public DbSet<AssetMaintenance> AssetMaintenances { get; set; }
      public DbSet<AssetDamage> AssetDamages { get; set; }
      public DbSet<AssetDisposal> AssetDisposals { get; set; }
      public DbSet<AssetMovementHistory> AssetMovementHistories { get; set; }
      public DbSet<AssetLocationLog> AssetLocationLogs { get; set; }
      public DbSet<ApprovalHistory> ApprovalHistories { get; set; }
      public DbSet<ApprovalConfiguration> ApprovalConfigurations { get; set; }
      public DbSet<AppNotification> AppNotifications { get; set; }
      public DbSet<ApprovalRouting> ApprovalRoutings { get; set; }
      public DbSet<StockIssueDoc> StockIssueDocs { get; set; }
      public DbSet<StockIssueDocDetail> StockIssueDocDetails { get; set; }
      public DbSet<StockIssueReturn> StockIssueReturns { get; set; }
      public DbSet<StockIssueReturnDetail> StockIssueReturnDetails { get; set; }
      public DbSet<StockTransfer> StockTransfers { get; set; }
      public DbSet<StockTransferDetail> StockTransferDetails { get; set; }
      public DbSet<StockAdjustment> StockAdjustments { get; set; }
      public DbSet<StockDamageLoss> StockDamageLosses { get; set; }
      public DbSet<StockRequest> StockRequests { get; set; }
      public DbSet<StockRequestDetail> StockRequestDetails { get; set; }
      public DbSet<StockIssueUniformLoanLink> StockIssueUniformLoanLinks { get; set; }
      ```
      Files: `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Models\OBMSDbContext.cs`
      Verify: `dotnet build` — 0 errors.

- [ ] 2-D. Update target IInventoryRepository interface.
      Replace `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Repositories\Interface\IInventoryRepository.cs` with the source version.
      Two signature differences to preserve: `GetSuppliers(string? category = null)` and `GetUtilityBillsMasterList(string userID)` — both have param additions that match the source exactly.
      Files: `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Repositories\Interface\IInventoryRepository.cs`
      Verify: `dotnet build` — 0 errors (will fail until 2-E completes, but interface itself should compile).

- [ ] 2-E. Copy new Repository Interfaces from source to target.
      Copy `IApprovalRepository.cs` and `INotificationRepository.cs` from `d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI\Repositories\Interface\` to `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Repositories\Interface\`.
      Files: `IApprovalRepository.cs`, `INotificationRepository.cs`
      Verify: `dotnet build` — 0 errors.

- [ ] 2-F. Replace InventoryRepository implementation.
      Replace `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Repositories\Implementation\InventoryRepository.cs` with the source version (full file copy).
      The source file implements all ~120 interface methods including the new Parts 2–9.
      Signature fix: the target's old `GetSuppliers()` is replaced by `GetSuppliers(string? category = null)` — the source version already has this signature.
      Verify: `dotnet build` — 0 errors.

- [ ] 2-G. Copy new Repository Implementations from source to target.
      Copy `ApprovalRepository.cs` and `NotificationRepository.cs` from `d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI\Repositories\Implementation\` to `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Repositories\Implementation\`.
      Files: `ApprovalRepository.cs`, `NotificationRepository.cs`
      Verify: `dotnet build` — 0 errors.

- [ ] 2-H. Copy SignalR NotificationHub.
      Create folder `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Hubs\`.
      Copy `NotificationHub.cs` from `d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI\Hubs\` to `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Hubs\`.
      Files: `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Hubs\NotificationHub.cs`
      Verify: `dotnet build` — 0 errors.

- [ ] 2-I. Replace InventoryController with full source version.
      Replace `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Controllers\InventoryController.cs` with the complete source version from `d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI\Controllers\InventoryController.cs`.
      No namespace change needed (both `OBMS.WebAPI.Controllers`).
      Verify: `dotnet build` — 0 errors.

- [ ] 2-J. Copy new Controllers from source to target.
      Copy `ApprovalController.cs`, `StockRequestController.cs`, and `NotificationController.cs` from `d:\Gunma\FWG-Malaysia-New\OBMSAPP-API\OBMS.WebAPI\Controllers\` to `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Controllers\`.
      Files: `ApprovalController.cs`, `StockRequestController.cs`, `NotificationController.cs`
      Verify: `dotnet build` — 0 errors.

- [ ] 2-K. Update Program.cs — add SignalR, CORS, and new DI registrations.
      Edit `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Program.cs`.
      - Add NuGet package `Microsoft.AspNetCore.SignalR` if not already referenced in the .csproj.
      - Add `using OBMS.WebAPI.Hubs;` at top.
      - Add DI registrations BEFORE the existing `IInventoryRepository` line:
        ```csharp
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
        builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
        ```
      - Add `builder.Services.AddSignalR();` after the existing `builder.Services.AddMvc()...` block.
      - Replace the `app.UseCors(...)` AllowAnyOrigin call with the source's explicit WithOrigins list (copy from source Program.cs). Change `AllowCredentials()` flag is required because SignalR needs it.
      - Add `app.MapHub<NotificationHub>("/hubs/notifications");` after `app.MapControllers()`.
      - **Do NOT change** the JWT key — target uses `"obms-authentication-secret-key-32chars!!"`.
      - **Do NOT change** any database connection string settings.
      Files: `d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\Program.cs`
      Verify: `dotnet build` — 0 errors. `dotnet run` — API starts on port 5003.

---

### Phase 3 — Angular UI

**Dependency note:** Service methods must be added before components that use them. Routing must be added after components exist. Sidebar is last.

---

- [ ] 3-A. Copy all 31 missing inventory module folders from source to target.
      For each folder listed in Gap Analysis Section J, copy the complete folder (all .ts, .html, .css files) from `d:\Gunma\FWG-Malaysia-New\OBMSAPP-UI\angularapp\src\app\modules\inventory\<module-name>\` to `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\modules\inventory\<module-name>\`.
      Modules to copy (31 total):
      `approval-history`, `approval-history-dialog`, `asset-allocation`, `asset-damage`, `asset-disposal`, `asset-location`, `asset-maintenance`, `asset-movement-history`, `asset-return`, `asset-transfer`, `branch-stock-request`, `brand-master`, `employee-issue-register`, `employee-item-holding`, `goods-receipt`, `inventory-timesheet`, `notification-list`, `opening-stock`, `pending-approvals`, `purchase-order`, `purchase-requisition`, `purchase-return`, `stock-adjustment`, `stock-balance-summary`, `stock-damage-loss`, `stock-issue`, `stock-issue-return`, `stock-ledger`, `stock-transfer`, `uom-master`, `warehouse-master`
      Files: All files under each of the 31 new subfolders in target modules/inventory.
      Verify: `ng build --configuration=development` from `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp` — 0 compilation errors.

- [ ] 3-B. Merge updated versions of existing inventory modules.
      Four modules exist in target but source has richer versions. For each, copy the source version over the target version:
      - `asset-master` — copy source over target (adds Code, SerialNo, Status fields to form)
      - `category-master` — copy source over target (adds ParentCategoryID hierarchy)
      - `item-master` — copy source over target (adds BrandID, UOMID, WarehouseID, ReorderLevel fields)
      - `material-issue` — copy source over target (updated to new StockIssueDoc flow)
      Source path: `d:\Gunma\FWG-Malaysia-New\OBMSAPP-UI\angularapp\src\app\modules\inventory\<module>\`
      Target path: `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\modules\inventory\<module>\`
      Files: All .ts, .html, .css files in the 4 above module folders.
      Verify: `ng build` — 0 errors.

- [ ] 3-C. Copy source notification.service.ts and signalr-notification.service.ts to target.
      Copy `d:\Gunma\FWG-Malaysia-New\OBMSAPP-UI\angularapp\src\app\service\notification.service.ts` and `signalr-notification.service.ts` to `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\service\`.
      Update the `apiUrl` / SignalR hub URL in `signalr-notification.service.ts` to point to `http://localhost:5003` (target API port) if the source has a different URL hardcoded.
      Files: `notification.service.ts`, `signalr-notification.service.ts`
      Verify: `ng build` — 0 errors.

- [ ] 3-D. Extend target inventory.service.ts with all missing service methods.
      Open `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\service\inventory.service.ts`.
      Append all service methods present in the source `inventory.service.ts` that are absent from the target. **Do not remove or replace any existing target methods.**
      Source: `d:\Gunma\FWG-Malaysia-New\OBMSAPP-UI\angularapp\src\app\service\inventory.service.ts`
      Method groups to add (75+ methods):
      - Brand CRUD (saveBrandMaster, getBrandMasters, getBrandByID, deleteBrand, getBrandCode)
      - UOM CRUD (saveUOMMaster, getUOMMasters, getUOMByID, deleteUOM, getUOMCode)
      - Warehouse CRUD (saveWarehouseMaster, getWarehouseMasters, getWarehouseByID, deleteWarehouse, getWarehouseCode)
      - Opening Stock (getOpeningStockList, getOpeningStockByID, getOpeningStockCode, saveOpeningStock, deleteOpeningStock)
      - Stock Ledger (getStockLedger, getCurrentStock, getItemsForStock)
      - PR full flow (getPRList, getPRByID, getPRCode, savePR, submitPR, approvePR, rejectPR, deletePR, getApprovedPRsForPO, getPRProcessingTasks, claimPR)
      - PO full flow (getPOList, getPOByID, getPOCode, savePO, approvePO, cancelPO, getPOEditability, getPOByPRID, getApprovedPOsForGRN)
      - GRN full flow (getGRNList, getGRNByID, getGRNCode, saveGRN, deleteGRN, getGRNByPOID, getPOLinesForGRN, checkGRNDependency)
      - Purchase Return (getPurchaseReturnList, getPurchaseReturnByID, savePurchaseReturn, getGRNsForReturn, getGRNLinesForReturn)
      - Stock Issue full flow (getStockIssueList, getStockIssueByID, getStockIssueCode, saveStockIssue, deleteStockIssue)
      - Stock Issue Return (getIssuableDocsForReturn, getIssueDetailsForReturn, getStockIssueReturnList, getStockIssueReturnByID, getStockIssueReturnCode, saveStockIssueReturn)
      - Stock Transfer (getStockTransferList, getStockTransferByID, getStockTransferCode, saveStockTransfer, deleteStockTransfer)
      - Stock Adjustment (getStockAdjustmentList, getStockAdjustmentByID, getStockAdjustmentCode, saveStockAdjustment, deleteStockAdjustment, submitSA, approveSA, rejectSA)
      - Stock Damage/Loss (getStockDamageLossList, getStockDamageLossByID, getStockDamageLossCode, saveStockDamageLoss, deleteStockDamageLoss)
      - Employee holding / registers (getEmployeeHoldingSummaryList, getEmployeeHoldingDetail, getAllEmployeeHoldings, getStockBalanceSummary, getEmployeeIssueRegister, getEmployeesByWarehouse, getPendingStockIssuesForEmployee, linkStockIssueToUniformLoan)
      - Asset Management (getAssetList, getAssetByIDEnriched, saveAsset, deleteAsset, getAssetAllocationList, saveAssetAllocation, cancelAssetAllocation, getAssetTransferList, saveAssetTransfer, getAssetReturnList, saveAssetReturn, getAssetMaintenanceList, saveAssetMaintenance, completeAssetMaintenance, getAssetDamageList, saveAssetDamage, getAssetDisposalList, saveAssetDisposal, submitAD, approveAD, rejectAD, getAssetMovementHistory, updateAssetLocation, getAssetLocationLog, getDefectLocationAssets)
      - BSR (getBSRList, getBSRByID, getBSRCode, saveBSR, submitBSR, approveBSR, rejectBSR, deleteBSR, linkBSRTransfer)
      - Approval (getPendingApprovals, getApprovalHistory, getAllApprovalHistory, submitPO, approvePOWithHistory, rejectPO, getApprovalConfigurations, updateApprovalConfiguration, isCurrentUserApprover, getApprovalRoutings, saveApprovalRouting, deactivateApprovalRouting)
      Files: `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\service\inventory.service.ts`
      Verify: `ng build` — 0 errors.

- [ ] 3-E. Add all new inventory routes to admin-layout-routing.module.ts.
      Edit `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\layout\admin-layout\admin-layout-routing.module.ts`.
      Add import statements for all 31 new module components at the top.
      Add route entries under the `//Inventory module` comment section, following the exact pattern already used (e.g. `{ path: 'inventory/brand-master', component: BrandMasterComponent, canActivate: [AuthGuard] }`).
      All routes require `canActivate: [AuthGuard]`.
      Route paths to add (representative — include new/edit sub-routes for each):
      - `inventory/brand-master`, `inventory/brand-master/new-brand`, `inventory/brand-master/new-brand/:ID`
      - `inventory/uom-master`, `inventory/uom-master/new-uom`, `inventory/uom-master/new-uom/:ID`
      - `inventory/warehouse-master`, `inventory/warehouse-master/new-warehouse`, `inventory/warehouse-master/new-warehouse/:ID`
      - `inventory/opening-stock`, `inventory/opening-stock/new-opening-stock`, `inventory/opening-stock/new-opening-stock/:ID`
      - `inventory/stock-ledger`
      - `inventory/purchase-requisition`, `inventory/purchase-requisition/new-pr`, `inventory/purchase-requisition/new-pr/:ID`
      - `inventory/purchase-order`, `inventory/purchase-order/new-po`, `inventory/purchase-order/new-po/:ID`
      - `inventory/goods-receipt`, `inventory/goods-receipt/new-grn`, `inventory/goods-receipt/new-grn/:ID`
      - `inventory/purchase-return`, `inventory/purchase-return/new-purchase-return`, `inventory/purchase-return/new-purchase-return/:ID`
      - `inventory/stock-issue`, `inventory/stock-issue/new-stock-issue`, `inventory/stock-issue/new-stock-issue/:ID`
      - `inventory/stock-issue-return`, `inventory/stock-issue-return/new-return`, `inventory/stock-issue-return/new-return/:ID`
      - `inventory/stock-transfer`, `inventory/stock-transfer/new-transfer`, `inventory/stock-transfer/new-transfer/:ID`
      - `inventory/stock-adjustment`, `inventory/stock-adjustment/new-adjustment`, `inventory/stock-adjustment/new-adjustment/:ID`
      - `inventory/stock-damage-loss`, `inventory/stock-damage-loss/new-damage`, `inventory/stock-damage-loss/new-damage/:ID`
      - `inventory/stock-balance-summary`
      - `inventory/employee-item-holding`
      - `inventory/employee-issue-register`
      - `inventory/asset-allocation`, `inventory/asset-allocation/new-allocation`, `inventory/asset-allocation/new-allocation/:ID`
      - `inventory/asset-transfer`, `inventory/asset-transfer/new-transfer`, `inventory/asset-transfer/new-transfer/:ID`
      - `inventory/asset-return`, `inventory/asset-return/new-return`, `inventory/asset-return/new-return/:ID`
      - `inventory/asset-maintenance`, `inventory/asset-maintenance/new-maintenance`, `inventory/asset-maintenance/new-maintenance/:ID`
      - `inventory/asset-damage`, `inventory/asset-damage/new-damage`, `inventory/asset-damage/new-damage/:ID`
      - `inventory/asset-disposal`, `inventory/asset-disposal/new-disposal`, `inventory/asset-disposal/new-disposal/:ID`
      - `inventory/asset-location`
      - `inventory/asset-movement-history`
      - `inventory/branch-stock-request`, `inventory/branch-stock-request/new-bsr`, `inventory/branch-stock-request/new-bsr/:ID`
      - `inventory/pending-approvals`
      - `inventory/approval-history`
      - `inventory/notification-list`
      - `inventory/inventory-timesheet`
      Files: `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\layout\admin-layout\admin-layout-routing.module.ts`
      Verify: `ng build` — 0 errors. Navigate to `/inventory/brand-master` in browser — component renders.

- [ ] 3-F. Add all new inventory menu items to the target sidebar HTML.
      Edit `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\components\sidebar\sidebar.component.html`.
      Study the exact HTML structure, CSS classes, and icon patterns already used in the target sidebar for Inventory menu items. Apply identical structure for all new menu entries.
      The source sidebar HTML is at `d:\Gunma\FWG-Malaysia-New\OBMSAPP-UI\angularapp\src\app\components\sidebar\sidebar.component.html` — use it as the reference for what menu entries exist, but rewrite the HTML to use the target's CSS classes, colour scheme, and layout exactly (do not paste source HTML verbatim).
      New menu groups/entries to add under the Inventory section:
      **Masters:** Brand Master, UOM Master, Warehouse Master
      **Stock:** Opening Stock, Stock Ledger, Purchase Requisition, Purchase Order, Goods Receipt, Purchase Return
      **Operations:** Stock Issue, Stock Issue Return, Stock Transfer, Stock Adjustment, Stock Damage/Loss
      **Reports:** Stock Balance Summary, Employee Item Holding, Employee Issue Register
      **Assets:** Asset Allocation, Asset Transfer, Asset Return, Asset Maintenance, Asset Damage, Asset Disposal, Asset Location, Asset Movement History
      **Branch:** Branch Stock Request
      **Workflow:** Pending Approvals, Approval History, Notifications
      Files: `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp\src\app\components\sidebar\sidebar.component.html`
      Verify: Start Angular dev server (`ng serve`). All new sidebar items appear and navigate to the correct routes.

---

### Phase 4 — Crystal Reports

- [ ] 4-A. Copy missing report files from source to target.
      Copy the 17 missing files listed in Gap Analysis Section N from `d:\Gunma\FWG-Malaysia-New\UatReports-new\UatReports\Inventory\` to `d:\Gunma\Gunma---Poilt\Gunma - Report\Inventory\`.
      Files: `AssetAllocationReport.aspx`, `AssetDamageReport.aspx`, `AssetDisposalReport.aspx`, `AssetMaintenanceReport.aspx`, `AssetMasterReport.aspx`, `AssetReturnReport.aspx`, `AssetTransferReport.aspx`, `BSRReport.aspx`, `GRNReport.aspx`, `IssueReturnReport.aspx`, `POReport.aspx`, `PRReport.aspx`, `StockAdjustmentReport.aspx`, `StockIssueReport.aspx`, `StockLedger.rpt`, `StockLedgerReport.aspx`, `StockTransferReport.aspx`
      Verify: Files present in `d:\Gunma\Gunma---Poilt\Gunma - Report\Inventory\`.

- [ ] 4-B. Update database connection strings in copied .aspx.cs report files.
      Each `.aspx.cs` file has a hard-coded connection string pointing to the source database. Inspect each copied `.aspx.cs` file (and `.cs` files for existing `HQStockLedger` and `BranchStockLedger`).
      Update the connection string to: `Data Source=DESKTOP-FHKDAA6;Initial Catalog=OBMSUATNEW;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connect Timeout=30`
      Also update the `.rpt` file embedded connection strings if they reference a named server/database different from the target.
      Files: All `.aspx.cs` files under `d:\Gunma\Gunma---Poilt\Gunma - Report\Inventory\`
      Verify: Open `http://localhost:58008/Inventory/GRNReport.aspx?ID=1` (or any valid test ID) — report renders without connection error.

---

### Phase 5 — Build Verification

- [ ] 5-A. Full API build and startup test.
      Run `dotnet build "d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\OBMS.WebAPI.csproj"` — expect 0 errors and 0 warnings (or only pre-existing warnings).
      Then run `dotnet run --project "d:\Gunma\Gunma---Poilt\Gunma - API\OBMS.WebAPI\OBMS.WebAPI.csproj"`.
      Verify: API starts on port 5003. Open `http://localhost:5003/swagger` — all Inventory, Approval, StockRequest, and Notification endpoints appear in the Swagger UI.

- [ ] 5-B. Full Angular build test.
      Run `ng build --configuration=production` from `d:\Gunma\Gunma---Poilt\Gunma - UI\angularapp`.
      Verify: Build completes with 0 errors. (Warnings about bundle size are acceptable.)

- [ ] 5-C. API integration smoke tests — Inventory Masters.
      With API running (`dotnet run`), test via Swagger or Postman:
      - `GET /api/Inventory/GetBrandMasters` → 200 OK, empty array or data
      - `GET /api/Inventory/GetUOMMasters` → 200 OK, returns seeded UOM data (Nos, Pair, Box, etc.)
      - `GET /api/Inventory/GetWarehouseMasters` → 200 OK
      - `POST /api/Inventory/SaveAndUpdateBrandMaster` with `{"ID":0,"Name":"Test Brand","IsActive":true}` → 200 OK
      - `GET /api/Inventory/GetInventoryCategories` → 200 OK (existing data intact)
      - `GET /api/Inventory/GetInventoryItems` → 200 OK (existing items intact)
      Verify: All 6 calls return 200. Existing category/item data is unchanged.

- [ ] 5-D. API integration smoke tests — Purchase Flow.
      - `GET /api/Inventory/GetOpeningStockList` → 200 OK
      - `GET /api/Inventory/GetPRList` → 200 OK
      - `GET /api/Inventory/GetPOList` → 200 OK
      - `GET /api/Inventory/GetGRNList` → 200 OK
      - `GET /api/Approval/GetApprovalConfigurations` → 200 OK
      - `GET /api/Approval/GetApprovalRoutings` → 200 OK
      - `GET /api/StockRequest/GetBSRList` → 200 OK
      Verify: All 7 calls return 200.

- [ ] 5-E. API integration smoke tests — Asset Management.
      - `GET /api/Inventory/GetAssetList` → 200 OK
      - `GET /api/Inventory/GetAssetAllocationList?activeOnly=true` → 200 OK
      - `GET /api/Inventory/GetStockBalanceSummary` → 200 OK
      Verify: All 3 calls return 200.

- [ ] 5-F. Angular end-to-end navigation tests.
      Start Angular dev server (`ng serve --port 4200`).
      Navigate to and verify page renders (no console errors) for each new module:
      - `/inventory/brand-master` — list table renders
      - `/inventory/uom-master` — list table renders
      - `/inventory/warehouse-master` — list table renders
      - `/inventory/opening-stock` — list table renders
      - `/inventory/purchase-requisition` — list table renders
      - `/inventory/purchase-order` — list table renders
      - `/inventory/goods-receipt` — list table renders
      - `/inventory/stock-issue` — list table renders
      - `/inventory/stock-transfer` — list table renders
      - `/inventory/asset-allocation` — list table renders
      - `/inventory/pending-approvals` — list table renders
      - `/inventory/branch-stock-request` — list table renders
      Verify: No browser console errors. Each page loads and renders a list (possibly empty on fresh DB).

- [ ] 5-G. Regression test — existing modules unaffected.
      Navigate to and verify existing modules still work:
      - `/inventory/item-master` — existing items load
      - `/inventory/supplier-master` — existing suppliers load
      - `/inventory/utility-bills` — existing utility bills load
      - `/finance/invoice` — existing invoices load
      - `/payroll/employee-monthly-advance` — page loads
      - `/master/employee-master` — employee list loads
      Verify: No regressions in existing functionality.

---

## Notes and Decisions

1. **JWT key**: Target uses `"obms-authentication-secret-key-32chars!!"`. Source uses `"obms-authentication"`. Do NOT change the target key — existing user sessions would break.

2. **CORS**: Source uses `WithOrigins(...)` + `AllowCredentials()` (required for SignalR). Target uses `AllowAnyOrigin()` which is incompatible with `AllowCredentials()`. When migrating to SignalR, the CORS policy MUST switch to explicit origins. Add `http://localhost:4200` and `http://localhost:5003` at minimum for local dev. Add the Gunma live server IPs when deploying.

3. **SignalR**: Target does not have SignalR. The notification system (real-time approval notifications) depends on it. Add `Microsoft.AspNetCore.SignalR` NuGet package to the target .csproj — it is already included in .NET 8 SDK but must be registered.

4. **Approval system**: The approval workflow (`ApprovalController` + `ApprovalRepository`) is a cross-cutting concern used by PR, PO, BSR, Stock Adjustment, Asset Disposal, and potentially more. It must be wired correctly in DI before `InventoryRepository` because the inventory repo calls into the approval repo internally.

5. **StockIssues vs StockIssueDoc**: The target already has `StockIssues`/`StockIssueDetail` tables (old material-issue flow). The source has BOTH the old tables AND the new `StockIssueDoc`/`StockIssueDocDetail` tables. Do not drop the old tables — they have existing data. The new Stock Issue module uses `StockIssueDoc`.

6. **Report connection strings**: The `.rpt` Crystal Reports files embed their own database connection. After copying, open each `.rpt` in Crystal Reports designer and update the connection to point to `DESKTOP-FHKDAA6\OBMSUATNEW`. The `.aspx.cs` code-behind files override the connection at runtime via `SetDataSourceCredentials` — update those connection strings to the target server.

7. **Angular sidebar CSS**: The target sidebar has a specific Gunma colour scheme (dark navy with gold accents, Gunma branding). Do NOT copy the source sidebar HTML structure wholesale. Read the target sidebar HTML first, identify the CSS classes and icon conventions in use, then add new menu items using the same markup pattern.

8. **Idempotency**: Every DB script step is idempotent (`IF NOT EXISTS` guards throughout). Running the scripts a second time is safe.
