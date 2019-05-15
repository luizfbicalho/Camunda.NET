using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using ProcessApplicationContext = org.camunda.bpm.application.ProcessApplicationContext;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	public class RestartProcessInstancesCmd : AbstractRestartProcessInstanceCmd<Void>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal bool writeUserOperationLog;

	  public RestartProcessInstancesCmd(CommandExecutor commandExecutor, RestartProcessInstanceBuilderImpl builder, bool writeUserOperationLog) : base(commandExecutor, builder)
	  {
		this.writeUserOperationLog = writeUserOperationLog;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public override Void execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Collection<String> processInstanceIds = collectProcessInstanceIds();
		ICollection<string> processInstanceIds = collectProcessInstanceIds();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<AbstractProcessInstanceModificationCommand> instructions = builder.getInstructions();
		IList<AbstractProcessInstanceModificationCommand> instructions = builder.Instructions;

		ensureNotEmpty(typeof(BadUserRequestException), "Restart instructions cannot be empty", "instructions", instructions);
		ensureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot be empty", "Process instance ids", processInstanceIds);
		ensureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "Process instance ids", processInstanceIds);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity processDefinition = getProcessDefinition(commandContext, builder.getProcessDefinitionId());
		ProcessDefinitionEntity processDefinition = getProcessDefinition(commandContext, builder.ProcessDefinitionId);
		ensureNotNull("Process definition cannot be found", "processDefinition", processDefinition);

		checkAuthorization(commandContext, processDefinition);

		if (writeUserOperationLog)
		{
		  writeUserOperationLog(commandContext, processDefinition, processInstanceIds.Count, false);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processDefinitionId = builder.getProcessDefinitionId();
		string processDefinitionId = builder.ProcessDefinitionId;

		ThreadStart runnable = () =>
		{

	foreach (string processInstanceId in processInstanceIds)
	{
	  HistoricProcessInstance historicProcessInstance = getHistoricProcessInstance(commandContext, processInstanceId);

	  ensureNotNull(typeof(BadUserRequestException), "Historic process instance cannot be found", "historicProcessInstanceId", historicProcessInstance);
	  ensureHistoricProcessInstanceNotActive(historicProcessInstance);
	  ensureSameProcessDefinition(historicProcessInstance, processDefinitionId);

	  ProcessInstantiationBuilderImpl instantiationBuilder = getProcessInstantiationBuilder(commandExecutor, processDefinitionId);
	  applyProperties(instantiationBuilder, processDefinition, historicProcessInstance);

	  ProcessInstanceModificationBuilderImpl modificationBuilder = instantiationBuilder.ModificationBuilder;
	  modificationBuilder.ModificationOperations = instructions;

	  VariableMap variables = collectVariables(commandContext, historicProcessInstance);
	  instantiationBuilder.Variables = variables;

	  instantiationBuilder.execute(builder.SkipCustomListeners, builder.SkipIoMappings);
	}
		};
		ProcessApplicationContextUtil.doContextSwitch(runnable, processDefinition);

		return null;
	  }

	  protected internal virtual void checkAuthorization(CommandContext commandContext, ProcessDefinition processDefinition)
	  {
		commandContext.AuthorizationManager.checkAuthorization(Permissions.READ_HISTORY, Resources.PROCESS_DEFINITION, processDefinition.Key);
	  }

	  protected internal virtual HistoricProcessInstance getHistoricProcessInstance(CommandContext commandContext, string processInstanceId)
	  {
		HistoryService historyService = commandContext.ProcessEngineConfiguration.HistoryService;
		return historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();
	  }

	  protected internal virtual void ensureSameProcessDefinition(HistoricProcessInstance instance, string processDefinitionId)
	  {
		if (!processDefinitionId.Equals(instance.ProcessDefinitionId))
		{
		  throw LOG.processDefinitionOfHistoricInstanceDoesNotMatchTheGivenOne(instance, processDefinitionId);
		}
	  }

	  protected internal virtual void ensureHistoricProcessInstanceNotActive(HistoricProcessInstance instance)
	  {
		if (instance.EndTime == null)
		{
		  throw LOG.historicProcessInstanceActive(instance);
		}
	  }

	  protected internal virtual ProcessInstantiationBuilderImpl getProcessInstantiationBuilder(CommandExecutor commandExecutor, string processDefinitionId)
	  {
		return (ProcessInstantiationBuilderImpl) ProcessInstantiationBuilderImpl.createProcessInstanceById(commandExecutor, processDefinitionId);
	  }

	  protected internal virtual void applyProperties(ProcessInstantiationBuilderImpl instantiationBuilder, ProcessDefinition processDefinition, HistoricProcessInstance processInstance)
	  {
		string tenantId = processInstance.TenantId;
		if (string.ReferenceEquals(processDefinition.TenantId, null) && !string.ReferenceEquals(tenantId, null))
		{
		  instantiationBuilder.tenantId(tenantId);
		}

		if (!builder.WithoutBusinessKey)
		{
		  instantiationBuilder.businessKey(processInstance.BusinessKey);
		}

	  }

	  protected internal virtual VariableMap collectVariables(CommandContext commandContext, HistoricProcessInstance processInstance)
	  {
		VariableMap variables = null;

		if (builder.InitialVariables)
		{
		  variables = collectInitialVariables(commandContext, processInstance);
		}
		else
		{
		  variables = collectLastVariables(commandContext, processInstance);
		}

		return variables;
	  }

	  protected internal virtual VariableMap collectInitialVariables(CommandContext commandContext, HistoricProcessInstance processInstance)
	  {
		HistoryService historyService = commandContext.ProcessEngineConfiguration.HistoryService;

		HistoricActivityInstance startActivityInstance = resolveStartActivityInstance(processInstance);

		HistoricDetailQueryImpl query = (HistoricDetailQueryImpl) historyService.createHistoricDetailQuery().variableUpdates().executionId(processInstance.Id).activityInstanceId(startActivityInstance.Id);

		IList<HistoricDetail> historicDetails = query.sequenceCounter(1).list();

		VariableMap variables = new VariableMapImpl();
		foreach (HistoricDetail detail in historicDetails)
		{
		  HistoricVariableUpdate variableUpdate = (HistoricVariableUpdate) detail;
		  variables.putValueTyped(variableUpdate.VariableName, variableUpdate.TypedValue);
		}

		return variables;
	  }

	  protected internal virtual VariableMap collectLastVariables(CommandContext commandContext, HistoricProcessInstance processInstance)
	  {
		HistoryService historyService = commandContext.ProcessEngineConfiguration.HistoryService;

		IList<HistoricVariableInstance> historicVariables = historyService.createHistoricVariableInstanceQuery().executionIdIn(processInstance.Id).list();

		VariableMap variables = new VariableMapImpl();
		foreach (HistoricVariableInstance variable in historicVariables)
		{
		  variables.putValueTyped(variable.Name, variable.TypedValue);
		}

		return variables;
	  }

	  protected internal virtual HistoricActivityInstance resolveStartActivityInstance(HistoricProcessInstance processInstance)
	  {
		HistoryService historyService = Context.ProcessEngineConfiguration.HistoryService;

		string processInstanceId = processInstance.Id;
		string startActivityId = processInstance.StartActivityId;

		ensureNotNull("startActivityId", startActivityId);

		IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstanceId).activityId(startActivityId).orderPartiallyByOccurrence().asc().list();

		ensureNotEmpty("historicActivityInstances", historicActivityInstances);

		HistoricActivityInstance startActivityInstance = historicActivityInstances[0];
		return startActivityInstance;
	  }

	}

}