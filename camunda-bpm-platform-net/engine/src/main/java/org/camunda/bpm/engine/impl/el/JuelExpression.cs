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
	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExpressionGetInvocation = org.camunda.bpm.engine.impl.@delegate.ExpressionGetInvocation;
	using ExpressionSetInvocation = org.camunda.bpm.engine.impl.@delegate.ExpressionSetInvocation;
	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;
	using MethodNotFoundException = org.camunda.bpm.engine.impl.javax.el.MethodNotFoundException;
	using PropertyNotFoundException = org.camunda.bpm.engine.impl.javax.el.PropertyNotFoundException;
	using ValueExpression = org.camunda.bpm.engine.impl.javax.el.ValueExpression;


	/// <summary>
	/// Expression implementation backed by a JUEL <seealso cref="ValueExpression"/>.
	/// 
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// </summary>
	public class JuelExpression : Expression
	{

	  protected internal string expressionText;
	  protected internal ValueExpression valueExpression;
	  protected internal ExpressionManager expressionManager;

	  public JuelExpression(ValueExpression valueExpression, ExpressionManager expressionManager, string expressionText)
	  {
		this.valueExpression = valueExpression;
		this.expressionManager = expressionManager;
		this.expressionText = expressionText;
	  }

	  public virtual object getValue(VariableScope variableScope)
	  {
		return getValue(variableScope, null);
	  }

	  public virtual object getValue(VariableScope variableScope, BaseDelegateExecution contextExecution)
	  {
		ELContext elContext = expressionManager.getElContext(variableScope);
		try
		{
		  ExpressionGetInvocation invocation = new ExpressionGetInvocation(valueExpression, elContext, contextExecution);
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
		  return invocation.InvocationResult;
		}
		catch (PropertyNotFoundException pnfe)
		{
		  throw new ProcessEngineException("Unknown property used in expression: " + expressionText + ". Cause: " + pnfe.Message, pnfe);
		}
		catch (MethodNotFoundException mnfe)
		{
		  throw new ProcessEngineException("Unknown method used in expression: " + expressionText + ". Cause: " + mnfe.Message, mnfe);
		}
		catch (ELException ele)
		{
		  throw new ProcessEngineException("Error while evaluating expression: " + expressionText + ". Cause: " + ele.Message, ele);
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Error while evaluating expression: " + expressionText + ". Cause: " + e.Message, e);
		}
	  }

	  public virtual void setValue(object value, VariableScope variableScope)
	  {
		setValue(value, variableScope, null);
	  }

	  public virtual void setValue(object value, VariableScope variableScope, BaseDelegateExecution contextExecution)
	  {
		ELContext elContext = expressionManager.getElContext(variableScope);
		try
		{
		  ExpressionSetInvocation invocation = new ExpressionSetInvocation(valueExpression, elContext, value, contextExecution);
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Error while evaluating expression: " + expressionText + ". Cause: " + e.Message, e);
		}
	  }

	  public override string ToString()
	  {
		if (valueExpression != null)
		{
		  return valueExpression.ExpressionString;
		}
		return base.ToString();
	  }

	  public virtual bool LiteralText
	  {
		  get
		  {
			return valueExpression.LiteralText;
		  }
	  }

	  public virtual string ExpressionText
	  {
		  get
		  {
			return expressionText;
		  }
	  }
	}

}