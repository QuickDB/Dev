namespace QuickDB.Core.Models
{
    public interface ISecurityStrings
    {
        string PasswordHash { set; get; }

        string SaltKey { set; get; }

        string VIKey { set; get; }
    }
}