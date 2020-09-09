using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace MSCRMPlugins
{
    public class FollowupPlugin : IPlugin
    {
        /// <summary>
        /// A plug-in that creates a fallow up task activity when a new account is created.
        /// </summary>
        /// <remarks>Register this plugin- on the create message, account entity
        /// and asynchronous mode</remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("FollowupPlugin  Tracing Strated At \t" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
            //Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // the input parameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];
                tracingService.Trace("entity.LogicalName \t" + entity.LogicalName);

                //Verify that the target entity represents an account.
                //If not, this plug-in was not registred correctly.
                if (entity.LogicalName!="account")
                {
                    return;
                }
                try
                {
                    // Create a task activity to follow up with the account customer in 7 days.
                    Entity followup = new Entity("task");
                    followup["subject"] = "Send E-Mail to the new customer."+entity.LogicalName;
                    followup["description"] = "Follow up with the customer.Check if there are any new issues that need resolution.";
                    followup["scheduledstart"] = DateTime.Now.AddDays(7);
                    followup["scheduledend"] = DateTime.Now.AddDays(7);
                    tracingService.Trace("DateTime.Now.AddDays(7)" + DateTime.Now.AddDays(7));

                    tracingService.Trace(context.PrimaryEntityName+ "context.PrimaryEntityName");
                    followup["category"] = context.PrimaryEntityName;

                    // Refer to the account in the task activity.
                    if (context.OutputParameters.Contains("id"))
                    {
                        Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                        tracingService.Trace("context.OutputParameters['id'].ToString() \t" + context.OutputParameters["id"].ToString());

                        string regardingobjectidType = context.PrimaryEntityName.ToString();

                        tracingService.Trace("context.PrimaryEntityName;\t" + context.PrimaryEntityName);

                        followup["regardingobjectid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                        
                    }

                    //Obtain the organization service refrence.
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                    service.Create(followup);
                    tracingService.Trace("Task Created Successfully");
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in the FollowupPlugin plug-in.", ex);
                }
                catch (Exception ex)
                {
                    tracingService.Trace("Folloup Plugin: {0}", ex.ToString());
                    throw;
                }
            }
            tracingService.Trace("FollowupPlugin  Tracing END At \t" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
        }
    }
}
