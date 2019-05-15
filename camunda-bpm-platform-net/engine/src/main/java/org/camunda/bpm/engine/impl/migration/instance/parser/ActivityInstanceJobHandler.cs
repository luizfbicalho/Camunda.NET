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

	using TimerDeclarationImpl = org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ActivityInstanceJobHandler : MigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<JobEntity>>
	{

	  public virtual void handle(MigratingInstanceParseContext parseContext, MigratingActivityInstance activityInstance, IList<JobEntity> elements)
	  {

		IDictionary<string, TimerDeclarationImpl> sourceTimerDeclarationsInEventScope = getTimerDeclarationsByTriggeringActivity(activityInstance.SourceScope);
		IDictionary<string, TimerDeclarationImpl> targetTimerDeclarationsInEventScope = getTimerDeclarationsByTriggeringActivity(activityInstance.TargetScope);

		foreach (JobEntity job in elements)
		{
		  if (!isTimerJob(job))
		  {
			// skip non timer jobs
			continue;
		  }

		  MigrationInstruction migrationInstruction = parseContext.findSingleMigrationInstruction(job.ActivityId);
		  ActivityImpl targetActivity = parseContext.getTargetActivity(migrationInstruction);

		  if (targetActivity != null && activityInstance.migratesTo(targetActivity.EventScope))
		  {
			// the timer job is migrated
			JobDefinitionEntity targetJobDefinitionEntity = parseContext.getTargetJobDefinition(targetActivity.ActivityId, job.JobHandlerType);

			TimerDeclarationImpl targetTimerDeclaration = targetTimerDeclarationsInEventScope.Remove(targetActivity.Id);

			MigratingJobInstance migratingTimerJobInstance = new MigratingTimerJobInstance(job, targetJobDefinitionEntity, targetActivity, migrationInstruction.UpdateEventTrigger, targetTimerDeclaration);
			activityInstance.addMigratingDependentInstance(migratingTimerJobInstance);
			parseContext.submit(migratingTimerJobInstance);

		  }
		  else
		  {
			// the timer job is removed
			MigratingJobInstance removingJobInstance = new MigratingTimerJobInstance(job);
			activityInstance.addRemovingDependentInstance(removingJobInstance);
			parseContext.submit(removingJobInstance);

		  }

		  parseContext.consume(job);
		}

		if (activityInstance.migrates())
		{
		  addEmergingTimerJobs(parseContext, activityInstance, sourceTimerDeclarationsInEventScope, targetTimerDeclarationsInEventScope);
		}
	  }

	  protected internal static bool isTimerJob(JobEntity job)
	  {
		return job != null && job.Type.Equals(TimerEntity.TYPE);
	  }

	  protected internal virtual void addEmergingTimerJobs(MigratingInstanceParseContext parseContext, MigratingActivityInstance activityInstance, IDictionary<string, TimerDeclarationImpl> sourceTimerDeclarationsInEventScope, IDictionary<string, TimerDeclarationImpl> targetTimerDeclarationsInEventScope)
	  {
		foreach (TimerDeclarationImpl targetTimerDeclaration in targetTimerDeclarationsInEventScope.Values)
		{
		  if (!isNonInterruptingTimerTriggeredAlready(parseContext, sourceTimerDeclarationsInEventScope, targetTimerDeclaration))
		  {
			activityInstance.addEmergingDependentInstance(new EmergingJobInstance(targetTimerDeclaration));
		  }
		}
	  }

	  protected internal virtual bool isNonInterruptingTimerTriggeredAlready(MigratingInstanceParseContext parseContext, IDictionary<string, TimerDeclarationImpl> sourceTimerDeclarationsInEventScope, TimerDeclarationImpl targetTimerDeclaration)
	  {
		if (targetTimerDeclaration.InterruptingTimer || !string.ReferenceEquals(targetTimerDeclaration.JobHandlerType, TimerExecuteNestedActivityJobHandler.TYPE) || sourceTimerDeclarationsInEventScope.Values.Count == 0)
		{
		  return false;
		}
		foreach (TimerDeclarationImpl sourceTimerDeclaration in sourceTimerDeclarationsInEventScope.Values)
		{
		  MigrationInstruction migrationInstruction = parseContext.findSingleMigrationInstruction(sourceTimerDeclaration.ActivityId);
		  ActivityImpl targetActivity = parseContext.getTargetActivity(migrationInstruction);

		  if (targetActivity != null && targetTimerDeclaration.ActivityId.Equals(targetActivity.ActivityId))
		  {
			return true;
		  }
		}
		return false;
	  }

	  protected internal virtual IDictionary<string, TimerDeclarationImpl> getTimerDeclarationsByTriggeringActivity(ScopeImpl scope)
	  {
		return new Dictionary<string, TimerDeclarationImpl>(TimerDeclarationImpl.getDeclarationsForScope(scope));
	  }

	}

}