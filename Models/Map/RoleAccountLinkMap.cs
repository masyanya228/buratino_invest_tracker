using Buratino.Models.Entities;
using Buratino.Models.Map.MapStructure;

namespace ServiceCenter.Map
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
