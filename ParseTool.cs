using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    using System.Data;
    using System.ComponentModel;
    
public class ParseTool
{
        public static T To<T>(this object obj)
        {
            if (obj == null) return default(T);

            var jsonResolver = new IgnorableSerializerContractResolver();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ContractResolver = jsonResolver,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Binder = new EntityFrameworkSerializationBinder()
            };
            string json = JsonConvert.SerializeObject(obj, settings);

            T res = JsonConvert.DeserializeObject<T>(json);

            return res;
        }       
}

internal class EntityFrameworkSerializationBinder : SerializationBinder
{
    public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;

        //if (serializedType.Namespace == "System.Data.Entity.DynamicProxies")
        //typeName = serializedType.BaseType.FullName;
        //else
        typeName = serializedType.FullName;
    }

    public override Type BindToType(string assemblyName, string typeName)
    {
        throw new NotImplementedException();
    }
}
