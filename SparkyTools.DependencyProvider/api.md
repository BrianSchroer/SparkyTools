# SparkyTools.DependencyProvider
_see also_:
* the rest of the [**"Sparky suite"** of .NET utilities and test helpers](https://www.nuget.org/profiles/BrianSchroer)
-----
Ludicrously lightweight dependency wrapper/provider to improve testability of classes that use dependencies that aren't easily mockable (e.g. System.DateTime):

```csharp
using SparkyTools.DependencyProvider;
```
```csharp
    public class Foo
    {
        private readonly IBar _bar;
        private readonly DependencyProvider<DateTime> _currentTimeProvider;
       
        public Foo(IBar bar, DependencyProvider<DateTime> currentTimeProvider) 
        {
            _bar = bar;
            _getDate = currentTimeProvider;
        }
       
        public void DoSomethingWithBazAndDate()
        {
            _bar.DoSomethingWithDate(_currentTimeProvider.GetValue());
        }
    }
```
The *DependencyProvider*.**GetValue()** function provides a value to the class.

A DependencyProvider instance can be created:

* via a "callback function" constructor parameter:
    ```csharp
    var realTimeProvider = new DependencyProvider<DateTime>(() => DateTime.Now);
    var fakeTimeProvider = new DependencyProvider<DateTime>(() => new DateTime(2018, 7, 4, 15, 32, 00));
    ```
* via the static *Create*(*callbackFunction*) method:
    ```csharp
    var realTimeProvider = DependencyProvider.Create(() => DateTime.Now);
    var fakeTimeProvider = DependencyProvider.Create(() => new DateTime(2018, 7, 4, 15, 32, 00));
    ```
* via a static value constructor parameter:
    ```csharp
    var realTimeProvider = new DependencyProvider<DateTime>(DateTime.Now);
    var fakeTimeProvider = new DependencyProvider<DateTime>(new DateTime(2018, 7, 4, 15, 32, 00));
    ```
* via the static *Create(staticValue)* method:
    ```csharp
    var realTimeProvider = DependencyProvider.Create(DateTime.Now);
    var fakeTimeProvider = DependencyProvider.Create(new DateTime(2018, 7, 4, 15, 32, 00));
    ```
* via *implicit conversion*:
    ```csharp
    DependencyProvider<DateTime> realTimeProvider = () => DateTime.Now(); 
    DependencyProvider<DateTime> fakeTimeProvider = new DateTime(2018, 7, 4, 15, 32, 00);
    ```

The implicit conversion option makes for very nice, terse syntax when declared inline for the constructor of a class using DependencyProvider, e.g.:

```csharp
    public class Foo
    {
        public Foo(IBar bar, DependencyProvider<DateTime> currentTimeProvider) { }
    }

    ...
    // implicit conversion:
    var fooWithRealTime = new Foo(bar, DateTime.Now());
    var fooWithFakeTime = new Foo(bar, new DateTime(2018, 7, 4, 15, 32, 00));

    // other syntaxes:
    var fooWithRealTime2 = new Foo(bar, DependencyProvider.Create(DateTime.Now));
    var foowithFakeTime2 = new Foo(bar, new DependencyProvider<DateTime>(new DateTime(2018, 7, 4, 15, 32, 00)));
```

The *DependencyProvider*.**Static()** method tells the provider to cache the first value
returned by **GetValue()** and return that same value for all subsequent calls:
```csharp
    var startTimeProvider = DependencyProvider.Create(() => DateTime.Now).Static();
```
DependencyProviders created via the static value constructor and *Create(staticValue)* methods get their static values when constructed. Providers that aren't constructed statically but are "dotted" with **.Static()** are "lazy". They don't get retrieve their value until the first **GetValue()** call.