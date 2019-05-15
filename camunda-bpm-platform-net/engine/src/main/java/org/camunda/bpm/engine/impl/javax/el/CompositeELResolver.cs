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
	/// Maintains an ordered composite list of child ELResolvers. Though only a single ELResolver is
	/// associated with an ELContext, there are usually multiple resolvers considered for any given
	/// variable or property resolution. ELResolvers are combined together using a CompositeELResolver,
	/// to define rich semantics for evaluating an expression. For the
	/// <seealso cref="#getValue(ELContext, Object, Object)"/>, <seealso cref="#getType(ELContext, Object, Object)"/>,
	/// <seealso cref="#setValue(ELContext, Object, Object, Object)"/> and
	/// <seealso cref="#isReadOnly(ELContext, Object, Object)"/> methods, an ELResolver is not responsible for
	/// resolving all possible (base, property) pairs. In fact, most resolvers will only handle a base of
	/// a single type. To indicate that a resolver has successfully resolved a particular (base,
	/// property) pair, it must set the propertyResolved property of the ELContext to true. If it could
	/// not handle the given pair, it must leave this property alone. The caller must ignore the return
	/// value of the method if propertyResolved is false. The CompositeELResolver initializes the
	/// ELContext.propertyResolved flag to false, and uses it as a stop condition for iterating through
	/// its component resolvers. The ELContext.propertyResolved flag is not used for the design-time
	/// methods <seealso cref="#getFeatureDescriptors(ELContext, Object)"/> and
	/// <seealso cref="#getCommonPropertyType(ELContext, Object)"/>. Instead, results are collected and combined
	/// from all child ELResolvers for these methods.
	/// </summary>
	public class CompositeELResolver : ELResolver
	{
		private readonly IList<ELResolver> resolvers = new List<ELResolver>();

		/// <summary>
		/// Adds the given resolver to the list of component resolvers. Resolvers are consulted in the
		/// order in which they are added.
		/// </summary>
		/// <param name="elResolver">
		///            The component resolver to add. </param>
		/// <exception cref="NullPointerException">
		///             If the provided resolver is null. </exception>
		public virtual void add(ELResolver elResolver)
		{
			if (elResolver == null)
			{
				throw new System.NullReferenceException("resolver must not be null");
			}
			resolvers.Add(elResolver);
		}

		/// <summary>
		/// Returns the most general type that this resolver accepts for the property argument, given a
		/// base object. One use for this method is to assist tools in auto-completion. The result is
		/// obtained by querying all component resolvers. The Class returned is the most specific class
		/// that is a common superclass of all the classes returned by each component resolver's
		/// getCommonPropertyType method. If null is returned by a resolver, it is skipped.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The base object to return the most general property type for, or null to enumerate
		///            the set of top-level variables that this resolver can evaluate. </param>
		/// <returns> null if this ELResolver does not know how to handle the given base object; otherwise
		///         Object.class if any type of property is accepted; otherwise the most general property
		///         type accepted for the given base. </returns>
		public override Type getCommonPropertyType(ELContext context, object @base)
		{
			Type result = null;
			foreach (ELResolver resolver in resolvers)
			{
				Type type = resolver.getCommonPropertyType(context, @base);
				if (type != null)
				{
					if (result == null || type.IsAssignableFrom(result))
					{
						result = type;
					}
					else if (!result.IsAssignableFrom(type))
					{
						result = typeof(object);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Returns information about the set of variables or properties that can be resolved for the
		/// given base object. One use for this method is to assist tools in auto-completion. The results
		/// are collected from all component resolvers. The propertyResolved property of the ELContext is
		/// not relevant to this method. The results of all ELResolvers are concatenated. The Iterator
		/// returned is an iterator over the collection of FeatureDescriptor objects returned by the
		/// iterators returned by each component resolver's getFeatureDescriptors method. If null is
		/// returned by a resolver, it is skipped.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The base object to return the most general property type for, or null to enumerate
		///            the set of top-level variables that this resolver can evaluate. </param>
		/// <returns> An Iterator containing zero or more (possibly infinitely more) FeatureDescriptor
		///         objects, or null if this resolver does not handle the given base object or that the
		///         results are too complex to represent with this method </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public java.util.Iterator<java.beans.FeatureDescriptor> getFeatureDescriptors(final ELContext context, final Object super)
		public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object @base)
		{
			return new IteratorAnonymousInnerClass(this, context, @base);
		}

		private class IteratorAnonymousInnerClass : IEnumerator<FeatureDescriptor>
		{
			private readonly CompositeELResolver outerInstance;

			private org.camunda.bpm.engine.impl.javax.el.ELContext context;
			private object @base;

			public IteratorAnonymousInnerClass(CompositeELResolver outerInstance, org.camunda.bpm.engine.impl.javax.el.ELContext context, object @base)
			{
				this.outerInstance = outerInstance;
				this.context = context;
				this.@base = @base;
				empty = System.Linq.Enumerable.Empty<FeatureDescriptor> ().GetEnumerator();
				outerInstance.resolvers = outerInstance.resolvers.GetEnumerator();
				features = empty;
			}

			internal IEnumerator<FeatureDescriptor> empty;
			internal IEnumerator<ELResolver> outerInstance.resolvers;
			internal IEnumerator<FeatureDescriptor> features;

			internal IEnumerator<FeatureDescriptor> features()
			{
				while (!features.hasNext() && outerInstance.resolvers.hasNext())
				{
					features = outerInstance.resolvers.next().getFeatureDescriptors(context, @base);
					if (features == null)
					{
						features = empty;
					}
				}
				return features;
			}

			public bool hasNext()
			{
				return features().hasNext();
			}

			public FeatureDescriptor next()
			{
				return features().next();
			}

			public void remove()
			{
				features().remove();
			}
		}

		/// <summary>
		/// For a given base and property, attempts to identify the most general type that is acceptable
		/// for an object to be passed as the value parameter in a future call to the
		/// <seealso cref="#setValue(ELContext, Object, Object, Object)"/> method. The result is obtained by
		/// querying all component resolvers. If this resolver handles the given (base, property) pair,
		/// the propertyResolved property of the ELContext object must be set to true by the resolver,
		/// before returning. If this property is not true after this method is called, the caller should
		/// ignore the return value. First, propertyResolved is set to false on the provided ELContext.
		/// Next, for each component resolver in this composite:
		/// <ol>
		/// <li>The getType() method is called, passing in the provided context, base and property.</li>
		/// <li>If the ELContext's propertyResolved flag is false then iteration continues.</li>
		/// <li>Otherwise, iteration stops and no more component resolvers are considered. The value
		/// returned by getType() is returned by this method.</li>
		/// </ol>
		/// If none of the component resolvers were able to perform this operation, the value null is
		/// returned and the propertyResolved flag remains set to false. Any exception thrown by
		/// component resolvers during the iteration is propagated to the caller of this method.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The base object to return the most general property type for, or null to enumerate
		///            the set of top-level variables that this resolver can evaluate. </param>
		/// <param name="property">
		///            The property or variable to return the acceptable type for. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then the most general
		///         acceptable type; otherwise undefined. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if base is not null and the specified property does not exist or is not readable. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing the property or variable resolution.
		///             The thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override Type getType(ELContext context, object @base, object property)
		{
			context.PropertyResolved = false;
			foreach (ELResolver resolver in resolvers)
			{
				Type type = resolver.getType(context, @base, property);
				if (context.PropertyResolved)
				{
					return type;
				}
			}
			return null;
		}

		/// <summary>
		/// Attempts to resolve the given property object on the given base object by querying all
		/// component resolvers. If this resolver handles the given (base, property) pair, the
		/// propertyResolved property of the ELContext object must be set to true by the resolver, before
		/// returning. If this property is not true after this method is called, the caller should ignore
		/// the return value. First, propertyResolved is set to false on the provided ELContext. Next,
		/// for each component resolver in this composite:
		/// <ol>
		/// <li>The getValue() method is called, passing in the provided context, base and property.</li>
		/// <li>If the ELContext's propertyResolved flag is false then iteration continues.</li>
		/// <li>Otherwise, iteration stops and no more component resolvers are considered. The value
		/// returned by getValue() is returned by this method.</li>
		/// </ol>
		/// If none of the component resolvers were able to perform this operation, the value null is
		/// returned and the propertyResolved flag remains set to false. Any exception thrown by
		/// component resolvers during the iteration is propagated to the caller of this method.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The base object to return the most general property type for, or null to enumerate
		///            the set of top-level variables that this resolver can evaluate. </param>
		/// <param name="property">
		///            The property or variable to return the acceptable type for. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then the result of the
		///         variable or property resolution; otherwise undefined. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if base is not null and the specified property does not exist or is not readable. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing the property or variable resolution.
		///             The thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override object getValue(ELContext context, object @base, object property)
		{
			context.PropertyResolved = false;
			foreach (ELResolver resolver in resolvers)
			{
				object value = resolver.getValue(context, @base, property);
				if (context.PropertyResolved)
				{
					return value;
				}
			}
			return null;
		}

		/// <summary>
		/// For a given base and property, attempts to determine whether a call to
		/// <seealso cref="#setValue(ELContext, Object, Object, Object)"/> will always fail. The result is obtained
		/// by querying all component resolvers. If this resolver handles the given (base, property)
		/// pair, the propertyResolved property of the ELContext object must be set to true by the
		/// resolver, before returning. If this property is not true after this method is called, the
		/// caller should ignore the return value. First, propertyResolved is set to false on the
		/// provided ELContext. Next, for each component resolver in this composite:
		/// <ol>
		/// <li>The isReadOnly() method is called, passing in the provided context, base and property.</li>
		/// <li>If the ELContext's propertyResolved flag is false then iteration continues.</li>
		/// <li>Otherwise, iteration stops and no more component resolvers are considered. The value
		/// returned by isReadOnly() is returned by this method.</li>
		/// </ol>
		/// If none of the component resolvers were able to perform this operation, the value false is
		/// returned and the propertyResolved flag remains set to false. Any exception thrown by
		/// component resolvers during the iteration is propagated to the caller of this method.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The base object to return the most general property type for, or null to enumerate
		///            the set of top-level variables that this resolver can evaluate. </param>
		/// <param name="property">
		///            The property or variable to return the acceptable type for. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then true if the
		///         property is read-only or false if not; otherwise undefined. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if base is not null and the specified property does not exist or is not readable. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing the property or variable resolution.
		///             The thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override bool isReadOnly(ELContext context, object @base, object property)
		{
			context.PropertyResolved = false;
			foreach (ELResolver resolver in resolvers)
			{
				bool readOnly = resolver.isReadOnly(context, @base, property);
				if (context.PropertyResolved)
				{
					return readOnly;
				}
			}
			return false;
		}

		/// <summary>
		/// Attempts to set the value of the given property object on the given base object. All
		/// component resolvers are asked to attempt to set the value. If this resolver handles the given
		/// (base, property) pair, the propertyResolved property of the ELContext object must be set to
		/// true by the resolver, before returning. If this property is not true after this method is
		/// called, the caller can safely assume no value has been set. First, propertyResolved is set to
		/// false on the provided ELContext. Next, for each component resolver in this composite:
		/// <ol>
		/// <li>The setValue() method is called, passing in the provided context, base, property and
		/// value.</li>
		/// <li>If the ELContext's propertyResolved flag is false then iteration continues.</li>
		/// <li>Otherwise, iteration stops and no more component resolvers are considered.</li>
		/// </ol>
		/// If none of the component resolvers were able to perform this operation, the propertyResolved
		/// flag remains set to false. Any exception thrown by component resolvers during the iteration
		/// is propagated to the caller of this method.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The base object to return the most general property type for, or null to enumerate
		///            the set of top-level variables that this resolver can evaluate. </param>
		/// <param name="property">
		///            The property or variable to return the acceptable type for. </param>
		/// <param name="value">
		///            The value to set the property or variable to. </param>
		/// <exception cref="NullPointerException">
		///             if context is null </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if base is not null and the specified property does not exist or is not readable. </exception>
		/// <exception cref="PropertyNotWritableException">
		///             if the given (base, property) pair is handled by this ELResolver but the
		///             specified variable or property is not writable. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while attempting to set the property or variable. The
		///             thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override void setValue(ELContext context, object @base, object property, object value)
		{
			context.PropertyResolved = false;
			foreach (ELResolver resolver in resolvers)
			{
				resolver.setValue(context, @base, property, value);
				if (context.PropertyResolved)
				{
					return;
				}
			}
		}

		/// <summary>
		/// Attempts to resolve and invoke the given <code>method</code> on the given <code>base</code>
		/// object by querying all component resolvers.
		/// 
		/// <para>
		/// If this resolver handles the given (base, method) pair, the <code>propertyResolved</code>
		/// property of the <code>ELContext</code> object must be set to <code>true</code> by the
		/// resolver, before returning. If this property is not <code>true</code> after this method is
		/// called, the caller should ignore the return value.
		/// </para>
		/// 
		/// <para>
		/// First, <code>propertyResolved</code> is set to <code>false</code> on the provided
		/// <code>ELContext</code>.
		/// </para>
		/// 
		/// <para>
		/// Next, for each component resolver in this composite:
		/// <ol>
		/// <li>The <code>invoke()</code> method is called, passing in the provided <code>context</code>,
		/// <code>base</code>, <code>method</code>, <code>paramTypes</code>, and <code>params</code>.</li>
		/// <li>If the <code>ELContext</code>'s <code>propertyResolved</code> flag is <code>false</code>
		/// then iteration continues.</li>
		/// <li>Otherwise, iteration stops and no more component resolvers are considered. The value
		/// returned by <code>getValue()</code> is returned by this method.</li>
		/// </ol>
		/// </para>
		/// 
		/// <para>
		/// If none of the component resolvers were able to perform this operation, the value
		/// <code>null</code> is returned and the <code>propertyResolved</code> flag remains set to
		/// <code>false</code>
		/// </para>
		/// 
		/// <para>
		/// Any exception thrown by component resolvers during the iteration is propagated to the caller
		/// of this method.
		/// </para>
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bean on which to invoke the method </param>
		/// <param name="method">
		///            The simple name of the method to invoke. Will be coerced to a <code>String</code>.
		///            If method is "&lt;init&gt;"or "&lt;clinit&gt;" a NoSuchMethodException is raised. </param>
		/// <param name="paramTypes">
		///            An array of Class objects identifying the method's formal parameter types, in
		///            declared order. Use an empty array if the method has no parameters. Can be
		///            <code>null</code>, in which case the method's formal parameter types are assumed
		///            to be unknown. </param>
		/// <param name="params">
		///            The parameters to pass to the method, or <code>null</code> if no parameters. </param>
		/// <returns> The result of the method invocation (<code>null</code> if the method has a
		///         <code>void</code> return type).
		/// @since 2.2 </returns>
		public override object invoke(ELContext context, object @base, object method, Type[] paramTypes, object[] @params)
		{
			context.PropertyResolved = false;
			foreach (ELResolver resolver in resolvers)
			{
				object result = resolver.invoke(context, @base, method, paramTypes, @params);
				if (context.PropertyResolved)
				{
					return result;
				}
			}
			return null;
		}
	}

}