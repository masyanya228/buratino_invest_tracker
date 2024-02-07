using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using Buratino.Models.Xtensions;
using NHibernate.Cfg;
using Buratino.Models.Map.Implementations;

namespace Buratino.Models.Repositories.Implementations.Postgres
{
    public class PGSessionFactory : IPGSessionFactory
    {
        private ISessionFactory sessionFactory;

        public ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                    sessionFactory = CreateSessionFactory();

                return sessionFactory;
            }
            set => sessionFactory = value;
        }

        private ISessionFactory CreateSessionFactory()
        {
            var mappings = typeof(NHibMapBase<>).GetImplementations();
            var db = Fluently
                .Configure()
                    .Database(
                        PostgreSQLConfiguration.Standard
                        .ConnectionString(c =>
                            c.Host("localhost")
                            .Port(5432)
                            .Database("postgres")
                            .Username("postgres")
                            .Password("007007Qq")));

            foreach (var item in mappings)
            {
                db.Mappings(x => x.FluentMappings.Add(item));
            }

            return db.ExposeConfiguration(TreatConfiguration)
                .BuildSessionFactory();
        }

        private void TreatConfiguration(Configuration configuration)
        {
            Action<string> updateExport = x =>
            {
                using (var file = new FileStream(@"update.sql", FileMode.Append, FileAccess.Write))
                using (var sw = new StreamWriter(file))
                {
                    sw.Write(x);
                    sw.Close();
                }
            };
            var update = new SchemaUpdate(configuration);
            update.Execute(updateExport, true);
        }
    }
}
