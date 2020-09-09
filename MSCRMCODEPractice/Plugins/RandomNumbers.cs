using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCRMPlugins
{
    public class RandomNumbers : IPlugin
    {
        /// <summary>
        /// A plug-in that auto generate account no. when an account is created.
        /// </summary>
        /// <remarks>Register this plugin on the create message, account Entity</remarks>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =(ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("Tracinng Started");
            tracingService.Trace("RandomNumbers Tracing Strated At \t" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));

            //Obtain the execution context from the service provider
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            tracingService.Trace("Context");

            // The input parameters collection contains all the data passed in the message rerquest.
            if (context.InputParameters.Contains("Target")&&context.InputParameters["Target"] is Entity)
            {
                // Obtain the target Entity from the input parameters
                Entity entity =(Entity)context.InputParameters["Target"];
                tracingService.Trace(entity.LogicalName+ "entity.LogicalName");
                tracingService.Trace(context.BusinessUnitId.ToString()+ "context.BusinessUnitId.ToString()");
                tracingService.Trace(context.InitiatingUserId.ToString()+ "context.InitiatingUserId.ToString()");
                tracingService.Trace(context.PrimaryEntityName+ "PrimaryEntityName");
                tracingService.Trace(context.UserId.ToString()+ "\t UserId");
                tracingService.Trace(context.InitiatingUserId.ToString()+ "\t InitiatingUserId");
                tracingService.Trace(context.Mode.ToString()+ "\t context.Mode.ToString()");

                // Verify That target entity represents an account.
                //If not, this plug-in was not registered correctly.
                if (entity.LogicalName != "account")
                    return;
                if (entity.LogicalName== "account")
                {
                    tracingService.Trace("entity.LogicalName== account");
                    // An ("accountnumber") attribute should not already exist because it is system generated
                    if (entity.Attributes.Contains("accountnumber") == false)
                    {
                        tracingService.Trace("entity.Attributes.Containsaccountnumber == false");
                        //create a new account number attribute, set its value, and add
                        //the attribute to the entity's attribute collection
                        Random rndgen = new Random();
                        tracingService.Trace("rndgen.Next().ToString()");
                        entity.Attributes.Add("accountnumber", rndgen.Next().ToString());
                        tracingService.Trace("End");
                        tracingService.Trace("The End");
                    }
                    else
                    {
                        // Throw an error , because account number must be system generated.
                        // 
                        throw new InvalidPluginExecutionException("The account no. can only be set by system.");
                    }
                }
            }
            tracingService.Trace("RandomNumbers  Tracing END At \t" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
        }
    }
}
