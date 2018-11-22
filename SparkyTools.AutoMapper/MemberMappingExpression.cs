using System;
using System.Linq.Expressions;
using AutoMapper;

namespace SparkyTools.AutoMapper
{
    /// <summary>
    /// Class used by this project's AutoMapper extension methods, inspired by a blog
    /// post by Matt Honeycutt:
    /// https://www.trycatchfail.com/2010/10/04/a-more-fluent-api-for-automapper/
    /// </summary>
    /// <typeparam name="TSource">Source ("map from") type.</typeparam>
    /// <typeparam name="TDestination">Destination ("map to") type.</typeparam>
    public class MemberMappingExpression<TSource, TDestination>
    {
        private readonly IMappingExpression<TSource, TDestination> _mappingExpression;
        private readonly Expression<Func<TDestination, object>> _member;

        /// <summary>
        /// Creates a new <see cref="MemberMappingExpression{TSource, TDestination}"/> instance.
        /// </summary>
        /// <param name="mappingExpression"><see cref="IMappingExpression{TSource, TDestination}"/> instance.</param>
        /// <param name="member">Member expression.</param>
        public MemberMappingExpression(
            IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, object>> member)
        {
            _mappingExpression = mappingExpression;
            _member = member;
        }

        /// <summary>
        /// Tells AutoMapper to ignore mapping for a specified property of the source
        /// type, with a less verbose syntax than the out-of-the-box AutoMapper
        /// syntax. Instead of:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName, opt => opt.Ignore())
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName).Ignore()
        /// ]]>
        /// </summary>
        /// <returns>
        /// "This" updated <see cref="IMappingExpression{TSource, TDestination}"/> instance, which can
        /// be used for fluent chaining of more mapping methods.
        /// </returns>
        /// <example>
        /// <![CDATA[
        ///     Mapper.Initialize(cfg =>
        ///         cfg.CreateMap<Foo, Bar>()
        ///         .ForMember(dest => dest.Baz).Ignore());
        /// ]]>
        /// </example>
        public IMappingExpression<TSource, TDestination> Ignore()
        {
            return _mappingExpression.ForMember(_member, opt => opt.Ignore());
        }

        /// <summary>
        /// Maps a property from a source member to a destination member,
        /// with a less verbose syntax than the out-of-the-box AutoMapper
        /// syntax. Instead of:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName, opt => opt.MapFrom(src => src.SourceProperty))
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName).MapFrom(src => src.SourceProperty)
        /// ]]>
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="sourceMember">
        /// Expression for a function that takes an instance of the
        /// source type and returns a value of the property type.
        /// </param>
        /// <returns>
        /// "This" updated <see cref="IMappingExpression{TSource, TDestination}"/> instance, which can
        /// be used for fluent chaining of more mapping methods.
        /// </returns>
        /// <example>
        /// <![CDATA[
        ///     Mapper.Initialize(cfg =>
        ///         cfg.CreateMap<Foo, Bar>()
        ///         .ForMember(dest => dest.Baz).MapFrom(src => src.Qux);
        /// ]]>
        /// </example>
        public IMappingExpression<TSource, TDestination> MapFrom<TResult>(
            Expression<Func<TSource, TResult>> sourceMember)
        {
            return _mappingExpression.ForMember(_member, opt => opt.MapFrom(sourceMember));
        }

        /// <summary>
        /// Specifies a vlue to be used for a destination member if the source value is null
        /// anywhere along the member chain,
        /// with a less verbose syntax than the out-of-the-box AutoMapper
        /// syntax. Instead of:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName, opt => opt.NullSubstitute(value))
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName).NullSubstitute(value)
        /// ]]>
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="value">The value to be substituted for null.</param>
        /// <returns>
        /// "This" updated <see cref="IMappingExpression{TSource, TDestination}"/> instance, which can
        /// be used for fluent chaining of more mapping methods.
        /// </returns>
        /// <example>
        /// <![CDATA[
        ///     Mapper.Initialize(cfg =>
        ///         cfg.CreateMap<Foo, Bar>()
        ///         .ForMember(dest => dest.Baz).NullSubstitute("");
        /// ]]>
        /// </example>
        public IMappingExpression<TSource, TDestination> NullSubstitute<TResult>(TResult value) =>
            _mappingExpression.ForMember(_member, opt => opt.NullSubstitute(value));

        /// <summary>
        /// Maps a "value getter" function to a destination member,
        /// with a less verbose syntax than the out-of-the-box AutoMapper
        /// syntax. Instead of:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName, opt => opt.UseValue(src => {...})
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName).UseValue(() => {...})
        /// ]]>
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="valueGetter">A function that returns a value of the property's type.</param>
        /// <returns>
        /// "This" updated <see cref="IMappingExpression{TSource, TDestination}"/> instance, which can
        /// be used for fluent chaining of more mapping methods.
        /// </returns>
        /// <example>
        /// <![CDATA[
        ///     Mapper.Initialize(cfg =>
        ///         cfg.CreateMap<Foo, Bar>()
        ///         .ForMember(dest => dest.).UseValue(() => DateTime.Now);
        /// ]]>
        /// </example>
        [Obsolete("AutoMapper's \"UseValue\" method was removed with version 8.0. The preferred alternative is \".MapFrom(src => value)\" or \".MapFrom(_ => value)\".")]
        public IMappingExpression<TSource, TDestination> UseValue<TResult>(Func<TResult> valueGetter) =>
            _mappingExpression.ForMember(_member, opt => opt.MapFrom(_ => valueGetter()));

        /// <summary>
        /// Maps a value to a destination member,
        /// with a less verbose syntax than the out-of-the-box AutoMapper
        /// syntax. Instead of:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName, opt => opt.UseValue("Value")
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName).UseValue("Value")
        /// ]]>
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="value">The value to be used.</param>
        /// <returns>
        /// "This" updated <see cref="IMappingExpression{TSource, TDestination}"/> instance, which can
        /// be used for fluent chaining of more mapping methods.
        /// </returns>
        /// <example>
        /// <![CDATA[
        ///     Mapper.Initialize(cfg =>
        ///         cfg.CreateMap<Foo, Bar>()
        ///         .ForMember(dest => dest.CurrentTime).UseValue(() => DateTime.Now);
        /// ]]>
        /// </example>
        public IMappingExpression<TSource, TDestination> UseValue<TResult>(TResult value) =>
            _mappingExpression.ForMember(_member, opt => opt.MapFrom(_ => value));
    }
}
