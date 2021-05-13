
_see also_:
* **[SparkyTools.XmlConfig](https://www.nuget.org/packages/SparkyTools.XmlConfig/)**, which supports .NET Core and .NET framework v.4.6 and above
* the rest of the [**"Sparky suite"** of .NET utilities and test helpers](https://www.nuget.org/profiles/BrianSchroer)
---

### ConfigurationSectionDeserializer / ConfigurationSectionListDeserializer

These classes makes it easy to load a strongly-typed object (or IList of objects) from a custom **web.config** or **app.config** file section without having to write a custom **[IConfigurationSectionHandler](https://msdn.microsoft.com/en-us/library/system.configuration.iconfigurationsectionhandler)** implementation.

In the .config file, register each custom section with a type of "SparkyTools.XmlConfig.**ConfigurationSectionDeserializer**" or
"SparkyTools.XmlConfig.**ConfigurationSectionListDeserializer**": 

```xml
<configuration>
  <configSections>
    <section name="Foo" type="SparkyTools.XmlConfig.Fx.ConfigurationSectionDeserializer, SparkyTools.XmlConfig.Fx" />
    <section name="FooList" type="SparkyTools.XmlConfig.Fx.ConfigurationSectionListDeserializer, SparkyTools.XmlConfig.Fx" />
  </configSections>
```
In each registered custom section, specify the object type via the ```type``` attribute. Here's an single instance section:
```xml
  <Foo type="FooNamespace.Foo, FooAssemblyName">
    <Bar>bar</Bar>
    <Baz>123.45</Baz>
  </Foo>
```
...and a "list" section: (the ```type``` is the instance type, not "IList..."):
```xml
  <FooList type="FooNamespace.Bar, FooAssemblyName">
      <Foo>
        <Bar>bar1</Bar>
        <Baz>111.11</Baz>
      </Foo>
      <Foo>
        <Bar>bar2</Bar>
        <Baz>222.22</Baz>
      </Foo>
  </FooList>
```

To read from your custom .config section, just call the **Load** method, specifying the object type and the .config section name:
```csharp
    Foo foo = ConfigurationSectionDeserializer.Load<Foo>("Foo");
    IList<Foo> fooList = ConfigurationSectionListDeserializer.Load<Foo>("FooList");
```

### DependencyProvider methods
These methods create [**DependencyProvider**](https://www.nuget.org/packages/SparkyTools.DependencyProvider/)s for use with apps that use app.config / web.config files:

* **ConfigurationSectionDeserializer.DependencyProvider / ConfigurationSectionListDeserializer.DependencyProvider**
  create DependencyProviders that load data from a .config file section:
    ```csharp
    using SparkyTools.DependencyProvider;

    public class Foo
    {
        public Foo(
            DependencyProvider<Bar> barProvider, 
            DependencyProvider<IList<Baz>> bazProvider)
        {
        }
    }
    ```
    ```csharp
    using SparkyTools.XmlConfig.Fx;
    . . .
        var foo = new Foo(
            ConfigurationSectionDeserializer.DependencyProvider<Bar>("Bar"),
            ConfigurationSectionListDeserializer.DependencyProvider<Baz>("BazList"));
    ```
* **AppSettings.DependencyProvider** creates a DependencyProvider that wraps ConfigurationManager.AppSettings:
    ```csharp
    using SparkyTools.DependencyProvider;

    public class Qux
    {
        private readonly Func<string, string> _getAppSetting;

        public Qux(DependencyProvider<Func<string, string> appSettingsProvider)
        {
            _getAppSetting = appSettingsProvider.GetValue();
        }
    
        public void MethodUsingAppSettings()
        {
            string valueFromAppSettings = _getAppSetting("key);
        }
    }
    ```
    ```csharp
    using SparkyTools.XmlConfig.Fx;
    ...
        var qux = new Qux(AppSettings.DependencyProvider());
    ```