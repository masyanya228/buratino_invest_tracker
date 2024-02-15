using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class PermissionRoleLinkNHMap : NHSubclassClassMap<PermissionRoleLink>
{
    public PermissionRoleLinkNHMap()
    {
        Map(x => x.Permission);
        References(item => item.Role, "RoleId");
        Table("PermissionRoleLinks");
    }
}