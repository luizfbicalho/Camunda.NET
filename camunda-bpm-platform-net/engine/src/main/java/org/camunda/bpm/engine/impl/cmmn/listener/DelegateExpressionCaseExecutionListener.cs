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
namespace org.camunda.bpm.engine.impl.cmmn.listener
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ClassDelegateUtil.applyFieldDeclaration;

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using CaseExecutionListenerInvocation = org.camunda.bpm.engine.impl.cmmn.@delegate.CaseExecutionListenerInvocation;
	using Context = org.camunda.bpm.engine.impl.context.Context;


	/// <summary>
	/// @author Roman Smirnov
	/// </summary>
	public class DelegateExpressionCaseExecutionListener : CaseExecutionListener
	{

	  protected internal Expression expression;
	  private readonly IList<FieldDeclaration> fieldDeclarations;

	  public DelegateExpressionCaseExecutionListener(Expression expression, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.expression = expression;
		this.fieldDeclarations = fieldDeclarations;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseExecution caseExecution) throws Exception
	  public virtual void notify(DelegateCaseExecution caseExecution)
	  {
		// Note: we can't cache the result of the expression, because the
		// caseExecution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'
		object @delegate = expression.getValue(caseExecution);
		applyFieldDeclaration(fieldDeclarations, @delegate);

		if (@delegate is CaseExecutionListener)
		{
		  CaseExecutionListener listenerInstance = (CaseExecutionListener) @delegate;
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new CaseExecutionListenerInvocation(listenerInstance, caseExecution));
		}
		else
		{
		  throw new ProcessEngineException("Delegate expression " + expression + " did not resolve to an implementation of " + typeof(CaseExecutionListener));
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

	  public virtual IList<FieldDeclaration> FieldDeclarations
	  {
		  get
		  {
			return fieldDeclarations;
		  }
	  }

	}

}