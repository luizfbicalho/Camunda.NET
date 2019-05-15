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
namespace org.camunda.bpm.engine.impl.migration.instance
{

	using MessageJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.MessageJobDeclaration;
	using AsyncContinuationConfiguration = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler.AsyncContinuationConfiguration;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingAsyncJobInstance : MigratingJobInstance
	{

	  public MigratingAsyncJobInstance(JobEntity jobEntity, JobDefinitionEntity jobDefinitionEntity, ScopeImpl targetScope) : base(jobEntity, jobDefinitionEntity, targetScope)
	  {
	  }

	  protected internal override void migrateJobHandlerConfiguration()
	  {
		AsyncContinuationConfiguration configuration = (AsyncContinuationConfiguration) jobEntity.JobHandlerConfiguration;

		if (AsyncAfter)
		{
		  updateAsyncAfterTargetConfiguration(configuration);
		}
		else
		{
		  updateAsyncBeforeTargetConfiguration();
		}
	  }


	  public virtual bool AsyncAfter
	  {
		  get
		  {
			JobDefinition jobDefinition = jobEntity.JobDefinition;
			return MessageJobDeclaration.ASYNC_AFTER.Equals(jobDefinition.JobConfiguration);
		  }
	  }

	  public virtual bool AsyncBefore
	  {
		  get
		  {
			return !AsyncAfter;
		  }
	  }

	  protected internal virtual void updateAsyncBeforeTargetConfiguration()
	  {

		AsyncContinuationConfiguration targetConfiguration = new AsyncContinuationConfiguration();
		AsyncContinuationConfiguration currentConfiguration = (AsyncContinuationConfiguration) jobEntity.JobHandlerConfiguration;

		if (org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_START.CanonicalName.Equals(currentConfiguration.AtomicOperation))
		{
		  // process start always stays process start
		  targetConfiguration.AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_START.CanonicalName;
		}
		else
		{
		  if (((ActivityImpl) targetScope).IncomingTransitions.Count == 0)
		  {
			targetConfiguration.AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE.CanonicalName;
		  }
		  else
		  {
			targetConfiguration.AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_CREATE_SCOPE.CanonicalName;
		  }
		}


		jobEntity.JobHandlerConfiguration = targetConfiguration;
	  }

	  protected internal virtual void updateAsyncAfterTargetConfiguration(AsyncContinuationConfiguration currentConfiguration)
	  {
		ActivityImpl targetActivity = (ActivityImpl) targetScope;
		IList<PvmTransition> outgoingTransitions = targetActivity.OutgoingTransitions;

		AsyncContinuationConfiguration targetConfiguration = new AsyncContinuationConfiguration();

		if (outgoingTransitions.Count == 0)
		{
		  targetConfiguration.AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_END.CanonicalName;
		}
		else
		{
		  targetConfiguration.AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE.CanonicalName;

		  if (outgoingTransitions.Count == 1)
		  {
			targetConfiguration.TransitionId = outgoingTransitions[0].Id;
		  }
		  else
		  {
			TransitionImpl matchingTargetTransition = null;
			string currentTransitionId = currentConfiguration.TransitionId;
			if (!string.ReferenceEquals(currentTransitionId, null))
			{
			  matchingTargetTransition = targetActivity.findOutgoingTransition(currentTransitionId);
			}

			if (matchingTargetTransition != null)
			{
			  targetConfiguration.TransitionId = matchingTargetTransition.Id;
			}
			else
			{
			  // should not happen since it is avoided by validation
			  throw new ProcessEngineException("Cannot determine matching outgoing sequence flow");
			}
		  }
		}

		jobEntity.JobHandlerConfiguration = targetConfiguration;
	  }

	}

}