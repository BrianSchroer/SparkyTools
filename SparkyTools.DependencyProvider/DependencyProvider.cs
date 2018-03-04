using System;
using System.Collections.Generic;

namespace SparkyTools.DependencyProvider
{
    /// <summary>
    /// Generic dependency provider. This class is useful for injecting testable dependencies
    /// into class constructors for dependencies that don't have a mockable interface (e.g. current time).
    /// </summary>
    /// <typeparam name="TDependency">The dependency type.</typeparam>
    /// <example>
    ///     <code><![CDATA[
    ///         public class Foo
    ///         {
    ///             private readonly IBar _bar;
    ///             private readonly DependencyProvider<DateTime> _currentTimeProvider;
    ///         
    ///             public Foo(IBar bar, DependencyProvider<DateTime> currentTimeProvider) 
    ///             {
    ///                 _bar = bar;
    ///                 _getDate = currentTimeProvider;
    ///             }
    ///         
    ///             public void DoSomethingWithBazAndDate()
    ///             {
    ///                 _bar.DoSomethingWithDate(_currentTimeProvider.GetValue());
    ///             }
    ///         }
    ///     ]]></code>
    ///     Constructor call from production code:
    ///     <code><![CDATA[
    ///         var foo = new Foo(bar, new DependencyProvider<DateTime>(() => DateTime.Now));
    ///         
    ///         // or:
    ///         // var foo = new Foo(bar, DependencyProvider.Create(() => DateTime.Now));
    ///     ]]></code> 
    ///     Constructor call from unit test:
    ///     <code><![CDATA[
    ///         var testDate = DateTime.Parse("4/20/2018 4:20 PM");
    ///         var foo = new Foo(mockBar.Object, new DependencyProvider<DateTime>(() => testDate));
    ///         
    ///         // or:
    ///         // var foo = new Foo(bar, DependencyProvider.Create(() => testDate));
    ///     ]]></code>
    /// </example>
    public class DependencyProvider<TDependency>
    {
        private static readonly object _instanceLock = new object();
        private static readonly Dictionary<string, TDependency> _staticDictionary = new Dictionary<string, TDependency>();

        private readonly Func<TDependency> _getValue;

        private bool _hasBeenCalled;
        private string _key = null;

        /// <summary>
        /// Initializes a new <see cref="DependencyProvider{TDependency}"/> instance.
        /// </summary>
        /// <param name="callback">"Callback" function that supplies the value.</param>
        /// <example>
        /// <code><![CDATA[
        /// var foo = new Foo(mockBar.Object, new DependencyProvider<DateTime>(() => testDate));
        /// ]]></code>
        /// </example>
        public DependencyProvider(Func<TDependency> callback)
        {
            _getValue = callback;
        }

        /// <summary>
        /// Initializes a new <see cref="DependencyProvider{TDependency}"/> instance
        /// that always returns the same static value.
        /// </summary>
        /// <param name="staticValue">The return value.</param>
        /// <example>
        /// <code><![CDATA[
        /// var foo = new Foo(mockBar.Object, new DependencyProvider<DateTime>(testDate));
        /// ]]></code>
        /// </example>
        public DependencyProvider(TDependency staticValue)
        {
            _getValue = () => staticValue;
            Static();
        }

        /// <summary>
        /// Causes the value retrieved by the first <see cref="GetValue"/> call to be cached and returned for subsequent calls.
        /// </summary>
        /// <returns>"This" <see cref="DependencyProvider{TDependency}"/> instance.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var dbProvider = new DependencyProvider<Connection>(() => CreateConnection()).Static();
        /// var baz = new Baz(qux, dbProvider);
        /// ]]></code>
        /// </example>
        public DependencyProvider<TDependency> Static()
        {
            if (_hasBeenCalled)
            {
                throw new InvalidOperationException(
                    $"The {nameof(Static)}() method cannot be called after {nameof(GetValue)}() has been called.");
            }

            if (_key == null)
            {
                _key = Guid.NewGuid().ToString();
            }

            return this;
        }

        /// <summary>
        /// Gets the dependency value.
        /// </summary>
        /// <returns>The <typeparamref name="TDependency"/> value.</returns>
        public TDependency GetValue()
        {
            _hasBeenCalled = true;

            TDependency value;

            if (string.IsNullOrWhiteSpace(_key))
            {
                value = _getValue();
            }
            else
            {
                if (!_staticDictionary.ContainsKey(_key))
                {
                    lock (_instanceLock)
                    {
                        if (!_staticDictionary.ContainsKey(_key))
                        {
                            _staticDictionary.Add(_key, _getValue());
                        }
                    }
                }

                value = _staticDictionary[_key];
            }

            return value;
        }
    }
}
