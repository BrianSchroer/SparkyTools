﻿using AutoMapper;
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
        /// <param name="afterMap">Optional callback action to do something with the mapped result.</param>
        /// <returns>A new instance of type <typeparamref name="TResult"/>.</returns>
        public static TResult MappedTo<TResult>(this object source, Action<TResult> afterMap = null)
        {
            var result = (source == null)
                ? default(TResult)
                : (TResult)Mapper.Map(source, source.GetType(), typeof(TResult));

            afterMap?.Invoke(result);

            return result;
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
        /// <param name="afterMap">Optional callback action to do something with the mapped result.</param>
        /// <returns>A new instance of type <typeparamref name="TResult"/>.</returns>
        public static TResult MappedTo<TResult>(this object source, IMapper mapper, Action<TResult> afterMap = null)
        {
            var result = (source == null)
                ? default(TResult)
                : (TResult)mapper.Map(source, source.GetType(), typeof(TResult));

            afterMap?.Invoke(result);

            return result;
        }

        /// <summary>
        /// Map <see cref="IEnumerable{T}"/> to array of <typeparamref name="TResult"/> using static Mapper.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sourceEnumerable">The source enumerable.</param>
        /// <param name="afterMap">Optional callback action to do something with each mapped result.</param>
        /// <returns>Array of <typeparamref name="TResult"/>(empty array if <paramref name="sourceEnumerable"/> is null.)</returns>
        public static TResult[] MappedToArrayOf<TResult>(this IEnumerable<object> sourceEnumerable, Action<TResult> afterMap = null)
        {
            return (sourceEnumerable == null)
                ? new TResult[0]
                : sourceEnumerable.Select(src => src.MappedTo(afterMap)).ToArray();
        }

        /// <summary>
        /// Map <see cref="IEnumerable{T}"/> to array of <typeparamref name="TResult"/> using <see cref="IMapper"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sourceEnumerable">The source enumerable.</param>
        /// <param name="mapper">The <see cref="IMapper"/>.</param>
        /// <param name="afterMap">Optional callback action to do something with each mapped result.</param>
        /// <returns>Array of <typeparamref name="TResult"/>(empty array if <paramref name="sourceEnumerable"/> is null.)</returns>
        public static TResult[] MappedToArrayOf<TResult>(this IEnumerable<object> sourceEnumerable, IMapper mapper, Action<TResult> afterMap = null)
        {
            return (sourceEnumerable == null)
                ? new TResult[0]
                : sourceEnumerable.Select(x => x.MappedTo(mapper, afterMap)).ToArray();
        }

        /// <summary>
        /// Map <see cref="IEnumerable{T}"/> to array of <typeparamref name="TResult"/> using static Mapper.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sourceEnumerable">The source enumerable.</param>
        /// <param name="afterMap">Optional callback action to do something with each mapped result.</param>
        /// <returns>Array of <typeparamref name="TResult"/>(empty array if <paramref name="sourceEnumerable"/> is null.)</returns>
        public static TResult[] MappedToArray<TSource, TResult>(this IEnumerable<TSource> sourceEnumerable, Action<TSource, TResult> afterMap = null)
        {
            if (sourceEnumerable == null) return new TResult[0];

            return sourceEnumerable.Select(src =>
            {
                TResult dest = Mapper.Map<TSource, TResult>(src);
                afterMap?.Invoke(src, dest);
                return dest;
            }
            ).ToArray();
        }

        /// <summary>
        /// Map <see cref="IEnumerable{T}"/> to array of <typeparamref name="TResult"/> using <see cref="IMapper"/> instance.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sourceEnumerable">The source enumerable.</param>
        /// <param name="mapper">The <see cref="IMapper"/>.</param>
        /// <param name="afterMap">Optional callback action to do something with each mapped result.</param>
        /// <returns>Array of <typeparamref name="TResult"/>(empty array if <paramref name="sourceEnumerable"/> is null.)</returns>
        public static TResult[] MappedToArray<TSource, TResult>(this IEnumerable<TSource> sourceEnumerable,
            IMapper mapper, Action<TSource, TResult> afterMap = null)
        {
            if (sourceEnumerable == null) return new TResult[0];

            return sourceEnumerable.Select(src =>
            {
                TResult dest = mapper.Map<TSource, TResult>(src);
                afterMap?.Invoke(src, dest);
                return dest;
            }
            ).ToArray();
        }

        /// <summary>
        /// Copy values from existing <typeparamref name="TSource"/> instance to existing <see typeparamref="TResult"/> instance.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="result">The <typeparamref name="TResult"/> instance.</param>
        /// <param name="source">The <typeparamref name="TSource"/> instance.</param>
        /// <returns>The updated <typeparamref name="TResult"/> instance.</returns>
        public static TResult CopyValuesFrom<TSource, TResult>(this TResult result, TSource source)
        {
            Mapper.Map(source, result);

            return result;
        }

        /// <summary>
        /// Copy values from existing <typeparamref name="TSource"/> instance to existing <see typeparamref="TResult"/> instance.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="result">The <typeparamref name="TResult"/> instance.</param>
        /// <param name="source">The <typeparamref name="TSource"/> instance.</param>
        /// <param name="mapper">The <see cref="IMapper"/>.</param>
        /// <returns>The updated <typeparamref name="TResult"/> instance.</returns>
        public static TResult CopyValuesFrom<TSource, TResult>(this TResult result, TSource source, IMapper mapper)
        {
            mapper.Map(source, result);

            return result;
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
