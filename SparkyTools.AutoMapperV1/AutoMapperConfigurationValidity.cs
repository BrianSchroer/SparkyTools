using AutoMapper;
using System;
using System.Text;

namespace SparkyTools.AutoMapper
{
    /// <summary>
    /// The "Assert" methods in this class assert mapper configuration validity with improved exception message formatting.
    /// </summary>
    public static class AutoMapperConfigurationValidity
    {
        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert()
        {
            WithExceptionHelper(Mapper.AssertConfigurationIsValid);
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert(IMapper mapper, string profileName = null)
        {
            Assert(mapper.ConfigurationProvider, profileName);
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert(IMapper mapper, TypeMap typeMap)
        {
            Assert(mapper.ConfigurationProvider, typeMap);
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert<TProfile>(IMapper mapper) where TProfile : Profile, new()
        {
            Assert<TProfile>(mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert(IConfigurationProvider cfg, string profileName = null)
        {
            WithExceptionHelper(() =>
            {
                if (profileName == null)
                {
                    cfg.AssertConfigurationIsValid();
                }
                else
                {
                    cfg.AssertConfigurationIsValid(profileName);
                }
            });
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert(IConfigurationProvider cfg, TypeMap typeMap)
        {
            WithExceptionHelper(() => cfg.AssertConfigurationIsValid(typeMap));
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert<TProfile>(IConfigurationProvider cfg) where TProfile : Profile, new()
        {
            WithExceptionHelper(() => cfg.AssertConfigurationIsValid<TProfile>());
        }

        private static void WithExceptionHelper(Action assertConfigurationIsValid)
        {
            try
            {
                assertConfigurationIsValid();
            }
            catch (AutoMapperConfigurationException ex)
            {
                string improvedMessage = ExceptionHelper.AddUnmappedPropertyHelperInfo(ex);

                if (improvedMessage == ex.Message)
                {
                    throw;
                }

                throw new AutoMapperConfigurationException(improvedMessage);
            }
        }
    }
}
