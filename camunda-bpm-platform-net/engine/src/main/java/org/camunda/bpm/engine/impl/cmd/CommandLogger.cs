using System;

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

	using ProcessApplicationIdentifier = org.camunda.bpm.application.impl.ProcessApplicationIdentifier;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using CorrelationSet = org.camunda.bpm.engine.impl.runtime.CorrelationSet;
	using ClassNameUtil = org.camunda.bpm.engine.impl.util.ClassNameUtil;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CommandLogger : ProcessEngineLogger
	{


	  public virtual void debugCreatingNewDeployment()
	  {
		logDebug("001", "Creating new deployment");
	  }

	  public virtual void usingExistingDeployment()
	  {
		logDebug("002", "Using existing deployment");
	  }

	  public virtual void debugModificationInstruction(string processInstanceId, int i, string describe)
	  {
		logDebug("003", "Modifying process instance '{}': Instruction {}: {}", processInstanceId, i, describe);
	  }

	  public virtual void debugStartingInstruction(string processInstanceId, int i, string describe)
	  {
		logDebug("004", "Starting process instance '{}': Instruction {}: {}", processInstanceId, i, describe);
	  }

	  public virtual void debugStartingCommand<T1>(Command<T1> cmd)
	  {
		logDebug("005", "Starting command -------------------- {} ----------------------", ClassNameUtil.getClassNameWithoutPackage(cmd));
	  }

	  public virtual void debugFinishingCommand<T1>(Command<T1> cmd)
	  {
		logDebug("006", "Finishing command -------------------- {} ----------------------", ClassNameUtil.getClassNameWithoutPackage(cmd));
	  }

	  public virtual void debugWaitingFor(long waitTime)
	  {
		logDebug("007", "Waiting for {} before retrying command", waitTime);
	  }

	  public virtual void debugCaughtOptimisticLockingException(OptimisticLockingException e)
	  {
		logDebug("008", "caught optimistic locking excpetion", e);
	  }

	  public virtual void debugOpeningNewCommandContext()
	  {
		logDebug("009", "opening new command context");
	  }

	  public virtual void debugReusingExistingCommandContext()
	  {
		logDebug("010", "reusing existing command context");
	  }

	  public virtual void closingCommandContext()
	  {
		logDebug("011", "closing existing command context");
	  }

	  public virtual void calledInsideTransaction()
	  {
		logDebug("012", "called inside transaction skipping");
	  }

	  public virtual void maskedExceptionInCommandContext(Exception throwable)
	  {
		logDebug("013", "masked exception in command context. for root cause, see below as it will be rethrown later.", throwable);
	  }

	  public virtual void exceptionWhileRollingBackTransaction(Exception e)
	  {
		logError("014", "exception while rolling back transaction", e);
	  }

	  public virtual void exceptionWhileGettingValueForVariable(Exception t)
	  {
		logDebug("015", "exception while getting value for variable {}", t.Message, t);
	  }

	  public virtual void couldNotFindProcessDefinitionForEventSubscription(EventSubscriptionEntity messageEventSubscription, string processDefinitionId)
	  {
		logDebug("016", "Found event subscription with {} but process definition {} could not be found.", messageEventSubscription, processDefinitionId);
	  }

	  public virtual void debugIgnoringEventSubscription(EventSubscriptionEntity eventSubscription, string processDefinitionId)
	  {
		logDebug("017", "Found event subscription with {} but process definition {} could not be found.", eventSubscription, processDefinitionId);
	  }

	  public virtual void debugProcessingDeployment(string name)
	  {
		logDebug("018", "Processing deployment {}", name);
	  }

	  public virtual void debugProcessingResource(string name)
	  {
		logDebug("019", "Processing resource {}", name);
	  }

	  public virtual ProcessEngineException paWithNameNotRegistered(string name)
	  {
		return new ProcessEngineException(exceptionMessage("020", "A process application with name '{}' is not registered", name));
	  }

	  public virtual ProcessEngineException cannotReolvePa(ProcessApplicationIdentifier processApplicationIdentifier)
	  {
		return new ProcessEngineException(exceptionMessage("021", "Cannot resolve process application based on {}", processApplicationIdentifier));
	  }

	  public virtual void warnDisabledDeploymentLock()
	  {
		logWarn("022", "No exclusive lock is aquired while deploying because it is disabled. " + "This can lead to problems when multiple process engines use the same data source (i.e. in cluster mode).");
	  }

	  public virtual BadUserRequestException exceptionStartProcessInstanceByIdAndTenantId()
	  {
		return new BadUserRequestException(exceptionMessage("023", "Cannot specify a tenant-id when start a process instance by process definition id."));
	  }

	  public virtual BadUserRequestException exceptionStartProcessInstanceAtStartActivityAndSkipListenersOrMapping()
	  {
		return new BadUserRequestException(exceptionMessage("024", "Cannot skip custom listeners or input/output mappings when start a process instance at default start activity."));
	  }

	  public virtual BadUserRequestException exceptionCorrelateMessageWithProcessDefinitionId()
	  {
		return new BadUserRequestException(exceptionMessage("025", "Cannot specify a process definition id when correlate a message, except for explicit correlation of a start message."));
	  }

	  public virtual BadUserRequestException exceptionCorrelateStartMessageWithCorrelationVariables()
	  {
		return new BadUserRequestException(exceptionMessage("026", "Cannot specify correlation variables of a process instance when correlate a start message."));
	  }

	  public virtual BadUserRequestException exceptionDeliverSignalToSingleExecutionWithTenantId()
	  {
		return new BadUserRequestException(exceptionMessage("027", "Cannot specify a tenant-id when deliver a signal to a single execution."));
	  }

	  public virtual BadUserRequestException exceptionCorrelateMessageWithProcessInstanceAndTenantId()
	  {
		return new BadUserRequestException(exceptionMessage("028", "Cannot specify a tenant-id when correlate a message to a single process instance."));
	  }

	  public virtual BadUserRequestException exceptionCorrelateMessageWithProcessDefinitionAndTenantId()
	  {
		return new BadUserRequestException(exceptionMessage("029", "Cannot specify a tenant-id when correlate a start message to a specific version of a process definition."));
	  }

	  public virtual MismatchingMessageCorrelationException exceptionCorrelateMessageToSingleProcessDefinition(string messageName, long processDefinitionCound, CorrelationSet correlationSet)
	  {
		return new MismatchingMessageCorrelationException(exceptionMessage("030", "Cannot correlate a message with name '{}' to a single process definition. {} process definitions match the correlations keys: {}", messageName, processDefinitionCound, correlationSet));
	  }

	  public virtual MismatchingMessageCorrelationException exceptionCorrelateMessageToSingleExecution(string messageName, long executionCound, CorrelationSet correlationSet)
	  {
		return new MismatchingMessageCorrelationException(exceptionMessage("031", "Cannot correlate a message with name '{}' to a single execution. {} executions match the correlation keys: {}", messageName, executionCound, correlationSet));
	  }

	  public virtual BadUserRequestException exceptionUpdateSuspensionStateForTenantOnlyByProcessDefinitionKey()
	  {
		return new BadUserRequestException(exceptionMessage("032", "Can only specify a tenant-id when update the suspension state which is referenced by process definition key."));
	  }

	  public virtual ProcessEngineException exceptionBpmnErrorPropagationFailed(string errorCode, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("033", "Propagation of bpmn error {} failed. ", errorCode), cause);
	  }

	  public virtual ProcessEngineException exceptionCommandWithUnauthorizedTenant(string command)
	  {
		return new ProcessEngineException(exceptionMessage("034", "Cannot {} because it belongs to no authenticated tenant.", command));
	  }

	  public virtual void warnDeploymentResourceHasWrongName(string resourceName, string[] suffixes)
	  {
		logWarn("035", string.Format("Deployment resource '{0}' will be ignored as its name must have one of suffixes {1}.", resourceName, Arrays.ToString(suffixes)));

	  }

	  public virtual ProcessEngineException processInstanceDoesNotExist(string processInstanceId)
	  {
		return new ProcessEngineException(exceptionMessage("036", "Process instance '{}' cannot be modified. The process instance does not exist", processInstanceId));
	  }

	  public virtual ProcessEngineException processDefinitionOfInstanceDoesNotMatchModification(ExecutionEntity processInstance, string processDefinitionId)
	  {
		return new ProcessEngineException(exceptionMessage("037", "Process instance '{}' cannot be modified. Its process definition '{}' does not match given process definition '{}'", processInstance.Id, processInstance.ProcessDefinitionId, processDefinitionId));
	  }

	  public virtual void debugHistoryCleanupWrongConfiguration()
	  {
		logDebug("038", "History cleanup won't be scheduled. Either configure batch window or call it with immediatelyDue = true.");
	  }

	  public virtual ProcessEngineException processDefinitionOfHistoricInstanceDoesNotMatchTheGivenOne(HistoricProcessInstance historicProcessInstance, string processDefinitionId)
	  {
		return new ProcessEngineException(exceptionMessage("039", "Historic process instance '{}' cannot be restarted. Its process definition '{}' does not match given process definition '{}'", historicProcessInstance.Id, historicProcessInstance.ProcessDefinitionId, processDefinitionId));
	  }

	  public virtual ProcessEngineException historicProcessInstanceActive(HistoricProcessInstance historicProcessInstance)
	  {
		return new ProcessEngineException(exceptionMessage("040", "Historic process instance '{}' cannot be restarted. It is not completed or terminated.", historicProcessInstance.Id, historicProcessInstance.ProcessDefinitionId));
	  }

	  public virtual ProcessEngineException exceptionWhenStartFormScriptEvaluation(string processDefinitionId, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("041", "Unable to evaluate script when rendering start form of the process definition '{}'.", processDefinitionId));
	  }

	  public virtual ProcessEngineException exceptionWhenEvaluatingConditionalStartEventByProcessDefinition(string processDefinitionId)
	  {
		return new ProcessEngineException(exceptionMessage("042", "Process definition with id '{}' does not declare conditional start event.", processDefinitionId));
	  }

	  public virtual ProcessEngineException exceptionWhenEvaluatingConditionalStartEvent()
	  {
		return new ProcessEngineException(exceptionMessage("043", "No subscriptions were found during evaluation of the conditional start events."));
	  }
	}

}