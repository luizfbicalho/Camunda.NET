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
namespace org.camunda.bpm.engine.test.api.runtime
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;

	public class BatchModificationHelper : BatchHelper
	{

	  protected internal ProcessEngineTestRule testRule;
	  protected internal IList<string> currentProcessInstances;

	  public BatchModificationHelper(ProcessEngineRule engineRule) : base(engineRule)
	  {
		this.testRule = new ProcessEngineTestRule(engineRule);
		currentProcessInstances = new List<string>();
	  }

	  public virtual Batch startAfterAsync(string key, int numberOfProcessInstances, string activityId, string processDefinitionId)
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;

		IList<string> processInstanceIds = startInstances(key, numberOfProcessInstances);

		return runtimeService.createModification(processDefinitionId).startAfterActivity(activityId).processInstanceIds(processInstanceIds).executeAsync();
	  }

	  public virtual Batch startBeforeAsync(string key, int numberOfProcessInstances, string activityId, string processDefinitionId)
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;

		IList<string> processInstanceIds = startInstances(key, numberOfProcessInstances);

		return runtimeService.createModification(processDefinitionId).startBeforeActivity(activityId).processInstanceIds(processInstanceIds).executeAsync();
	  }

	  public virtual Batch startTransitionAsync(string key, int numberOfProcessInstances, string transitionId, string processDefinitionId)
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;

		IList<string> processInstanceIds = startInstances(key, numberOfProcessInstances);

		return runtimeService.createModification(processDefinitionId).startTransition(transitionId).processInstanceIds(processInstanceIds).executeAsync();
	  }

	  public virtual Batch cancelAllAsync(string key, int numberOfProcessInstances, string activityId, string processDefinitionId)
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;

		IList<string> processInstanceIds = startInstances(key, numberOfProcessInstances);

		return runtimeService.createModification(processDefinitionId).cancelAllForActivity(activityId).processInstanceIds(processInstanceIds).executeAsync();
	  }

	  public virtual IList<string> startInstances(string key, int numOfInstances)
	  {
		IList<string> instances = new List<string>();
		for (int i = 0; i < numOfInstances; i++)
		{
		  ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey(key);
		  instances.Add(processInstance.Id);
		}

		currentProcessInstances = instances;
		return instances;
	  }

	  public override JobDefinition getExecutionJobDefinition(Batch batch)
	  {
		return engineRule.ManagementService.createJobDefinitionQuery().jobDefinitionId(batch.BatchJobDefinitionId).jobType(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MODIFICATION).singleResult();
	  }

	}

}