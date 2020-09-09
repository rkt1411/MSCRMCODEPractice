using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace CustomWFActivityTiwari21
{
    public class LeadAssignment : CodeActivity
    {


        //this is the name of the parameter that will be returned back to the workflow
        [Output("Created Day")]
        //this line declares the output parameter and declares the proper data type of the parameter being passed back.
        public OutArgument<string> CreatedDay { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            try
            {
                string _Day = string.Empty;
                DateTime _DateTime = DateTime.MinValue;
                Guid _UserID = Guid.Empty;

                ITracingService tracingService = executionContext.GetExtension<ITracingService>();

                tracingService.Trace("Tracing Strated");
                tracingService.Trace("Tracing _DateTime {0}", _DateTime);

                IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
                IOrganizationServiceFactory factory = executionContext.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = factory.CreateOrganizationService(context.UserId);

                Entity _LeadEntity = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(new string[] { "createdon" }));

                if (_LeadEntity.Contains("createdon"))
                {
                    _Day = ((DateTime)_LeadEntity.Attributes["createdon"]).DayOfWeek.ToString();
                }

                // throw new InvalidPluginExecutionException("An error occurred in the FollowupPlugin plug-in.");
                //  throw new Exception("DTPH");


                tracingService.Trace("_Day {0}", _Day);
                CreatedDay.Set(executionContext, _Day);
                tracingService.Trace("Completed");



            }
            catch (Exception ex)
            {
                throw new Exception("DTPH");
                throw new InvalidPluginExecutionException("An error occurred in the FollowupPlugin plug-in.", ex);
            }




        }
    }
}
