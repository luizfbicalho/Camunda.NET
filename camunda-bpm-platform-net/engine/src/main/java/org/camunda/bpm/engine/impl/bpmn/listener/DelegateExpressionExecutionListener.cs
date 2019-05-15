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
namespace org.camunda.bpm.engine.impl.bpmn.listener
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ClassDelegateUtil.applyFieldDeclaration;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using BpmnBehaviorLogger = org.camunda.bpm.engine.impl.bpmn.behavior.BpmnBehaviorLogger;
	using ExecutionListenerInvocation = org.camunda.bpm.engine.impl.bpmn.@delegate.ExecutionListenerInvocation;
	using JavaDelegateInvocation = org.camunda.bpm.engine.impl.bpmn.@delegate.JavaDelegateInvocation;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class DelegateExpressionExecutionListener : ExecutionListener
	{

	  protected internal static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  protected internal Expression expression;
	  private readonly IList<FieldDeclaration> fieldDeclarations;

	  public DelegateExpressionExecutionListener(Expression expression, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.expression = expression;
		this.fieldDeclarations = fieldDeclarations;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		// Note: we can't cache the result of the expression, because the
		// execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'
		object @delegate = expression.getValue(execution);
		applyFieldDeclaration(fieldDeclarations, @delegate);

		if (@delegate is ExecutionListener)
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ExecutionListenerInvocation((ExecutionListener) @delegate, execution));
		}
		else if (@delegate is JavaDelegate)
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new JavaDelegateInvocation((JavaDelegate) @delegate, execution));
		}
		else
		{
		  throw LOG.resolveDelegateExpressionException(expression, typeof(ExecutionListener), typeof(JavaDelegate));
		}
	  }

	  /// <summary>
	  /// returns the expression text for this execution listener. Comes in handy if you want to
	  /// check which listeners you already have.
	  /// </summary>
	  public virtual string ExpressionText
	  {
		  get
		  {
			return expression.ExpressionText;
		  }
	  }

	}

}