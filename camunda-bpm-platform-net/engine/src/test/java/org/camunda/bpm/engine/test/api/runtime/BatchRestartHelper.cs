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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;


	public class BatchRestartHelper : BatchHelper
	{

	  public BatchRestartHelper(ProcessEngineRule engineRule) : base(engineRule)
	  {
	  }

	  public BatchRestartHelper(PluggableProcessEngineTestCase testCase) : base(testCase)
	  {
	  }

	  public override JobDefinition getExecutionJobDefinition(Batch batch)
	  {
		return ManagementService.createJobDefinitionQuery().jobDefinitionId(batch.BatchJobDefinitionId).jobType(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_RESTART).singleResult();
	  }


	  public override void executeJob(Job job)
	  {
		assertNotNull("Job to execute does not exist", job);
		ManagementService.executeJob(job.Id);
	  }
	}

}