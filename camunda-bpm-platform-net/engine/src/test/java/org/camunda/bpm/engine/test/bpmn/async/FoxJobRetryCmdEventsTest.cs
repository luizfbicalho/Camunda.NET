using System;
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
namespace org.camunda.bpm.engine.test.bpmn.async
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.async.RetryCmdDeployment.deployment;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.async.RetryCmdDeployment.prepareCompensationEventProcess;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.async.RetryCmdDeployment.prepareEscalationEventProcess;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.async.RetryCmdDeployment.prepareMessageEventProcess;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.async.RetryCmdDeployment.prepareSignalEventProcess;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class FoxJobRetryCmdEventsTest
	public class FoxJobRetryCmdEventsTest
	{
		private bool InstanceFieldsInitialized = false;

		public FoxJobRetryCmdEventsTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  public ProcessEngineRule engineRule = new ProcessEngineRule(true);
	  public ProcessEngineTestRule testRule;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public RetryCmdDeployment deployment;
	  public RetryCmdDeployment deployment;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "deployment {index}") public static java.util.Collection<RetryCmdDeployment[]> scenarios()
	  public static ICollection<RetryCmdDeployment[]> scenarios()
	  {
		return RetryCmdDeployment.asParameters(deployment().withEventProcess(prepareSignalEventProcess()), deployment().withEventProcess(prepareMessageEventProcess()), deployment().withEventProcess(prepareEscalationEventProcess()), deployment().withEventProcess(prepareCompensationEventProcess()));
	  }

	  private Deployment currentDeployment;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		currentDeployment = testRule.deploy(deployment.BpmnModelInstances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailedIntermediateThrowingSignalEventAsync()
	  public virtual void testFailedIntermediateThrowingSignalEventAsync()
	  {
		ProcessInstance pi = engineRule.RuntimeService.startProcessInstanceByKey(RetryCmdDeployment.PROCESS_ID);
		assertJobRetries(pi);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		engineRule.RepositoryService.deleteDeployment(currentDeployment.Id,true,true);
	  }

	  protected internal virtual void assertJobRetries(ProcessInstance pi)
	  {
		assertThat(pi,@is(notNullValue()));

		Job job = fetchJob(pi.ProcessInstanceId);

		try
		{
		  engineRule.ManagementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		}

		// update job
		job = fetchJob(pi.ProcessInstanceId);
		assertThat(job.Retries,@is(4));
	  }

	  protected internal virtual Job fetchJob(string processInstanceId)
	  {
		return engineRule.ManagementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
	  }


	}

}