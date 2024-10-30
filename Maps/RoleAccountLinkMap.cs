using Buratino.Entities;
using Buratino.Maps.MapStructure;

namespace Buratino.Maps
{
    public class RoleAccountLinkMap : MapBase<RoleAccountLink>
    {
        public override void Setup()
        {
            Reference(x => x.Role, "Role");
            Reference(x => x.Account, "Account");
        }
    }
}
