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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using FormData = org.camunda.bpm.engine.form.FormData;
	using CreateIncidentCmd = org.camunda.bpm.engine.impl.cmd.CreateIncidentCmd;
	using DeleteProcessInstanceCmd = org.camunda.bpm.engine.impl.cmd.DeleteProcessInstanceCmd;
	using DeleteProcessInstancesCmd = org.camunda.bpm.engine.impl.cmd.DeleteProcessInstancesCmd;
	using FindActiveActivityIdsCmd = org.camunda.bpm.engine.impl.cmd.FindActiveActivityIdsCmd;
	using GetActivityInstanceCmd = org.camunda.bpm.engine.impl.cmd.GetActivityInstanceCmd;
	using GetExecutionVariableCmd = org.camunda.bpm.engine.impl.cmd.GetExecutionVariableCmd;
	using GetExecutionVariableTypedCmd = org.camunda.bpm.engine.impl.cmd.GetExecutionVariableTypedCmd;
	using GetExecutionVariablesCmd = org.camunda.bpm.engine.impl.cmd.GetExecutionVariablesCmd;
	using GetStartFormCmd = org.camunda.bpm.engine.impl.cmd.GetStartFormCmd;
	using MessageEventReceivedCmd = org.camunda.bpm.engine.impl.cmd.MessageEventReceivedCmd;
	using PatchExecutionVariablesCmd = org.camunda.bpm.engine.impl.cmd.PatchExecutionVariablesCmd;
	using RemoveExecutionVariablesCmd = org.camunda.bpm.engine.impl.cmd.RemoveExecutionVariablesCmd;
	using ResolveIncidentCmd = org.camunda.bpm.engine.impl.cmd.ResolveIncidentCmd;
	using SetExecutionVariablesCmd = org.camunda.bpm.engine.impl.cmd.SetExecutionVariablesCmd;
	using SignalCmd = org.camunda.bpm.engine.impl.cmd.SignalCmd;
	using DeleteProcessInstanceBatchCmd = org.camunda.bpm.engine.impl.cmd.batch.DeleteProcessInstanceBatchCmd;
	using MigrationPlanBuilderImpl = org.camunda.bpm.engine.impl.migration.MigrationPlanBuilderImpl;
	using MigrationPlanExecutionBuilderImpl = org.camunda.bpm.engine.impl.migration.MigrationPlanExecutionBuilderImpl;
	using UpdateProcessInstanceSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.runtime.UpdateProcessInstanceSuspensionStateBuilderImpl;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanBuilder = org.camunda.bpm.engine.migration.MigrationPlanBuilder;
	using MigrationPlanExecutionBuilder = org.camunda.bpm.engine.migration.MigrationPlanExecutionBuilder;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ConditionEvaluationBuilder = org.camunda.bpm.engine.runtime.ConditionEvaluationBuilder;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;
	using MessageCorrelationBuilder = org.camunda.bpm.engine.runtime.MessageCorrelationBuilder;
	using ModificationBuilder = org.camunda.bpm.engine.runtime.ModificationBuilder;
	using NativeExecutionQuery = org.camunda.bpm.engine.runtime.NativeExecutionQuery;
	using NativeProcessInstanceQuery = org.camunda.bpm.engine.runtime.NativeProcessInstanceQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceModificationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationBuilder;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProcessInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder;
	using RestartProcessInstanceBuilder = org.camunda.bpm.engine.runtime.RestartProcessInstanceBuilder;
	using SignalEventReceivedBuilder = org.camunda.bpm.engine.runtime.SignalEventReceivedBuilder;
	using UpdateProcessInstanceSuspensionStateSelectBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstanceSuspensionStateSelectBuilder;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class RuntimeServiceImpl : ServiceImpl, RuntimeService
	{

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey)
	  {
		return createProcessInstanceByKey(processDefinitionKey).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey)
	  {
		return createProcessInstanceByKey(processDefinitionKey).businessKey(businessKey).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey, string caseInstanceId)
	  {
		return createProcessInstanceByKey(processDefinitionKey).businessKey(businessKey).caseInstanceId(caseInstanceId).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, IDictionary<string, object> variables)
	  {
		return createProcessInstanceByKey(processDefinitionKey).setVariables(variables).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey, IDictionary<string, object> variables)
	  {
		return createProcessInstanceByKey(processDefinitionKey).businessKey(businessKey).setVariables(variables).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey, string caseInstanceId, IDictionary<string, object> variables)
	  {
		return createProcessInstanceByKey(processDefinitionKey).businessKey(businessKey).caseInstanceId(caseInstanceId).setVariables(variables).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId)
	  {
		return createProcessInstanceById(processDefinitionId).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey)
	  {
		return createProcessInstanceById(processDefinitionId).businessKey(businessKey).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey, string caseInstanceId)
	  {
		return createProcessInstanceById(processDefinitionId).businessKey(businessKey).caseInstanceId(caseInstanceId).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId, IDictionary<string, object> variables)
	  {
		return createProcessInstanceById(processDefinitionId).setVariables(variables).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey, IDictionary<string, object> variables)
	  {
		return createProcessInstanceById(processDefinitionId).businessKey(businessKey).setVariables(variables).execute();
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey, string caseInstanceId, IDictionary<string, object> variables)
	  {
		return createProcessInstanceById(processDefinitionId).businessKey(businessKey).caseInstanceId(caseInstanceId).setVariables(variables).execute();
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason)
	  {
		deleteProcessInstance(processInstanceId,deleteReason,false);
	  }

	  public virtual Batch deleteProcessInstancesAsync(IList<string> processInstanceIds, ProcessInstanceQuery processInstanceQuery, string deleteReason)
	  {
		return deleteProcessInstancesAsync(processInstanceIds, processInstanceQuery, deleteReason, false);
	  }

	  public virtual Batch deleteProcessInstancesAsync(IList<string> processInstanceIds, string deleteReason)
	  {
		return deleteProcessInstancesAsync(processInstanceIds, null, deleteReason, false);
	  }

	  public virtual Batch deleteProcessInstancesAsync(ProcessInstanceQuery processInstanceQuery, string deleteReason)
	  {
		return deleteProcessInstancesAsync(null, processInstanceQuery, deleteReason, false);
	  }

	  public virtual Batch deleteProcessInstancesAsync(IList<string> processInstanceIds, ProcessInstanceQuery processInstanceQuery, string deleteReason, bool skipCustomListeners)
	  {
		return deleteProcessInstancesAsync(processInstanceIds, processInstanceQuery, deleteReason, skipCustomListeners, false);
	  }

	  public virtual Batch deleteProcessInstancesAsync(IList<string> processInstanceIds, ProcessInstanceQuery processInstanceQuery, string deleteReason, bool skipCustomListeners, bool skipSubprocesses)
	  {
		return commandExecutor.execute(new DeleteProcessInstanceBatchCmd(processInstanceIds, processInstanceQuery, deleteReason, skipCustomListeners, skipSubprocesses));
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason, bool skipCustomListeners)
	  {
		deleteProcessInstance(processInstanceId,deleteReason,skipCustomListeners,false);
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated)
	  {
		deleteProcessInstance(processInstanceId, deleteReason, skipCustomListeners, externallyTerminated, false);
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipIoMappings)
	  {
		deleteProcessInstance(processInstanceId, deleteReason, skipCustomListeners, externallyTerminated, skipIoMappings, false);
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipIoMappings, bool skipSubprocesses)
	  {
		commandExecutor.execute(new DeleteProcessInstanceCmd(processInstanceId, deleteReason, skipCustomListeners, externallyTerminated, skipIoMappings, skipSubprocesses, true));
	  }

	  public virtual void deleteProcessInstanceIfExists(string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipIoMappings, bool skipSubprocesses)
	  {
		commandExecutor.execute(new DeleteProcessInstanceCmd(processInstanceId, deleteReason, skipCustomListeners, externallyTerminated, skipIoMappings, skipSubprocesses, false));
	  }

	  public virtual void deleteProcessInstances(IList<string> processInstanceIds, string deleteReason, bool skipCustomListeners, bool externallyTerminated)
	  {
		deleteProcessInstances(processInstanceIds, deleteReason, skipCustomListeners, externallyTerminated, false);
	  }

	  public virtual void deleteProcessInstances(IList<string> processInstanceIds, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipSubprocesses)
	  {
		commandExecutor.execute(new DeleteProcessInstancesCmd(processInstanceIds, deleteReason, skipCustomListeners, externallyTerminated, skipSubprocesses, true));
	  }

	  public virtual void deleteProcessInstancesIfExists(IList<string> processInstanceIds, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipSubprocesses)
	  {
		commandExecutor.execute(new DeleteProcessInstancesCmd(processInstanceIds, deleteReason, skipCustomListeners, externallyTerminated, skipSubprocesses, false));
	  }

	  public virtual ExecutionQuery createExecutionQuery()
	  {
		return new ExecutionQueryImpl(commandExecutor);
	  }

	  public virtual NativeExecutionQuery createNativeExecutionQuery()
	  {
		return new NativeExecutionQueryImpl(commandExecutor);
	  }

	  public virtual NativeProcessInstanceQuery createNativeProcessInstanceQuery()
	  {
		return new NativeProcessInstanceQueryImpl(commandExecutor);
	  }

	  public virtual IncidentQuery createIncidentQuery()
	  {
		return new IncidentQueryImpl(commandExecutor);
	  }


	  public virtual EventSubscriptionQuery createEventSubscriptionQuery()
	  {
		return new EventSubscriptionQueryImpl(commandExecutor);
	  }

	  public virtual VariableInstanceQuery createVariableInstanceQuery()
	  {
		return new VariableInstanceQueryImpl(commandExecutor);
	  }

	  public virtual VariableMap getVariables(string executionId)
	  {
		return getVariablesTyped(executionId);
	  }

	  public virtual VariableMap getVariablesTyped(string executionId)
	  {
		return getVariablesTyped(executionId, true);
	  }

	  public virtual VariableMap getVariablesTyped(string executionId, bool deserializeObjectValues)
	  {
		return commandExecutor.execute(new GetExecutionVariablesCmd(executionId, null, false, deserializeObjectValues));
	  }

	  public virtual VariableMap getVariablesLocal(string executionId)
	  {
		return getVariablesLocalTyped(executionId);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string executionId)
	  {
		return getVariablesLocalTyped(executionId, true);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string executionId, bool deserializeObjectValues)
	  {
		return commandExecutor.execute(new GetExecutionVariablesCmd(executionId, null, true, deserializeObjectValues));
	  }

	  public virtual VariableMap getVariables(string executionId, ICollection<string> variableNames)
	  {
		return getVariablesTyped(executionId, variableNames, true);
	  }

	  public virtual VariableMap getVariablesTyped(string executionId, ICollection<string> variableNames, bool deserializeObjectValues)
	  {
		return commandExecutor.execute(new GetExecutionVariablesCmd(executionId, variableNames, false, deserializeObjectValues));
	  }

	  public virtual VariableMap getVariablesLocal(string executionId, ICollection<string> variableNames)
	  {
		return getVariablesLocalTyped(executionId, variableNames, true);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string executionId, ICollection<string> variableNames, bool deserializeObjectValues)
	  {
		return commandExecutor.execute(new GetExecutionVariablesCmd(executionId, variableNames, true, deserializeObjectValues));
	  }

	  public virtual object getVariable(string executionId, string variableName)
	  {
		return commandExecutor.execute(new GetExecutionVariableCmd(executionId, variableName, false));
	  }

	  public virtual T getVariableTyped<T>(string executionId, string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableTyped(executionId, variableName, true);
	  }

	  public virtual T getVariableTyped<T>(string executionId, string variableName, bool deserializeObjectValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return commandExecutor.execute(new GetExecutionVariableTypedCmd<T>(executionId, variableName, false, deserializeObjectValue));
	  }

	  public virtual T getVariableLocalTyped<T>(string executionId, string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableLocalTyped(executionId, variableName, true);
	  }

	  public virtual T getVariableLocalTyped<T>(string executionId, string variableName, bool deserializeObjectValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return commandExecutor.execute(new GetExecutionVariableTypedCmd<T>(executionId, variableName, true, deserializeObjectValue));
	  }

	  public virtual object getVariableLocal(string executionId, string variableName)
	  {
		return commandExecutor.execute(new GetExecutionVariableCmd(executionId, variableName, true));
	  }

	  public virtual void setVariable(string executionId, string variableName, object value)
	  {
		ensureNotNull("variableName", variableName);
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = value;
		setVariables(executionId, variables);
	  }

	  public virtual void setVariableLocal(string executionId, string variableName, object value)
	  {
		ensureNotNull("variableName", variableName);
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = value;
		setVariablesLocal(executionId, variables);
	  }

	  public virtual void setVariables<T1>(string executionId, IDictionary<T1> variables) where T1 : object
	  {
		setVariables(executionId, variables, false);
	  }

	  public virtual void setVariablesLocal<T1>(string executionId, IDictionary<T1> variables) where T1 : object
	  {
		setVariables(executionId, variables, true);
	  }

	  protected internal virtual void setVariables<T1>(string executionId, IDictionary<T1> variables, bool local) where T1 : object
	  {
		try
		{
		  commandExecutor.execute(new SetExecutionVariablesCmd(executionId, variables, local));
		}
		catch (ProcessEngineException ex)
		{
		  if (ExceptionUtil.checkValueTooLongException(ex))
		  {
			throw new BadUserRequestException("Variable value is too long", ex);
		  }
		  throw ex;
		}
	  }

	  public virtual void removeVariable(string executionId, string variableName)
	  {
		ICollection<string> variableNames = new List<string>();
		variableNames.Add(variableName);
		commandExecutor.execute(new RemoveExecutionVariablesCmd(executionId, variableNames, false));
	  }

	  public virtual void removeVariableLocal(string executionId, string variableName)
	  {
		ICollection<string> variableNames = new List<string>();
		variableNames.Add(variableName);
		commandExecutor.execute(new RemoveExecutionVariablesCmd(executionId, variableNames, true));

	  }

	  public virtual void removeVariables(string executionId, ICollection<string> variableNames)
	  {
		commandExecutor.execute(new RemoveExecutionVariablesCmd(executionId, variableNames, false));
	  }

	  public virtual void removeVariablesLocal(string executionId, ICollection<string> variableNames)
	  {
		commandExecutor.execute(new RemoveExecutionVariablesCmd(executionId, variableNames, true));
	  }

	  public virtual void updateVariables<T1>(string executionId, IDictionary<T1> modifications, ICollection<string> deletions) where T1 : object
	  {
		updateVariables(executionId, modifications, deletions, false);
	  }

	  public virtual void updateVariablesLocal<T1>(string executionId, IDictionary<T1> modifications, ICollection<string> deletions) where T1 : object
	  {
		updateVariables(executionId, modifications, deletions, true);
	  }

	  protected internal virtual void updateVariables<T1>(string executionId, IDictionary<T1> modifications, ICollection<string> deletions, bool local) where T1 : object
	  {
		try
		{
		  commandExecutor.execute(new PatchExecutionVariablesCmd(executionId, modifications, deletions, local));
		}
		catch (ProcessEngineException ex)
		{
		  if (ExceptionUtil.checkValueTooLongException(ex))
		  {
			throw new BadUserRequestException("Variable value is too long", ex);
		  }
		  throw ex;
		}
	  }



	  public virtual void signal(string executionId)
	  {
		commandExecutor.execute(new SignalCmd(executionId, null, null, null));
	  }

	  public virtual void signal(string executionId, string signalName, object signalData, IDictionary<string, object> processVariables)
	  {
		commandExecutor.execute(new SignalCmd(executionId, signalName, signalData, processVariables));
	  }

	  public virtual void signal(string executionId, IDictionary<string, object> processVariables)
	  {
		commandExecutor.execute(new SignalCmd(executionId, null, null, processVariables));
	  }

	  public virtual ProcessInstanceQuery createProcessInstanceQuery()
	  {
		return new ProcessInstanceQueryImpl(commandExecutor);
	  }

	  public virtual IList<string> getActiveActivityIds(string executionId)
	  {
		return commandExecutor.execute(new FindActiveActivityIdsCmd(executionId));
	  }

	  public virtual ActivityInstance getActivityInstance(string processInstanceId)
	  {
		return commandExecutor.execute(new GetActivityInstanceCmd(processInstanceId));
	  }

	  public virtual FormData getFormInstanceById(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetStartFormCmd(processDefinitionId));
	  }

	  public virtual void suspendProcessInstanceById(string processInstanceId)
	  {
		updateProcessInstanceSuspensionState().byProcessInstanceId(processInstanceId).suspend();
	  }

	  public virtual void suspendProcessInstanceByProcessDefinitionId(string processDefinitionId)
	  {
		updateProcessInstanceSuspensionState().byProcessDefinitionId(processDefinitionId).suspend();
	  }

	  public virtual void suspendProcessInstanceByProcessDefinitionKey(string processDefinitionKey)
	  {
		updateProcessInstanceSuspensionState().byProcessDefinitionKey(processDefinitionKey).suspend();
	  }

	  public virtual void activateProcessInstanceById(string processInstanceId)
	  {
		updateProcessInstanceSuspensionState().byProcessInstanceId(processInstanceId).activate();
	  }

	  public virtual void activateProcessInstanceByProcessDefinitionId(string processDefinitionId)
	  {
		updateProcessInstanceSuspensionState().byProcessDefinitionId(processDefinitionId).activate();
	  }

	  public virtual void activateProcessInstanceByProcessDefinitionKey(string processDefinitionKey)
	  {
		updateProcessInstanceSuspensionState().byProcessDefinitionKey(processDefinitionKey).activate();
	  }

	  public virtual UpdateProcessInstanceSuspensionStateSelectBuilder updateProcessInstanceSuspensionState()
	  {
		return new UpdateProcessInstanceSuspensionStateBuilderImpl(commandExecutor);
	  }

	  public virtual ProcessInstance startProcessInstanceByMessage(string messageName)
	  {
		return createMessageCorrelation(messageName).correlateStartMessage();
	  }

	  public virtual ProcessInstance startProcessInstanceByMessage(string messageName, string businessKey)
	  {
		return createMessageCorrelation(messageName).processInstanceBusinessKey(businessKey).correlateStartMessage();
	  }

	  public virtual ProcessInstance startProcessInstanceByMessage(string messageName, IDictionary<string, object> processVariables)
	  {
		return createMessageCorrelation(messageName).setVariables(processVariables).correlateStartMessage();
	  }

	  public virtual ProcessInstance startProcessInstanceByMessage(string messageName, string businessKey, IDictionary<string, object> processVariables)
	  {
		return createMessageCorrelation(messageName).processInstanceBusinessKey(businessKey).setVariables(processVariables).correlateStartMessage();
	  }

	  public virtual ProcessInstance startProcessInstanceByMessageAndProcessDefinitionId(string messageName, string processDefinitionId)
	  {
		return createMessageCorrelation(messageName).processDefinitionId(processDefinitionId).correlateStartMessage();
	  }

	  public virtual ProcessInstance startProcessInstanceByMessageAndProcessDefinitionId(string messageName, string processDefinitionId, string businessKey)
	  {
		return createMessageCorrelation(messageName).processDefinitionId(processDefinitionId).processInstanceBusinessKey(businessKey).correlateStartMessage();
	  }

	  public virtual ProcessInstance startProcessInstanceByMessageAndProcessDefinitionId(string messageName, string processDefinitionId, IDictionary<string, object> processVariables)
	  {
		return createMessageCorrelation(messageName).processDefinitionId(processDefinitionId).setVariables(processVariables).correlateStartMessage();
	  }

	  public virtual ProcessInstance startProcessInstanceByMessageAndProcessDefinitionId(string messageName, string processDefinitionId, string businessKey, IDictionary<string, object> processVariables)
	  {
		return createMessageCorrelation(messageName).processDefinitionId(processDefinitionId).processInstanceBusinessKey(businessKey).setVariables(processVariables).correlateStartMessage();
	  }

	  public virtual void signalEventReceived(string signalName)
	  {
		createSignalEvent(signalName).send();
	  }

	  public virtual void signalEventReceived(string signalName, IDictionary<string, object> processVariables)
	  {
		createSignalEvent(signalName).setVariables(processVariables).send();
	  }

	  public virtual void signalEventReceived(string signalName, string executionId)
	  {
		createSignalEvent(signalName).executionId(executionId).send();
	  }

	  public virtual void signalEventReceived(string signalName, string executionId, IDictionary<string, object> processVariables)
	  {
		createSignalEvent(signalName).executionId(executionId).setVariables(processVariables).send();
	  }

	  public virtual SignalEventReceivedBuilder createSignalEvent(string signalName)
	  {
		return new SignalEventReceivedBuilderImpl(commandExecutor, signalName);
	  }

	  public virtual void messageEventReceived(string messageName, string executionId)
	  {
		ensureNotNull("messageName", messageName);
		commandExecutor.execute(new MessageEventReceivedCmd(messageName, executionId, null));
	  }

	  public virtual void messageEventReceived(string messageName, string executionId, IDictionary<string, object> processVariables)
	  {
		ensureNotNull("messageName", messageName);
		commandExecutor.execute(new MessageEventReceivedCmd(messageName, executionId, processVariables));
	  }

	  public virtual MessageCorrelationBuilder createMessageCorrelation(string messageName)
	  {
		return new MessageCorrelationBuilderImpl(commandExecutor, messageName);
	  }

	  public virtual void correlateMessage(string messageName, IDictionary<string, object> correlationKeys, IDictionary<string, object> processVariables)
	  {
		createMessageCorrelation(messageName).processInstanceVariablesEqual(correlationKeys).setVariables(processVariables).correlate();
	  }

	  public virtual void correlateMessage(string messageName, string businessKey, IDictionary<string, object> correlationKeys, IDictionary<string, object> processVariables)
	  {

		createMessageCorrelation(messageName).processInstanceVariablesEqual(correlationKeys).processInstanceBusinessKey(businessKey).setVariables(processVariables).correlate();
	  }

	  public virtual void correlateMessage(string messageName)
	  {
		createMessageCorrelation(messageName).correlate();
	  }

	  public virtual void correlateMessage(string messageName, string businessKey)
	  {
		createMessageCorrelation(messageName).processInstanceBusinessKey(businessKey).correlate();
	  }

	  public virtual void correlateMessage(string messageName, IDictionary<string, object> correlationKeys)
	  {
		createMessageCorrelation(messageName).processInstanceVariablesEqual(correlationKeys).correlate();
	  }

	  public virtual void correlateMessage(string messageName, string businessKey, IDictionary<string, object> processVariables)
	  {
		createMessageCorrelation(messageName).processInstanceBusinessKey(businessKey).setVariables(processVariables).correlate();
	  }

	  public virtual ProcessInstanceModificationBuilder createProcessInstanceModification(string processInstanceId)
	  {
		return new ProcessInstanceModificationBuilderImpl(commandExecutor, processInstanceId);
	  }

	  public virtual ProcessInstantiationBuilder createProcessInstanceById(string processDefinitionId)
	  {
		return ProcessInstantiationBuilderImpl.createProcessInstanceById(commandExecutor, processDefinitionId);
	  }

	  public virtual ProcessInstantiationBuilder createProcessInstanceByKey(string processDefinitionKey)
	  {
		return ProcessInstantiationBuilderImpl.createProcessInstanceByKey(commandExecutor, processDefinitionKey);
	  }

	  public virtual MigrationPlanBuilder createMigrationPlan(string sourceProcessDefinitionId, string targetProcessDefinitionId)
	  {
		return new MigrationPlanBuilderImpl(commandExecutor, sourceProcessDefinitionId, targetProcessDefinitionId);
	  }

	  public virtual MigrationPlanExecutionBuilder newMigration(MigrationPlan migrationPlan)
	  {
		return new MigrationPlanExecutionBuilderImpl(commandExecutor, migrationPlan);
	  }

	  public virtual ModificationBuilder createModification(string processDefinitionId)
	  {
		return new ModificationBuilderImpl(commandExecutor, processDefinitionId);
	  }

	  public virtual RestartProcessInstanceBuilder restartProcessInstances(string processDefinitionId)
	  {
		return new RestartProcessInstanceBuilderImpl(commandExecutor, processDefinitionId);
	  }

	  public virtual Incident createIncident(string incidentType, string executionId, string configuration)
	  {
		return createIncident(incidentType, executionId, configuration, null);
	  }

	  public virtual Incident createIncident(string incidentType, string executionId, string configuration, string message)
	  {
		return commandExecutor.execute(new CreateIncidentCmd(incidentType, executionId, configuration, message));
	  }

	  public virtual void resolveIncident(string incidentId)
	  {
		commandExecutor.execute(new ResolveIncidentCmd(incidentId));
	  }

	  public virtual ConditionEvaluationBuilder createConditionEvaluation()
	  {
		return new ConditionEvaluationBuilderImpl(commandExecutor);
	  }
	}
}