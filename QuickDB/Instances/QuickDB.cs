using QuickDB.Core;
using QuickDB.Core.Core;
using QuickDB.Core.Models;

namespace QuickDB.Instances
{
    public class QuickDB<TClientModelObject> : CoreFacade<TClientModelObject> where TClientModelObject : new()
    {
        public QuickDB(QuickDBDependencySetUpObject quickDBDependencySetUpObject, bool readOnly = false) : base(quickDBDependencySetUpObject, readOnly)
        {
        }
    }
}