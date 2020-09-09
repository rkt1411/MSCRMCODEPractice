﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using System.Net;

//using Microsoft.Xrm.Tooling.Connector;

namespace ExecuteMultipleSample1
{
   public class CRMHelper
    {
        OrganizationServiceProxy _serviceProxy;
        IOrganizationService iOrgService;
        public IOrganizationService setSrvice()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ClientCredentials cre = new ClientCredentials();
                cre.UserName.UserName = "rahul@tiwari22.onmicrosoft.com";
                cre.UserName.Password = "welcome@123";
                Uri serviceUri = new Uri("https://tiwari22.api.crm.dynamics.com/XRMServices/2011/Organization.svc");
                _serviceProxy = new OrganizationServiceProxy(serviceUri, null, cre, null);
                iOrgService =(IOrganizationService)_serviceProxy;
            }
            catch (Exception)
            {

                throw;
            }
            return iOrgService;
        }
    }
}
