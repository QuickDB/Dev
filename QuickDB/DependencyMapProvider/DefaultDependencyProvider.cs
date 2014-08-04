using QuickDB.Core.Models;
using QuickDB.Dependencies.Contracts;
using QuickDB.Dependencies.Encryptions;
using QuickDB.Dependencies.FileIO;
using QuickDB.Dependencies.SecuritySettings;
using QuickDB.Dependencies.Serializations;
using QuickDB.DependencyInjector;
using System.Collections.Generic;

namespace QuickDB.DependencyMapProvider
{
    public static class DefaultDependencyProvider
    {
        public static List<DependencyMaping> DefaultMapping = new List<DependencyMaping>
        {
            WhenDependencyRequestedIs<AbstractFileAccessHandler>.Provide<FileAccessHandler>(),
            WhenDependencyRequestedIs<AbstractEncryptionHandler>.Provide<EncryptionHandler>(),
            WhenDependencyRequestedIs<AbstractSerializationHandler>.Provide<JSonSerializationHandler>(),

                WhenDependencyRequestedIs<ISecurityStrings>.Provide<DefaultSecurityStrings>()
        };
    }
}