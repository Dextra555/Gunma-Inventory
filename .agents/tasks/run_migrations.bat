@echo off
setlocal

set SERVER=DESKTOP-FHKDAA6
set DB=OBMSUATNEW
set DBDIR=d:\Gunma\Gunma---Poilt\Database
set LOGDIR=d:\Gunma\Gunma---Poilt\.agents\tasks\logs

mkdir "%LOGDIR%" 2>nul

echo Running migration scripts...

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\15.ApprovalSystem_BugFix_Migration.sql" > "%LOGDIR%\15.log" 2>&1
echo [%ERRORLEVEL%] 15.ApprovalSystem_BugFix_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\16.StockAdjustment_ApprovalWorkflow_Migration.sql" > "%LOGDIR%\16.log" 2>&1
echo [%ERRORLEVEL%] 16.StockAdjustment_ApprovalWorkflow_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\17.StockLedgerReport_Migration.sql" > "%LOGDIR%\17.log" 2>&1
echo [%ERRORLEVEL%] 17.StockLedgerReport_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\18.AssetDisposal_ApprovalWorkflow_Migration.sql" > "%LOGDIR%\18.log" 2>&1
echo [%ERRORLEVEL%] 18.AssetDisposal_ApprovalWorkflow_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\19.GRN_CreditorInvoice_Backfill_Migration.sql" > "%LOGDIR%\19.log" 2>&1
echo [%ERRORLEVEL%] 19.GRN_CreditorInvoice_Backfill_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\20.TransactionPDFReports_Migration.sql" > "%LOGDIR%\20.log" 2>&1
echo [%ERRORLEVEL%] 20.TransactionPDFReports_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\20a.TransactionPDFReports_Fix.sql" > "%LOGDIR%\20a.log" 2>&1
echo [%ERRORLEVEL%] 20a.TransactionPDFReports_Fix.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\21.StockIssuePDFReport_Migration.sql" > "%LOGDIR%\21.log" 2>&1
echo [%ERRORLEVEL%] 21.StockIssuePDFReport_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\22.IssueReturnPDFReport_Migration.sql" > "%LOGDIR%\22.log" 2>&1
echo [%ERRORLEVEL%] 22.IssueReturnPDFReport_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\23.StockTransferPDFReport_Migration.sql" > "%LOGDIR%\23.log" 2>&1
echo [%ERRORLEVEL%] 23.StockTransferPDFReport_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\24.StockAdjustmentPDFReport_Migration.sql" > "%LOGDIR%\24.log" 2>&1
echo [%ERRORLEVEL%] 24.StockAdjustmentPDFReport_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\25.AssetPDFReports_Migration.sql" > "%LOGDIR%\25.log" 2>&1
echo [%ERRORLEVEL%] 25.AssetPDFReports_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\26.PRDepartmentNotification_Migration.sql" > "%LOGDIR%\26.log" 2>&1
echo [%ERRORLEVEL%] 26.PRDepartmentNotification_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\27.PRProcessingTask_Migration.sql" > "%LOGDIR%\27.log" 2>&1
echo [%ERRORLEVEL%] 27.PRProcessingTask_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\ApprovalRoutingBugFix_Migration.sql" > "%LOGDIR%\ApprovalRoutingBugFix.log" 2>&1
echo [%ERRORLEVEL%] ApprovalRoutingBugFix_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\ApprovalRouting_Part7_UserWise_Migration.sql" > "%LOGDIR%\ApprovalRouting_UserWise.log" 2>&1
echo [%ERRORLEVEL%] ApprovalRouting_Part7_UserWise_Migration.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\Fix_PRList_SubmittedBy_Columns.sql" > "%LOGDIR%\Fix_PRList.log" 2>&1
echo [%ERRORLEVEL%] Fix_PRList_SubmittedBy_Columns.sql

sqlcmd -S %SERVER% -d %DB% -E -C -i "%DBDIR%\Fix_SupplierInvoiceView_ExcludeGRN.sql" > "%LOGDIR%\Fix_SupplierInvoice.log" 2>&1
echo [%ERRORLEVEL%] Fix_SupplierInvoiceView_ExcludeGRN.sql

echo.
echo All scripts completed.
