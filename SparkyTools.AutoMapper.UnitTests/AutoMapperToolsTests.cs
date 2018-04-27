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
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            var dest = _source.MappedTo<Dest>();

            _mapTester.IgnoringAllOtherMembers().AssertMappedValues(_source, dest);
        }

        [TestMethod]
        public void MappedTo_should_return_null_for_null_input()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            Source source = null;

            var dest = source.MappedTo<Dest>();

            Assert.IsNull(dest);
        }

        [TestMethod]
        public void MappedTo_with_IMapper_should_map_to_destination_type()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            var dest = _source.MappedTo<Dest>(mapper);

            _mapTester.IgnoringAllOtherMembers().AssertMappedValues(_source, dest);
        }

        [TestMethod]
        public void MappedTo_with_IMapper_should_return_null_for_null_input()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            Source source = null;

            var dest = source.MappedTo<Dest>(mapper);

            Assert.IsNull(dest);
        }

        [TestMethod]
        public void MappedToArrayOf_should_map_to_array_of_destination_type()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            var dest = (new[] { _source, _source, _source }).MappedToArrayOf<Dest>();

            Assert.AreEqual(3, dest.Length);
            _mapTester.IgnoringAllOtherMembers().AssertMappedValues(_source, dest[0]);
        }

        [TestMethod]
        public void MappedToArrayOf_should_return_empty_array_for_null_input()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            Source[] sourceArray = null;

            var dest = sourceArray.MappedToArrayOf<Dest>();

            Assert.AreEqual(0, dest.Length);
        }

        [TestMethod]
        public void MappedToArrayOf_with_IMapper_should_map_to_array_of_destination_type()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            var dest = (new[] { _source, _source, _source }).MappedToArrayOf<Dest>(mapper);

            Assert.AreEqual(3, dest.Length);
            _mapTester.IgnoringAllOtherMembers().AssertMappedValues(_source, dest[0]); ;
        }

        [TestMethod]
        public void MappedToArrayOf_with_IMapper_should_return_empty_array_for_null_input()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            Source[] sourceArray = null;

            var dest = sourceArray.MappedToArrayOf<Dest>(mapper);

            Assert.AreEqual(0, dest.Length);
        }

        [TestMethod]
        public void IgnoreMember_should_ignore_specified_member()
        {
            CreateStaticMap(map => map
            .IgnoreMember(x => x.Name)
            .IgnoreMember(x => x.Address));

            Dest dest = _mapTester
                .IgnoringMember(x => x.Name)
                .IgnoringMember(x => x.Address)
                .AssertAutoMappedValues(_source);

            Assert.IsNull(dest.Name);
        }

        [TestMethod]
        public void IgnoreMembers_should_work_as_expected()
        {
            CreateStaticMap(map => map.IgnoreMembers(x => x.Name, x => x.Address));

            Dest dest = _mapTester.IgnoringMembers(x => x.Name, x => x.Address).AssertAutoMappedValues(_source);

            Assert.IsNull(dest.Name);
        }

        [TestMethod]
        public void MapFrom_should_use_specified_source_callback()
        {
            CreateStaticMap(map => map
                .IgnoreMember(x => x.Address)
                .ForMember(x => x.Name).MapFrom(src => CombineNames(src)));

            _mapTester
                .WhereMember(x => x.Name).ShouldEqualValue(CombineNames(_source))
                .IgnoringMember(x => x.Address)
                .AssertAutoMappedValues(_source);
        }

        [TestMethod]
        public void NullSubstitute_should_use_specified_substitute()
        {
            CreateStaticMap(map => map
                .ForMember(x => x.Name).NullSubstitute("Sparky")
                .IgnoreMember(x => x.Address));

            _source.Name = "Brian";
            _mapTester
                .IgnoringMember(x => x.Address)
                .AssertAutoMappedValues(_source);

            _source.Name = null;
            Dest dest = _source.MappedTo<Dest>();
            Assert.AreEqual("Sparky", dest.Name);
            _mapTester
                .WhereMember(x => x.Name).ShouldEqualValue("Sparky")
                .IgnoringMember(x => x.Address)
                .AssertAutoMappedValues(_source);
        }

        [TestMethod]
        public void UseValue_should_call_specified_callback_function()
        {
            CreateStaticMap(map => map
                .ForMember(x => x.Id).UseValue(() => 123456)
                .IgnoreMember(x => x.Address));

            Dest dest = _source.MappedTo<Dest>();

            Assert.AreEqual(123456, dest.Id);

            //_mapTester
            //    .WhereMember(x => x.Id).ShouldEqualValue(123456)
            //    .AssertAutoMappedValues(_source);
        }

        [TestMethod]
        public void UseValue_should_use_specified_value()
        {
            CreateStaticMap(map => map
                .ForMember(x => x.Id).UseValue(654321)
                .IgnoreMember(x => x.Address));

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

        private void CreateStaticMap(Action<IMappingExpression<Source, Dest>> callback = null)
        {
            Mapper.Initialize(cfg =>
            {
                IMappingExpression<Source, Dest> mappingExpression = cfg.CreateMap<Source, Dest>();
                callback?.Invoke(mappingExpression);
            });

            Mapper.AssertConfigurationIsValid();
        }

        private IMapper CreateInstanceMap(Action<IMappingExpression<Source, Dest>> callback = null)
        {
            var config = new MapperConfiguration(cfg => {
                IMappingExpression<Source, Dest> mappingExpression = cfg.CreateMap<Source, Dest>();
                callback?.Invoke(mappingExpression);
            });

            IMapper mapper = config.CreateMapper();

            return mapper;
        }
    }
}
