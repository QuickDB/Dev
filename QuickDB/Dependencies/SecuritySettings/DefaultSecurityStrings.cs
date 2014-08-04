using QuickDB.Core.Models;

namespace QuickDB.Dependencies.SecuritySettings
{
    public class DefaultSecurityStrings : ISecurityStrings
    {
        public DefaultSecurityStrings()
        {

            PasswordHash = "P@@Sw0rd";
            SaltKey = "S@LT&KEY";
            VIKey = "@1B2c3D4e5F6g7H8";
        }

        public string PasswordHash { set; get; }

        public string SaltKey { set; get; }

        public string VIKey { set; get; }
    }
}