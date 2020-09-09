using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
namespace MSCRMCODEPractice.Plugins
{
    public class PreImageContact : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("Plugin Exicution Started");
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService servive = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                if (context.MessageName.ToLower() == "update")
                {
                    tracingService.Trace("Inside update message");

                    Entity contactEntity = (Entity)context.InputParameters["Target"];
                    tracingService.Trace("context.PrimaryEntityName {0}", context.PrimaryEntityName);

                    if (context.PrimaryEntityName != "contact")
                    {
                        tracingService.Trace("context.PrimaryEntityName {0} in not ConTACt", context.PrimaryEntityName);
                        return;
                    }
                    if (context.PrimaryEntityName == "contact")
                    {
                        //PreImage will work on PreOperationOnly 
                        Entity preImageContact = (Entity)context.PreEntityImages["PREIMAGE"] as Entity;
                        //emailaddress1

                        if (contactEntity.Attributes["emailaddress1"].ToString().ToLower() == preImageContact.GetAttributeValue<string>("emailaddress1").ToLower())
                        {
                            throw new InvalidPluginExecutionException("Email is not Changed");
                        }
                        if (contactEntity.Attributes["emailaddress1"].ToString().ToLower() != preImageContact.GetAttributeValue<string>("emailaddress1").ToLower())
                        {
                            throw new InvalidPluginExecutionException("Email Address Changed " + preImageContact.Attributes["emailaddress1"].ToString());
                        }
                    }
                }
            }
        }
    }
}
