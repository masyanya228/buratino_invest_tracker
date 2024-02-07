using NHibernate;

namespace Buratino.Models.Repositories.Implementations.Postgres
{
    public interface IPGSessionFactory
    {
        ISessionFactory SessionFactory { get; set; }
    }
}