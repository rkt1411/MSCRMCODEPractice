using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;

using Microsoft.Xrm.Tooling.Connector;

namespace DownLoadContactsImage
{
   public class CRMHelper
    {
        OrganizationServiceProxy _serviceProxy;
        IOrganizationService iOrgService;
        public IOrganizationService setSrvice()
        {
            try
            {
                ClientCredentials cre = new ClientCredentials();
                cre.UserName.UserName = "rahul@tiwari20.onmicrosoft.com";
                cre.UserName.Password = "welcome@123";
                Uri serviceUri = new Uri("https://tiwari20.api.crm8.dynamics.com/XRMServices/2011/Organization.svc");
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
