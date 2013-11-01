using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Spring.Context;
using Spring.Context.Support;

namespace uter.sociallistener.service
{
    public partial class SocialListener : ServiceBase
    {

        //set configuration path
        private const string OBJECT_CONFIG_LOCATION =
            "assembly://uter.sociallistener.service/uter.sociallistener.service/object.xml";

        private IApplicationContext appCtx;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public SocialListener()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {

                Log.Info("Social Listener started successfully.");

                appCtx = new XmlApplicationContext(OBJECT_CONFIG_LOCATION);


            }
            catch (Exception e)
            {
                Log.FatalException("Critical unexpected error, shutting down", e);
                throw;
            }
        }

        protected override void OnStop()
        {
            if (appCtx != null)
                appCtx.Dispose();


            Log.Info("Social Listener stopped successfully");
        }
    }
}
