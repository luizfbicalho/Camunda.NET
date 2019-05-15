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
namespace org.camunda.bpm.engine.cdi
{

	using ExecutionId = org.camunda.bpm.engine.cdi.annotation.ExecutionId;
	using ProcessInstanceId = org.camunda.bpm.engine.cdi.annotation.ProcessInstanceId;
	using TaskId = org.camunda.bpm.engine.cdi.annotation.TaskId;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// Allows to access executions and tasks of a managed process instance via
	/// dependency injection. A process instance can be managed, using the
	/// <seealso cref="BusinessProcess"/>-bean.
	/// 
	/// The producer methods provided by this class have been extracted from the
	/// <seealso cref="BusinessProcess"/>-bean in order to allow for specializing it.
	/// 
	/// @author Falko Menge
	/// </summary>
	public class CurrentProcessInstance
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private BusinessProcess businessProcess;
	  private BusinessProcess businessProcess;

	  /// <summary>
	  /// Returns the <seealso cref="ProcessInstance"/> currently associated or 'null'
	  /// </summary>
	  /// <exception cref="ProcessEngineCdiException">
	  ///           if no <seealso cref="Execution"/> is associated. Use
	  ///           <seealso cref="BusinessProcess#isAssociated()"/> to check whether an
	  ///           association exists. </exception>
	  /* Makes the current ProcessInstance available for injection */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @Typed(org.camunda.bpm.engine.runtime.ProcessInstance.class) public org.camunda.bpm.engine.runtime.ProcessInstance getProcessInstance()
	  public virtual ProcessInstance ProcessInstance
	  {
		  get
		  {
			return businessProcess.ProcessInstance;
		  }
	  }

	  /// <summary>
	  /// Returns the id of the currently associated process instance or 'null'
	  /// </summary>
	  /* Makes the processId available for injection */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ProcessInstanceId public String getProcessInstanceId()
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return businessProcess.ProcessInstanceId;
		  }
	  }

	  /// <summary>
	  /// Returns the currently associated execution or 'null'
	  /// </summary>
	  /* Makes the current Execution available for injection */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named public org.camunda.bpm.engine.runtime.Execution getExecution()
	  public virtual Execution Execution
	  {
		  get
		  {
			return businessProcess.Execution;
		  }
	  }

	  /// <seealso cref= BusinessProcess#getExecution() </seealso>
	  /* Makes the id of the current Execution available for injection */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @ExecutionId public String getExecutionId()
	  public virtual string ExecutionId
	  {
		  get
		  {
			return businessProcess.ExecutionId;
		  }
	  }

	  /// <summary>
	  /// Returns the currently associated <seealso cref="Task"/> or 'null'
	  /// </summary>
	  /// <exception cref="ProcessEngineCdiException">
	  ///           if no <seealso cref="Task"/> is associated. Use
	  ///           <seealso cref="BusinessProcess#isTaskAssociated()"/> to check whether an
	  ///           association exists. </exception>
	  /* Makes the current Task available for injection */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named public org.camunda.bpm.engine.task.Task getTask()
	  public virtual Task Task
	  {
		  get
		  {
			return businessProcess.Task;
		  }
	  }

	  /// <summary>
	  /// Returns the id of the task associated with the current conversation or
	  /// 'null'.
	  /// </summary>
	  /* Makes the taskId available for injection */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named @TaskId public String getTaskId()
	  public virtual string TaskId
	  {
		  get
		  {
			return businessProcess.TaskId;
		  }
	  }

	}

}