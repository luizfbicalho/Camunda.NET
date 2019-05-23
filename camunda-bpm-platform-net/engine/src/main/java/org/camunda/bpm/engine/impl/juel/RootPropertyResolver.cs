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
namespace org.camunda.bpm.engine.impl.juel
{

	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using PropertyNotFoundException = org.camunda.bpm.engine.impl.javax.el.PropertyNotFoundException;
	using PropertyNotWritableException = org.camunda.bpm.engine.impl.javax.el.PropertyNotWritableException;

	/// <summary>
	/// Simple root property resolver implementation. This resolver handles root properties (i.e.
	/// <code>base == null &amp;&amp; property instanceof String</code>), which are stored in a map. The
	/// properties can be accessed via the <seealso cref="getProperty(string)"/>,
	/// <seealso cref="setProperty(string, object)"/>, <seealso cref="isProperty(string)"/> and <seealso cref="properties()"/>
	/// methods.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class RootPropertyResolver : ELResolver
	{
		private readonly IDictionary<string, object> map = Collections.synchronizedMap(new Dictionary<string, object>());
		private readonly bool readOnly;

		/// <summary>
		/// Create a read/write root property resolver
		/// </summary>
		public RootPropertyResolver() : this(false)
		{
		}

		/// <summary>
		/// Create a root property resolver
		/// </summary>
		/// <param name="readOnly"> </param>
		public RootPropertyResolver(bool readOnly)
		{
			this.readOnly = readOnly;
		}

		private bool isResolvable(object @base)
		{
			return @base == null;
		}

		private bool resolve(ELContext context, object @base, object property)
		{
			context.PropertyResolved = isResolvable(@base) && property is string;
			return context.PropertyResolved;
		}

		public override Type getCommonPropertyType(ELContext context, object @base)
		{
			return isResolvable(context) ? typeof(string) : null;
		}

		public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object @base)
		{
			return null;
		}

		public override Type getType(ELContext context, object @base, object property)
		{
			return resolve(context, @base, property) ? typeof(object) : null;
		}

		public override object getValue(ELContext context, object @base, object property)
		{
			if (resolve(context, @base, property))
			{
				if (!isProperty((string) property))
				{
					throw new PropertyNotFoundException("Cannot find property " + property);
				}
				return getProperty((string) property);
			}
			return null;
		}

		public override bool isReadOnly(ELContext context, object @base, object property)
		{
			return resolve(context, @base, property) ? readOnly : false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setValue(org.camunda.bpm.engine.impl.javax.el.ELContext context, Object super, Object property, Object value) throws org.camunda.bpm.engine.impl.javax.el.PropertyNotWritableException
		public override void setValue(ELContext context, object @base, object property, object value)
		{
			if (resolve(context, @base, property))
			{
				if (readOnly)
				{
					throw new PropertyNotWritableException("Resolver is read only!");
				}
				setProperty((string) property, value);
			}
		}

		public override object invoke(ELContext context, object @base, object method, Type[] paramTypes, object[] @params)
		{
			if (resolve(context, @base, method))
			{
				throw new System.NullReferenceException("Cannot invoke method " + method + " on null");
			}
			return null;
		}

		/// <summary>
		/// Get property value
		/// </summary>
		/// <param name="property">
		///            property name </param>
		/// <returns> value associated with the given property </returns>
		public virtual object getProperty(string property)
		{
			return map[property];
		}

		/// <summary>
		/// Set property value
		/// </summary>
		/// <param name="property">
		///            property name </param>
		/// <param name="value">
		///            property value </param>
		public virtual void setProperty(string property, object value)
		{
			map[property] = value;
		}

		/// <summary>
		/// Test property
		/// </summary>
		/// <param name="property">
		///            property name </param>
		/// <returns> <code>true</code> if the given property is associated with a value </returns>
		public virtual bool isProperty(string property)
		{
			return map.ContainsKey(property);
		}

		/// <summary>
		/// Get properties
		/// </summary>
		/// <returns> all property names (in no particular order) </returns>
		public virtual IEnumerable<string> properties()
		{
			return map.Keys;
		}
	}

}