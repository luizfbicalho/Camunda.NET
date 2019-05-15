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
namespace org.camunda.bpm.engine.test.api.authorization.batch.creation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;


	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using Test = org.junit.Test;
	using Parameterized = org.junit.runners.Parameterized;

	public class SetJobRetriesBatchAuthorizationTest : BatchCreationAuthorizationTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE), grant(Resources.BATCH, "batchId", "userId", BatchPermissions.CREATE_BATCH_SET_JOB_RETRIES)), scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE)), scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", BatchPermissions.CREATE_BATCH_SET_JOB_RETRIES)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchSetJobRetriesByJobs()
	  public virtual void testBatchSetJobRetriesByJobs()
	  {
		//given
		IList<string> jobIds = setupFailedJobs();
		authRule.init(scenario).withUser("userId").bindResource("batchId", "*").start();

		// when

		managementService.setJobRetriesAsync(jobIds, 5);

		// then
		authRule.assertScenario(scenario);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchSetJobRetriesByProcesses()
	  public virtual void testBatchSetJobRetriesByProcesses()
	  {
		//given
		setupFailedJobs();
		IList<string> processInstanceIds = Collections.singletonList(processInstance.Id);
		authRule.init(scenario).withUser("userId").bindResource("batchId", "*").start();

		// when

		managementService.setJobRetriesAsync(processInstanceIds, (ProcessInstanceQuery) null, 5);

		// then
		authRule.assertScenario(scenario);
	  }

	  protected internal virtual IList<string> setupFailedJobs()
	  {
		IList<string> jobIds = new List<string>();

		Deployment deploy = testHelper.deploy(JOB_EXCEPTION_DEFINITION_XML);
		ProcessDefinition sourceDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().deploymentId(deploy.Id).singleResult();
		processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		IList<Job> jobs = managementService.createJobQuery().processInstanceId(processInstance.Id).list();
		foreach (Job job in jobs)
		{
		  jobIds.Add(job.Id);
		}
		return jobIds;
	  }

	}

}