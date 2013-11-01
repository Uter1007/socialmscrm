using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace uter.sociallistener.plugins
{
    public class SetFacebookImage : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //// Get a reference to the organization service.
            //IOrganizationServiceFactory factory =
            //    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            //IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            // Get a reference to the tracing service.
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parmameters.
                    Entity entity = (Entity)context.InputParameters["Target"];

                    if (entity.Contains("cott_facebookuserid"))
                    {
                        if (entity.Contains("cott_profilepic"))
                        {
                            entity["cott_profilepic"] = String.Format("https://graph.facebook.com/{0}/picture", (string)entity["cott_facebookuserid"]);
                        }
                        else
                        {
                            entity.Attributes.Add("cott_profilepic", String.Format("https://graph.facebook.com/{0}/picture", (string)entity["cott_facebookuserid"]));
                        }
                    }
                
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                // Handle the exception.
            }
        }

    }
}
