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
namespace org.camunda.bpm.engine.test.api.runtime.migration.batch
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;

	public class BatchMigrationHelper : BatchHelper
	{

	  protected internal MigrationTestRule migrationRule;

	  public ProcessDefinition sourceProcessDefinition;
	  public ProcessDefinition targetProcessDefinition;

	  public BatchMigrationHelper(ProcessEngineRule engineRule, MigrationTestRule migrationRule) : base(engineRule)
	  {
		this.migrationRule = migrationRule;
	  }

	  public BatchMigrationHelper(ProcessEngineRule engineRule) : this(engineRule, null)
	  {
	  }

	  public virtual ProcessDefinition SourceProcessDefinition
	  {
		  get
		  {
			return sourceProcessDefinition;
		  }
	  }

	  public virtual ProcessDefinition TargetProcessDefinition
	  {
		  get
		  {
			return targetProcessDefinition;
		  }
	  }

	  public virtual Batch createMigrationBatchWithSize(int batchSize)
	  {
		int invocationsPerBatchJob = ((ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration).InvocationsPerBatchJob;
		return migrateProcessInstancesAsync(invocationsPerBatchJob * batchSize);
	  }

	  public virtual Batch migrateProcessInstancesAsync(int numberOfProcessInstances)
	  {
		sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		targetProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		return migrateProcessInstancesAsync(numberOfProcessInstances, sourceProcessDefinition, targetProcessDefinition);
	  }

	  public virtual Batch migrateProcessInstancesAsyncForTenant(int numberOfProcessInstances, string tenantId)
	  {
		sourceProcessDefinition = migrationRule.deployForTenantAndGetDefinition(tenantId, ProcessModels.ONE_TASK_PROCESS);
		targetProcessDefinition = migrationRule.deployForTenantAndGetDefinition(tenantId, ProcessModels.ONE_TASK_PROCESS);
		return migrateProcessInstancesAsync(numberOfProcessInstances, sourceProcessDefinition, targetProcessDefinition);
	  }

	  public virtual Batch migrateProcessInstanceAsync(ProcessDefinition sourceProcessDefinition, ProcessDefinition targetProcessDefinition)
	  {
		return migrateProcessInstancesAsync(1, sourceProcessDefinition, targetProcessDefinition);
	  }

	  public virtual Batch migrateProcessInstancesAsync(int numberOfProcessInstances, ProcessDefinition sourceProcessDefinition, ProcessDefinition targetProcessDefinition)
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;

		IList<string> processInstanceIds = new List<string>(numberOfProcessInstances);
		for (int i = 0; i < numberOfProcessInstances; i++)
		{
		  processInstanceIds.Add(runtimeService.startProcessInstanceById(sourceProcessDefinition.Id).Id);
		}

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		return runtimeService.newMigration(migrationPlan).processInstanceIds(processInstanceIds).executeAsync();
	  }

	  public override JobDefinition getExecutionJobDefinition(Batch batch)
	  {
		return engineRule.ManagementService.createJobDefinitionQuery().jobDefinitionId(batch.BatchJobDefinitionId).jobType(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MIGRATION).singleResult();
	  }

	  public virtual long countSourceProcessInstances()
	  {
		return engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id).count();
	  }

	  public virtual long countTargetProcessInstances()
	  {
		return engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionId(targetProcessDefinition.Id).count();
	  }
	}

}