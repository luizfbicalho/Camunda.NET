using System;

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
	using IoParameter = org.camunda.bpm.engine.impl.core.variable.mapping.IoParameter;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using EnsureUtil = org.camunda.commons.utils.EnsureUtil;

	/// <summary>
	/// Makes it possible to use expression in <seealso cref="IoParameter"/> mappings.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ElValueProvider : ParameterValueProvider, IComparable<ElValueProvider>
	{

	  protected internal Expression expression;

	  public ElValueProvider(Expression expression)
	  {
		this.expression = expression;
	  }

	  public virtual object getValue(VariableScope variableScope)
	  {
		EnsureUtil.ensureNotNull("variableScope", variableScope);
		return expression.getValue(variableScope);
	  }

	  public virtual Expression Expression
	  {
		  get
		  {
			return expression;
		  }
		  set
		  {
			this.expression = value;
		  }
	  }


	  public virtual int CompareTo(ElValueProvider o)
	  {
		return expression.ExpressionText.compareTo(o.Expression.ExpressionText);
	  }

	}

}