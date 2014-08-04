using System;
using System.Collections.Generic;
using System.Management.Instrumentation;

namespace QuickDB.DependencyInjector
{
    public class QInjector
    {
        private static List<DependencyMaping> DIContainer { set; get; }

        public static bool HasMappings()
        {
            if (DIContainer == null) return false;

            return DIContainer.Count != 0;
        }

        public static void CreateMappings(List<DependencyMaping> map)
        {
            DIContainer = new List<DependencyMaping>();
            DIContainer = map;
        }

        public static T Inject<T>(params object[] arg)
        {
            if (!HasMappings()) throw new Exception("No dependency mapping has been created");
            var mapedType = DIContainer.Find(x => IsSubclassOfRawGeneric(x.MapFrom, typeof(T)));

            if (mapedType == null) throw new InstanceNotFoundException("Type " + typeof(T).FullName + " has not been configured to be dependently injected");

            var instance = arg.Length == 0 ? Activator.CreateInstance(mapedType.MapTo) : Activator.CreateInstance(mapedType.MapTo, arg);

            return (T)instance;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}