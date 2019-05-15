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
namespace org.camunda.bpm.engine.impl.task.listener
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ClassDelegateUtil.applyFieldDeclaration;

	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using TaskListenerInvocation = org.camunda.bpm.engine.impl.task.@delegate.TaskListenerInvocation;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class DelegateExpressionTaskListener : TaskListener
	{

	  protected internal Expression expression;
	  private readonly IList<FieldDeclaration> fieldDeclarations;

	  public DelegateExpressionTaskListener(Expression expression, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.expression = expression;
		this.fieldDeclarations = fieldDeclarations;
	  }

	  public virtual void notify(DelegateTask delegateTask)
	  {
		// Note: we can't cache the result of the expression, because the
		// execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'

		VariableScope variableScope = delegateTask.Execution;
		if (variableScope == null)
		{
		  variableScope = delegateTask.CaseExecution;
		}

		object @delegate = expression.getValue(variableScope);
		applyFieldDeclaration(fieldDeclarations, @delegate);

		if (@delegate is TaskListener)
		{
		  try
		  {
			Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new TaskListenerInvocation((TaskListener)@delegate, delegateTask));
		  }
		  catch (Exception e)
		  {
			throw new ProcessEngineException("Exception while invoking TaskListener: " + e.Message, e);
		  }
		}
		else
		{
		  throw new ProcessEngineException("Delegate expression " + expression + " did not resolve to an implementation of " + typeof(TaskListener));
		}
	  }

	  /// <summary>
	  /// returns the expression text for this task listener. Comes in handy if you want to
	  /// check which listeners you already have.
	  /// </summary>
	  public virtual string ExpressionText
	  {
		  get
		  {
			return expression.ExpressionText;
		  }
	  }

	  public virtual IList<FieldDeclaration> FieldDeclarations
	  {
		  get
		  {
			return fieldDeclarations;
		  }
	  }

	}

}