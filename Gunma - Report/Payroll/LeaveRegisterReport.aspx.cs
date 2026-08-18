using System;
using System.Configuration;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;

public partial class PayRoll_LeaveRegisterReport : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        try
        {

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in crptLeaveRegister.ReportDocument.Database.Tables)
            {
                // Optionally, ensure the Tool Panel is hidden
                crptLeaveRegisterList.ToolPanelView = ToolPanelViewType.None;
                TableLogOnInfo logonInfo = table.LogOnInfo;
                logonInfo.ConnectionInfo.ServerName = ConfigurationManager.AppSettings["Server"];
                logonInfo.ConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["Database"];
                logonInfo.ConnectionInfo.Password = ConfigurationManager.AppSettings["Password"];
                logonInfo.ConnectionInfo.UserID = ConfigurationManager.AppSettings["UserID"];
                table.ApplyLogOnInfo(logonInfo);
            }
            //constant header fields
            ParameterDiscreteValue paramCompanyName = new ParameterDiscreteValue();
            paramCompanyName.Value = ConfigurationManager.AppSettings["CompanyName"];

            ParameterDiscreteValue paramAddress1 = new ParameterDiscreteValue();
            paramAddress1.Value = ConfigurationManager.AppSettings["Address1"];

            ParameterDiscreteValue paramAddress2 = new ParameterDiscreteValue();
            paramAddress2.Value = ConfigurationManager.AppSettings["Address2"];

            ParameterDiscreteValue paramPostCodeCity = new ParameterDiscreteValue();
            paramPostCodeCity.Value = ConfigurationManager.AppSettings["PostCode"] + " " + ConfigurationManager.AppSettings["City"];

            ParameterDiscreteValue paramState = new ParameterDiscreteValue();
            paramState.Value = ConfigurationManager.AppSettings["State"];

            ParameterDiscreteValue paramRegistration = new ParameterDiscreteValue();
            paramRegistration.Value = ConfigurationManager.AppSettings["Registration"];

            ParameterDiscreteValue paramPhone = new ParameterDiscreteValue();
            paramPhone.Value = ConfigurationManager.AppSettings["Phone"];

            //parameter fields

            ParameterDiscreteValue paramUserName = new ParameterDiscreteValue();
            paramUserName.Value = Request.QueryString["LoginID"];

            ParameterDiscreteValue paramPeriod = new ParameterDiscreteValue();
            paramPeriod.Value = Request.QueryString["Period"];
            
            ParameterDiscreteValue paramEmployeeType = new ParameterDiscreteValue();
            paramEmployeeType.Value = Request.QueryString["EmployeeType"];

            crptLeaveRegisterList.ParameterFieldInfo["CompanyName"].CurrentValues.Add(paramCompanyName);
            crptLeaveRegisterList.ParameterFieldInfo["CompanyAddress1"].CurrentValues.Add(paramAddress1);
            crptLeaveRegisterList.ParameterFieldInfo["CompanyAddress2"].CurrentValues.Add(paramAddress2);
            crptLeaveRegisterList.ParameterFieldInfo["CompanyAddress3"].CurrentValues.Add(paramPostCodeCity);
            crptLeaveRegisterList.ParameterFieldInfo["CompanyAddress4"].CurrentValues.Add(paramState);
            crptLeaveRegisterList.ParameterFieldInfo["CompanyRegistration"].CurrentValues.Add(paramRegistration);
            crptLeaveRegisterList.ParameterFieldInfo["CompanyPhone"].CurrentValues.Add(paramPhone);
            crptLeaveRegisterList.ParameterFieldInfo["LoginID"].CurrentValues.Add(paramUserName);
            crptLeaveRegisterList.ParameterFieldInfo["Period"].CurrentValues.Add(paramPeriod);
            crptLeaveRegisterList.ParameterFieldInfo["EmployeeType"].CurrentValues.Add(paramEmployeeType);
            crptLeaveRegisterList.ParameterFieldInfo["PeriodYear"].CurrentValues.Add(paramPeriod);

            // Handle optional Branch parameter
            string branch = Request.QueryString["Branch"];
            if (!string.IsNullOrEmpty(branch))
            {
                if (DoesParameterExist("Branch"))
                {
                    if(branch != "")
                    {
                        ParameterDiscreteValue paramBranch = new ParameterDiscreteValue { Value = branch };
                        crptLeaveRegisterList.ParameterFieldInfo["Branch"].CurrentValues.Add(paramBranch);

                        // Apply record selection formula
                        crptLeaveRegister.ReportDocument.RecordSelectionFormula += " AND {BranchMaster.Code} = '" + branch + "'";
                    }
                    else
                    {
                        ParameterDiscreteValue paramBranchAll = new ParameterDiscreteValue { Value = "ALL BRANCHES" };
                        crptLeaveRegisterList.ParameterFieldInfo["Branch"].CurrentValues.Add(paramBranchAll);
                    }                    
                }                
            }

        }
        catch (ArgumentNullException ex)
        {
            ShowMessage("Data Cannot be null." + ex.Message);
        }
        catch (Exception ex)
        {
            ShowMessage("Error Found." + ex.Message);
        }

	}
    private bool DoesParameterExist(string parameterName)
    {
        try
        {
            var paramField = crptLeaveRegisterList.ParameterFieldInfo[parameterName];
            return paramField != null;
        }
        catch
        {
            return false;
        }
    }
    protected void ShowMessage(string Message)
    {
        Response.Write("Error: " + Message);
    }
}
