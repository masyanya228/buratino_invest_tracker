using Buratino.Models.Entities;
using Buratino.Models.Map.Implementations;

public class RoleNHMap : NHibMapBase<Role>
{
    public RoleNHMap()
    {
        Id(item => item.Id).GeneratedBy.Increment();
        Map(x => x.Name);
        Map(x => x.TimeStamp);
        Map(x => x.IsDeleted);
        Table("Roles");
    }
}