using Buratino.Entities;

namespace Buratino.Xtensions
{
    public static class DataXtentions
    {
        public static IEnumerable<decimal> ToSumList(this Dictionary<InvestSource, List<decimal>> data)
        {
            var result = new List<decimal>();
            if (!data.Any())
                return result;
            var max = data.Max(x => x.Value.Count);
            for (var i = 0; i < max; i++)
            {
                result.Add(data.Where(x => x.Value.Count > i).Sum(x => x.Value[i]));
            }
            return result;
        }
    }
}
