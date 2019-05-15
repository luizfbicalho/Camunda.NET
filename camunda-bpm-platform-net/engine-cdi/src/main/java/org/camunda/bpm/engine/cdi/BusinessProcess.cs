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
namespace org.camunda.bpm.engine.cdi
{


	using BusinessProcessScoped = org.camunda.bpm.engine.cdi.annotation.BusinessProcessScoped;
	using ContextAssociationManager = org.camunda.bpm.engine.cdi.impl.context.ContextAssociationManager;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Bean supporting contextual business process management. This allows us to
	/// implement a unit of work, in which a particular CDI scope (Conversation /
	/// Request / Thread) is associated with a particular Execution / ProcessInstance
	/// or Task.
	/// <p />
	/// The protocol is that we <em>associate</em> the <seealso cref="BusinessProcess"/> bean
	/// with a particular Execution / Task, then perform some changes (retrieve / set process
	/// variables) and then end the unit of work. This bean makes sure that our changes are
	/// only "flushed" to the process engine when we successfully complete the unit of work.
	/// <p />
	/// A typical usage scenario might look like this:<br />
	/// <strong>1st unit of work ("process instantiation"):</strong>
	/// <pre>
	/// conversation.begin();
	/// ...
	/// businessProcess.setVariable("billingId", "1"); // setting variables before starting the process
	/// businessProcess.startProcessByKey("billingProcess");
	/// conversation.end();
	/// </pre>
	/// <strong>2nd unit of work ("perform a user task"):</strong>
	/// <pre>
	/// conversation.begin();
	/// businessProcess.startTask(id); // now we have associated a task with the current conversation
	/// ...                            // this allows us to retrieve and change process variables
	///                                // and @BusinessProcessScoped beans
	/// businessProcess.setVariable("billingDetails", "someValue"); // these changes are cached in the conversation
	/// ...
	/// businessProcess.completeTask(); // now all changed process variables are flushed
	/// conversation.end();
	/// </pre>
	/// <p />
	/// <strong>NOTE:</strong> in the absence of a conversation, (non faces request, i.e. when processing a JAX-RS,
	/// JAX-WS, JMS, remote EJB or plain Servlet requests), the <seealso cref="BusinessProcess"/> bean associates with the
	/// current Request (see <seealso cref="RequestScoped @RequestScoped"/>).
	/// <p />
	/// <strong>NOTE:</strong> in the absence of a request, ie. when the JobExecutor accesses
	/// <seealso cref="BusinessProcessScoped @BusinessProcessScoped"/> beans, the execution is associated with the
	/// current thread.
	/// 
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public class BusinessProcess implements java.io.Serializable
	[Serializable]
	public class BusinessProcess
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.ProcessEngine processEngine;
	  private ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.cdi.impl.context.ContextAssociationManager associationManager;
	  private ContextAssociationManager associationManager;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private javax.enterprise.inject.Instance<javax.enterprise.context.Conversation> conversationInstance;
	  private Instance<Conversation> conversationInstance;

	  public virtual ProcessInstance startProcessById(string processDefinitionId)
	  {
		assertCommandContextNotActive();

		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceById(processDefinitionId, AndClearCachedVariableMap);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessById(string processDefinitionId, string businessKey)
	  {
		assertCommandContextNotActive();

		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceById(processDefinitionId, businessKey, AndClearCachedVariableMap);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessById(string processDefinitionId, IDictionary<string, object> variables)
	  {
		assertCommandContextNotActive();

		VariableMap cachedVariables = AndClearCachedVariableMap;
		cachedVariables.putAll(variables);
		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceById(processDefinitionId, cachedVariables);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessById(string processDefinitionId, string businessKey, IDictionary<string, object> variables)
	  {
		assertCommandContextNotActive();

		VariableMap cachedVariables = AndClearCachedVariableMap;
		cachedVariables.putAll(variables);
		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceById(processDefinitionId, businessKey, cachedVariables);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessByKey(string key)
	  {
		assertCommandContextNotActive();

		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceByKey(key, AndClearCachedVariableMap);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessByKey(string key, string businessKey)
	  {
		assertCommandContextNotActive();

		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceByKey(key, businessKey, AndClearCachedVariableMap);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessByKey(string key, IDictionary<string, object> variables)
	  {
		assertCommandContextNotActive();

		VariableMap cachedVariables = AndClearCachedVariableMap;
		cachedVariables.putAll(variables);
		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceByKey(key, cachedVariables);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessByKey(string key, string businessKey, IDictionary<string, object> variables)
	  {
		assertCommandContextNotActive();

		VariableMap cachedVariables = AndClearCachedVariableMap;
		cachedVariables.putAll(variables);
		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceByKey(key, businessKey, cachedVariables);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessByMessage(string messageName)
	  {
		assertCommandContextNotActive();

		VariableMap cachedVariables = AndClearCachedVariableMap;
		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceByMessage(messageName, cachedVariables);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessByMessage(string messageName, IDictionary<string, object> processVariables)
	  {
		assertCommandContextNotActive();

		VariableMap cachedVariables = AndClearCachedVariableMap;
		cachedVariables.putAll(processVariables);
		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceByMessage(messageName, cachedVariables);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }

	  public virtual ProcessInstance startProcessByMessage(string messageName, string businessKey, IDictionary<string, object> processVariables)
	  {
		assertCommandContextNotActive();

		VariableMap cachedVariables = AndClearCachedVariableMap;
		cachedVariables.putAll(processVariables);
		ProcessInstance instance = processEngine.RuntimeService.startProcessInstanceByMessage(messageName, businessKey, cachedVariables);
		if (!instance.Ended)
		{
		  Execution = instance;
		}
		return instance;
	  }


	  /// <summary>
	  /// Associate with the provided execution. This starts a unit of work.
	  /// </summary>
	  /// <param name="executionId">
	  ///          the id of the execution to associate with.
	  /// @throw ProcessEngineCdiException
	  ///          if no such execution exists </param>
	  public virtual void associateExecutionById(string executionId)
	  {
		Execution execution = processEngine.RuntimeService.createExecutionQuery().executionId(executionId).singleResult();
		if (execution == null)
		{
		  throw new ProcessEngineCdiException("Cannot associate execution by id: no execution with id '" + executionId + "' found.");
		}
		associationManager.Execution = execution;
	  }

	  /// <summary>
	  /// returns true if an <seealso cref="Execution"/> is associated.
	  /// </summary>
	  /// <seealso cref= #associateExecutionById(String) </seealso>
	  public virtual bool Associated
	  {
		  get
		  {
			return !string.ReferenceEquals(associationManager.ExecutionId, null);
		  }
	  }

	  /// <summary>
	  /// Signals the current execution, see <seealso cref="RuntimeService#signal(String)"/>
	  /// <p/>
	  /// Ends the current unit of work (flushes changes to process variables set
	  /// using <seealso cref="#setVariable(String, Object)"/> or made on
	  /// <seealso cref="BusinessProcessScoped @BusinessProcessScoped"/> beans).
	  /// </summary>
	  /// <exception cref="ProcessEngineCdiException">
	  ///           if no execution is currently associated </exception>
	  /// <exception cref="ProcessEngineException">
	  ///           if the activiti command fails </exception>
	  public virtual void signalExecution()
	  {
		assertExecutionAssociated();
		processEngine.RuntimeService.setVariablesLocal(associationManager.ExecutionId, AndClearCachedLocalVariableMap);
		processEngine.RuntimeService.signal(associationManager.ExecutionId, AndClearCachedVariableMap);
		associationManager.disAssociate();
	  }

	  /// <seealso cref= #signalExecution()
	  /// 
	  /// In addition, this method allows to end the current conversation </seealso>
	  public virtual void signalExecution(bool endConversation)
	  {
		signalExecution();
		if (endConversation)
		{
		  conversationInstance.get().end();
		}
	  }

	  // -------------------------------------

	  /// <summary>
	  /// Associates the task with the provided taskId with the current conversation.
	  /// <p/>
	  /// </summary>
	  /// <param name="taskId">
	  ///          the id of the task
	  /// </param>
	  /// <returns> the resumed task
	  /// </returns>
	  /// <exception cref="ProcessEngineCdiException">
	  ///           if no such task is found </exception>
	  public virtual Task startTask(string taskId)
	  {
		Task currentTask = associationManager.Task;
		if (currentTask != null && currentTask.Id.Equals(taskId))
		{
		  return currentTask;
		}
		Task task = processEngine.TaskService.createTaskQuery().taskId(taskId).singleResult();
		if (task == null)
		{
		  throw new ProcessEngineCdiException("Cannot resume task with id '" + taskId + "', no such task.");
		}
		associationManager.Task = task;
		associateExecutionById(task.ExecutionId);
		return task;
	  }

	  /// <seealso cref= #startTask(String)
	  /// 
	  /// this method allows to start a conversation if no conversation is active </seealso>
	  public virtual Task startTask(string taskId, bool beginConversation)
	  {
		if (beginConversation)
		{
		  Conversation conversation = conversationInstance.get();
		  if (conversation.Transient)
		  {
		   conversation.begin();
		  }
		}
		return startTask(taskId);
	  }

	  /// <summary>
	  /// Completes the current UserTask, see <seealso cref="TaskService#complete(String)"/>
	  /// <p/>
	  /// Ends the current unit of work (flushes changes to process variables set
	  /// using <seealso cref="#setVariable(String, Object)"/> or made on
	  /// <seealso cref="BusinessProcessScoped @BusinessProcessScoped"/> beans).
	  /// </summary>
	  /// <exception cref="ProcessEngineCdiException">
	  ///           if no task is currently associated </exception>
	  /// <exception cref="ProcessEngineException">
	  ///           if the activiti command fails </exception>
	  public virtual void completeTask()
	  {
		assertTaskAssociated();
		processEngine.TaskService.setVariablesLocal(Task.Id, AndClearCachedLocalVariableMap);
		processEngine.TaskService.setVariables(Task.Id, AndClearCachedVariableMap);
		processEngine.TaskService.complete(Task.Id);
		associationManager.disAssociate();
	  }

	  /// <seealso cref= BusinessProcess#completeTask()
	  /// 
	  /// In addition this allows to end the current conversation.
	  ///  </seealso>
	  public virtual void completeTask(bool endConversation)
	  {
		completeTask();
		if (endConversation)
		{
		  conversationInstance.get().end();
		}
	  }

	  public virtual bool TaskAssociated
	  {
		  get
		  {
			return associationManager.Task != null;
		  }
	  }

	  /// <summary>
	  /// Save the currently associated task.
	  /// </summary>
	  /// <exception cref="ProcessEngineCdiException"> if called from a process engine command or if no Task is currently associated.
	  ///  </exception>
	  public virtual void saveTask()
	  {
		assertCommandContextNotActive();
		assertTaskAssociated();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = getTask();
		Task task = Task;
		// save the task
		processEngine.TaskService.saveTask(task);
	  }

	  /// <summary>
	  /// <para>Stop working on a task. Clears the current association.</para>
	  /// 
	  /// <para>NOTE: this method does not flush any changes.</para>
	  /// <ul>
	  ///  <li>If you want to flush changes to process variables, call <seealso cref="#flushVariableCache()"/> prior to calling this method,</li>
	  ///  <li>If you need to flush changes to the task object, use <seealso cref="#saveTask()"/> prior to calling this method.</li>
	  /// </ul>
	  /// </summary>
	  /// <exception cref="ProcessEngineCdiException"> if called from a process engine command or if no Task is currently associated. </exception>
	  public virtual void stopTask()
	  {
		assertCommandContextNotActive();
		assertTaskAssociated();
		associationManager.disAssociate();
	  }

	  /// <summary>
	  /// <para>Stop working on a task. Clears the current association.</para>
	  /// 
	  /// <para>NOTE: this method does not flush any changes.</para>
	  /// <ul>
	  ///  <li>If you want to flush changes to process variables, call <seealso cref="#flushVariableCache()"/> prior to calling this method,</li>
	  ///  <li>If you need to flush changes to the task object, use <seealso cref="#saveTask()"/> prior to calling this method.</li>
	  /// </ul>
	  /// 
	  /// <para>This method allows you to optionally end the current conversation</para>
	  /// </summary>
	  /// <param name="endConversation"> if true, end current conversation. </param>
	  /// <exception cref="ProcessEngineCdiException"> if called from a process engine command or if no Task is currently associated. </exception>
	  public virtual void stopTask(bool endConversation)
	  {
		stopTask();
		if (endConversation)
		{
		  conversationInstance.get().end();
		}
	  }

	  // -------------------------------------------------

	  /// <param name="variableName">
	  ///          the name of the process variable for which the value is to be
	  ///          retrieved </param>
	  /// <returns> the value of the provided process variable or 'null' if no such
	  ///         variable is set </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T getVariable(String variableName)
	  public virtual T getVariable<T>(string variableName)
	  {
		TypedValue variable = getVariableTyped(variableName);
		if (variable != null)
		{
		  object value = variable.Value;
		  if (value != null)
		  {
			return (T) value;
		  }
		}
		return null;
	  }

	  /// <param name="variableName">
	  ///          the name of the process variable for which the value is to be
	  ///          retrieved </param>
	  /// <returns> the typed value of the provided process variable or 'null' if no
	  ///         such variable is set
	  /// 
	  /// @since 7.3 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getVariableTyped(String variableName)
	  public virtual T getVariableTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		TypedValue variable = associationManager.getVariable(variableName);
		return variable != null ? (T)(variable) : null;
	  }

	  /// <summary>
	  /// Set a value for a process variable.
	  /// <p />
	  /// 
	  /// <strong>NOTE:</strong> If no execution is currently associated,
	  /// the value is temporarily cached and flushed to the process instance
	  /// at the end of the unit of work
	  /// </summary>
	  /// <param name="variableName">
	  ///          the name of the process variable for which a value is to be set </param>
	  /// <param name="value">
	  ///          the value to be set
	  ///  </param>
	  public virtual void setVariable(string variableName, object value)
	  {
		associationManager.setVariable(variableName, value);
	  }

	  /// <summary>
	  /// Get the <seealso cref="VariableMap"/> of cached variables and clear the internal variable cache.
	  /// </summary>
	  /// <returns> the <seealso cref="VariableMap"/> of cached variables
	  /// 
	  /// @since 7.3 </returns>
	  public virtual VariableMap AndClearCachedVariableMap
	  {
		  get
		  {
			VariableMap cachedVariables = associationManager.CachedVariables;
			VariableMap copy = new VariableMapImpl(cachedVariables);
			cachedVariables.clear();
			return copy;
		  }
	  }

	  /// <summary>
	  /// Get the map of cached variables and clear the internal variable cache.
	  /// </summary>
	  /// <returns> the map of cached variables </returns>
	  /// @deprecated use <seealso cref="#getAndClearCachedVariableMap()"/> instead 
	  [Obsolete("use <seealso cref="#getAndClearCachedVariableMap()"/> instead")]
	  public virtual IDictionary<string, object> AndClearVariableCache
	  {
		  get
		  {
			return AndClearCachedVariableMap;
		  }
	  }

	  /// <summary>
	  /// Get a copy of the <seealso cref="VariableMap"/> of cached variables.
	  /// </summary>
	  /// <returns> a copy of the <seealso cref="VariableMap"/> of cached variables.
	  /// 
	  /// @since 7.3 </returns>
	  public virtual VariableMap CachedVariableMap
	  {
		  get
		  {
			return new VariableMapImpl(associationManager.CachedVariables);
		  }
	  }

	  /// <summary>
	  /// Get a copy of the map of cached variables.
	  /// </summary>
	  /// <returns> a copy of the map of cached variables. </returns>
	  /// @deprecated use <seealso cref="#getCachedVariableMap()"/> instead 
	  [Obsolete("use <seealso cref="#getCachedVariableMap()"/> instead")]
	  public virtual IDictionary<string, object> VariableCache
	  {
		  get
		  {
			return CachedVariableMap;
		  }
	  }

	  /// <param name="variableName">
	  ///          the name of the local process variable for which the value is to be
	  ///          retrieved </param>
	  /// <returns> the value of the provided local process variable or 'null' if no such
	  ///         variable is set </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T getVariableLocal(String variableName)
	  public virtual T getVariableLocal<T>(string variableName)
	  {
		TypedValue variable = getVariableLocalTyped(variableName);
		if (variable != null)
		{
		  object value = variable.Value;
		  if (value != null)
		  {
			return (T) value;
		  }
		}
		return null;
	  }

	  /// <param name="variableName">
	  ///          the name of the local process variable for which the value is to
	  ///          be retrieved </param>
	  /// <returns> the typed value of the provided local process variable or 'null' if
	  ///         no such variable is set
	  /// 
	  /// @since 7.3 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getVariableLocalTyped(String variableName)
	  public virtual T getVariableLocalTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		TypedValue variable = associationManager.getVariableLocal(variableName);
		return variable != null ? (T) variable : null;
	  }

	  /// <summary>
	  /// Set a value for a local process variable.
	  /// <p />
	  /// 
	  /// <strong>NOTE:</strong> If a task or execution is currently associated,
	  /// the value is temporarily cached and flushed to the process instance
	  /// at the end of the unit of work - otherwise an Exception will be thrown
	  /// </summary>
	  /// <param name="variableName">
	  ///          the name of the local process variable for which a value is to be set </param>
	  /// <param name="value">
	  ///          the value to be set
	  ///  </param>
	  public virtual void setVariableLocal(string variableName, object value)
	  {
		associationManager.setVariableLocal(variableName, value);
	  }

	  /// <summary>
	  /// Get the <seealso cref="VariableMap"/> of local cached variables and clear the internal variable cache.
	  /// </summary>
	  /// <returns> the <seealso cref="VariableMap"/> of cached variables
	  /// 
	  /// @since 7.3 </returns>
	  public virtual VariableMap AndClearCachedLocalVariableMap
	  {
		  get
		  {
			VariableMap cachedVariablesLocal = associationManager.CachedLocalVariables;
			VariableMap copy = new VariableMapImpl(cachedVariablesLocal);
			cachedVariablesLocal.clear();
			return copy;
		  }
	  }

	  /// <summary>
	  /// Get the map of local cached variables and clear the internal variable cache.
	  /// </summary>
	  /// <returns> the map of cached variables </returns>
	  /// @deprecated use <seealso cref="#getAndClearCachedLocalVariableMap()"/> instead 
	  [Obsolete("use <seealso cref="#getAndClearCachedLocalVariableMap()"/> instead")]
	  public virtual IDictionary<string, object> AndClearVariableLocalCache
	  {
		  get
		  {
			return AndClearCachedLocalVariableMap;
		  }
	  }

	  /// <summary>
	  /// Get a copy of the <seealso cref="VariableMap"/> of local cached variables.
	  /// </summary>
	  /// <returns> a copy of the <seealso cref="VariableMap"/> of local cached variables.
	  /// 
	  /// @since 7.3 </returns>
	  public virtual VariableMap CachedLocalVariableMap
	  {
		  get
		  {
			return new VariableMapImpl(associationManager.CachedLocalVariables);
		  }
	  }

	  /// <summary>
	  /// Get a copy of the map of local cached variables.
	  /// </summary>
	  /// <returns> a copy of the map of local cached variables. </returns>
	  /// @deprecated use <seealso cref="#getCachedLocalVariableMap()"/> instead 
	  [Obsolete("use <seealso cref="#getCachedLocalVariableMap()"/> instead")]
	  public virtual IDictionary<string, object> VariableLocalCache
	  {
		  get
		  {
			return CachedLocalVariableMap;
		  }
	  }

	  /// <summary>
	  /// <para>This method allows to flush the cached variables to the Task or Execution.<p>
	  /// 
	  /// <ul>
	  ///   <li>If a Task instance is currently associated,
	  ///       the variables will be flushed using <seealso cref="TaskService#setVariables(String, Map)"/></li>
	  ///   <li>If an Execution instance is currently associated,
	  ///       the variables will be flushed using <seealso cref="RuntimeService#setVariables(String, Map)"/></li>
	  ///   <li>If neither a Task nor an Execution is currently associated,
	  ///       ProcessEngineCdiException is thrown.</li>
	  /// </ul>
	  /// 
	  /// </para>
	  /// <para>A successful invocation of this method will empty the variable cache.</para>
	  /// 
	  /// <para>If this method is called from an active command (ie. from inside a Java Delegate).
	  /// <seealso cref="ProcessEngineCdiException"/> is thrown.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineCdiException"> if called from a process engine command or if neither a Task nor an Execution is associated. </exception>
	  public virtual void flushVariableCache()
	  {
		associationManager.flushVariableCache();
	  }

	  // ----------------------------------- Getters / Setters

	  /*
	   * Note that Producers should go into {@link CurrentProcessInstance} in
	   * order to allow for specializing {@link BusinessProcess}.
	   */

	  /// <seealso cref= #startTask(String) </seealso>
	  public virtual Task Task
	  {
		  set
		  {
			startTask(value.Id);
		  }
		  get
		  {
			return associationManager.Task;
		  }
	  }

	  /// <seealso cref= #startTask(String) </seealso>
	  public virtual string TaskId
	  {
		  set
		  {
			startTask(value);
		  }
		  get
		  {
			Task task = Task;
			return task != null ? task.Id : null;
		  }
	  }

	  /// <seealso cref= #associateExecutionById(String) </seealso>
	  public virtual Execution Execution
	  {
		  set
		  {
			associateExecutionById(value.Id);
		  }
		  get
		  {
			return associationManager.Execution;
		  }
	  }

	  /// <seealso cref= #associateExecutionById(String) </seealso>
	  protected internal virtual string ExecutionId
	  {
		  set
		  {
			associateExecutionById(value);
		  }
		  get
		  {
			Execution e = Execution;
			return e != null ? e.Id : null;
		  }
	  }

	  /// <summary>
	  /// Returns the id of the currently associated process instance or 'null'
	  /// </summary>
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			Execution execution = associationManager.Execution;
			return execution != null ? execution.ProcessInstanceId : null;
		  }
	  }





	  /// <summary>
	  /// Returns the <seealso cref="ProcessInstance"/> currently associated or 'null'
	  /// </summary>
	  /// <exception cref="ProcessEngineCdiException">
	  ///           if no <seealso cref="Execution"/> is associated. Use
	  ///           <seealso cref="#isAssociated()"/> to check whether an association exists. </exception>
	  public virtual ProcessInstance ProcessInstance
	  {
		  get
		  {
			Execution execution = Execution;
			if (execution != null && !(execution.ProcessInstanceId.Equals(execution.Id)))
			{
			  return processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(execution.ProcessInstanceId).singleResult();
			}
			return (ProcessInstance) execution;
		  }
	  }

	  // internal implementation //////////////////////////////////////////////////////////

	  protected internal virtual void assertExecutionAssociated()
	  {
		if (associationManager.Execution == null)
		{
		  throw new ProcessEngineCdiException("No execution associated. Call busniessProcess.associateExecutionById() or businessProcess.startTask() first.");
		}
	  }

	  protected internal virtual void assertTaskAssociated()
	  {
		if (associationManager.Task == null)
		{
		  throw new ProcessEngineCdiException("No task associated. Call businessProcess.startTask() first.");
		}
	  }

	  protected internal virtual void assertCommandContextNotActive()
	  {
		if (Context.CommandContext != null)
		{
		  throw new ProcessEngineCdiException("Cannot use this method of the BusinessProcess bean from an active command context.");
		}
	  }

	}

}