using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class RoleNHMap : NHSubclassClassMap<Role>
{
    public RoleNHMap()
    {
        Table("Roles");
    }
}