using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class AccountNHMap : NHSubclassClassMap<Account>
{
    public AccountNHMap()
    {
        Map(x => x.Email);
        Map(x => x.Pass);
    }
}