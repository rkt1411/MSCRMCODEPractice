using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ExecuteMultipleSample1
{
    public class ExecuteMultiple
    {
        CRMHelper objCRMHelper;
        IOrganizationService service;
        public void Run(int batchSize)
        {
            objCRMHelper = new CRMHelper();
            service = objCRMHelper.setSrvice();

            //int batchSize = 2500;

            ExecuteMultipleRequest req = new ExecuteMultipleRequest();
            req.Requests = new OrganizationRequestCollection();
            req.Settings = new ExecuteMultipleSettings();
            req.Settings.ContinueOnError = true;
            req.Settings.ReturnResponses = true;

            try
            {
                for (int i = 0; i < batchSize; i++)
                {
                    Entity lead = new Entity("lead");
                    lead["subject"] = "Test Lead";
                    lead["firstname"] = "Requested Lead";
                    lead["lastname"] = "Name " + i;
                    var createRequest = new CreateRequest();

                    createRequest.Target = lead;
                    req.Requests.Add(createRequest);
                }

                var res = service.Execute(req) as ExecuteMultipleResponse;  //Execute the collection of requests
            }

            //If the BatchSize exceeds 1000 fault will be thrown.In the catch block divide the records into batchable records and create
            catch (FaultException<OrganizationServiceFault> fault)
            {
                if (fault.Detail.ErrorDetails.Contains("MaxBatchSize"))
                {
                    var allowedBatchSize = Convert.ToInt32(fault.Detail.ErrorDetails["MaxBatchSize"]);
                    int remainingCreates = batchSize;

                    while (remainingCreates > 0)
                    {
                        var recordsToCreate = Math.Min(remainingCreates, allowedBatchSize);
                        Run(recordsToCreate);
                        remainingCreates -= recordsToCreate;
                    }
                }
            }

            //catch (FaultException<OrganizationServiceFault> fault)
            //{
            //    // Check if the maximum batch size has been exceeded. The maximum batch size is only included in the fault if it
            //    // the input request collection count exceeds the maximum batch size.
            //    if (fault.Detail.ErrorDetails.Contains("MaxBatchSize"))
            //    {
            //        int maxBatchSize = Convert.ToInt32(fault.Detail.ErrorDetails["MaxBatchSize"]);
            //        if (maxBatchSize < requestWithResults.Requests.Count)
            //        {
            //            // Here you could reduce the size of your request collection and re-submit the ExecuteMultiple request.
            //            // For this sample, that only issues a few requests per batch, we will just print out some info. However,
            //            // this code will never be executed because the default max batch size is 1000.
            //            Console.WriteLine("The input request collection contains %0 requests, which exceeds the maximum allowed (%1)",
            //                requestWithResults.Requests.Count, maxBatchSize);
            //        }
            //    }
            // Re-throw so Main() can process the fault.
          //  throw;
        }
    }


}

