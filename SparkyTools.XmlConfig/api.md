
_see also_:
* the rest of the [**"Sparky suite"** of .NET utilities and test helpers](https://www.nuget.org/profiles/BrianSchroer)
---
This package makes it easy to load a strongly-typed object (or IList of objects) from a custom **web.config** or **app.config** file section without having to write a custom **[IConfigurationSectionHandler](https://msdn.microsoft.com/en-us/library/system.configuration.iconfigurationsectionhandler)** implementation.

In the .config file, register each custom section with a type of "SparkyTools.XmlConfig.**ConfigurationSectionDeserializer**" or
"SparkyTools.XmlConfig.**ConfigurationSectionListDeserializer**": 

```xml
<configuration>
  <configSections>
    <section name="Foo" type="SparkyTools.XmlConfig.ConfigurationSectionDeserializer, SparkyTools.XmlConfig" />
    <section name="FooList" type="SparkyTools.XmlConfig.ConfigurationSectionListDeserializer, SparkyTools.XmlConfig" />
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
