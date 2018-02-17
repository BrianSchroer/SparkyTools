# SparkyTools.DependencyProvider

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
    var fakeTimeProvider = new DependencyProvider<DateTime>(() => DateTime.Parse("4/20/2018 4:20 PM"));
    ```
* via the static *Create*(*callbackFunction*) method:
    ```csharp
    var realTimeProvider = DependencyProvider.Create(() => DateTime.Now);
    var fakeTimeProvider = DependencyProvider.Create(() => DateTime.Parse("4/20/2018 4:20 PM"));
    ```
* via a static value constructor parameter:
    ```csharp
    var realTimeProvider = new DependencyProvider<DateTime>(DateTime.Now);
    var fakeTimeProvider = new DependencyProvider<DateTime>(DateTime.Parse("4/20/2018 4:20 PM"));
    ```
* via the static *Create(staticValue)* method:
    ```csharp
    var realTimeProvider = DependencyProvider.Create(DateTime.Now);
    var fakeTimeProvider = DependencyProvider.Create(DateTime.Parse("4/20/2018 4:20 PM"));
    ```

The *DependencyProvider*.**Static()** method tells the provider to cache the first value
returned by **GetValue()** and return that same value for all subsequent calls:
```csharp
    var startTimeProvider = DependencyProvider.Create(() => DateTime.Now).Static();
```
DependencyProviders created via the static value constructor and *Create(staticValue)* methods get their static values when constructed. Providers that aren't constructed statically but are "dotted" with **.Static()** are "lazy". They don't get retrieve their value until the first **GetValue()** call.