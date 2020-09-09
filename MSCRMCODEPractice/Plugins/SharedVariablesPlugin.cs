using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCRMCODEPractice.Plugins
{
    public class PreEventPlugin : IPlugin

    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService iOrsService = factory.CreateOrganizationService(context.InitiatingUserId);

            string sharedvariable = "DDLH";

            context.SharedVariables.Add("SHAREDVARIABLE", (object)sharedvariable.ToString());
        }
    }

    public class PostEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService iOrsService = factory.CreateOrganizationService(context.InitiatingUserId);

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target Entity from the input parameters
                Entity entity = new Entity("rah_bosch");
                

                

                //if (entity.Attributes.Contains("description")==false)
                //{

                    string SHARED = (context.SharedVariables["SHAREDVARIABLE"]).ToString();
                    entity["rah_name"] = SHARED;

                    iOrsService.Create(entity);
                   
                //}
                //else
                //{
                //    // Throw an error , because account number must be system generated.
                //    // 
                //    throw new InvalidPluginExecutionException("The account no. can only be set by system.");
                //}
            }

        }
    }
}

