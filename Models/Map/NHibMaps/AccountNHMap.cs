using Buratino.Models.Entities;
using Buratino.Models.Map.Implementations;

public class AccountNHMap : NHibMapBase<Account>
{
    public AccountNHMap()
    {
        Id(item => item.Id).GeneratedBy.Increment();
        Map(x => x.Name);
        Map(x => x.TimeStamp);
        Map(x => x.IsDeleted);
        Map(x => x.Email);
        Map(x => x.Pass);
        Table("Accounts");
    }
}