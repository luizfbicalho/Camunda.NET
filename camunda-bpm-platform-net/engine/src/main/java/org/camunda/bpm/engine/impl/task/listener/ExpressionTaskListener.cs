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
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ExpressionTaskListener : TaskListener
	{

	  protected internal Expression expression;

	  public ExpressionTaskListener(Expression expression)
	  {
		this.expression = expression;
	  }

	  public virtual void notify(DelegateTask delegateTask)
	  {
		expression.getValue(delegateTask);
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

	}

}