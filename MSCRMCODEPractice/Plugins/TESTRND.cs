using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace PluginCodes
{
    public class EmailAttachments : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
               (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference which you will need for
            // web service calls.
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            Guid _userID = context.InitiatingUserId;
            //Extract the tracing service for use in debugging sandboxed plug-ins.

            if (context.InputParameters.Contains("Target"))

            {
                //Confirm that Target is actually an Entity

                if (context.InputParameters["Target"] is Entity)

                {
                    try
                    {
                        Entity AccountID = (Entity)service.Retrieve("account", context.PrimaryEntityId, new ColumnSet(new string[] { "accountid" }));


                        //step 1: creating Email

                        Entity fromParty = new Entity("activityparty");
                        Entity toParty = new Entity("activityparty");

                        //Email body text is in HTML

                        //  string emailBody = "<html lang='en'><head><meta charset='UTF-8'></head><body><p>Hello,</p><p>Please be advised that I have just changed the Company Name of a Lead record in CRM:</p><p>Lead Record URL:  <a href='http://mycrm/MyCrmInstance/main.aspx?etn=lead&pagetype=entityrecord&id=%7B" + lead.Id + "%7D'>" + postCompanyName + "</a></p><p>Old Company Name Value: " + preCompanyName + "</p><p>New Company Name Value: " + postCompanyName + "</p><p>Kind Regards</p><p>" + userName + "</p></body></html>";

                        fromParty["partyid"] = new EntityReference("systemuser", _userID);
                        toParty["partyid"] = new EntityReference("systemuser", _userID);

                        Entity email = new Entity("email");

                        email["from"] = new Entity[] { fromParty };
                        email["to"] = new Entity[] { toParty };
                        email["subject"] = "Lead Company Name Changed";
                        email["directioncode"] = true;
                        email["description"] = "emailBody";

                        //This bit just creates the e-mail record and gives us the GUID for the new record...


                        Guid _emailID = service.Create(email);

                        tracingService.Trace("before function call");
                        // call function
                        AddAttachmentToEmailRecord(service, AccountID.Id, _emailID);
                        tracingService.Trace("after function call");


                    }
                    catch (Exception ex)
                    {

                        tracingService.Trace("MyPlugin: {0}", ex.ToString());
                    }

                    tracingService.Trace("Attachment created");

                }
            }
        }

        private void AddAttachmentToEmailRecord(IOrganizationService service, Guid SourceAccountID, Guid _emailID)

        {

            //create email object

            Entity _ResultEntity = service.Retrieve("email", _emailID, new ColumnSet(true));


            //get annotation 
            QueryExpression _QueryNotes = new QueryExpression("annotation");

            _QueryNotes.ColumnSet = new ColumnSet(new string[] { "subject", "mimetype", "filename", "documentbody" });

            _QueryNotes.Criteria = new FilterExpression();

            _QueryNotes.Criteria.FilterOperator = LogicalOperator.And;

            _QueryNotes.Criteria.AddCondition(new ConditionExpression("objectid", ConditionOperator.Equal, SourceAccountID));

            EntityCollection _MimeCollection = service.RetrieveMultiple(_QueryNotes);

            if (_MimeCollection.Entities.Count > 0)

            {  //we need to fetch first attachment

                Entity _NotesAttachment = _MimeCollection.Entities.First();

                //Create email attachment

                Entity _EmailAttachment = new Entity("activitymimeattachment");

                if (_NotesAttachment.Contains("subject"))

                    _EmailAttachment["subject"] = _NotesAttachment.GetAttributeValue<string>("subject");

                _EmailAttachment["objectid"] = new EntityReference("email", _ResultEntity.Id);

                _EmailAttachment["objecttypecode"] = "email";

                if (_NotesAttachment.Contains("filename"))

                    _EmailAttachment["filename"] = _NotesAttachment.GetAttributeValue<string>("filename");

                if (_NotesAttachment.Contains("documentbody"))

                    _EmailAttachment["body"] = _NotesAttachment.GetAttributeValue<string>("documentbody");

                if (_NotesAttachment.Contains("mimetype"))

                    _EmailAttachment["mimetype"] = _NotesAttachment.GetAttributeValue<string>("mimetype");

                service.Create(_EmailAttachment);

            }

            // Sending email

            SendEmailRequest SendEmail = new SendEmailRequest();

            SendEmail.EmailId = _ResultEntity.Id;

            SendEmail.TrackingToken = "";

            SendEmail.IssueSend = true;

            SendEmailResponse res = (SendEmailResponse)service.Execute(SendEmail);

        }

    }

}
