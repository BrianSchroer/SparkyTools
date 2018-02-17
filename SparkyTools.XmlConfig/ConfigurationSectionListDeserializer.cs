using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml;

namespace SparkyTools.XmlConfig
{
    /// <summary>
    /// Generic configuration section deserializer for lists.
    /// </summary>
    /// <remarks>
    /// This class allows the addition of a .config section to load a list of any type without having
    /// to write a custom <see cref="IConfigurationSectionHandler"/> for the section.
    /// </remarks>
    /// <seealso cref="ConfigurationSectionDeserializer"/>.
    public class ConfigurationSectionListDeserializer : IConfigurationSectionHandler
    {
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Loads a new <see cref="IList{T}"/> from a section in a .config file.
        /// </summary>
        /// <typeparam name="T">The types into which the .config XML should be deserialized.</typeparam>
        /// <param name="sectionName">The .config file section name</param>
        /// <param name="shouldThrowExceptionIfSectionNotFound">
        /// Should an exception be thrown if the section is not found? (default = <c>true</c>).
        /// </param>
        /// <param name="shouldAllowEmptyList">
        /// Should an empty list be allowed? (default = <c>false</c>.)
        /// </param>
        /// <returns>New <see cref="IList{T}"/> instance.</returns>
        /// <exception cref="ConfigurationErrorsException"> if
        ///     <para>exception is thrown by <see cref="ConfigurationManager.GetSection(string)"/></para>
        ///     <para>* unable to cast .config section to <see cref="IList{T}"/>.</para>
        ///     <para>* <paramref name="shouldAllowEmptyList"/> is <c>false</c> and the list is empty.</para>
        /// </exception>
        /// <example>
        ///     <para>.config XML:</para>
        ///     <code><![CDATA[
        ///     <configSections> 
        ///         ...
        ///         <section name="foos" type="SparkyTools.XmlConfig.ConfigurationSectionListDeserializer, SparkyTools.XmlConfig" />
        ///         ...
        ///     </configSections>
        ///     ...
        ///     <foos type="FooNamespace.Foo, FooAssemblyName">
        ///         <Foo>
        ///             <Bar>bar 1</Bar>
        ///             <Baz>baz1 value</Baz>
        ///             <Qux>qux1 value</Qux>
        ///         </Foo>
        ///         <Foo>
        ///             <Bar>bar1 value</Bar>
        ///             <Baz>baz1 value</Baz>
        ///             <Qux>qux1 value</Qux>
        ///         </Foo>
        ///         <Foo>
        ///             <Bar>bar1 value</Bar>
        ///             <Baz>baz1 value</Baz>
        ///             <Qux>qux1 value</Qux>
        ///         </Foo>
        ///     </foos>
        ///     ]]></code>
        ///     <para>C# code to load instance from .config file:</para>
        ///     <code><![CDATA[
        ///     Foo foo = ConfigurationSectionDeserializer.Load<Foo>("fooConfiguration");
        ///     ]]></code>
        /// <example
        public static IList<T> Load<T>(
            string sectionName, 
            bool shouldThrowExceptionIfSectionNotFound = true,
            bool shouldAllowEmptyList = false)
        {
            var list = ConfigurationSectionDeserializerHelper.GetSection(sectionName, shouldThrowExceptionIfSectionNotFound)
                as IList<T>;

            if (list == null)
            {
                throw ConfigurationSectionDeserializerHelper.NewConfigurationErrorsException(
                    sectionName, $"Unable to cast to IList<{typeof(T).FullName}>.");
            }

            if (list.Count == 0 && !shouldAllowEmptyList)
            {
                throw ConfigurationSectionDeserializerHelper.NewConfigurationErrorsException(sectionName, "Item count = zero.");
            }

            return list;
        }
    }
}
