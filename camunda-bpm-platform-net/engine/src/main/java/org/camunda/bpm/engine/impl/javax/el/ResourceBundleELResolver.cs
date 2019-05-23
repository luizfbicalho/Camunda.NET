using System;
using System.Collections.Generic;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.javax.el
{

	/// <summary>
	/// Defines property resolution behavior on instances of java.util.ResourceBundle. This resolver
	/// handles base objects of type java.util.ResourceBundle. It accepts any object as a property and
	/// coerces it to a java.lang.String for invoking java.util.ResourceBundle.getObject(String). This
	/// resolver is read only and will throw a <seealso cref="PropertyNotWritableException"/> if setValue is
	/// called. ELResolvers are combined together using <seealso cref="CompositeELResolver"/>s, to define rich
	/// semantics for evaluating an expression. See the javadocs for <seealso cref="ELResolver"/> for details.
	/// </summary>
	public class ResourceBundleELResolver : ELResolver
	{
		/// <summary>
		/// If the base object is a ResourceBundle, returns the most general type that this resolver
		/// accepts for the property argument. Otherwise, returns null. Assuming the base is a
		/// ResourceBundle, this method will always return String.class.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bundle to analyze. Only bases of type ResourceBundle are handled by this
		///            resolver. </param>
		/// <returns> null if base is not a ResourceBundle; otherwise String.class. </returns>
		public override Type getCommonPropertyType(ELContext context, object @base)
		{
			return isResolvable(@base) ? typeof(string) : null;
		}

		/// <summary>
		/// If the base object is a ResourceBundle, returns an Iterator containing the set of keys
		/// available in the ResourceBundle. Otherwise, returns null. The Iterator returned must contain
		/// zero or more instances of java.beans.FeatureDescriptor. Each info object contains information
		/// about a key in the ResourceBundle, and is initialized as follows:
		/// <ul>
		/// <li>displayName - The String key name</li>
		/// <li>name - Same as displayName property</li>
		/// <li>shortDescription - Empty string</li>
		/// <li>expert - false</li>
		/// <li>hidden - false</li>
		/// <li>preferred - true</li>
		/// </ul>
		/// In addition, the following named attributes must be set in the returned FeatureDescriptors:
		/// <ul>
		/// <li><seealso cref="ELResolver.TYPE"/> - String.class.</li>
		/// <li><seealso cref="ELResolver.RESOLVABLE_AT_DESIGN_TIME"/> - true</li>
		/// </ul>
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bundle to analyze. Only bases of type ResourceBundle are handled by this
		///            resolver. </param>
		/// <returns> An Iterator containing zero or more (possibly infinitely more) FeatureDescriptor
		///         objects, each representing a key in this bundle, or null if the base object is not a
		///         ResourceBundle. </returns>
		public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object @base)
		{
			if (isResolvable(@base))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<String> keys = ((java.util.ResourceBundle) super).getKeys();
				IEnumerator<string> keys = ((ResourceBundle) @base).Keys;
				return new IteratorAnonymousInnerClass(this, keys);
			}
			return null;
		}

		private class IteratorAnonymousInnerClass : IEnumerator<FeatureDescriptor>
		{
			private readonly ResourceBundleELResolver outerInstance;

			private IEnumerator<string> keys;

			public IteratorAnonymousInnerClass(ResourceBundleELResolver outerInstance, IEnumerator<string> keys)
			{
				this.outerInstance = outerInstance;
				this.keys = keys;
			}

			public bool hasNext()
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return keys.hasMoreElements();
			}
			public FeatureDescriptor next()
			{
				FeatureDescriptor feature = new FeatureDescriptor();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				feature.DisplayName = keys.nextElement();
				feature.Name = feature.DisplayName;
				feature.ShortDescription = "";
				feature.Expert = true;
				feature.Hidden = false;
				feature.Preferred = true;
				feature.setValue(TYPE, typeof(string));
				feature.setValue(RESOLVABLE_AT_DESIGN_TIME, true);
				return feature;
			}
			public void remove()
			{
				throw new System.NotSupportedException("Cannot remove");

			}
		}

		/// <summary>
		/// If the base object is an instance of ResourceBundle, return null, since the resolver is read
		/// only. If the base is ResourceBundle, the propertyResolved property of the ELContext object
		/// must be set to true by this resolver, before returning. If this property is not true after
		/// this method is called, the caller should ignore the return value.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bundle to analyze. Only bases of type ResourceBundle are handled by this
		///            resolver. </param>
		/// <param name="property">
		///            The name of the property to analyze. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then null; otherwise
		///         undefined. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null </exception>
		public override Type getType(ELContext context, object @base, object property)
		{
			if (context == null)
			{
				throw new System.NullReferenceException("context is null");
			}
			if (isResolvable(@base))
			{
				context.PropertyResolved = true;
			}
			return null;
		}

		/// <summary>
		/// If the base object is an instance of ResourceBundle, the provided property will first be
		/// coerced to a String. The Object returned by getObject on the base ResourceBundle will be
		/// returned. If the base is ResourceBundle, the propertyResolved property of the ELContext
		/// object must be set to true by this resolver, before returning. If this property is not true
		/// after this method is called, the caller should ignore the return value.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bundle to analyze. Only bases of type ResourceBundle are handled by this
		///            resolver. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then null if property
		///         is null; otherwise the Object for the given key (property coerced to String) from the
		///         ResourceBundle. If no object for the given key can be found, then the String "???" +
		///         key + "???". </returns>
		/// <exception cref="NullPointerException">
		///             if context is null. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing the property or variable resolution.
		///             The thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override object getValue(ELContext context, object @base, object property)
		{
			if (context == null)
			{
				throw new System.NullReferenceException("context is null");
			}
			object result = null;
			if (isResolvable(@base))
			{
				if (property != null)
				{
					try
					{
						result = ((ResourceBundle) @base).getObject(property.ToString());
					}
					catch (MissingResourceException)
					{
						result = "???" + property + "???";
					}
				}
				context.PropertyResolved = true;
			}
			return result;
		}

		/// <summary>
		/// If the base object is not null and an instanceof java.util.ResourceBundle, return true.
		/// </summary>
		/// <returns> If the propertyResolved property of ELContext was set to true, then true; otherwise
		///         undefined. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null. </exception>
		public override bool isReadOnly(ELContext context, object @base, object property)
		{
			if (context == null)
			{
				throw new System.NullReferenceException("context is null");
			}
			if (isResolvable(@base))
			{
				context.PropertyResolved = true;
			}
			return true;
		}

		/// <summary>
		/// If the base object is a ResourceBundle, throw a <seealso cref="PropertyNotWritableException"/>.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bundle to analyze. Only bases of type ResourceBundle are handled by this
		///            resolver. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <param name="value">
		///            The value to be set. </param>
		/// <exception cref="NullPointerException">
		///             if context is null. </exception>
		/// <exception cref="PropertyNotWritableException">
		///             Always thrown if base is an instance of ResourceBundle. </exception>
		public override void setValue(ELContext context, object @base, object property, object value)
		{
			if (context == null)
			{
				throw new System.NullReferenceException("context is null");
			}
			if (isResolvable(@base))
			{
				throw new PropertyNotWritableException("resolver is read-only");
			}
		}

		/// <summary>
		/// Test whether the given base should be resolved by this ELResolver.
		/// </summary>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <returns> base instanceof ResourceBundle </returns>
		private bool isResolvable(object @base)
		{
			return @base is ResourceBundle;
		}
	}

}