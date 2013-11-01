namespace uter.tuwien.authservice.Repository
{
    public class CrmServerConfig
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }
        public string ServerDomain { get; set; }
        public string OrganizationName { get; set; }
        public bool IFDEnabled { get; set; }
        public bool HTTPSEnabled { get; set; }
        public int Port { get; set; }

        //Live Integration
        public bool LiveEnabled { get; set; }
        public string LiveUrl { get; set; }
        public string HomeRealmUri { get; set; }
        public string HomeRealmOrg { get; set; }
        public string LiveUserName { get; set; }
        public string LivePassword { get; set; }
        public string LiveOrgUrl { get; set; }
    }
}
