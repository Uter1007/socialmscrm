using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace uter.twitter.plugins
{
    public class PluginConfiguration
    {
        public static string GetConfigDataString(XmlDocument doc, string label)
        {
            return GetValueNode(doc, label);
        }

        private static string GetValueNode(XmlDocument doc, string key)
        {
            XmlNode node = doc.SelectSingleNode(String.Format("Settings/setting[@name='{0}']", key));

            if (node != null)
            {
                return node.SelectSingleNode("value").InnerText;
            }
            return string.Empty;
        }
    }
}
