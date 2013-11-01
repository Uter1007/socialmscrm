using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace uter.sociallistener.plugins
{
    public class FilloutHashSearchURL : IPlugin
    {
        private static string TOFILL = @"cott_twitterurl";
        private static string FROM = @"cott_name";

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

                    if (entity.Contains(FROM))
                    {
                        var name = String.Format("http://twitter.com/search?q=%23{0}", entity[FROM]);

                        if (entity.Contains(TOFILL))
                        {
                            entity[TOFILL] = name;
                        }
                        else
                        {
                            entity.Attributes.Add(TOFILL, name);
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
