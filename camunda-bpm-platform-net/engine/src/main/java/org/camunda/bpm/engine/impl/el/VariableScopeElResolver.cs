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
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;



	/// <summary>
	/// Implementation of an <seealso cref="ELResolver"/> that resolves expressions
	/// with the process variables of a given <seealso cref="VariableScope"/> as context.
	/// <br>
	/// Also exposes the currently logged in username to be used in expressions (if any)
	/// 
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	public class VariableScopeElResolver : ELResolver
	{

	  public const string EXECUTION_KEY = "execution";
	  public const string CASE_EXECUTION_KEY = "caseExecution";
	  public const string TASK_KEY = "task";
	  public const string LOGGED_IN_USER_KEY = "authenticatedUserId";

	  public override object getValue(ELContext context, object @base, object property)
	  {

		object @object = context.getContext(typeof(VariableScope));
		if (@object != null)
		{
		  VariableScope variableScope = (VariableScope) @object;
		  if (@base == null)
		  {
			string variable = (string) property; // according to javadoc, can only be a String

			if ((EXECUTION_KEY.Equals(property) && variableScope is ExecutionEntity) || (TASK_KEY.Equals(property) && variableScope is TaskEntity) || (variableScope is CaseExecutionEntity && (CASE_EXECUTION_KEY.Equals(property) || EXECUTION_KEY.Equals(property))))
			{
			  context.PropertyResolved = true;
			  return variableScope;
			}
			else if (EXECUTION_KEY.Equals(property) && variableScope is TaskEntity)
			{
			  context.PropertyResolved = true;
			  return ((TaskEntity) variableScope).getExecution();
			}
			else if (LOGGED_IN_USER_KEY.Equals(property))
			{
			  context.PropertyResolved = true;
			  return Context.CommandContext.AuthenticatedUserId;
			}
			else
			{
			  if (variableScope.hasVariable(variable))
			  {
				context.PropertyResolved = true; // if not set, the next elResolver in the CompositeElResolver will be called
				return variableScope.getVariable(variable);
			  }
			}
		  }
		}

		// property resolution (eg. bean.value) will be done by the BeanElResolver (part of the CompositeElResolver)
		// It will use the bean resolved in this resolver as base.

		return null;
	  }

	  public override bool isReadOnly(ELContext context, object @base, object property)
	  {
		if (@base == null)
		{
		  string variable = (string) property;
		  object @object = context.getContext(typeof(VariableScope));
		  return @object != null && !((VariableScope)@object).hasVariable(variable);
		}
		return true;
	  }

	  public override void setValue(ELContext context, object @base, object property, object value)
	  {
		if (@base == null)
		{
		  string variable = (string) property;
		  object @object = context.getContext(typeof(VariableScope));
		  if (@object != null)
		  {
			VariableScope variableScope = (VariableScope) @object;
			if (variableScope.hasVariable(variable))
			{
			  variableScope.setVariable(variable, value);
			}
		  }
		}
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

	}

}