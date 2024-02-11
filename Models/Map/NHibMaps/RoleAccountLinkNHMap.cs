using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class RoleAccountLinkNHMap : NHSubclassClassMap<RoleAccountLink>
{
    public RoleAccountLinkNHMap()
    {
        References(item => item.Role, "RoleId");
        References(item => item.Account, "AccountId");
        Table("RoleAccountLinks");
    }
}