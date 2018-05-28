using SparkyTools.DependencyProvider;
using System;
using System.Configuration;

namespace SparkyTools.XmlConfig.Fx
{
    /// <summary>
    /// <see cref="ConfigurationManager.AppSettings"/> helper methods.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Creates new <see cref="DependencyProvider"/> for retrieving values from
        /// <see cref="ConfigurationManager.AppSettings"/>.
        /// </summary>
        /// <returns>The DependencyProvider.</returns>
        public static DependencyProvider<Func<string, string>> DependencyProvider()
        {
            return new DependencyProvider<Func<string, string>>(key => ConfigurationManager.AppSettings[key]).Static();
        }
    }
}
