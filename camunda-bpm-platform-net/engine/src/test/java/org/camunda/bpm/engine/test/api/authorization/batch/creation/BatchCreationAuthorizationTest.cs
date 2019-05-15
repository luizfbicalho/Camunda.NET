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

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public abstract class BatchCreationAuthorizationTest
	public abstract class BatchCreationAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchCreationAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal const string TEST_REASON = "test reason";
	  protected internal const string JOB_EXCEPTION_DEFINITION_XML = "org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testHelper;

	  protected internal ProcessInstance processInstance;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployProcesses()
	  public virtual void deployProcesses()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanBatch()
	  public virtual void cleanBatch()
	  {
		Batch batch = engineRule.ManagementService.createBatchQuery().singleResult();
		if (batch != null)
		{
		  engineRule.ManagementService.deleteBatch(batch.Id, true);
		}

		HistoricBatch historicBatch = engineRule.HistoryService.createHistoricBatchQuery().singleResult();
		if (historicBatch != null)
		{
		  engineRule.HistoryService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

	  protected internal virtual IList<string> setupHistory()
	  {
		runtimeService.deleteProcessInstance(processInstance.Id, null);
		IList<string> historicProcessInstances = new List<string>();

		foreach (HistoricProcessInstance hpi in historyService.createHistoricProcessInstanceQuery().list())
		{
		  historicProcessInstances.Add(hpi.Id);
		}
		return historicProcessInstances;
	  }

	}

}