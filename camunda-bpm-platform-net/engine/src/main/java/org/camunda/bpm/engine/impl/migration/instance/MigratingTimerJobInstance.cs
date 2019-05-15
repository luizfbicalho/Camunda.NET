﻿/*
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
namespace org.camunda.bpm.engine.impl.migration.instance
{
	using TimerDeclarationImpl = org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerJobConfiguration = org.camunda.bpm.engine.impl.jobexecutor.TimerEventJobHandler.TimerJobConfiguration;
	using TimerStartEventSubprocessJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventSubprocessJobHandler;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingTimerJobInstance : MigratingJobInstance
	{

	  protected internal ScopeImpl timerTriggerTargetScope;
	  protected internal TimerDeclarationImpl targetJobDeclaration;
	  protected internal bool updateEvent;

	  public MigratingTimerJobInstance(JobEntity jobEntity) : base(jobEntity)
	  {
	  }

	  public MigratingTimerJobInstance(JobEntity jobEntity, JobDefinitionEntity jobDefinitionEntity, ScopeImpl targetScope, bool updateEvent, TimerDeclarationImpl targetTimerDeclaration) : base(jobEntity, jobDefinitionEntity, targetScope)
	  {
		timerTriggerTargetScope = determineTimerTriggerTargetScope(jobEntity, targetScope);
		this.updateEvent = updateEvent;
		this.targetJobDeclaration = targetTimerDeclaration;
	  }

	  protected internal virtual ScopeImpl determineTimerTriggerTargetScope(JobEntity jobEntity, ScopeImpl targetScope)
	  {
		if (TimerStartEventSubprocessJobHandler.TYPE.Equals(jobEntity.JobHandlerType))
		{
		  // for event subprocess start jobs, the job handler configuration references the subprocess while
		  // the job references the start event
		  return targetScope.FlowScope;
		}
		else
		{
		  return targetScope;
		}
	  }

	  protected internal override void migrateJobHandlerConfiguration()
	  {
		TimerJobConfiguration configuration = (TimerJobConfiguration) jobEntity.JobHandlerConfiguration;
		configuration.TimerElementKey = timerTriggerTargetScope.Id;
		jobEntity.JobHandlerConfiguration = configuration;

		if (updateEvent)
		{
		  targetJobDeclaration.updateJob((TimerEntity) jobEntity);
		}
	  }

	}

}