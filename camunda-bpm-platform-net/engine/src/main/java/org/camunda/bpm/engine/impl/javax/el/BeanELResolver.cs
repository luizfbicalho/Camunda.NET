using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
	/// Defines property resolution behavior on objects using the JavaBeans component architecture. This
	/// resolver handles base objects of any type, as long as the base is not null. It accepts any object
	/// as a property, and coerces it to a string. That string is then used to find a JavaBeans compliant
	/// property on the base object. The value is accessed using JavaBeans getters and setters. This
	/// resolver can be constructed in read-only mode, which means that isReadOnly will always return
	/// true and <seealso cref="#setValue(ELContext, Object, Object, Object)"/> will always throw
	/// PropertyNotWritableException. ELResolvers are combined together using <seealso cref="CompositeELResolver"/>
	/// s, to define rich semantics for evaluating an expression. See the javadocs for <seealso cref="ELResolver"/>
	/// for details. Because this resolver handles base objects of any type, it should be placed near the
	/// end of a composite resolver. Otherwise, it will claim to have resolved a property before any
	/// resolvers that come after it get a chance to test if they can do so as well.
	/// </summary>
	/// <seealso cref= CompositeELResolver </seealso>
	/// <seealso cref= ELResolver </seealso>
	public class BeanELResolver : ELResolver
	{
		protected internal sealed class BeanProperties
		{
			internal readonly IDictionary<string, BeanProperty> map = new Dictionary<string, BeanProperty>();

			public BeanProperties(Type baseClass)
			{
				PropertyDescriptor[] descriptors;
				try
				{
					descriptors = Introspector.getBeanInfo(baseClass).PropertyDescriptors;
				}
				catch (IntrospectionException e)
				{
					throw new ELException(e);
				}
				foreach (PropertyDescriptor descriptor in descriptors)
				{
					map[descriptor.Name] = new BeanProperty(descriptor);
				}
			}

			public BeanProperty getBeanProperty(string property)
			{
				return map[property];
			}
		}

		protected internal sealed class BeanProperty
		{
			internal readonly PropertyDescriptor descriptor;

			public BeanProperty(PropertyDescriptor descriptor)
			{
				this.descriptor = descriptor;
			}

			public Type PropertyType
			{
				get
				{
					return descriptor.PropertyType;
				}
			}

			public System.Reflection.MethodInfo ReadMethod
			{
				get
				{
					return findAccessibleMethod(descriptor.ReadMethod);
				}
			}

			public System.Reflection.MethodInfo WriteMethod
			{
				get
				{
					return findAccessibleMethod(descriptor.WriteMethod);
				}
			}

			public bool ReadOnly
			{
				get
				{
					return findAccessibleMethod(descriptor.WriteMethod) == null;
				}
			}
		}

		private static System.Reflection.MethodInfo findAccessibleMethod(System.Reflection.MethodInfo method)
		{
			if (method == null || method.Accessible)
			{
				return method;
			}
			try
			{
				method.Accessible = true;
			}
			catch (SecurityException)
			{
				foreach (Type cls in method.DeclaringClass.Interfaces)
				{
					System.Reflection.MethodInfo mth = null;
					try
					{
						mth = cls.GetMethod(method.Name, method.ParameterTypes);
						mth = findAccessibleMethod(mth);
						if (mth != null)
						{
							return mth;
						}
					}
					catch (NoSuchMethodException)
					{
						// do nothing
					}
				}
				Type cls = method.DeclaringClass.BaseType;
				if (cls != null)
				{
					System.Reflection.MethodInfo mth = null;
					try
					{
						mth = cls.GetMethod(method.Name, method.ParameterTypes);
						mth = findAccessibleMethod(mth);
						if (mth != null)
						{
							return mth;
						}
					}
					catch (NoSuchMethodException)
					{
						// do nothing
					}
				}
				return null;
			}
			return method;
		}

		private readonly bool readOnly;
		private readonly ConcurrentDictionary<Type, BeanProperties> cache;

		private ExpressionFactory defaultFactory;

		/// <summary>
		/// Creates a new read/write BeanELResolver.
		/// </summary>
		public BeanELResolver() : this(false)
		{
		}

		/// <summary>
		/// Creates a new BeanELResolver whose read-only status is determined by the given parameter.
		/// </summary>
		public BeanELResolver(bool readOnly)
		{
			this.readOnly = readOnly;
			this.cache = new ConcurrentDictionary<Type, BeanProperties>();
		}

		/// <summary>
		/// If the base object is not null, returns the most general type that this resolver accepts for
		/// the property argument. Otherwise, returns null. Assuming the base is not null, this method
		/// will always return Object.class. This is because any object is accepted as a key and is
		/// coerced into a string.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <returns> null if base is null; otherwise Object.class. </returns>
		public override Type getCommonPropertyType(ELContext context, object @base)
		{
			return isResolvable(@base) ? typeof(object) : null;
		}

		/// <summary>
		/// If the base object is not null, returns an Iterator containing the set of JavaBeans
		/// properties available on the given object. Otherwise, returns null. The Iterator returned must
		/// contain zero or more instances of java.beans.FeatureDescriptor. Each info object contains
		/// information about a property in the bean, as obtained by calling the
		/// BeanInfo.getPropertyDescriptors method. The FeatureDescriptor is initialized using the same
		/// fields as are present in the PropertyDescriptor, with the additional required named
		/// attributes "type" and "resolvableAtDesignTime" set as follows:
		/// <ul>
		/// <li><seealso cref="ELResolver#TYPE"/> - The runtime type of the property, from
		/// PropertyDescriptor.getPropertyType().</li>
		/// <li><seealso cref="ELResolver#RESOLVABLE_AT_DESIGN_TIME"/> - true.</li>
		/// </ul>
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <returns> An Iterator containing zero or more FeatureDescriptor objects, each representing a
		///         property on this bean, or null if the base object is null. </returns>
		public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object @base)
		{
			if (isResolvable(@base))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.beans.PropertyDescriptor[] properties;
				PropertyDescriptor[] properties;
				try
				{
					properties = Introspector.getBeanInfo(@base.GetType()).PropertyDescriptors;
				}
				catch (IntrospectionException)
				{
					return System.Linq.Enumerable.Empty<FeatureDescriptor> ().GetEnumerator();
				}
				return new IteratorAnonymousInnerClass(this, properties);
			}
			return null;
		}

		private class IteratorAnonymousInnerClass : IEnumerator<FeatureDescriptor>
		{
			private readonly BeanELResolver outerInstance;

			private PropertyDescriptor[] properties;

			public IteratorAnonymousInnerClass(BeanELResolver outerInstance, PropertyDescriptor[] properties)
			{
				this.outerInstance = outerInstance;
				this.properties = properties;
				next = 0;
			}

			internal int next;

			public bool hasNext()
			{
				return properties != null && next < properties.Length;
			}

			public FeatureDescriptor next()
			{
				PropertyDescriptor property = properties[next++];
				FeatureDescriptor feature = new FeatureDescriptor();
				feature.DisplayName = property.DisplayName;
				feature.Name = property.Name;
				feature.ShortDescription = property.ShortDescription;
				feature.Expert = property.Expert;
				feature.Hidden = property.Hidden;
				feature.Preferred = property.Preferred;
				feature.setValue(TYPE, property.PropertyType);
				feature.setValue(RESOLVABLE_AT_DESIGN_TIME, true);
				return feature;
			}

			public void remove()
			{
				throw new System.NotSupportedException("cannot remove");
			}
		}

		/// <summary>
		/// If the base object is not null, returns the most general acceptable type that can be set on
		/// this bean property. If the base is not null, the propertyResolved property of the ELContext
		/// object must be set to true by this resolver, before returning. If this property is not true
		/// after this method is called, the caller should ignore the return value. The provided property
		/// will first be coerced to a String. If there is a BeanInfoProperty for this property and there
		/// were no errors retrieving it, the propertyType of the propertyDescriptor is returned.
		/// Otherwise, a PropertyNotFoundException is thrown.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
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
			if (context == null)
			{
				throw new System.NullReferenceException();
			}
			Type result = null;
			if (isResolvable(@base))
			{
				result = toBeanProperty(@base, property).PropertyType;
				context.PropertyResolved = true;
			}
			return result;
		}

		/// <summary>
		/// If the base object is not null, returns the current value of the given property on this bean.
		/// If the base is not null, the propertyResolved property of the ELContext object must be set to
		/// true by this resolver, before returning. If this property is not true after this method is
		/// called, the caller should ignore the return value. The provided property name will first be
		/// coerced to a String. If the property is a readable property of the base object, as per the
		/// JavaBeans specification, then return the result of the getter call. If the getter throws an
		/// exception, it is propagated to the caller. If the property is not found or is not readable, a
		/// PropertyNotFoundException is thrown.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then the value of the
		///         given property. Otherwise, undefined. </returns>
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
			if (context == null)
			{
				throw new System.NullReferenceException();
			}
			object result = null;
			if (isResolvable(@base))
			{
				System.Reflection.MethodInfo method = toBeanProperty(@base, property).ReadMethod;
				if (method == null)
				{
					throw new PropertyNotFoundException("Cannot read property " + property);
				}
				try
				{
					result = method.invoke(@base);
				}
				catch (InvocationTargetException e)
				{
					throw new ELException(e.InnerException);
				}
				catch (Exception e)
				{
					throw new ELException(e);
				}
				context.PropertyResolved = true;
			}
			return result;
		}

		/// <summary>
		/// If the base object is not null, returns whether a call to
		/// <seealso cref="#setValue(ELContext, Object, Object, Object)"/> will always fail. If the base is not
		/// null, the propertyResolved property of the ELContext object must be set to true by this
		/// resolver, before returning. If this property is not true after this method is called, the
		/// caller can safely assume no value was set.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <returns> If the propertyResolved property of ELContext was set to true, then true if calling
		///         the setValue method will always fail or false if it is possible that such a call may
		///         succeed; otherwise undefined. </returns>
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
			if (context == null)
			{
				throw new System.NullReferenceException();
			}
			bool result = readOnly;
			if (isResolvable(@base))
			{
				result |= toBeanProperty(@base, property).ReadOnly;
				context.PropertyResolved = true;
			}
			return result;
		}

		/// <summary>
		/// If the base object is not null, attempts to set the value of the given property on this bean.
		/// If the base is not null, the propertyResolved property of the ELContext object must be set to
		/// true by this resolver, before returning. If this property is not true after this method is
		/// called, the caller can safely assume no value was set. If this resolver was constructed in
		/// read-only mode, this method will always throw PropertyNotWritableException. The provided
		/// property name will first be coerced to a String. If property is a writable property of base
		/// (as per the JavaBeans Specification), the setter method is called (passing value). If the
		/// property exists but does not have a setter, then a PropertyNotFoundException is thrown. If
		/// the property does not exist, a PropertyNotFoundException is thrown.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <param name="value">
		///            The value to be associated with the specified key. </param>
		/// <exception cref="NullPointerException">
		///             if context is null </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if base is not null and the specified property does not exist or is not readable. </exception>
		/// <exception cref="PropertyNotWritableException">
		///             if this resolver was constructed in read-only mode, or if there is no setter for
		///             the property </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing the property or variable resolution.
		///             The thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public override void setValue(ELContext context, object @base, object property, object value)
		{
			if (context == null)
			{
				throw new System.NullReferenceException();
			}
			if (isResolvable(@base))
			{
				if (readOnly)
				{
					throw new PropertyNotWritableException("resolver is read-only");
				}
				System.Reflection.MethodInfo method = toBeanProperty(@base, property).WriteMethod;
				if (method == null)
				{
					throw new PropertyNotWritableException("Cannot write property: " + property);
				}
				try
				{
					method.invoke(@base, value);
				}
				catch (InvocationTargetException e)
				{
					throw new ELException("Cannot write property: " + property, e.InnerException);
				}
				catch (IllegalAccessException e)
				{
					throw new PropertyNotWritableException("Cannot write property: " + property, e);
				}
				context.PropertyResolved = true;
			}
		}

		/// <summary>
		/// If the base object is not <code>null</code>, invoke the method, with the given parameters on
		/// this bean. The return value from the method is returned.
		/// 
		/// <para>
		/// If the base is not <code>null</code>, the <code>propertyResolved</code> property of the
		/// <code>ELContext</code> object must be set to <code>true</code> by this resolver, before
		/// returning. If this property is not <code>true</code> after this method is called, the caller
		/// should ignore the return value.
		/// </para>
		/// 
		/// <para>
		/// The provided method object will first be coerced to a <code>String</code>. The methods in the
		/// bean is then examined and an attempt will be made to select one for invocation. If no
		/// suitable can be found, a <code>MethodNotFoundException</code> is thrown.
		/// 
		/// If the given paramTypes is not <code>null</code>, select the method with the given name and
		/// parameter types.
		/// 
		/// Else select the method with the given name that has the same number of parameters. If there
		/// are more than one such method, the method selection process is undefined.
		/// 
		/// Else select the method with the given name that takes a variable number of arguments.
		/// 
		/// Note the resolution for overloaded methods will likely be clarified in a future version of
		/// the spec.
		/// 
		/// The provided parameters are coerced to the corresponding parameter types of the method, and
		/// the method is then invoked.
		/// 
		/// </para>
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="base">
		///            The bean on which to invoke the method </param>
		/// <param name="method">
		///            The simple name of the method to invoke. Will be coerced to a <code>String</code>.
		///            If method is "&lt;init&gt;"or "&lt;clinit&gt;" a MethodNotFoundException is
		///            thrown. </param>
		/// <param name="paramTypes">
		///            An array of Class objects identifying the method's formal parameter types, in
		///            declared order. Use an empty array if the method has no parameters. Can be
		///            <code>null</code>, in which case the method's formal parameter types are assumed
		///            to be unknown. </param>
		/// <param name="params">
		///            The parameters to pass to the method, or <code>null</code> if no parameters. </param>
		/// <returns> The result of the method invocation (<code>null</code> if the method has a
		///         <code>void</code> return type). </returns>
		/// <exception cref="MethodNotFoundException">
		///             if no suitable method can be found. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing (base, method) resolution. The thrown
		///             exception must be included as the cause property of this exception, if available.
		///             If the exception thrown is an <code>InvocationTargetException</code>, extract its
		///             <code>cause</code> and pass it to the <code>ELException</code> constructor.
		/// @since 2.2 </exception>
		public override object invoke(ELContext context, object @base, object method, Type[] paramTypes, object[] @params)
		{
			if (context == null)
			{
				throw new System.NullReferenceException();
			}
			object result = null;
			if (isResolvable(@base))
			{
				if (@params == null)
				{
					@params = new object[0];
				}
				string name = method.ToString();
				System.Reflection.MethodInfo target = findMethod(@base, name, paramTypes, @params.Length);
				if (target == null)
				{
					throw new MethodNotFoundException("Cannot find method " + name + " with " + @params.Length + " parameters in " + @base.GetType());
				}
				try
				{
					result = target.invoke(@base, coerceParams(getExpressionFactory(context), target, @params));
				}
				catch (InvocationTargetException e)
				{
					throw new ELException(e.InnerException);
				}
				catch (IllegalAccessException e)
				{
					throw new ELException(e);
				}
				context.PropertyResolved = true;
			}
			return result;
		}

	  private System.Reflection.MethodInfo findMethod(object @base, string name, Type[] types, int paramCount)
	  {
			if (types != null)
			{
				try
				{
					return findAccessibleMethod(@base.GetType().GetMethod(name, types));
				}
				catch (NoSuchMethodException)
				{
					return null;
				}
			}
			System.Reflection.MethodInfo varArgsMethod = null;
			foreach (System.Reflection.MethodInfo method in @base.GetType().GetMethods())
			{
				if (method.Name.Equals(name))
				{
					int formalParamCount = method.ParameterTypes.length;
					if (method.VarArgs && paramCount >= formalParamCount - 1)
					{
						varArgsMethod = method;
					}
					else if (paramCount == formalParamCount)
					{
						return findAccessibleMethod(method);
					}
				}
			}
			return varArgsMethod == null ? null : findAccessibleMethod(varArgsMethod);
	  }

		/// <summary>
		/// Lookup an expression factory used to coerce method parameters in context under key
		/// <code>"javax.el.ExpressionFactory"</code>.
		/// If no expression factory can be found under that key, use a default instance created with
		/// <seealso cref="ExpressionFactory#newInstance()"/>. </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <returns> expression factory instance </returns>
		private ExpressionFactory getExpressionFactory(ELContext context)
		{
			object obj = context.getContext(typeof(ExpressionFactory));
			if (obj is ExpressionFactory)
			{
				return (ExpressionFactory)obj;
			}
			if (defaultFactory == null)
			{
				defaultFactory = ExpressionFactory.newInstance();
			}
			return defaultFactory;
		}

		private object[] coerceParams(ExpressionFactory factory, System.Reflection.MethodInfo method, object[] @params)
		{
			Type[] types = method.ParameterTypes;
			object[] args = new object[types.Length];
			if (method.VarArgs)
			{
				int varargIndex = types.Length - 1;
				if (@params.Length < varargIndex)
				{
					throw new ELException("Bad argument count");
				}
				for (int i = 0; i < varargIndex; i++)
				{
					coerceValue(args, i, factory, @params[i], types[i]);
				}
				Type varargType = types[varargIndex].GetElementType();
				int length = @params.Length - varargIndex;
				object array = null;
				if (length == 1)
				{
					object source = @params[varargIndex];
					if (source != null && source.GetType().IsArray)
					{
						if (types[varargIndex].IsInstanceOfType(source))
						{ // use source array as is
							array = source;
						}
						else
						{ // coerce array elements
							length = Array.getLength(source);
							array = Array.CreateInstance(varargType, length);
							for (int i = 0; i < length; i++)
							{
								coerceValue(array, i, factory, Array.get(source, i), varargType);
							}
						}
					}
					else
					{ // single element array
						array = Array.CreateInstance(varargType, 1);
						coerceValue(array, 0, factory, source, varargType);
					}
				}
				else
				{
					array = Array.CreateInstance(varargType, length);
					for (int i = 0; i < length; i++)
					{
						coerceValue(array, i, factory, @params[varargIndex + i], varargType);
					}
				}
				args[varargIndex] = array;
			}
			else
			{
				if (@params.Length != args.Length)
				{
					throw new ELException("Bad argument count");
				}
				for (int i = 0; i < args.Length; i++)
				{
					coerceValue(args, i, factory, @params[i], types[i]);
				}
			}
			return args;
		}

		private void coerceValue(object array, int index, ExpressionFactory factory, object value, Type type)
		{
			if (value != null || type.IsPrimitive)
			{
				((Array)array).SetValue(factory.coerceToType(value, type), index);
			}
		}

		/// <summary>
		/// Test whether the given base should be resolved by this ELResolver.
		/// </summary>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <returns> base != null </returns>
		private bool isResolvable(object @base)
		{
			return @base != null;
		}

		/// <summary>
		/// Lookup BeanProperty for the given (base, property) pair.
		/// </summary>
		/// <param name="base">
		///            The bean to analyze. </param>
		/// <param name="property">
		///            The name of the property to analyze. Will be coerced to a String. </param>
		/// <returns> The BeanProperty representing (base, property). </returns>
		/// <exception cref="PropertyNotFoundException">
		///             if no BeanProperty can be found. </exception>
		private BeanProperty toBeanProperty(object @base, object property)
		{
			BeanProperties beanProperties = cache[@base.GetType()];
			if (beanProperties == null)
			{
				BeanProperties newBeanProperties = new BeanProperties(@base.GetType());
				beanProperties = cache.GetOrAdd(@base.GetType(), newBeanProperties);
				if (beanProperties == null)
				{ // put succeeded, use new value
					beanProperties = newBeanProperties;
				}
			}
			BeanProperty beanProperty = property == null ? null : beanProperties.getBeanProperty(property.ToString());
			if (beanProperty == null)
			{
				throw new PropertyNotFoundException("Could not find property " + property + " in " + @base.GetType());
			}
			return beanProperty;
		}

		/// <summary>
		/// This method is not part of the API, though it can be used (reflectively) by clients of this
		/// class to remove entries from the cache when the beans are being unloaded.
		/// 
		/// Note: this method is present in the reference implementation, so we're adding it here to ease
		/// migration.
		/// </summary>
		/// <param name="classloader">
		///            The classLoader used to load the beans. </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private final void purgeBeanClasses(ClassLoader loader)
		private void purgeBeanClasses(ClassLoader loader)
		{
			IEnumerator<Type> classes = cache.Keys.GetEnumerator();
			while (classes.MoveNext())
			{
				if (loader == classes.Current.ClassLoader)
				{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
					classes.remove();
				}
			}
		}
	}

}