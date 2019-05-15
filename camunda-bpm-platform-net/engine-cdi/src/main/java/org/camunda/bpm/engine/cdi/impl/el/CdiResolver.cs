using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
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
namespace org.camunda.bpm.engine.cdi.impl.el
{

	using BeanManagerLookup = org.camunda.bpm.engine.cdi.impl.util.BeanManagerLookup;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;


	/// <summary>
	/// Resolver wrapping an instance of javax.el.ELResolver obtained from the
	/// <seealso cref="BeanManager"/>. Allows the process engine to resolve Cdi-Beans.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class CdiResolver : ELResolver
	{

	  protected internal virtual BeanManager BeanManager
	  {
		  get
		  {
			return BeanManagerLookup.BeanManager;
		  }
	  }

	  protected internal virtual javax.el.ELResolver WrappedResolver
	  {
		  get
		  {
			BeanManager beanManager = BeanManager;
			javax.el.ELResolver resolver = beanManager.ELResolver;
			return resolver;
		  }
	  }

	  public override Type getCommonPropertyType(ELContext context, object @base)
	  {
		return WrappedResolver.getCommonPropertyType(wrapContext(context), @base);
	  }

	  public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object @base)
	  {
		return WrappedResolver.getFeatureDescriptors(wrapContext(context), @base);
	  }

	  public override Type getType(ELContext context, object @base, object property)
	  {
		return WrappedResolver.getType(wrapContext(context), @base, property);
	  }

	  public override object getValue(ELContext context, object @base, object property)
	  {
		//we need to resolve a bean only for the first "member" of expression, e.g. bean.property1.property2
		if (@base == null)
		{
		  object result = ProgrammaticBeanLookup.lookup(property.ToString(), BeanManager);
		  if (result != null)
		  {
			context.PropertyResolved = true;
		  }
		  return result;
		}
		else
		{
		  return null;
		}
	  }

	  public override bool isReadOnly(ELContext context, object @base, object property)
	  {
		return WrappedResolver.isReadOnly(wrapContext(context), @base, property);
	  }

	  public override void setValue(ELContext context, object @base, object property, object value)
	  {
		WrappedResolver.setValue(wrapContext(context), @base, property, value);
	  }

	  public override object invoke(ELContext context, object @base, object method, Type[] paramTypes, object[] @params)
	  {
		return WrappedResolver.invoke(wrapContext(context), @base, method, paramTypes, @params);
	  }

	  protected internal virtual javax.el.ELContext wrapContext(ELContext context)
	  {
		return new ElContextDelegate(context, WrappedResolver);
	  }

	}

}