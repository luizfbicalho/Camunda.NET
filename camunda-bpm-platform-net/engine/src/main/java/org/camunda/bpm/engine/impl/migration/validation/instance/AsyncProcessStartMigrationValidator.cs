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
namespace org.camunda.bpm.engine.impl.migration.validation.instance
{
	using AsyncContinuationConfiguration = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler.AsyncContinuationConfiguration;
	using MigratingProcessInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance;
	using MigratingTransitionInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingTransitionInstance;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AsyncProcessStartMigrationValidator : MigratingTransitionInstanceValidator
	{

	  public virtual void validate(MigratingTransitionInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingTransitionInstanceValidationReportImpl instanceReport)
	  {

		ActivityImpl targetActivity = (ActivityImpl) migratingInstance.TargetScope;

		if (targetActivity != null)
		{
		  if (isProcessStartJob(migratingInstance.JobInstance.JobEntity) && !isTopLevelActivity(targetActivity))
		  {
			instanceReport.addFailure("A transition instance that instantiates the process can only be migrated to a process-level flow node");
		  }
		}
	  }

	  protected internal virtual bool isProcessStartJob(JobEntity job)
	  {
		AsyncContinuationConfiguration configuration = (AsyncContinuationConfiguration) job.JobHandlerConfiguration;
		return org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_START.CanonicalName.Equals(configuration.AtomicOperation);
	  }

	  protected internal virtual bool isTopLevelActivity(ActivityImpl activity)
	  {
		return activity.FlowScope == activity.ProcessDefinition;
	  }
	}

}