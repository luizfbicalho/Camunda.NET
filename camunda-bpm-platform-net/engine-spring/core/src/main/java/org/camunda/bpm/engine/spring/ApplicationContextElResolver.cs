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
namespace org.camunda.bpm.engine.spring
{

	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using ApplicationContext = org.springframework.context.ApplicationContext;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Frederik Heremans
	/// </summary>
	public class ApplicationContextElResolver : ELResolver
	{

	  protected internal ApplicationContext applicationContext;

	  public ApplicationContextElResolver(ApplicationContext applicationContext)
	  {
		this.applicationContext = applicationContext;
	  }

	  public override object getValue(ELContext context, object @base, object property)
	  {
		if (@base == null)
		{
		  // according to javadoc, can only be a String
		  string key = (string) property;

		  if (applicationContext.containsBean(key))
		  {
			context.PropertyResolved = true;
			return applicationContext.getBean(key);
		  }
		}

		return null;
	  }

	  public override bool isReadOnly(ELContext context, object @base, object property)
	  {
		return true;
	  }

	  public override void setValue(ELContext context, object @base, object property, object value)
	  {
		if (@base == null)
		{
		  string key = (string) property;
		  if (applicationContext.containsBean(key))
		  {
			throw new ProcessEngineException("Cannot set value of '" + property + "', it resolves to a bean defined in the Spring application-context.");
		  }
		}
	  }

	  public override Type getCommonPropertyType(ELContext context, object arg)
	  {
		return typeof(object);
	  }

	  public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object arg)
	  {
		return null;
	  }

	  public override Type getType(ELContext context, object arg1, object arg2)
	  {
		return typeof(object);
	  }
	}

}