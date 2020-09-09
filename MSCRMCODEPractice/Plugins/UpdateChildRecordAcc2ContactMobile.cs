using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace MSCRMCODEPractice.Plugins
{
    public class UpdateChildRecordAcc2ContactMobile : IPlugin
    {

        Guid accId = Guid.Empty;
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity accEntity = (Entity)context.InputParameters["Target"];
                accId = context.PrimaryEntityId;
                tracingService.Trace("accId \t" + accId);

                try
                {
                    string contactQuery = @"<fetch mapping='logical'>
                                            <entity name='contact'> 
                                            <attribute name='contactid' />
                                                <filter type='and'>
                                                   <condition attribute='parentcustomerid' operator='eq' value='{0}' />
                                                </filter>                                       
                                          </entity>
                                         </fetch>";
                    string formatXML = string.Format(contactQuery, accId.ToString());
                    EntityCollection contactResults = service.RetrieveMultiple(new FetchExpression(formatXML));
                    if (contactResults.Entities.Count() > 0 && accEntity["telephone1"] != null)
                    {
                        foreach (Entity c in contactResults.Entities)
                        {
                            Entity contactEntity = new Entity("contact");
                            contactEntity.Id = c.Id;
                            contactEntity["telephone1"] = accEntity["telephone1"];
                            service.Update(contactEntity);
                        }
                    }
                }
                catch (FaultException ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in the plug-in.", ex);
                }
            }
        }
    }
}
