namespace SparkyTools.DependencyProvider
{
    /// <summary>
    /// Interface for generic dependency provider. This class is useful for injecting testable dependencies
    /// into class constructors for dependencies that don't have a mockable interface (e.g. current time).
    /// </summary>
    /// <typeparam name="TDependency"></typeparam>
    public interface IDependencyProvider<TDependency>
    {
        /// <summary>
        /// Causes the value retrieved by the first <see cref="DependencyProvider{TDependency}.GetValue"/> call to be cached and returned for subsequent calls.
        /// </summary>
        /// <returns>"This" <see cref="DependencyProvider{TDependency}"/> instance.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var dbProvider = new DependencyProvider<Connection>(() => CreateConnection()).Static();
        /// var baz = new Baz(qux, dbProvider);
        /// ]]></code>
        /// </example>
        DependencyProvider<TDependency> Static();

        /// <summary>
        /// Gets the dependency value.
        /// </summary>
        /// <returns>The <typeparamref name="TDependency"/> value.</returns>
        TDependency GetValue();
    }
}