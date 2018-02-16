
_see also_:
* **[SparkyTestHelpers.Mapping](https://www.nuget.org/packages/SparkyTestHelpers.Mapping)**: Helpers for testing properties "mapped" from one type to another
* **[SparkyTestHelpers.AutoMapper](https://www.nuget.org/packages/SparkyTestHelpers.AutoMapper/)**: Extends **SparkyTestHelpers.Mapping** with with additional extension methods specifically for use with **AutoMapper**.
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

#### MappedTo extension method ####
The package also contains an extension method that allows you to replace
```csharp
    Bar bar = Mapper.Map<Foo, Bar>(foo);
```
with:
```csharp
    Bar bar = foo.MappedTo<Bar>();
```
