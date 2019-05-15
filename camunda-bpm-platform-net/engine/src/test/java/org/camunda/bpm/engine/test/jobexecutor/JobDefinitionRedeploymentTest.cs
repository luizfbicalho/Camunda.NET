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
namespace org.camunda.bpm.engine.test.jobexecutor
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// Redeploy process definition and assert that no new job definitions were created.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class JobDefinitionRedeploymentTest
	public class JobDefinitionRedeploymentTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "{index}: process definition = {0}") public static java.util.Collection<Object[]> data()
		public static ICollection<object[]> data()
		{
		return Arrays.asList(new object[][]
		{
			new object[] {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testTimerStartEvent.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testTimerBoundaryEvent.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testMultipleTimerBoundaryEvents.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testEventBasedGateway.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testTimerIntermediateEvent.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testAsyncContinuation.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testAsyncContinuationOfMultiInstance.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testAsyncContinuationOfActivityWrappedInMultiInstance.bpmn20.xml"}
		});
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public String processDefinitionResource;
	  public string processDefinitionResource;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

	  protected internal ManagementService managementService;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		managementService = rule.ManagementService;
		repositoryService = rule.RepositoryService;
		runtimeService = rule.RuntimeService;
		processEngineConfiguration = (ProcessEngineConfigurationImpl) rule.ProcessEngine.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobDefinitionsAfterRedeploment()
	  public virtual void testJobDefinitionsAfterRedeploment()
	  {

		// initially there are no job definitions:
		assertEquals(0, managementService.createJobDefinitionQuery().count());

		// initial deployment
		string deploymentId = repositoryService.createDeployment().addClasspathResource(processDefinitionResource).deploy().Id;

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertNotNull(processDefinition);

		// this parses the process and created the Job definitions:
		IList<JobDefinition> jobDefinitions = managementService.createJobDefinitionQuery().list();
		ISet<string> jobDefinitionIds = getJobDefinitionIds(jobDefinitions);

		// now clear the cache:
		processEngineConfiguration.DeploymentCache.discardProcessDefinitionCache();

		// if we start an instance of the process, the process will be parsed again:
		runtimeService.startProcessInstanceByKey(processDefinition.Key);

		// no new definitions were created
		assertEquals(jobDefinitions.Count, managementService.createJobDefinitionQuery().count());

		// the job has the correct definitionId set:
		IList<Job> jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  assertTrue(jobDefinitionIds.Contains(job.JobDefinitionId));
		}

		// delete the deployment
		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  protected internal virtual ISet<string> getJobDefinitionIds(IList<JobDefinition> jobDefinitions)
	  {
		ISet<string> definitionIds = new HashSet<string>();
		foreach (JobDefinition definition in jobDefinitions)
		{
		  definitionIds.Add(definition.Id);
		}
		return definitionIds;
	  }

	}

}