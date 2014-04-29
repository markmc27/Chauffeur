﻿using System.Configuration;
using System.Linq;
using System.IO;
using Umbraco.Core;
using Umbraco.Core.Persistence.SqlSyntax;
using System;
using System.Data.SqlServerCe;
namespace Chauffeur.DependencyBuilders
{
    class SqlSyntaxProviderBuilder : IBuildDependencies
    {
        public void Build(ShittyIoC container)
        {
            var providerName = ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ProviderName;

            var provider = TypeFinder.FindClassesWithAttribute<SqlSyntaxProviderAttribute>()
                .FirstOrDefault(p => p.GetCustomAttribute<SqlSyntaxProviderAttribute>(false).ProviderName == providerName);

            if (provider == null)
                throw new FileNotFoundException(string.Format("Unable to find SqlSyntaxProvider that is used for the provider type '{0}'", providerName));

            SqlSyntaxContext.SqlSyntaxProvider = (ISqlSyntaxProvider)Activator.CreateInstance(provider);

            container.Register<Func<string, ISqlCeEngine>>(() => s => new SqlCeEngineWrapper(s));
        }
    }

    public interface ISqlCeEngine
    {
        void CreateDatabase();
    }

    internal class SqlCeEngineWrapper : ISqlCeEngine
    {
        private readonly SqlCeEngine engine;
        public SqlCeEngineWrapper(string connectionString)
        {
            engine = new SqlCeEngine(connectionString);
        }
        public void CreateDatabase()
        {
            engine.CreateDatabase();
        }
    }

}
