using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCRMCODEPractice.Plugins
{
 public class Duplicate_Detection_Plugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                var Name = entity.GetAttributeValue<string>("name");

                #region  Retrieve All Account Record

                QueryExpression Account = new QueryExpression { EntityName = entity.LogicalName, ColumnSet = new ColumnSet("name") };
                Account.Criteria.AddCondition("name", ConditionOperator.Equal, Name);
                Account.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

                EntityCollection RetrieveAccount = service.RetrieveMultiple(Account);

                //If the retrieved Account Count Greater Than 1 , following Error Message Throw

                if (RetrieveAccount.Entities.Count > 1)
                {
                    throw new InvalidPluginExecutionException("Following Record with Same Name Exists");

                }
                else if (RetrieveAccount.Entities.Count == 0)
                {
                    return;
                }
                #endregion
            }
        }
    }
}
