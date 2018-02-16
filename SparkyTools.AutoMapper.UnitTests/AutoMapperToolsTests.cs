using AutoMapper;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparkyTools.AutoMapper.UnitTests.TestClasses;
using SparkyTestHelpers.Mapping;
using SparkyTestHelpers.AutoMapper;

namespace SparkyTools.AutoMapper.UnitTests
{
    [TestClass]
    public class AutoMapperToolsTests
    {
        private MapTester<Source, Dest> _mapTester;
        private Source _source;

        [TestInitialize]
        public void TestInitialize()
        {
            _source = new RandomValuesHelper().CreateInstanceWithRandomValues<Source>();
            _mapTester = MapTester.ForMap<Source, Dest>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Mapper.Reset();
        }

        [TestMethod]
        public void MappedTo_should_map_to_destination_type()
        {
            CreateMap();

            var dest = _source.MappedTo<Dest>();

            _mapTester.AssertMappedValues(_source, dest);
        }

        [TestMethod]
        public void Ignore_should_ignore_specified_member()
        {
            CreateMap(map => map.ForMember(x => x.Name).Ignore());

            Dest dest = _mapTester.IgnoringMember(x => x.Name).AssertAutoMappedValues(_source);

            Assert.IsNull(dest.Name);
        }

        [TestMethod]
        public void IgnoreMember_should_ignore_specified_member()
        {
            CreateMap(map => map.IgnoreMember(x => x.Name));

            Dest dest = _mapTester.IgnoringMember(x => x.Name).AssertAutoMappedValues(_source);

            Assert.IsNull(dest.Name);
        }

        [TestMethod]
        public void MapFrom_should_use_specified_source_callback()
        {
            CreateMap(map => map.ForMember(x => x.Name).MapFrom(src => CombineNames(src)));

            _mapTester
                .WhereMember(x => x.Name).ShouldEqualValue(CombineNames(_source))
                .AssertAutoMappedValues(_source);
        }

        [TestMethod]
        public void NullSubstitute_should_use_specified_substitute()
        {
            CreateMap(map => map.ForMember(x => x.Name).NullSubstitute("Sparky"));

            _source.Name = "Brian";
            _mapTester.AssertAutoMappedValues(_source);

            _source.Name = null;
            _mapTester
                .WhereMember(x => x.Name).ShouldEqualValue("Sparky")
                .AssertAutoMappedValues(_source);
        }

        [TestMethod]
        public void UseValue_should_call_specified_callback_function()
        {
            CreateMap(map => map.ForMember(x => x.Id).UseValue(() => 123456));

            Dest dest = _source.MappedTo<Dest>();
            Assert.AreEqual(123456, dest.Id);

            //_mapTester
            //    .WhereMember(x => x.Id).ShouldEqualValue(123456)
            //    .AssertAutoMappedValues(_source);
        }

        [TestMethod]
        public void UseValue_should_use_specified_value()
        {
            CreateMap(map => map.ForMember(x => x.Id).UseValue(654321));

            Dest dest = _source.MappedTo<Dest>();
            Assert.AreEqual(654321, dest.Id);

            //_mapTester
            //    .WhereMember(x => x.Id).ShouldEqualValue(654321)
            //    .AssertAutoMappedValues(_source);
        }

        private string CombineNames(Source source)
        {
            return $"{source.FirstName} {source.LastName}";
        }

        private void CreateMap(Action<IMappingExpression<Source, Dest>> callback = null)
        {
            Mapper.Initialize(cfg =>
            {
                IMappingExpression<Source, Dest> mappingExpression = cfg.CreateMap<Source, Dest>();
                callback?.Invoke(mappingExpression);
            });

            Mapper.AssertConfigurationIsValid();
        }
    }
}
