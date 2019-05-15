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
namespace org.camunda.bpm.engine.impl.migration.instance.parser
{

	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using CompensationUtil = org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil;
	using Properties = org.camunda.bpm.engine.impl.core.model.Properties;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CompensationInstanceHandler : MigratingInstanceParseHandler<EventSubscriptionEntity>
	{

	  public virtual void handle(MigratingInstanceParseContext parseContext, EventSubscriptionEntity element)
	  {

		MigratingProcessElementInstance migratingInstance;
		if (!string.ReferenceEquals(element.Configuration, null))
		{
		  migratingInstance = createMigratingEventScopeInstance(parseContext, element);
		}
		else
		{
		  migratingInstance = createMigratingEventSubscriptionInstance(parseContext, element);
		}


		ExecutionEntity owningExecution = element.Execution;
		MigratingScopeInstance parentInstance = null;
		if (owningExecution.EventScope)
		{
		  parentInstance = parseContext.getMigratingCompensationInstanceByExecutionId(owningExecution.Id);
		}
		else
		{
		  parentInstance = parseContext.getMigratingActivityInstanceById(owningExecution.ParentActivityInstanceId);
		}
		migratingInstance.Parent = parentInstance;
	  }

	  protected internal virtual MigratingProcessElementInstance createMigratingEventSubscriptionInstance(MigratingInstanceParseContext parseContext, EventSubscriptionEntity element)
	  {
		ActivityImpl compensationHandler = parseContext.SourceProcessDefinition.findActivity(element.ActivityId);

		MigrationInstruction migrationInstruction = getMigrationInstruction(parseContext, compensationHandler);

		ActivityImpl targetScope = null;
		if (migrationInstruction != null)
		{
		  ActivityImpl targetEventScope = (ActivityImpl) parseContext.getTargetActivity(migrationInstruction).EventScope;
		  targetScope = targetEventScope.findCompensationHandler();
		}

		MigratingCompensationEventSubscriptionInstance migratingCompensationInstance = parseContext.MigratingProcessInstance.addCompensationSubscriptionInstance(migrationInstruction, element, compensationHandler, targetScope);

		parseContext.consume(element);

		return migratingCompensationInstance;
	  }

	  protected internal virtual MigratingProcessElementInstance createMigratingEventScopeInstance(MigratingInstanceParseContext parseContext, EventSubscriptionEntity element)
	  {

		ActivityImpl compensatingActivity = parseContext.SourceProcessDefinition.findActivity(element.ActivityId);

		MigrationInstruction migrationInstruction = getMigrationInstruction(parseContext, compensatingActivity);

		ActivityImpl eventSubscriptionTargetScope = null;

		if (migrationInstruction != null)
		{
		  if (compensatingActivity.CompensationHandler)
		  {
			ActivityImpl targetEventScope = (ActivityImpl) parseContext.getTargetActivity(migrationInstruction).EventScope;
			eventSubscriptionTargetScope = targetEventScope.findCompensationHandler();
		  }
		  else
		  {
			eventSubscriptionTargetScope = parseContext.getTargetActivity(migrationInstruction);
		  }
		}

		ExecutionEntity eventScopeExecution = CompensationUtil.getCompensatingExecution(element);
		MigrationInstruction eventScopeInstruction = parseContext.findSingleMigrationInstruction(eventScopeExecution.ActivityId);
		ActivityImpl targetScope = parseContext.getTargetActivity(eventScopeInstruction);

		MigratingEventScopeInstance migratingCompensationInstance = parseContext.MigratingProcessInstance.addEventScopeInstance(eventScopeInstruction, eventScopeExecution, eventScopeExecution.getActivity(), targetScope, migrationInstruction, element, compensatingActivity, eventSubscriptionTargetScope);

		parseContext.consume(element);
		parseContext.submit(migratingCompensationInstance);

		parseDependentEntities(parseContext, migratingCompensationInstance);

		return migratingCompensationInstance;
	  }

	  protected internal virtual MigrationInstruction getMigrationInstruction(MigratingInstanceParseContext parseContext, ActivityImpl activity)
	  {
		if (activity.CompensationHandler)
		{
		  Properties compensationHandlerProperties = activity.Properties;
		  ActivityImpl eventTrigger = compensationHandlerProperties.get(BpmnProperties.COMPENSATION_BOUNDARY_EVENT);
		  if (eventTrigger == null)
		  {
			eventTrigger = compensationHandlerProperties.get(BpmnProperties.INITIAL_ACTIVITY);
		  }

		  return parseContext.findSingleMigrationInstruction(eventTrigger.ActivityId);
		}
		else
		{
		  return parseContext.findSingleMigrationInstruction(activity.ActivityId);
		}
	  }

	  protected internal virtual void parseDependentEntities(MigratingInstanceParseContext parseContext, MigratingEventScopeInstance migratingInstance)
	  {

		ExecutionEntity representativeExecution = migratingInstance.resolveRepresentativeExecution();

		IList<VariableInstanceEntity> variables = new List<VariableInstanceEntity>(representativeExecution.VariablesInternal);
		parseContext.handleDependentVariables(migratingInstance, variables);
	  }

	}

}