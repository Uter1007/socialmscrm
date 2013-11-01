using System;
using System.Configuration;
using uter.tuwien.authservice.Repository;

public static class ConfigHelper
{
    public static CrmServerConfig ReadConfig()
    {
        return new CrmServerConfig
        {
            Domain = ConfigurationManager.AppSettings["Domain"],
            HostName = ConfigurationManager.AppSettings["Hostname"],
            HTTPSEnabled =
                Convert.ToBoolean(ConfigurationManager.AppSettings["HTTPSEnabled"]),
            IFDEnabled =
                Convert.ToBoolean(ConfigurationManager.AppSettings["IFDEnabled"]),
            OrganizationName = ConfigurationManager.AppSettings["OrganizationName"],
            Password = ConfigurationManager.AppSettings["Password"],
            Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]),
            ServerDomain = ConfigurationManager.AppSettings["ServerDomain"],
            UserName = ConfigurationManager.AppSettings["Username"],

            LiveEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["LiveEnabled"]),
            LiveUrl = ConfigurationManager.AppSettings["LiveUrl"],
            HomeRealmUri = ConfigurationManager.AppSettings["HomeRealmUri"],
            HomeRealmOrg = ConfigurationManager.AppSettings["HomeRealmOrg"],
            LiveUserName = ConfigurationManager.AppSettings["LiveUserName"],
            LivePassword = ConfigurationManager.AppSettings["LivePassword"],
            LiveOrgUrl = ConfigurationManager.AppSettings["LiveOrgUrl"]
        };
    } 

    public static string GetCorrectUrl(CrmServerConfig config)
    {
        if (!config.IFDEnabled)
        {
            /*return String.Format("{0}://{1}.{2}:{3}/{4}/main.aspx", config.HTTPSEnabled ? "https" : "http",
                config.HostName, config.ServerDomain, config.Port, config.OrganizationName);
            */
            return String.Format("{0}://{1}:{2}/{3}/main.aspx", config.HTTPSEnabled ? "https" : "http",
                config.HostName, config.Port, config.OrganizationName);
        }
        else if (config.LiveEnabled)
        {
            return String.Format("{0}/main.aspx", config.LiveOrgUrl);
        }
        else
        {
            return String.Format("{0}://{1}.{2}:{3}/main.aspx", config.HTTPSEnabled ? "https" : "http",
                config.OrganizationName, config.ServerDomain, config.Port);
        }
        
    }
}