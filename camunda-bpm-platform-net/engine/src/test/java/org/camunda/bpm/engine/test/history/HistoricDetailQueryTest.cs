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
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TestPojo = org.camunda.bpm.engine.test.cmmn.decisiontask.TestPojo;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// 
	/// <summary>
	/// @author Svetlana Dorokhova
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricDetailQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricDetailQueryTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			chain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal const string PROCESS_KEY = "oneTaskProcess";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain chain;


	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;
	  protected internal TaskService taskService;
	  private IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
		taskService = engineRule.TaskService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryByUserOperationId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByUserOperationId()
	  {
		startProcessInstance(PROCESS_KEY);

		identityService.AuthenticatedUserId = "demo";

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.resolveTask(taskId, Variables);

		//then
		string userOperationId = historyService.createHistoricDetailQuery().singleResult().UserOperationId;

		HistoricDetailQuery query = historyService.createHistoricDetailQuery().userOperationId(userOperationId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryByInvalidUserOperationId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByInvalidUserOperationId()
	  {
		startProcessInstance(PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.resolveTask(taskId, Variables);

		//then
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().userOperationId("invalid");

		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  query.userOperationId(null);
		  fail("It was possible to set a null value as userOperationId.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryByExecutionId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByExecutionId()
	  {
		startProcessInstance(PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.resolveTask(taskId, Variables);

		//then
		string executionId = historyService.createHistoricDetailQuery().singleResult().ExecutionId;

		HistoricDetailQuery query = historyService.createHistoricDetailQuery().executionId(executionId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryByInvalidExecutionId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByInvalidExecutionId()
	  {
		startProcessInstance(PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.resolveTask(taskId, Variables);

		//then
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().executionId("invalid");

		assertEquals(0, query.list().size());
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableTypeIn()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableTypeIn()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["boolVar"] = true;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableTypeIn("string");

		// then
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
		HistoricDetail historicDetail = query.list().get(0);
		if (historicDetail is HistoricVariableUpdate)
		{
		  HistoricVariableUpdate variableUpdate = (HistoricVariableUpdate) historicDetail;
		  assertEquals(variableUpdate.VariableName, "stringVar");
		  assertEquals(variableUpdate.TypeName, "string");
		}
		else
		{
		  fail("Historic detail should be a variable update!");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableTypeInWithCapitalLetter()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableTypeInWithCapitalLetter()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["boolVar"] = true;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableTypeIn("Boolean");

		// then
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
		HistoricDetail historicDetail = query.list().get(0);
		if (historicDetail is HistoricVariableUpdate)
		{
		  HistoricVariableUpdate variableUpdate = (HistoricVariableUpdate) historicDetail;
		  assertEquals(variableUpdate.VariableName, "boolVar");
		  assertEquals(variableUpdate.TypeName, "boolean");
		}
		else
		{
		  fail("Historic detail should be a variable update!");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableTypeInWithSeveralTypes()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableTypeInWithSeveralTypes()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["boolVar"] = true;
		variables1["intVar"] = 5;
		variables1["nullVar"] = null;
		variables1["pojoVar"] = new TestPojo("str", .0);
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableTypeIn("boolean", "integer", "Serializable");

		// then
		assertEquals(3, query.list().size());
		assertEquals(3, query.count());
		ISet<string> allowedVariableTypes = new HashSet<string>();
		allowedVariableTypes.Add("boolean");
		allowedVariableTypes.Add("integer");
		allowedVariableTypes.Add("object");
		foreach (HistoricDetail detail in query.list())
		{
		  if (detail is HistoricVariableUpdate)
		  {
			HistoricVariableUpdate variableUpdate = (HistoricVariableUpdate) detail;
			assertTrue(allowedVariableTypes.Contains(variableUpdate.TypeName));
		  }
		  else
		  {
			fail("Historic detail should be a variable update!");
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByInvalidVariableTypeIn()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByInvalidVariableTypeIn()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["boolVar"] = true;
		variables1["intVar"] = 5;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableTypeIn("invalid");

		// then
		assertEquals(0, query.count());

		try
		{
		  // when
		  query.variableTypeIn(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // then fails
		}

		try
		{
		  // when
		  query.variableTypeIn((string)null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // then fails
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryBySingleProcessInstanceId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryBySingleProcessInstanceId()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates().processInstanceIdIn(processInstance.ProcessInstanceId);

		// then
		assertEquals(1, query.count());
		assertEquals(query.list().get(0).ProcessInstanceId, processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryBySeveralProcessInstanceIds()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryBySeveralProcessInstanceIds()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates().processInstanceIdIn(processInstance.ProcessInstanceId, processInstance2.ProcessInstanceId);

		// then
		ISet<string> expectedProcessInstanceIds = new HashSet<string>();
		expectedProcessInstanceIds.Add(processInstance.Id);
		expectedProcessInstanceIds.Add(processInstance2.Id);
		assertEquals(2, query.count());
		assertTrue(expectedProcessInstanceIds.Contains(query.list().get(0).ProcessInstanceId));
		assertTrue(expectedProcessInstanceIds.Contains(query.list().get(1).ProcessInstanceId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryByNonExistingProcessInstanceId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNonExistingProcessInstanceId()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().processInstanceIdIn("foo");

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByInvalidProcessInstanceIds()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByInvalidProcessInstanceIds()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables1);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		try
		{
		  // when
		  query.processInstanceIdIn(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // then fails
		}

		try
		{
		  // when
		  query.processInstanceIdIn((string)null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // then fails
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryByOccurredBefore()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByOccurredBefore()
	  {
		// given
		DateTime startTime = new DateTime();
		ClockUtil.CurrentTime = startTime;

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);
		DateTime hourFromNow = new DateTime();
		hourFromNow.AddHours(1);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		// then
		assertEquals(1, query.occurredBefore(hourFromNow).count());
		assertEquals(0, query.occurredBefore(hourAgo).count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryByOccurredAfter()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByOccurredAfter()
	  {
		// given
		DateTime startTime = new DateTime();
		ClockUtil.CurrentTime = startTime;

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);
		DateTime hourFromNow = new DateTime();
		hourFromNow.AddHours(1);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		// then
		assertEquals(0, query.occurredAfter(hourFromNow).count());
		assertEquals(1, query.occurredAfter(hourAgo).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryByOccurredAfterAndOccurredBefore()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByOccurredAfterAndOccurredBefore()
	  {
		// given
		DateTime startTime = new DateTime();
		ClockUtil.CurrentTime = startTime;

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);
		DateTime hourFromNow = new DateTime();
		hourFromNow.AddHours(1);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		// then
		assertEquals(0, query.occurredAfter(hourFromNow).occurredBefore(hourFromNow).count());
		assertEquals(1, query.occurredAfter(hourAgo).occurredBefore(hourFromNow).count());
		assertEquals(0, query.occurredAfter(hourFromNow).occurredBefore(hourAgo).count());
		assertEquals(0, query.occurredAfter(hourAgo).occurredBefore(hourAgo).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByInvalidOccurredBeforeDate()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByInvalidOccurredBeforeDate()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables1);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		try
		{
		  // when
		  query.occurredBefore(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // then fails
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByInvalidOccurredAfterDate()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByInvalidOccurredAfterDate()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, variables1);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		try
		{
		  // when
		  query.occurredAfter(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // then fails
		}
	  }

	  protected internal virtual VariableMap Variables
	  {
		  get
		  {
			return Variables.createVariables().putValue("aVariableName", "aVariableValue");
		  }
	  }

	  protected internal virtual void startProcessInstance(string key)
	  {
		startProcessInstances(key, 1);
	  }

	  protected internal virtual void startProcessInstances(string key, int numberOfInstances)
	  {
		for (int i = 0; i < numberOfInstances; i++)
		{
		  runtimeService.startProcessInstanceByKey(key);
		}

		testHelper.executeAvailableJobs();
	  }
	}

}