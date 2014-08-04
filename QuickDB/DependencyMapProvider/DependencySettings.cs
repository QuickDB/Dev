using System;
using System.Collections.Generic;
using QuickDB.DependencyInjector;

namespace QuickDB.DependencyMapProvider
{
    public static class DependencyInjection
    {
        public static void SetUp(List<DependencyMaping> map)
        {
            if (map == null) throw new ArgumentNullException("map");
            QInjector.CreateMappings(map);
        }
        /// <summary>
        /// If mappings already exist, it uses it, else it sets up the dafeiult mapping
        /// </summary>
        public static void SetUpDefault()
        {
            if(!QInjector.HasMappings())
            SetUp(DefaultDependencyProvider.DefaultMapping);
        }
    }
}
