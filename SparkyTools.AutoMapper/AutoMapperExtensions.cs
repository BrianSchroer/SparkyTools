using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SparkyTools.AutoMapper
{
    /// <summary>
    /// AutoMapper extension methods, inspired by a blog post by Matt Honeycutt: 
    /// https://www.trycatchfail.com/2010/10/04/a-more-fluent-api-for-automapper/
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Use AutoMapper with static Mapper to map object to another type, using an alternate syntax.
        /// Instead of:
        /// <![CDATA[
        ///     Bar bar = Mapper.Map<Foo, Bar>(foo);
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     Bar bar = foo.MappedTo<Bar>();
        /// ]]>
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>A new instance of type <typeparamref name="TResult"/>.</returns>
        public static TResult MappedTo<TResult>(this object source)
        {
            return (source == null)
                ? default(TResult)
                : (TResult)Mapper.Map(source, source.GetType(), typeof(TResult));
        }

        /// <summary>
        /// Use AutoMapper to map to map object to another type, using an alternate syntax.
        /// Instead of:
        /// <![CDATA[
        ///     Bar bar = Mapper.Map<Foo, Bar>(foo);
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     Bar bar = foo.MappedTo<Bar>();
        /// ]]>
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="mapper">The <see cref="IMapper"/>.</param>
        /// <returns>A new instance of type <typeparamref name="TResult"/>.</returns>
        public static TResult MappedTo<TResult>(this object source, IMapper mapper)
        {
            return (source == null)
                ? default(TResult)
                : (TResult)mapper.Map(source, source.GetType(), typeof(TResult));
        }

        /// <summary>
        /// Map <see cref="IEnumerable{T}"/> to array of <typeparamref name="TResult"/> using static Mapper.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sourceEnumerable">The source enumerable.</param>
        /// <returns>Array of <typeparamref name="TResult"/>(empty array if <paramref name="sourceEnumerable"/> is null.)</returns>
        public static TResult[] MappedToArrayOf<TResult>(this IEnumerable<object> sourceEnumerable)
        {
            return (sourceEnumerable == null)
                ? new TResult[0]
                : sourceEnumerable.Select(MappedTo<TResult>).ToArray();
        }

        /// <summary>
        /// Map <see cref="IEnumerable{T}"/> to array of <typeparamref name="TResult"/> using <see cref="IMapper"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sourceEnumerable">The source enumerable.</param>
        /// <param name="mapper">The <see cref="IMapper"/>.</param>
        /// <returns>Array of <typeparamref name="TResult"/>(empty array if <paramref name="sourceEnumerable"/> is null.)</returns>
        public static TResult[] MappedToArrayOf<TResult>(this IEnumerable<object> sourceEnumerable, IMapper mapper)
        {
            return (sourceEnumerable == null)
                ? new TResult[0]
                : sourceEnumerable.Select(x => x.MappedTo<TResult>(mapper)).ToArray();
        }

        /// <summary>
        /// Creates a <see cref="MemberMappingExpression{TSource, TDestination}"/> instance against which
        /// <see cref="MemberMappingExpression{TSource, TDestination}.MapFrom{TResult}(Expression{Func{TSource, TResult}})"/>,
        /// <see cref="MemberMappingExpression{TSource, TDestination}.UseValue{TResult}(Func{TResult})"/>,
        /// <see cref="MemberMappingExpression{TSource, TDestination}.UseValue{TResult}(TResult)"/> and
        /// <see cref="MemberMappingExpression{TSource, TDestination}.Ignore"/> methods may be used.
        /// </summary>
        /// <typeparam name="TSource">Source ("map from") type.</typeparam>
        /// <typeparam name="TDestination">Destination ("map to") type.</typeparam>
        /// <param name="mappingExpression">
        /// AutoMapper <see cref="IMappingExpression{TSource, TDestination}"/> instance.
        /// </param>
        /// <param name="member">
        /// Expression for a function that takes an instance of the destination type and
        /// returns an object.
        /// </param>
        /// <returns>New <see cref="MemberMappingExpression{TSource, TDestination}"/> instance.</returns>
        public static MemberMappingExpression<TSource, TDestination> ForMember<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, object>> member)
        {
            return new MemberMappingExpression<TSource, TDestination>(mappingExpression, member);
        }

        /// <summary>
        /// Tells AutoMapper to ignore mapping for a specified property of the source
        /// type, with a less verbose syntax than the out-of-the-box AutoMapper
        /// syntax. Instead of:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName, opt => opt.Ignore())
        /// ]]>
        /// ...or:
        /// <![CDATA[
        ///     .ForMember(dest => dest.PropertyName).Ignore()
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     .IgnoreMember(dest => dest.PropertyName)
        /// ]]>
        /// </summary>
        /// <typeparam name="TSource">Source ("map from") type.</typeparam>
        /// <typeparam name="TDestination">Destination ("map to") type.</typeparam>
        /// <param name="mappingExpression">
        /// AutoMapper <see cref="IMappingExpression{TSource, TDestination}"/> instance.
        /// </param>
        /// <param name="member">
        /// Expression for a function that takes an instance of the destination type and
        /// returns an object.
        /// </param>
        /// <returns>
        /// Updated <see cref="IMappingExpression{TSource, TDestination}"/> instance, which can
        /// be used for fluent chaining of more mapping methods.
        /// </returns>
        /// <example>
        /// <![CDATA[
        ///     Mapper.Initialize(cfg => 
        ///         cfg.CreateMap<Foo, Bar>()
        ///         .IgnoreMember(dest => dest.Baz);
        /// ]]>
        /// </example>
        public static IMappingExpression<TSource, TDestination> IgnoreMember<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, object>> member)
        {
            return mappingExpression.IgnoreMembers(member);
        }

        /// <summary>
        /// Tells AutoMapper to ignore mapping for specified properties of the source
        /// type, with a less verbose syntax than the out-of-the-box AutoMapper
        /// syntax. Instead of:
        /// <![CDATA[
        ///     .ForMember(dest => dest.Property1, opt => opt.Ignore())
        ///     .ForMember(dest => dest.Property2, opt => opt.Ignore())
        /// ]]>
        /// ...or:
        /// <![CDATA[
        ///     .ForMember(dest => dest.Property1).Ignore()
        ///     .ForMember(dest => dest.Propery2).Ignore()
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     .IgnoreMembers(dest => dest.Property1, dest => dest.Property2)
        /// ]]>
        /// </summary>
        /// <typeparam name="TSource">Source ("map from") type.</typeparam>
        /// <typeparam name="TDestination">Destination ("map to") type.</typeparam>
        /// <param name="mappingExpression">
        /// AutoMapper <see cref="IMappingExpression{TSource, TDestination}"/> instance.
        /// </param>
        /// <param name="members">
        /// Array of expressions for functions that takes an instance of the destination type and
        /// returns an object.
        /// </param>
        /// <returns>
        /// Updated <see cref="IMappingExpression{TSource, TDestination}"/> instance, which can
        /// be used for fluent chaining of more mapping methods.
        /// </returns>
        /// <example>
        /// <![CDATA[
        ///     Mapper.Initialize(cfg => 
        ///         cfg.CreateMap<Foo, Bar>()
        ///         .IgnoreMembers(dest => dest.Baz, dest => dest.Qux);
        /// ]]>
        /// </example>
        public static IMappingExpression<TSource, TDestination> IgnoreMembers<TSource, TDestination>(
          this IMappingExpression<TSource, TDestination> mappingExpression,
          params Expression<Func<TDestination, object>>[] members)
        {
            foreach (var member in members)
            {
                new MemberMappingExpression<TSource, TDestination>(mappingExpression, member).Ignore();
            }

            return mappingExpression;
        }

        /// <summary>
        /// Alternate syntax for ".ForAllOtherMembers(dest => dest.Ignore());
        /// Instead of:
        /// <![CDATA[
        ///     .ForAllOtherMembers(dest => dest.Ignore());
        /// ]]>
        /// ...you can code:
        /// <![CDATA[
        ///     .IgnoringAllOtherMembers()
        /// ]]>
        /// </summary>
        /// <typeparam name="TSource">Source ("map from") type.</typeparam>
        /// <typeparam name="TDestination">Destination ("map to") type.</typeparam>
        /// <param name="mappingExpression">
        /// AutoMapper <see cref="IMappingExpression{TSource, TDestination}"/> instance.
        /// </param>
        public static void IgnoreAllOtherMembers<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression)
        {
            mappingExpression.ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
