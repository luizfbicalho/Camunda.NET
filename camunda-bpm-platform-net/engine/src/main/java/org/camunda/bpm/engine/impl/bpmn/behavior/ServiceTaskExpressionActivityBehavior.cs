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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// ActivityBehavior that evaluates an expression when executed. Optionally, it
	/// sets the result of the expression as a variable on the execution.
	/// 
	/// @author Tom Baeyens
	/// @author Christian Stettler
	/// @author Frederik Heremans
	/// @author Slawomir Wojtasiak (Patch for ACT-1159)
	/// @author Falko Menge
	/// </summary>
	public class ServiceTaskExpressionActivityBehavior : TaskActivityBehavior
	{

	  protected internal Expression expression;
	  protected internal string resultVariable;

	  public ServiceTaskExpressionActivityBehavior(Expression expression, string resultVariable)
	  {
		this.expression = expression;
		this.resultVariable = resultVariable;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void performExecution(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public override void performExecution(ActivityExecution execution)
	  {
		executeWithErrorPropagation(execution, new CallableAnonymousInnerClass(this, execution));

	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly ServiceTaskExpressionActivityBehavior outerInstance;

		  private ActivityExecution execution;

		  public CallableAnonymousInnerClass(ServiceTaskExpressionActivityBehavior outerInstance, ActivityExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			//getValue() can have side-effects, that's why we have to call it independently from the result variable
			object value = outerInstance.expression.getValue(execution);
			if (!string.ReferenceEquals(outerInstance.resultVariable, null))
			{
			  execution.setVariable(outerInstance.resultVariable, value);
			}
			outerInstance.leave(execution);
			return null;
		  }
	  }
	}

}