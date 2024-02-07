using Buratino.Models.Entities;
using Buratino.Models.Map.Implementations;

public class RoleAccountLinkNHMap : NHibMapBase<RoleAccountLink>
{
    public RoleAccountLinkNHMap()
    {
        Id(item => item.Id).GeneratedBy.Increment();
        References(item => item.Role, "RoleId");
        References(item => item.Account, "AccountId");
        Table("RoleAccountLinks");
    }
}