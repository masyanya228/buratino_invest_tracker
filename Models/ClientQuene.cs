using Buratino.Entities;

namespace Buratino.Models
{
    public enum ClientState
    {
        AuthedActive,
        AuthedNotActive,
        NotAuthedActive,
        NotAuthedNotActive
    }
    public class ClientQuene
    {
        public Account Accaunt { get; set; }
        public string Session { get; set; }
        public string IP { get; set; }

        public ClientState GetState()
        {
            if (Session != null)
            {
                if (Accaunt.Id != Guid.Empty)
                {
                    return ClientState.AuthedActive;
                }
                else
                {
                    return ClientState.NotAuthedActive;
                }
            }
            else
            {
                if (Accaunt.Id != Guid.Empty)
                {
                    return ClientState.AuthedNotActive;
                }
                else
                {
                    return ClientState.NotAuthedNotActive;
                }
            }
        }
        //public void SetCache(string key, object data)
        //{
        //    var newVal = new CachedDataItem() { CachedData = data, Key = key };
        //    Cache.AddOrUpdate(key, newVal, (key, existingVal) =>
        //    {
        //        return newVal;
        //    });
        //}
        //public T[] TryGetCache<T>(string key)
        //{
        //    var needToDelete = Cache.Values
        //        .Where(x => x.Key.StartsWith("vTable") && x.Key != key && x.TimeStamp.AddMinutes(120) < DateTime.Now)
        //        .Select(x => x.Key)
        //        .ToList();
        //    needToDelete.ForEach(x =>
        //    {
        //        if (Cache.TryRemove(x, out CachedDataItem deleted))
        //        {
        //            Console.WriteLine("Delted from cache");
        //        }
        //    });

        //    if (!Cache.TryRemove(key, out CachedDataItem item))
        //    {
        //        //wtf??
        //        return null;
        //    }
        //    else
        //    {
        //        return (item.CachedData as IEnumerable<T>).ToArray();
        //    }
        //}
    }
    public class CommandResult
    {
        public CommandResult(object result)
        {
            Result = result;
        }
        public object Result { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}