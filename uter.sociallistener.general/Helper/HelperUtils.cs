using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace uter.sociallistener.general.Helper
{
    public static class HelperUtils
    {
        #region SetCRMProperty

        public static void SetCRMProperty(Entity entity, string property, object value)
        {
            if (value.GetType() == typeof(string))
            {
                SetCRMStringProperty(entity,property,(string)value);
            }else if (value.GetType() == typeof(DateTime))
            {
                SetCRMDateTimeProperty(entity, property, (DateTime)value);
            }else if(value.GetType() == typeof(decimal))
            {
                SetCRMDecimalProperty(entity, property, (decimal)value);
            }
            else if (value.GetType() == typeof(EntityReference))
            {
                SetCRMEntityReference(entity, property, (EntityReference)value);
            }else if(value.GetType() == typeof(OptionSetValue))
            {
                SetCRMOptionSetProperty(entity, property, (OptionSetValue)value);
            }
            else if (value.GetType() == typeof(Boolean))
            {
                SetCRMBooleanProperty(entity, property, (bool)value);
            }
            else if (value.GetType() == typeof(int))
            {
                SetCRMIntegerProperty(entity, property, (int)value);
            }

        }

        public static void SetCRMStringProperty(Entity entity, string property, string value)
        {
            if (!entity.Contains(property))
            {
                entity.Attributes.Add(property, value);
            }
            else
            {
                entity[property] = value;
            }
        }

        public static void SetCRMDateTimeProperty(Entity entity, string property, DateTime value)
        {
            if (!entity.Contains(property))
            {
                entity.Attributes.Add(property, value);
            }
            else
            {
                entity[property] = value;
            }
        }

        public static void SetCRMIntegerProperty(Entity entity, string property, int value)
        {
            if (!entity.Contains(property))
            {
                entity.Attributes.Add(property, value);
            }
            else
            {
                entity[property] = value;
            }
        }

        public static void SetCRMDecimalProperty(Entity entity, string property, decimal value)
        {
            if (!entity.Contains(property))
            {
                entity.Attributes.Add(property, value);
            }
            else
            {
                entity[property] = value;
            }
        }

        public static void SetCRMEntityReference(Entity entity, string property, EntityReference value)
        {
            if (!entity.Contains(property))
            {
                entity.Attributes.Add(property, value);
            }
            else
            {
                entity[property] = value;
            }
        }

        public static void SetCRMOptionSetProperty(Entity entity, string property, OptionSetValue value)
        {
            if (!entity.Contains(property))
            {
                entity.Attributes.Add(property, value);
            }
            else
            {
                entity[property] = value;
            }

        }

        public static void SetCRMBooleanProperty(Entity entity, string property, bool value)
        {
            if (!entity.Contains(property))
            {
                entity.Attributes.Add(property, value);
            }
            else
            {
                entity[property] = value;
            }

        }

        #endregion

      
    }
}
