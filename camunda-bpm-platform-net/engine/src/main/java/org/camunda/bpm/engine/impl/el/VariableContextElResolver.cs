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
namespace org.camunda.bpm.engine.impl.el
{

	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class VariableContextElResolver : ELResolver
	{

	  public const string VAR_CTX_KEY = "variableContext";

	  public override object getValue(ELContext context, object @base, object property)
	  {
		VariableContext variableContext = (VariableContext) context.getContext(typeof(VariableContext));
		if (variableContext != null)
		{
		  if (VAR_CTX_KEY.Equals(property))
		  {
			context.PropertyResolved = true;
			return variableContext;
		  }
		  TypedValue typedValue = variableContext.resolve((string) property);
		  if (typedValue != null)
		  {
			context.PropertyResolved = true;
			return unpack(typedValue);
		  }
		}
		return null;
	  }

	  public override void setValue(ELContext context, object @base, object property, object value)
	  {
		// read only
	  }

	  public override bool isReadOnly(ELContext context, object @base, object property)
	  {
		// always read only
		return true;
	  }

	  public override Type getCommonPropertyType(ELContext arg0, object arg1)
	  {
		return typeof(object);
	  }

	  public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext arg0, object arg1)
	  {
		return null;
	  }

	  public override Type getType(ELContext arg0, object arg1, object arg2)
	  {
		return typeof(object);
	  }

	  protected internal virtual object unpack(TypedValue typedValue)
	  {
		if (typedValue != null)
		{
		  return typedValue.Value;
		}
		return null;
	  }

	}

}