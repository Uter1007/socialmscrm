using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace uter.sociallistener.plugins
{
    public class SetFromField : IPlugin
    {
       
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //// Get a reference to the organization service.
            IOrganizationServiceFactory factory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            // Get a reference to the tracing service.
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parmameters.
                    Entity entity = (Entity)context.InputParameters["Target"];

                    var twitteruserid = ((EntityReference)entity["cott_twitteruser"]).Id;

                    tracingService.Trace("TwitterUserId: {0}", twitteruserid);

                    var accounts = FetchAccounts(service, twitteruserid);

                    tracingService.Trace("Retrieved Accounts: {0}", accounts.Entities.Count());

                    var contacts = FetchContacts(service, twitteruserid);

                    tracingService.Trace("Retrieved Contacts: {0}", contacts.Entities.Count());

                    var systemusers = FetchSystemUser(service, twitteruserid);

                    tracingService.Trace("Retrieved SystemUsers: {0}", systemusers.Entities.Count());

                    var leads = FetchLead(service, twitteruserid);

                    tracingService.Trace("Retrieved Leads: {0}", leads.Entities.Count());

                    var list = new List<Entity>();

                    foreach (var account in accounts.Entities)
                    {
                        var party = new Entity("activityparty");
                        party.Attributes.Add("partyid", new EntityReference(account.LogicalName, account.Id));
                        list.Add(party);
                    }

                    foreach (var contact in contacts.Entities)
                    {
                        var party = new Entity("activityparty");
                        party.Attributes.Add("partyid", new EntityReference(contact.LogicalName, contact.Id));
                        list.Add(party);
                    }

                    foreach (var systemuser in systemusers.Entities)
                    {
                        var party = new Entity("activityparty");
                        party.Attributes.Add("partyid", new EntityReference(systemuser.LogicalName, systemuser.Id));
                        list.Add(party);
                    }

                    foreach (var lead in leads.Entities)
                    {
                        var party = new Entity("activityparty");
                        party.Attributes.Add("partyid", new EntityReference(lead.LogicalName, lead.Id));
                        list.Add(party);
                    }

                    tracingService.Trace("Set Optionalattendees");

                    if (list.Count() > 0)
                    {

                        var entitycollection = new EntityCollection(list);

                        if (entity.Contains("optionalattendees"))
                        {
                            entity["optionalattendees"] = entitycollection;
                        }
                        else
                        {
                            entity.Attributes.Add("optionalattendees", entitycollection);
                        }

                        tracingService.Trace("Call Update");
                        service.Update(entity);
                    }

                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                tracingService.Trace("Exception: {0}", ex.ToString());
                throw;
            }
        }


        public EntityCollection FetchAccounts(IOrganizationService service, Guid twitterid)
        {

            var query = new QueryByAttribute("account") { ColumnSet = new ColumnSet("accountid", "name") };
            query.Attributes.Add("cott_twitteruser");
            query.Values.Add(twitterid);
            return service.RetrieveMultiple(query);
            
        }

        public EntityCollection FetchContacts(IOrganizationService service, Guid twitterid)
        {

            var query = new QueryByAttribute("contact") { ColumnSet = new ColumnSet("contactid", "fullname", "firstname", "lastname") };
            query.Attributes.Add("cott_twitteruser");
            query.Values.Add(twitterid);
            return service.RetrieveMultiple(query);

        }

        public EntityCollection FetchSystemUser(IOrganizationService service, Guid twitterid)
        {

            var query = new QueryByAttribute("systemuser") { ColumnSet = new ColumnSet("systemuserid", "fullname") };
            query.Attributes.Add("cott_twitteruser");
            query.Values.Add(twitterid);
            return service.RetrieveMultiple(query);

        }

        public EntityCollection FetchLead(IOrganizationService service, Guid twitterid)
        {
            var query = new QueryByAttribute("lead") { ColumnSet = new ColumnSet("leadid", "lastname","companyname","firstname", "fullname") };
            query.Attributes.Add("cott_twitteruser");
            query.Values.Add(twitterid);
            return service.RetrieveMultiple(query);

        }
    }
}


