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

	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TransitionInstanceJobHandler : MigratingDependentInstanceParseHandler<MigratingTransitionInstance, IList<JobEntity>>
	{

	  public virtual void handle(MigratingInstanceParseContext parseContext, MigratingTransitionInstance transitionInstance, IList<JobEntity> elements)
	  {

		foreach (JobEntity job in elements)
		{
		  if (!isAsyncContinuation(job))
		  {
			continue;
		  }

		  ScopeImpl targetScope = transitionInstance.TargetScope;
		  if (targetScope != null)
		  {
			JobDefinitionEntity targetJobDefinitionEntity = parseContext.getTargetJobDefinition(transitionInstance.TargetScope.Id, job.JobHandlerType);

			MigratingAsyncJobInstance migratingJobInstance = new MigratingAsyncJobInstance(job, targetJobDefinitionEntity, transitionInstance.TargetScope);

			transitionInstance.DependentJobInstance = migratingJobInstance;
			parseContext.submit(migratingJobInstance);
		  }

		  parseContext.consume(job);
		}
	  }

	  protected internal static bool isAsyncContinuation(JobEntity job)
	  {
		return job != null && AsyncContinuationJobHandler.TYPE.Equals(job.JobHandlerType);
	  }

	}

}