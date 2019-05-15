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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureInstanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using PropertyNotFoundException = org.camunda.bpm.engine.impl.javax.el.PropertyNotFoundException;


	/// <summary>
	/// <seealso cref="Condition"/> that resolves an UEL expression at runtime.
	/// 
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	public class UelExpressionCondition : Condition
	{

	  protected internal Expression expression;

	  public UelExpressionCondition(Expression expression)
	  {
		this.expression = expression;
	  }

	  public virtual bool evaluate(DelegateExecution execution)
	  {
		return evaluate(execution, execution);
	  }

	  public virtual bool evaluate(VariableScope scope, DelegateExecution execution)
	  {
		object result = expression.getValue(scope, execution);
		ensureNotNull("condition expression returns null", "result", result);
		ensureInstanceOf("condition expression returns non-Boolean", "result", result, typeof(Boolean));
		return (bool?) result.Value;
	  }

	  public virtual bool tryEvaluate(VariableScope scope, DelegateExecution execution)
	  {
		bool result = false;
		try
		{
		  result = evaluate(scope, execution);
		}
		catch (ProcessEngineException pee)
		{
		  if (!(pee.InnerException is PropertyNotFoundException))
		  {
			throw pee;
		  }
		}
		return result;
	  }
	}

}