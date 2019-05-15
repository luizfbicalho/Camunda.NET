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
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;



	/// <summary>
	/// Parent class for all BPMN 2.0 task types such as ServiceTask, ScriptTask, UserTask, etc.
	/// 
	/// When used on its own, it behaves just as a pass-through activity.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class TaskActivityBehavior : AbstractBpmnActivityBehavior
	{

	  /// <summary>
	  /// Activity instance id before execution.
	  /// </summary>
	  protected internal string activityInstanceId;

	  /// <summary>
	  /// The method which will be called before the execution is performed.
	  /// </summary>
	  /// <param name="execution"> the execution which is used during execution </param>
	  /// <exception cref="Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void preExecution(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  protected internal virtual void preExecution(ActivityExecution execution)
	  {
		activityInstanceId = execution.ActivityInstanceId;
	  }

	  /// <summary>
	  /// The method which should be overridden by the sub classes to perform an execution.
	  /// </summary>
	  /// <param name="execution"> the execution which is used during performing the execution </param>
	  /// <exception cref="Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void performExecution(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  protected internal virtual void performExecution(ActivityExecution execution)
	  {
		leave(execution);
	  }

	  /// <summary>
	  /// The method which will be called after performing the execution.
	  /// </summary>
	  /// <param name="execution"> the execution </param>
	  /// <exception cref="Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void postExecution(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  protected internal virtual void postExecution(ActivityExecution execution)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		performExecution(execution);
	  }



	}

}