
_see also_:
* **[SparkyTestHelpers.Mapping](https://www.nuget.org/packages/SparkyTestHelpers.Mapping)**: Helpers for testing properties "mapped" from one type to another
* **[SparkyTestHelpers.AutoMapper](https://www.nuget.org/packages/SparkyTestHelpers.AutoMapper/)**: Extends **SparkyTestHelpers.Mapping** with with additional extension methods specifically for use with **AutoMapper**.
* the rest of the [**"Sparky suite"** of .NET utilities and test helpers](https://www.nuget.org/profiles/BrianSchroer)
---
This package enables an alternate, less verbose syntax for some of the functions that "dot on" to **[AutoMapper](http://automapper.org/)** "**ForMember**" functions, including **.MapFrom**, **.NullSubstitute**, **.UseValue** and **.Ignore**.

*Without **SparkyTools.AutoMapper***:

```csharp
    Mapper.Initialize(cfg => 
        cfg.CreateMap<Foo, Bar>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusCode))
            .ForMember(dest => dest.Comments, opt => opt.NullSubstitute("(none)"))
            .ForMember(dest => dest.IsImportant, opt => opt.UseValue(true))
            .ForMember(dest => dest.UniqueId, opt => opt.UseValue(Guid.NewGuid()))
            .ForMember(dest => dest.Useless, opt => opt.Ignore());
``` 

*With **SparkyTools.AutoMapper***:

```csharp
using SparkyTools.AutoMapper;
```
```csharp
    Mapper.Initialize(cfg => 
        cfg.CreateMap<Foo, Bar>()
            .ForMember(dest => dest.Status).MapFrom(src => src.StatusCode)
            .ForMember(dest => dest.Comments).NullSubstitute("(none)")
            .ForMember(dest => dest.IsImportant).UseValue(true)
            .ForMember(dest => dest.UniqueId).UseValue(Guid.NewGuid())
            .ForMember(dest => dest.Useless).Ignore();
```  

Ignoring members can be simplified even further.  
```csharp
.ForMember(dest => dest.Useless).Ignore()
``` 
can be coded as:
```csharp
.IgnoreMember(dest => dest.Useless)
```
...or, to ignore multiple members:
```csharp
.IgnoringMembers(dest => dest.Useless, dest => dest.Useless2, dest => dest.Useless3)
```

#### MappedTo extension methods ####
The package also contains an extension method that allows you to replace
```csharp
    Bar bar = Mapper.Map<Foo, Bar>(foo); // with static Mapper
    Bar bar = mapper.Map<foo, Bar>(foo): // with IMapper instance
```
with:
```csharp
    Bar bar = foo.MappedTo<Bar>(); // with static Mapper
    Bar bar = foo.MappedTo<Bar>(mapper); // with IMapper instance
```

#### AutoMapperConfigurationValidity ####
The **Assert** methods in this static class wrap **Mapper.AssertConfigurationIsValid** and **IConfigurationProvider.AssertConfigurationIsValid** with an exception handler that provides suggestions for dealing with unmapped properties.