using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Praxio.Folga.Api.Configurations
{
    /// <summary/>
    public class CustomContractResolver : DefaultContractResolver
    {

        /// <summary/>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = type.GetProperties()
               .Select(s =>
               {
                   var property = base.CreateProperty(s, memberSerialization);

                   property.ValueProvider = new CustomValueProvider(s);

                   return property;
               })
               .ToList();

            return properties;
        }
    }

    /// <summary/>
    public class CustomValueProvider : IValueProvider
    {
        private readonly PropertyInfo _memberInfo;

        /// <summary/>
        public CustomValueProvider(PropertyInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }

        /// <summary/>
        public object GetValue(object target)
        {
            var value = _memberInfo.GetValue(target);

            if (
                 (value is System.Collections.IList lista && !lista.Any()) ||
                 (_memberInfo.Name == "Id" && value.ToString() == "0")
                )
                value = null;

            return value;
        }

        /// <summary/>
        public void SetValue(object target, object value)
        {
            _memberInfo.SetValue(target, value);
        }
    }
}
