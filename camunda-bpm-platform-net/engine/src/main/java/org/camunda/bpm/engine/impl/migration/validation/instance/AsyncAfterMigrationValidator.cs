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
namespace org.camunda.bpm.engine.impl.migration.validation.instance
{
	using AsyncContinuationConfiguration = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler.AsyncContinuationConfiguration;
	using MigratingJobInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingJobInstance;
	using MigratingProcessInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance;
	using MigratingTransitionInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingTransitionInstance;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;

	public class AsyncAfterMigrationValidator : MigratingTransitionInstanceValidator
	{

	  public virtual void validate(MigratingTransitionInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingTransitionInstanceValidationReportImpl instanceReport)
	  {
		ActivityImpl targetActivity = (ActivityImpl) migratingInstance.TargetScope;

		if (targetActivity != null && migratingInstance.AsyncAfter)
		{
		  MigratingJobInstance jobInstance = migratingInstance.JobInstance;
		  AsyncContinuationConfiguration config = (AsyncContinuationConfiguration) jobInstance.JobEntity.JobHandlerConfiguration;
		  string sourceTransitionId = config.TransitionId;

		  if (targetActivity.OutgoingTransitions.Count > 1)
		  {
			if (string.ReferenceEquals(sourceTransitionId, null))
			{
			  instanceReport.addFailure("Transition instance is assigned to no sequence flow" + " and target activity has more than one outgoing sequence flow");
			}
			else
			{
			  TransitionImpl matchingOutgoingTransition = targetActivity.findOutgoingTransition(sourceTransitionId);
			  if (matchingOutgoingTransition == null)
			  {
				instanceReport.addFailure("Transition instance is assigned to a sequence flow" + " that cannot be matched in the target activity");
			  }
			}
		  }
		}

	  }

	}

}