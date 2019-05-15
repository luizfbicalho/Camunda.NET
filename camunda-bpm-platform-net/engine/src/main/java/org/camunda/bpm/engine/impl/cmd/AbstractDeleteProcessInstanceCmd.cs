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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// Created by aakhmerov on 16.09.16.
	/// <para>
	/// Provide common logic for process instance deletion operations.
	/// Permissions checking and single process instance removal included.
	/// </para>
	/// </summary>
	public abstract class AbstractDeleteProcessInstanceCmd
	{

	  protected internal bool externallyTerminated;
	  protected internal string deleteReason;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipSubprocesses;
	  protected internal bool failIfNotExists = true;

	  protected internal virtual void checkDeleteProcessInstance(ExecutionEntity execution, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteProcessInstance(execution);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void deleteProcessInstance(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, String processInstanceId, final String deleteReason, final boolean skipCustomListeners, boolean externallyTerminated, final boolean skipIoMappings, boolean skipSubprocesses)
	  protected internal virtual void deleteProcessInstance(CommandContext commandContext, string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipIoMappings, bool skipSubprocesses)
	  {
		ensureNotNull(typeof(BadUserRequestException), "processInstanceId is null", "processInstanceId", processInstanceId);

		// fetch process instance
		ExecutionManager executionManager = commandContext.ExecutionManager;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity execution = executionManager.findExecutionById(processInstanceId);
		ExecutionEntity execution = executionManager.findExecutionById(processInstanceId);

		if (!failIfNotExists && execution == null)
		{
		  return;
		}

		ensureNotNull(typeof(BadUserRequestException), "No process instance found for id '" + processInstanceId + "'", "processInstance", execution);

		checkDeleteProcessInstance(execution, commandContext);

		// delete process instance
		commandContext.ExecutionManager.deleteProcessInstance(processInstanceId, deleteReason, false, skipCustomListeners, externallyTerminated, skipIoMappings, skipSubprocesses);

		if (skipSubprocesses)
		{
		  IList<ProcessInstance> superProcesslist = commandContext.ProcessEngineConfiguration.RuntimeService.createProcessInstanceQuery().superProcessInstanceId(processInstanceId).list();
		  triggerHistoryEvent(superProcesslist);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity superExecution = execution.getSuperExecution();
		ExecutionEntity superExecution = execution.getSuperExecution();
		if (superExecution != null)
		{
		  commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, deleteReason, skipCustomListeners, skipIoMappings, superExecution));

		}

		// create user operation log
		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, processInstanceId, null, null, Collections.singletonList(PropertyChange.EMPTY_CHANGE));
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly AbstractDeleteProcessInstanceCmd outerInstance;

		  private CommandContext commandContext;
		  private string deleteReason;
		  private bool skipCustomListeners;
		  private bool skipIoMappings;
		  private ExecutionEntity superExecution;

		  public CallableAnonymousInnerClass(AbstractDeleteProcessInstanceCmd outerInstance, CommandContext commandContext, string deleteReason, bool skipCustomListeners, bool skipIoMappings, ExecutionEntity superExecution)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.deleteReason = deleteReason;
			  this.skipCustomListeners = skipCustomListeners;
			  this.skipIoMappings = skipIoMappings;
			  this.superExecution = superExecution;
		  }

		  public Void call()
		  {
			ProcessInstanceModificationBuilderImpl builder = (ProcessInstanceModificationBuilderImpl) (new ProcessInstanceModificationBuilderImpl(commandContext, superExecution.ProcessInstanceId, deleteReason)).cancelActivityInstance(superExecution.ActivityInstanceId);
			builder.execute(false, skipCustomListeners, skipIoMappings);
			return null;
		  }
	  }

	  public virtual void triggerHistoryEvent(IList<ProcessInstance> subProcesslist)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		HistoryLevel historyLevel = configuration.HistoryLevel;

		foreach (ProcessInstance processInstance in subProcesslist)
		{
		  // TODO: This smells bad, as the rest of the history is done via the
		  // ParseListener
		  if (historyLevel.isHistoryEventProduced(HistoryEventTypes.PROCESS_INSTANCE_UPDATE, processInstance))
		  {

			HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
		  }
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly AbstractDeleteProcessInstanceCmd outerInstance;

		  public HistoryEventCreatorAnonymousInnerClass(AbstractDeleteProcessInstanceCmd outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createProcessInstanceUpdateEvt((DelegateExecution) processInstance);
		  }
	  }

	}

}