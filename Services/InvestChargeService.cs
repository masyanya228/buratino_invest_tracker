using Buratino.Entities;

namespace Buratino.Services
{
    public class InvestChargeService
    {
        public InvestSourceService InvestSourceService { get; set; } = new InvestSourceService();
        public InvestCharge AddCharge(params string[] parts)
        {
            if (parts.Length == 2)
            {
                if (!decimal.TryParse(parts[1].Replace(".", ","), out decimal amount))
                {
                    Console.WriteLine($"{parts[1]} - не является денежным выражением");
                    return null;
                }
                return AddCharge(parts[0], amount);
            }
            else if (parts.Length == 3)
            {
                if (!decimal.TryParse(parts[1].Replace(".", ","), out decimal amount))
                {
                    Console.WriteLine($"{parts[1]} - не является денежным выражением");
                    return null;
                }
                if (!DateTime.TryParse(parts[2].Replace(".", ","), out DateTime dateTime))
                {
                    Console.WriteLine($"{parts[2]} - не является временем");
                    return null;
                }
                return AddCharge(parts[0], amount, dateTime);
            }

            return null;
        }

        public string ListCharge(params string[] parts)
        {
            if (parts.Length == 1)
                return ListCharge(parts[0]);
            return null;
        }



        protected InvestCharge AddCharge(string name, decimal amount)
        {
            var source = InvestSourceService.Find(name);

            var charge = new InvestCharge()
            {
                Source = source,
                Increment = amount
            };

            DBContext.InvestCharges.Insert(charge);
            return charge;
        }

        protected InvestCharge AddCharge(string name, decimal amount, DateTime dateTime)
        {
            var source = InvestSourceService.Find(name);

            var charge = new InvestCharge()
            {
                Source = source,
                Increment = amount,
                TimeStamp = dateTime
            };

            DBContext.InvestCharges.Insert(charge);
            return charge;
        }

        protected string ListCharge(string name)
        {
            var source = InvestSourceService.Find(name);

            var all = DBContext.InvestCharges.Query().Where(x => x.Source.Id == source.Id).ToArray();
            var body = string.Join("\r\n", all
                .OrderByDescending(x => x.TimeStamp)
                .Select(x => $"[{x.Id}]\t{x.Increment}\t{x.TimeStamp}"));
            return $"[{source.Id}]\t{source.Name}\r\n"
                + body
                + $"\r\n{all.Count()} на сумму {all.Sum(x => x.Increment)}";
        }
    }
}
