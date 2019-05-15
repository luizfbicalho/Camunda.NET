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
namespace org.camunda.bpm.engine.test.standalone.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.DB_SCHEMA_UPDATE_CREATE_DROP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class CustomHistoryLevelIdentityLinkTest
	public class CustomHistoryLevelIdentityLinkTest
	{
		private bool InstanceFieldsInitialized = false;

		public CustomHistoryLevelIdentityLinkTest()
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
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> data()
		public static ICollection<object[]> data()
		{
		return Arrays.asList(new object[][]
		{
			new object[]{Arrays.asList(HistoryEventTypes.IDENTITY_LINK_ADD)},
			new object[]{Arrays.asList(HistoryEventTypes.IDENTITY_LINK_DELETE, HistoryEventTypes.IDENTITY_LINK_ADD)}
		});
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public java.util.List<org.camunda.bpm.engine.impl.history.event.HistoryEventTypes> eventTypes;
	  public IList<HistoryEventTypes> eventTypes;

	  internal CustomHistoryLevelIdentityLink customHisstoryLevelIL = new CustomHistoryLevelIdentityLink();

	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl processEngineConfiguration)
		  {
			processEngineConfiguration.JdbcUrl = "jdbc:h2:mem:" + this.GetType().Name;
			IList<HistoryLevel> levels = new List<HistoryLevel>();
			levels.Add(outerInstance.customHisstoryLevelIL);
			processEngineConfiguration.CustomHistoryLevels = levels;
			processEngineConfiguration.History = "aCustomHistoryLevelIL";
			processEngineConfiguration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_CREATE_DROP;
			return processEngineConfiguration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal HistoryService historyService;
	  protected internal RuntimeService runtimeService;
	  protected internal IdentityService identityService;
	  protected internal RepositoryService repositoryService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		identityService = engineRule.IdentityService;
		repositoryService = engineRule.RepositoryService;
		taskService = engineRule.TaskService;

		customHisstoryLevelIL.EventTypes = eventTypes;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		customHisstoryLevelIL.EventTypes = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testDeletingIdentityLinkByProcDefId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeletingIdentityLinkByProcDefId()
	  {
		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		identityService.AuthenticatedUserId = "anAuthUser";
		taskService.addCandidateUser(taskId, "aUser");
		taskService.deleteCandidateUser(taskId, "aUser");

		// assume
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertTrue(historicIdentityLinks.Count > 0);

		// when
		repositoryService.deleteProcessDefinitions().byKey("oneTaskProcess").cascade().delete();

		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(0, historicIdentityLinks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeletingIdentityLinkByTaskId()
	  public virtual void testDeletingIdentityLinkByTaskId()
	  {
		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		Task task = taskService.newTask();
		taskService.saveTask(task);
		string taskId = task.Id;

		identityService.AuthenticatedUserId = "anAuthUser";
		taskService.addCandidateUser(taskId, "aUser");
		taskService.deleteCandidateUser(taskId, "aUser");

		// assume
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertTrue(historicIdentityLinks.Count > 0);

		// when
		taskService.deleteTask(taskId, true);

		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(0, historicIdentityLinks.Count);
	  }

	}

}