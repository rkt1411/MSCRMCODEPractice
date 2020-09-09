using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace MSCRMCODEPractice.Plugins
{
    public class PREandPostImageLEAD : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {

                Guid _userID = context.InitiatingUserId;

                //Retrieve the name of user(used Later)

                Entity _user = service.Retrieve("systemuser", _userID, new ColumnSet("fullname"));
                string _username = _user.GetAttributeValue<string>("fullname");

                tracingService.Trace("User _userID{0}", _userID);
                tracingService.Trace("User Name {0}", _username);

                Entity entityLead = context.InputParameters["Target"] as Entity;
                if (context.PrimaryEntityName != "lead")
                {
                    tracingService.Trace("context.PrimaryEntityName is not lead");
                    return;
                }
                if (context.PrimaryEntityName == "lead")
                {
                    Entity preImageLead = context.PreEntityImages["PreLeadImage"] as Entity;
                    Entity postImageLead = context.PostEntityImages["PostLeadImage"] as Entity;

                    string preCompanyName = preImageLead.GetAttributeValue<string>("companyname");
                    tracingService.Trace("Companyname before change preCompanyName{0}", preCompanyName);

                    string postCompanyName = postImageLead.GetAttributeValue<string>("companyname");
                    tracingService.Trace("Companyname before change postCompanyName{0}", postCompanyName);


                    tracingService.Trace("Pre-Company Name: " + preCompanyName + " Post-Company Name: " + postCompanyName);

                    // throw new InvalidPluginExecutionException("Pre-Company Name: " + preCompanyName + " Post-Company Name: " + postCompanyName);

                    if (preImageLead!= postImageLead)
                    {
                        tracingService.Trace("Pre company name does not match with Post-Company Name, Alerting Sales Manage");

                        Entity fromParty = new Entity("activityparty");
                        Entity toParty= new Entity("activityparty");

                        fromParty["partyid"] = new EntityReference("systemuser", _userID);
                        //toParty["partyid"] = new EntityReference("systemuser", _userID);

                        toParty["partyid"] = new EntityReference("lead", context.PrimaryEntityId);

                        tracingService.Trace("context.PrimaryEntityId");

                        Entity email = new Entity("email");
                        email["from"] = new Entity[] { fromParty };
                        email["to"] = new Entity[] { toParty };
                        email["subject"] = "Lead Company Name Changed";
                        email["directioncode"] = true;
                        email["description"] = "Pre-Company Name: " + preCompanyName + " Post-Company Name: " + postCompanyName;
                        email["regardingobjectid"] = new EntityReference("lead", context.PrimaryEntityId);

                        Guid _emailID = service.Create(email);

                        tracingService.Trace("Email record "+ _emailID+ " successfull  created ");
                        // ..to actually send it, we need to use SendEmailRequest & SendEmailResponse,
                        //... using the _emailID to reference the record.

                        SendEmailRequest sendEmailReq = new SendEmailRequest();
                        sendEmailReq.EmailId = _emailID;
                        sendEmailReq.TrackingToken = "";
                        sendEmailReq.IssueSend = true;

                        SendEmailResponse sendEmailResp = (SendEmailResponse)service.Execute(sendEmailReq);

                        tracingService.Trace("Email send Successfully");
                    }
                    else
                    {
                        tracingService.Trace("Company Name does not appear to have changed, is this correct?");
                    }
                    tracingService.Trace("Ending Plugin Execution");
                }
            }

        }
    }
}
