using Quartz;

namespace Buratino.Models.Jobs.Structures
{
    public interface IQuartzProvider
    {
        IScheduler Schedule { get; }
    }
}