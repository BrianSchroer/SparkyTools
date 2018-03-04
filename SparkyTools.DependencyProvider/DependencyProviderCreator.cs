using System;

namespace SparkyTools.DependencyProvider
{
    /// <summary>
    /// Lightweight inversion of control dependency provider.
    /// </summary>
    public class DependencyProvider
    {
        /// <summary>
        /// Creates a new <see cref="DependencyProvider{TDependency}"/> instance.
        /// </summary>
        /// <typeparam name="TDependency">The dependency type.</typeparam>
        /// <param name="callback">"Callback" function that supplies the value.</param>
        /// <returns>New <see cref="DependencyProvider{TDependency}"/> instance.</returns>
        public static DependencyProvider<TDependency> Create<TDependency>(Func<TDependency> callback)
        {
            return new DependencyProvider<TDependency>(callback);
        }

        /// <summary>
        /// Creates a new <see cref="DependencyProvider{TDependency}"/> instance that always
        /// returns the same static value.
        /// </summary>
        /// <typeparam name="TDependency">The dependency type.</typeparam>
        /// <param name="staticValue">The return value.</param>
        /// <returns></returns>
        public static DependencyProvider<TDependency> Create<TDependency>(TDependency staticValue)
        {
            return new DependencyProvider<TDependency>(staticValue);
        }

        /// <summary>
        /// Creates a new <see cref="DependencyProvider{TDependency}"/> instance, using
        /// <see cref="DependencyProvider{TDependency}.Static()"/>
        /// </summary>
        /// <typeparam name="TDependency">The dependency type.</typeparam>
        /// <param name="callback">"Callback" function that supplies the value.</param>
        /// <returns>New <see cref="DependencyProvider{TDependency}"/> instance.</returns>
        public static DependencyProvider<TDependency> CreateStatic<TDependency>(Func<TDependency> callback)
        {
            return Create(callback).Static();
        }
    }
}
