using QuickDB.Core;
using QuickDB.Core.Core;
using QuickDB.Core.Models;

namespace QuickDB.Instances
{
    public class QuickDB<TConfigurationObject> : CoreQuickDB<TConfigurationObject> where TConfigurationObject : new()
    {
        public QuickDB(QuickDBDependencySetUpObject quickDBDependencySetUpObject, bool readOnly = false) : base(quickDBDependencySetUpObject, readOnly)
        {
        }
    }
}