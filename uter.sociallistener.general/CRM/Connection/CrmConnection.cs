using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using NLog;

namespace uter.sociallistener.general.CRM.Connection
{
    public class CrmConnection
    {

        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string Organization { get; set; }
        public string URL { get; set; }

        public IOrganizationService BuildConnection()
        {

            Log.Trace("Create CRM Connection");
            var xrmsuffix = @"XRMServices/2011/Organization.svc";

            var credentials = new ClientCredentials();

            credentials.Windows.ClientCredential = new System.Net.NetworkCredential(UserName, Password, Domain);

            var uristring = String.Format("{0}/{1}/{2}",URL,Organization,xrmsuffix);

            Uri OrganizationUri = new Uri(uristring);

            IServiceConfiguration<IOrganizationService> orgConfigInfo =
                   ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(OrganizationUri);

            using (OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(orgConfigInfo, credentials))
            {

                //serviceProxy.CallerId = userId;

                serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
                //serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new MessageInspectorBehaviour());

                return (IOrganizationService)serviceProxy;


            }
        }
    }
}
