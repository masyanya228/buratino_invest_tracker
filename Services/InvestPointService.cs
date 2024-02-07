using Buratino.Models.Entities;

namespace Buratino.Services
{
    public class InvestPointService
    {
        public InvestSourceService InvestSourceService { get; set; } = new InvestSourceService();
        public InvestPoint AddPoint(params string[] parts)
        {
            if (parts.Length == 2)
            {
                if (!decimal.TryParse(parts[1].Replace(".", ","), out decimal amount))
                {
                    Console.WriteLine($"{parts[1]} - не является денежным выражением");
                    return null;
                }
                return AddPoint(parts[0], amount);
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
                return AddPoint(parts[0], amount, dateTime);
            }

            return null;
        }

        protected InvestPoint AddPoint(string name, decimal amount)
        {
            var source = InvestSourceService.Find(name);

            var point = new InvestPoint()
            {
                Source = source,
                Amount = amount
            };

            DBContext.InvestPoints.Insert(point);
            return point;
        }

        protected InvestPoint AddPoint(string name, decimal amount, DateTime dateTime)
        {
            var source = InvestSourceService.Find(name);

            var point = new InvestPoint()
            {
                Source = source,
                Amount = amount,
                TimeStamp = dateTime
            };

            DBContext.InvestPoints.Insert(point);
            return point;
        }
    }
}
