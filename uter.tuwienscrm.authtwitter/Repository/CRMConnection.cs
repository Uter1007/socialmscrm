using System;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace uter.tuwien.authservice.Repository
{
    public class CRMConnection
    {
        private static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
       
        public static IOrganizationService Service(CrmServerConfig crmServerConfig, bool admin)
        {


            if (!crmServerConfig.LiveEnabled){
                Log.Trace(String.Format("Initialize Service - Admin: {0}", admin));

                var credentials = GetClientCredentials(crmServerConfig,admin);

                var uristring = GetOrganizationURI(crmServerConfig);

                Uri OrganizationUri = new Uri(uristring);
                //Uri HomeRealmUri = null;

                IServiceConfiguration<IOrganizationService> orgConfigInfo =
                    ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(OrganizationUri);

                //OrganizationServiceProxy serviceProxy; 
                using (OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(orgConfigInfo,credentials))
                {

                    //serviceProxy.CallerId = userId;

                    serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
                    //serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new MessageInspectorBehaviour());

                    IOrganizationService service = (IOrganizationService)serviceProxy;
                    
                    Log.Trace("Return Service");

                    return service;
                }
            }else{
                OrganizationServiceProxy serviceProxy;
                ClientCredentials deviceCredentials = null;
                ClientCredentials clientCredentials = new ClientCredentials();;
                        

                Uri OrganizationUri = new Uri(String.Format(crmServerConfig.LiveUrl, crmServerConfig.OrganizationName));

                Uri HomeRealmUri = new Uri(String.Format(crmServerConfig.HomeRealmUri, crmServerConfig.HomeRealmOrg));

                clientCredentials.UserName.UserName = crmServerConfig.LiveUserName;
                clientCredentials.UserName.Password = crmServerConfig.LivePassword;
                        
                deviceCredentials = GetDeviceCredentials();

                using (serviceProxy = new OrganizationServiceProxy(OrganizationUri, HomeRealmUri, clientCredentials, deviceCredentials))
                {

                    //serviceProxy.CallerId = userId;

                    serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
                    //serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new MessageInspectorBehaviour());

                    IOrganizationService service = (IOrganizationService)serviceProxy;

                    Log.Trace("Return Service");

                    return service;
                }        
            }
            
        }

        public static ClientCredentials GetDeviceCredentials()
        {
            return Microsoft.Crm.Services.Utility.DeviceIdManager.LoadOrRegisterDevice();
        }

        public static ClientCredentials GetClientCredentials(CrmServerConfig crmServerConfig,bool admin)
        {
            var credentials = new ClientCredentials();

            if (!crmServerConfig.IFDEnabled)
            {
                if (admin)
                {
                    credentials.Windows.ClientCredential = new System.Net.NetworkCredential(crmServerConfig.UserName,
                                                                                            crmServerConfig.Password,
                                                                                            crmServerConfig.Domain);

                    Log.Trace(String.Format("Credentials: {0}", credentials.Windows.ClientCredential.UserName));

                }
                else
                {
                    Log.Trace("Use Default Credentials");
                    credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
                }

                            
            }
            else
            {
                credentials.UserName.UserName = crmServerConfig.UserName;
                credentials.UserName.Password = crmServerConfig.Password;
            }

            return credentials;
        }

        public static string GetOrganizationURI(CrmServerConfig crmServerConfig)
        {
            string uristring;
            var xrmsuffix = @"XRMServices/2011/Organization.svc";

            if (!crmServerConfig.IFDEnabled)
            {


                uristring = String.Format("{0}://{1}.{2}:{3}/{4}/{5}", crmServerConfig.HTTPSEnabled ? "https" : "http",
                                          crmServerConfig.HostName, crmServerConfig.ServerDomain, crmServerConfig.Port,
                                          crmServerConfig.OrganizationName,
                                          xrmsuffix);

                

            }
            else
            {
                

                uristring = String.Format("{0}://{1}.{2}:{3}/{4}", crmServerConfig.HTTPSEnabled ? "https" : "http",
                                          crmServerConfig.OrganizationName,crmServerConfig.ServerDomain ,crmServerConfig.Port, xrmsuffix);
            }

            Log.Trace(String.Format("Org URI: {0}", uristring));

            return uristring;
        }
    }
}
