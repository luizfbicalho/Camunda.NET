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

	using ArrayELResolver = org.camunda.bpm.engine.impl.javax.el.ArrayELResolver;
	using BeanELResolver = org.camunda.bpm.engine.impl.javax.el.BeanELResolver;
	using CompositeELResolver = org.camunda.bpm.engine.impl.javax.el.CompositeELResolver;
	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using ListELResolver = org.camunda.bpm.engine.impl.javax.el.ListELResolver;
	using MapELResolver = org.camunda.bpm.engine.impl.javax.el.MapELResolver;
	using ResourceBundleELResolver = org.camunda.bpm.engine.impl.javax.el.ResourceBundleELResolver;

	/// <summary>
	/// Simple resolver implementation. This resolver handles root properties (top-level identifiers).
	/// Resolving "real" properties (<code>base != null</code>) is delegated to a resolver specified at
	/// construction time.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class SimpleResolver : ELResolver
	{
		private static readonly ELResolver DEFAULT_RESOLVER_READ_ONLY = new CompositeELResolverAnonymousInnerClass();

		private class CompositeELResolverAnonymousInnerClass : CompositeELResolver
		{
	//		{
	//			add(new ArrayELResolver(true));
	//			add(new ListELResolver(true));
	//			add(new MapELResolver(true));
	//			add(new ResourceBundleELResolver());
	//			add(new BeanELResolver(true));
	//		}
		}
		private static readonly ELResolver DEFAULT_RESOLVER_READ_WRITE = new CompositeELResolverAnonymousInnerClass2();

		private class CompositeELResolverAnonymousInnerClass2 : CompositeELResolver
		{
	//		{
	//			add(new ArrayELResolver(false));
	//			add(new ListELResolver(false));
	//			add(new MapELResolver(false));
	//			add(new ResourceBundleELResolver());
	//			add(new BeanELResolver(false));
	//		}
		}

		private readonly RootPropertyResolver root;
		private readonly CompositeELResolver @delegate;

		/// <summary>
		/// Create a resolver capable of resolving top-level identifiers. Everything else is passed to
		/// the supplied delegate.
		/// </summary>
		public SimpleResolver(ELResolver resolver, bool readOnly)
		{
			@delegate = new CompositeELResolver();
			@delegate.add(root = new RootPropertyResolver(readOnly));
			@delegate.add(resolver);
		}

		/// <summary>
		/// Create a read/write resolver capable of resolving top-level identifiers. Everything else is
		/// passed to the supplied delegate.
		/// </summary>
		public SimpleResolver(ELResolver resolver) : this(resolver, false)
		{
		}

		/// <summary>
		/// Create a resolver capable of resolving top-level identifiers, array values, list values, map
		/// values, resource values and bean properties.
		/// </summary>
		public SimpleResolver(bool readOnly) : this(readOnly ? DEFAULT_RESOLVER_READ_ONLY : DEFAULT_RESOLVER_READ_WRITE, readOnly)
		{
		}

		/// <summary>
		/// Create a read/write resolver capable of resolving top-level identifiers, array values, list
		/// values, map values, resource values and bean properties.
		/// </summary>
		public SimpleResolver() : this(DEFAULT_RESOLVER_READ_WRITE, false)
		{
		}

		/// <summary>
		/// Answer our root resolver which provides an API to access top-level properties.
		/// </summary>
		/// <returns> root property resolver </returns>
		public virtual RootPropertyResolver RootPropertyResolver
		{
			get
			{
				return root;
			}
		}

		public override Type getCommonPropertyType(ELContext context, object @base)
		{
			return @delegate.getCommonPropertyType(context, @base);
		}

		public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object @base)
		{
			return @delegate.getFeatureDescriptors(context, @base);
		}

		public override Type getType(ELContext context, object @base, object property)
		{
			return @delegate.getType(context, @base, property);
		}

		public override object getValue(ELContext context, object @base, object property)
		{
			return @delegate.getValue(context, @base, property);
		}

		public override bool isReadOnly(ELContext context, object @base, object property)
		{
			return @delegate.isReadOnly(context, @base, property);
		}

		public override void setValue(ELContext context, object @base, object property, object value)
		{
			@delegate.setValue(context, @base, property, value);
		}

		public override object invoke(ELContext context, object @base, object method, Type[] paramTypes, object[] @params)
		{
			return @delegate.invoke(context, @base, method, paramTypes, @params);
		}
	}

}