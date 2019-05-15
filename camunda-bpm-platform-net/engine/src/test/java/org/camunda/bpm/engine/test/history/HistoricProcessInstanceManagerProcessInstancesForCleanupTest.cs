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
namespace org.camunda.bpm.engine.test.history
{
	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class HistoricProcessInstanceManagerProcessInstancesForCleanupTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricProcessInstanceManagerProcessInstancesForCleanupTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricProcessInstanceManagerProcessInstancesForCleanupTest()
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


	  protected internal const string ONE_TASK_PROCESS = "oneTaskProcess";
	  protected internal const string TWO_TASKS_PROCESS = "twoTasksProcess";

	  public ProcessEngineRule engineRule = new ProcessEngineRule(true);
	  public ProcessEngineTestRule testRule;

	  private HistoryService historyService;
	  private RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(0) public int processDefiniotion1TTL;
	  public int processDefiniotion1TTL;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(1) public int processDefiniotion2TTL;
	  public int processDefiniotion2TTL;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(2) public int processInstancesOfProcess1Count;
	  public int processInstancesOfProcess1Count;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(3) public int processInstancesOfProcess2Count;
	  public int processInstancesOfProcess2Count;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(4) public int daysPassedAfterProcessEnd;
	  public int daysPassedAfterProcessEnd;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(5) public int batchSize;
	  public int batchSize;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(6) public int resultCount;
	  public int resultCount;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters public static java.util.Collection<Object[]> scenarios()
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {3, 5, 3, 7, 4, 50, 3},
			new object[] {3, 5, 3, 7, 2, 50, 0},
			new object[] {3, 5, 3, 7, 6, 50, 10},
			new object[] {3, 5, 3, 7, 6, 4, 4}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml" }) public void testFindHistoricProcessInstanceIdsForCleanup()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml" })]
	  public virtual void testFindHistoricProcessInstanceIdsForCleanup()
	  {

		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		//start processes
		IList<string> ids = prepareHistoricProcesses(ONE_TASK_PROCESS, processInstancesOfProcess1Count);
		((IList<string>)ids).AddRange(prepareHistoricProcesses(TWO_TASKS_PROCESS, processInstancesOfProcess2Count));

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//some days passed
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, daysPassedAfterProcessEnd);

		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));

	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly HistoricProcessInstanceManagerProcessInstancesForCleanupTest outerInstance;

		  public CommandAnonymousInnerClass(HistoricProcessInstanceManagerProcessInstancesForCleanupTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {

			//given
			//set different TTL for two process definition
			outerInstance.updateTimeToLive(commandContext, ONE_TASK_PROCESS, outerInstance.processDefiniotion1TTL);
			outerInstance.updateTimeToLive(commandContext, TWO_TASKS_PROCESS, outerInstance.processDefiniotion2TTL);
			return null;
		  }
	  }

	  private class CommandAnonymousInnerClass2 : Command<object>
	  {
		  private readonly HistoricProcessInstanceManagerProcessInstancesForCleanupTest outerInstance;

		  public CommandAnonymousInnerClass2(HistoricProcessInstanceManagerProcessInstancesForCleanupTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			//when
			IList<string> historicProcessInstanceIdsForCleanup = commandContext.HistoricProcessInstanceManager.findHistoricProcessInstanceIdsForCleanup(outerInstance.batchSize, 0, 60);

			//then
			assertEquals(outerInstance.resultCount, historicProcessInstanceIdsForCleanup.Count);

			if (outerInstance.resultCount > 0)
			{

			  IList<HistoricProcessInstance> historicProcessInstances = outerInstance.historyService.createHistoricProcessInstanceQuery().processInstanceIds(new HashSet<string>(historicProcessInstanceIdsForCleanup)).list();

			  foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
			  {
				assertNotNull(historicProcessInstance.EndTime);
				IList<ProcessDefinition> processDefinitions = outerInstance.engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionId(historicProcessInstance.ProcessDefinitionId).list();
				assertEquals(1, processDefinitions.Count);
				ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) processDefinitions[0];
				assertTrue(historicProcessInstance.EndTime < DateUtils.addDays(ClockUtil.CurrentTime, processDefinition.HistoryTimeToLive));
			  }
			}

			return null;
		  }
	  }

	  private void updateTimeToLive(CommandContext commandContext, string businessKey, int timeToLive)
	  {
		IList<ProcessDefinition> processDefinitions = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(businessKey).list();
		assertEquals(1, processDefinitions.Count);
		ProcessDefinitionEntity processDefinition1 = (ProcessDefinitionEntity) processDefinitions[0];
		processDefinition1.HistoryTimeToLive = timeToLive;
		commandContext.DbEntityManager.merge(processDefinition1);
	  }

	  private IList<string> prepareHistoricProcesses(string businessKey, int? processInstanceCount)
	  {
		IList<string> processInstanceIds = new List<string>();

		for (int i = 0; i < processInstanceCount.Value; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(businessKey);
		  processInstanceIds.Add(processInstance.Id);
		}

		return processInstanceIds;
	  }

	}

}