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
namespace org.camunda.bpm.engine.test.api.history
{
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessInstanceQueryTest = org.camunda.bpm.engine.test.api.runtime.ProcessInstanceQueryTest;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Assert = org.junit.Assert;
	using Logger = org.slf4j.Logger;


	/// <summary>
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoryServiceTest : PluggableProcessEngineTestCase
	{

	  public const string ONE_TASK_PROCESS = "oneTaskProcess";
	  protected internal static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceQuery()
	  {
		// With a clean ProcessEngine, no instances should be available
		assertTrue(historyService.createHistoricProcessInstanceQuery().count() == 0);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		assertTrue(historyService.createHistoricProcessInstanceQuery().count() == 1);

		// Complete the task and check if the size is count 1
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		assertEquals(1, tasks.Count);
		taskService.complete(tasks[0].Id);
		assertTrue(historyService.createHistoricProcessInstanceQuery().count() == 1);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceQueryOrderBy()
	  {
		// With a clean ProcessEngine, no instances should be available
		assertTrue(historyService.createHistoricProcessInstanceQuery().count() == 0);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		assertEquals(1, tasks.Count);
		taskService.complete(tasks[0].Id);

		historyService.createHistoricTaskInstanceQuery().orderByDeleteReason().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByExecutionId().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByHistoricActivityInstanceId().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByHistoricActivityInstanceStartTime().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByHistoricTaskInstanceDuration().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByHistoricTaskInstanceEndTime().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByProcessDefinitionId().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByProcessInstanceId().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByTaskAssignee().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByTaskDefinitionKey().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByTaskDescription().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByTaskId().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByTaskName().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByTaskOwner().asc().list();
		historyService.createHistoricTaskInstanceQuery().orderByTaskPriority().asc().list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") @Deployment(resources = {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testHistoricProcessInstanceUserIdAndActivityId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceUserIdAndActivityId()
	  {
		identityService.AuthenticatedUserId = "johndoe";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertEquals("johndoe", historicProcessInstance.StartUserId);
		assertEquals("theStart", historicProcessInstance.StartActivityId);

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		assertEquals(1, tasks.Count);
		taskService.complete(tasks[0].Id);

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertEquals("theEnd", historicProcessInstance.EndActivityId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/history/orderProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/checkCreditProcess.bpmn20.xml"})]
	  public virtual void testOrderProcessWithCallActivity()
	  {
		// After the process has started, the 'verify credit history' task should be
		// active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("orderProcess");
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task verifyCreditTask = taskQuery.singleResult();

		// Completing the task with approval, will end the subprocess and continue
		// the original process
		taskService.complete(verifyCreditTask.Id, CollectionUtil.singletonMap("creditApproved", true));
		Task prepareAndShipTask = taskQuery.singleResult();
		assertEquals("Prepare and Ship", prepareAndShipTask.Name);

		// verify
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(pi.Id).singleResult();
		assertNotNull(historicProcessInstance);
		assertTrue(historicProcessInstance.ProcessDefinitionId.Contains("checkCreditProcess"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/orderProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/checkCreditProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceQueryByProcessDefinitionKey()
	  {

		string processDefinitionKey = ONE_TASK_PROCESS;
		runtimeService.startProcessInstanceByKey(processDefinitionKey);
		runtimeService.startProcessInstanceByKey("orderProcess");
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(processDefinitionKey).singleResult();
		assertNotNull(historicProcessInstance);
		assertTrue(historicProcessInstance.ProcessDefinitionId.StartsWith(processDefinitionKey, StringComparison.Ordinal));
		assertEquals("theStart", historicProcessInstance.StartActivityId);

		// now complete the task to end the process instance
		Task task = taskService.createTaskQuery().processDefinitionKey("checkCreditProcess").singleResult();
		IDictionary<string, object> map = new Dictionary<string, object>();
		map["creditApproved"] = true;
		taskService.complete(task.Id, map);

		// and make sure the super process instance is set correctly on the
		// HistoricProcessInstance
		HistoricProcessInstance historicProcessInstanceSub = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("checkCreditProcess").singleResult();
		HistoricProcessInstance historicProcessInstanceSuper = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("orderProcess").singleResult();
		assertEquals(historicProcessInstanceSuper.Id, historicProcessInstanceSub.SuperProcessInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/otherOneTaskProcess.bpmn20.xml" })]
	  public virtual void testHistoricProcessInstanceQueryByProcessInstanceIds()
	  {
		HashSet<string> processInstanceIds = new HashSet<string>();
		for (int i = 0; i < 4; i++)
		{
		  processInstanceIds.Add(runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, i + "").Id);
		}
		processInstanceIds.Add(runtimeService.startProcessInstanceByKey("otherOneTaskProcess", "1").Id);

		// start an instance that will not be part of the query
		runtimeService.startProcessInstanceByKey("otherOneTaskProcess", "2");

		HistoricProcessInstanceQuery processInstanceQuery = historyService.createHistoricProcessInstanceQuery().processInstanceIds(processInstanceIds);
		assertEquals(5, processInstanceQuery.count());

		IList<HistoricProcessInstance> processInstances = processInstanceQuery.list();
		assertNotNull(processInstances);
		assertEquals(5, processInstances.Count);

		foreach (HistoricProcessInstance historicProcessInstance in processInstances)
		{
		  assertTrue(processInstanceIds.Contains(historicProcessInstance.Id));
		}

		// making a query that has contradicting conditions should succeed
		assertEquals(0, processInstanceQuery.processInstanceId("dummy").count());
	  }

	  public virtual void testHistoricProcessInstanceQueryByProcessInstanceIdsEmpty()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().processInstanceIds(new HashSet<string>());
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("Set of process instance ids is empty", re.Message);
		}
	  }

	  public virtual void testHistoricProcessInstanceQueryByProcessInstanceIdsNull()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().processInstanceIds(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("Set of process instance ids is null", re.Message);
		}
	  }

	  public virtual void testQueryByRootProcessInstances()
	  {
		// given
		string superProcess = "calling";
		string subProcess = "called";
		BpmnModelInstance callingInstance = ProcessModels.newModel(superProcess).startEvent().callActivity().calledElement(subProcess).endEvent().done();

		BpmnModelInstance calledInstance = ProcessModels.newModel(subProcess).startEvent().userTask().endEvent().done();

		deployment(callingInstance, calledInstance);
		string processInstanceId1 = runtimeService.startProcessInstanceByKey(superProcess).ProcessInstanceId;

		// when
		IList<HistoricProcessInstance> list = historyService.createHistoricProcessInstanceQuery().rootProcessInstances().list();

		// then
		assertEquals(1, list.Count);
		assertEquals(processInstanceId1, list[0].Id);
	  }

	  public virtual void testQueryByRootProcessInstancesAndSuperProcess()
	  {
		// when
		try
		{
		  historyService.createHistoricProcessInstanceQuery().rootProcessInstances().superProcessInstanceId("processInstanceId");

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("Invalid query usage: cannot set both rootProcessInstances and superProcessInstanceId"));
		}

		// when
		try
		{
		  historyService.createHistoricProcessInstanceQuery().superProcessInstanceId("processInstanceId").rootProcessInstances();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("Invalid query usage: cannot set both rootProcessInstances and superProcessInstanceId"));
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/concurrentExecution.bpmn20.xml"})]
	  public virtual void testHistoricVariableInstancesOnParallelExecution()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["rootValue"] = "test";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("concurrent", vars);

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		foreach (Task task in tasks)
		{
		  IDictionary<string, object> variables = new Dictionary<string, object>();
		  // set token local variable
		  LOG.debug("setting variables on task " + task.Id + ", execution " + task.ExecutionId);
		  runtimeService.setVariableLocal(task.ExecutionId, "parallelValue1", task.Name);
		  runtimeService.setVariableLocal(task.ExecutionId, "parallelValue2", "test");
		  taskService.complete(task.Id, variables);
		}
		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().variableValueEquals("rootValue", "test").count());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().variableValueEquals("parallelValue1", "Receive Payment").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().variableValueEquals("parallelValue1", "Ship Order").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().variableValueEquals("parallelValue2", "test").count());
	  }

	  /// <summary>
	  /// basically copied from <seealso cref="ProcessInstanceQueryTest"/>
	  /// </summary>
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryStringVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["stringVar"] = "abcdef";
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, vars);
		taskService.complete(taskService.createTaskQuery().processInstanceId(processInstance1.Id).singleResult().Id);

		vars = new Dictionary<string, object>();
		vars["stringVar"] = "abcdef";
		vars["stringVar2"] = "ghijkl";
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, vars);
		taskService.complete(taskService.createTaskQuery().processInstanceId(processInstance2.Id).singleResult().Id);

		vars = new Dictionary<string, object>();
		vars["stringVar"] = "azerty";
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, vars);
		taskService.complete(taskService.createTaskQuery().processInstanceId(processInstance3.Id).singleResult().Id);

		// Test EQUAL on single string variable, should result in 2 matches
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().variableValueEquals("stringVar", "abcdef");
		IList<HistoricProcessInstance> processInstances = query.list();
		Assert.assertNotNull(processInstances);
		Assert.assertEquals(2, processInstances.Count);

		// Test EQUAL on two string variables, should result in single match
		query = historyService.createHistoricProcessInstanceQuery().variableValueEquals("stringVar", "abcdef").variableValueEquals("stringVar2", "ghijkl");
		HistoricProcessInstance resultInstance = query.singleResult();
		Assert.assertNotNull(resultInstance);
		Assert.assertEquals(processInstance2.Id, resultInstance.Id);

		// Test NOT_EQUAL, should return only 1 resultInstance
		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("stringVar", "abcdef").singleResult();
		Assert.assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		// Test GREATER_THAN, should return only matching 'azerty'
		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueGreaterThan("stringVar", "abcdef").singleResult();
		Assert.assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueGreaterThan("stringVar", "z").singleResult();
		Assert.assertNull(resultInstance);

		// Test GREATER_THAN_OR_EQUAL, should return 3 results
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().variableValueGreaterThanOrEqual("stringVar", "abcdef").count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueGreaterThanOrEqual("stringVar", "z").count());

		// Test LESS_THAN, should return 2 results
		processInstances = historyService.createHistoricProcessInstanceQuery().variableValueLessThan("stringVar", "abcdeg").list();
		Assert.assertEquals(2, processInstances.Count);
		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueLessThan("stringVar", "abcdef").count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().variableValueLessThanOrEqual("stringVar", "z").count());

		// Test LESS_THAN_OR_EQUAL
		processInstances = historyService.createHistoricProcessInstanceQuery().variableValueLessThanOrEqual("stringVar", "abcdef").list();
		Assert.assertEquals(2, processInstances.Count);
		expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		assertEquals(3, historyService.createHistoricProcessInstanceQuery().variableValueLessThanOrEqual("stringVar", "z").count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueLessThanOrEqual("stringVar", "aa").count());

		// Test LIKE
		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueLike("stringVar", "azert%").singleResult();
		assertNotNull(resultInstance);
		assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueLike("stringVar", "%y").singleResult();
		assertNotNull(resultInstance);
		assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueLike("stringVar", "%zer%").singleResult();
		assertNotNull(resultInstance);
		assertEquals(processInstance3.Id, resultInstance.Id);

		assertEquals(3, historyService.createHistoricProcessInstanceQuery().variableValueLike("stringVar", "a%").count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueLike("stringVar", "%x%").count());

		historyService.deleteHistoricProcessInstance(processInstance1.Id);
		historyService.deleteHistoricProcessInstance(processInstance2.Id);
		historyService.deleteHistoricProcessInstance(processInstance3.Id);
	  }

	  /// <summary>
	  /// Only do one second type, as the logic is same as in <seealso cref="ProcessInstanceQueryTest"/> and I do not want to duplicate
	  /// all test case logic here.
	  /// Basically copied from <seealso cref="ProcessInstanceQueryTest"/>
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryDateVariable() throws Exception
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryDateVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		DateTime date1 = new DateTime();
		vars["dateVar"] = date1;

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, vars);
		taskService.complete(taskService.createTaskQuery().processInstanceId(processInstance1.Id).singleResult().Id);

		DateTime date2 = new DateTime();
		vars = new Dictionary<string, object>();
		vars["dateVar"] = date1;
		vars["dateVar2"] = date2;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, vars);
		taskService.complete(taskService.createTaskQuery().processInstanceId(processInstance2.Id).singleResult().Id);

		DateTime nextYear = new DateTime();
		nextYear.AddYears(1);
		vars = new Dictionary<string, object>();
		vars["dateVar"] = nextYear;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, vars);
		taskService.complete(taskService.createTaskQuery().processInstanceId(processInstance3.Id).singleResult().Id);

		DateTime nextMonth = new DateTime();
		nextMonth.AddMonths(1);

		DateTime twoYearsLater = new DateTime();
		twoYearsLater.AddYears(2);

		DateTime oneYearAgo = new DateTime();
		oneYearAgo.AddYears(-1);

		// Query on single short variable, should result in 2 matches
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().variableValueEquals("dateVar", date1);
		IList<HistoricProcessInstance> processInstances = query.list();
		Assert.assertNotNull(processInstances);
		Assert.assertEquals(2, processInstances.Count);

		// Query on two short variables, should result in single value
		query = historyService.createHistoricProcessInstanceQuery().variableValueEquals("dateVar", date1).variableValueEquals("dateVar2", date2);
		HistoricProcessInstance resultInstance = query.singleResult();
		Assert.assertNotNull(resultInstance);
		Assert.assertEquals(processInstance2.Id, resultInstance.Id);

		// Query with unexisting variable value
		DateTime unexistingDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/01/1989 12:00:00");
		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueEquals("dateVar", unexistingDate).singleResult();
		Assert.assertNull(resultInstance);

		// Test NOT_EQUALS
		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("dateVar", date1).singleResult();
		Assert.assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		// Test GREATER_THAN
		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueGreaterThan("dateVar", nextMonth).singleResult();
		Assert.assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueGreaterThan("dateVar", nextYear).count());
		Assert.assertEquals(3, historyService.createHistoricProcessInstanceQuery().variableValueGreaterThan("dateVar", oneYearAgo).count());

		// Test GREATER_THAN_OR_EQUAL
		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueGreaterThanOrEqual("dateVar", nextMonth).singleResult();
		Assert.assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = historyService.createHistoricProcessInstanceQuery().variableValueGreaterThanOrEqual("dateVar", nextYear).singleResult();
		Assert.assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(3, historyService.createHistoricProcessInstanceQuery().variableValueGreaterThanOrEqual("dateVar", oneYearAgo).count());

		// Test LESS_THAN
		processInstances = historyService.createHistoricProcessInstanceQuery().variableValueLessThan("dateVar", nextYear).list();
		Assert.assertEquals(2, processInstances.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueLessThan("dateVar", date1).count());
		Assert.assertEquals(3, historyService.createHistoricProcessInstanceQuery().variableValueLessThan("dateVar", twoYearsLater).count());

		// Test LESS_THAN_OR_EQUAL
		processInstances = historyService.createHistoricProcessInstanceQuery().variableValueLessThanOrEqual("dateVar", nextYear).list();
		Assert.assertEquals(3, processInstances.Count);

		Assert.assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueLessThanOrEqual("dateVar", oneYearAgo).count());

		historyService.deleteHistoricProcessInstance(processInstance1.Id);
		historyService.deleteHistoricProcessInstance(processInstance2.Id);
		historyService.deleteHistoricProcessInstance(processInstance3.Id);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testNativeHistoricProcessInstanceTest()
	  {
		// just test that the query will be constructed and executed, details are tested in the TaskQueryTest
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		assertEquals(1, historyService.createNativeHistoricProcessInstanceQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(HistoricProcessInstance))).count());
		assertEquals(1, historyService.createNativeHistoricProcessInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(HistoricProcessInstance))).list().size());
	//  assertEquals(1, historyService.createNativeHistoricProcessInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(HistoricProcessInstance.class)).listPage(0, 1).size());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testNativeHistoricTaskInstanceTest()
	  {
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		assertEquals(1, historyService.createNativeHistoricTaskInstanceQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(HistoricProcessInstance))).count());
		assertEquals(1, historyService.createNativeHistoricTaskInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(HistoricProcessInstance))).list().size());
		assertEquals(1, historyService.createNativeHistoricTaskInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(HistoricProcessInstance))).listPage(0, 1).size());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testNativeHistoricActivityInstanceTest()
	  {
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		assertEquals(1, historyService.createNativeHistoricActivityInstanceQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(HistoricProcessInstance))).count());
		assertEquals(1, historyService.createNativeHistoricActivityInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(HistoricProcessInstance))).list().size());
		assertEquals(1, historyService.createNativeHistoricActivityInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(HistoricProcessInstance))).listPage(0, 1).size());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testNativeHistoricVariableInstanceTest()
	  {
		DateTime date = new DateTime();
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["stringVar"] = "abcdef";
		vars["dateVar"] = date;
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, vars);

		assertEquals(2, historyService.createNativeHistoricVariableInstanceQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(HistoricVariableInstance))).count());
		assertEquals(1, historyService.createNativeHistoricVariableInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(HistoricVariableInstance))).listPage(0, 1).size());

		IList<HistoricVariableInstance> variables = historyService.createNativeHistoricVariableInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(HistoricVariableInstance))).list();
		assertEquals(2, variables.Count);
		foreach (HistoricVariableInstance variable in variables)
		{
		  assertTrue(vars.ContainsKey(variable.Name));
		  assertEquals(vars[variable.Name], variable.Value);
		  vars.Remove(variable.Name);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueEqualsNumber() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueEqualsNumber()
	  {
		// long
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", 123L));

		// non-matching long
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", 12345L));

		// short
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", (short) 123));

		// double
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", 123.0d));

		// integer
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", 123));

		// untyped null (should not match)
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", null));

		// typed null (should not match)
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", Variables.longValue(null)));

		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", "123"));

		assertEquals(4, historyService.createHistoricProcessInstanceQuery().variableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, historyService.createHistoricProcessInstanceQuery().variableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, historyService.createHistoricProcessInstanceQuery().variableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, historyService.createHistoricProcessInstanceQuery().variableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().variableValueEquals("var", Variables.numberValue(null)).count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());

		runtimeService.deleteProcessInstance(processInstance.Id, null);
		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());

		historyService.deleteHistoricProcessInstance(processInstance.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteRunningProcessInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		try
		{
		  historyService.deleteHistoricProcessInstance(processInstance.Id);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Process instance is still running, cannot delete historic process instance", ae.Message);
		}
	  }

	  public virtual void testDeleteProcessInstanceWithFake()
	  {
		try
		{
		  historyService.deleteHistoricProcessInstance("aFake");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("No historic process instance found with id", ae.Message);
		}
	  }

	  public virtual void testDeleteProcessInstanceIfExistsWithFake()
	  {
		  historyService.deleteHistoricProcessInstanceIfExists("aFake");
		  //don't expect exception
	  }

	  public virtual void testDeleteProcessInstanceNullId()
	  {
		try
		{
		  historyService.deleteHistoricProcessInstance(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("processInstanceId is null", ae.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstances()
	  {
		//given
		IList<string> ids = prepareHistoricProcesses();

		//when
		historyService.deleteHistoricProcessInstances(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesWithFake()
	  {
		//given
		IList<string> ids = prepareHistoricProcesses();
		ids.Add("aFake");

		try
		{
		  //when
		  historyService.deleteHistoricProcessInstances(ids);
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  //expected
		  Assert.assertThat(e.Message, CoreMatchers.containsString("No historic process instance found with id: [aFake]"));
		}

		//then expect no instance is deleted
		assertEquals(2, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesIfExistsWithFake()
	  {
		//given
		IList<string> ids = prepareHistoricProcesses();
		ids.Add("aFake");

		//when
		historyService.deleteHistoricProcessInstancesIfExists(ids);

		//then expect no exception and all instances are deleted
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesWithNull()
	  {
		try
		{
		  //when
		  historyService.deleteHistoricProcessInstances(null);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  //expected
		}
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteHistoricVariableAndDetails()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		string executionId = processInstance.Id;
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		runtimeService.setVariable(executionId, "myVariable", "testValue1");
		runtimeService.setVariable(executionId, "myVariable", "testValue2");
		runtimeService.setVariable(executionId, "myVariable", "testValue3");
		runtimeService.setVariable(executionId, "mySecondVariable", 5L);

		runtimeService.deleteProcessInstance(executionId, null);
		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());

		HistoricVariableInstanceQuery histVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("myVariable");
		HistoricVariableInstanceQuery secondHistVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("mySecondVariable");
		assertEquals(1, histVariableQuery.count());
		assertEquals(1, secondHistVariableQuery.count());

		string variableInstanceId = histVariableQuery.singleResult().Id;
		string secondVariableInstanceId = secondHistVariableQuery.singleResult().Id;
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(variableInstanceId);
		HistoricDetailQuery secondDetailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(secondVariableInstanceId);
		assertEquals(3, detailsQuery.count());
		assertEquals(1, secondDetailsQuery.count());

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		assertEquals(0, histVariableQuery.count());
		assertEquals(1, secondHistVariableQuery.count());
		assertEquals(0, detailsQuery.count());
		assertEquals(1, secondDetailsQuery.count());
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteHistoricVariableAndDetailsOnRunningInstance()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		string executionId = processInstance.Id;
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		runtimeService.setVariable(executionId, "myVariable", "testValue1");
		runtimeService.setVariable(executionId, "myVariable", "testValue2");
		runtimeService.setVariable(executionId, "myVariable", "testValue3");

		VariableInstanceQuery variableQuery = runtimeService.createVariableInstanceQuery().processInstanceIdIn(executionId).variableName("myVariable");
		assertEquals(1, variableQuery.count());
		assertEquals("testValue3", variableQuery.singleResult().Value);

		HistoricVariableInstanceQuery histVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("myVariable");
		assertEquals(1, histVariableQuery.count());

		string variableInstanceId = histVariableQuery.singleResult().Id;
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(variableInstanceId);
		assertEquals(3, detailsQuery.count());

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		assertEquals(0, histVariableQuery.count());
		assertEquals(0, detailsQuery.count());
		assertEquals(1, variableQuery.count());
		assertEquals("testValue3", variableQuery.singleResult().Value);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteHistoricVariableAndDetailsOnRunningInstanceAndSetAgain()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		string executionId = processInstance.Id;
		runtimeService.setVariable(executionId, "myVariable", "testValue1");
		runtimeService.setVariable(executionId, "myVariable", "testValue2");
		runtimeService.setVariable(executionId, "myVariable", "testValue3");

		VariableInstanceQuery variableQuery = runtimeService.createVariableInstanceQuery().processInstanceIdIn(executionId).variableName("myVariable");
		HistoricVariableInstanceQuery histVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("myVariable");

		string variableInstanceId = histVariableQuery.singleResult().Id;

		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(variableInstanceId);


		historyService.deleteHistoricVariableInstance(variableInstanceId);

		assertEquals(0, histVariableQuery.count());
		assertEquals(0, detailsQuery.count());
		assertEquals(1, variableQuery.count());
		assertEquals("testValue3", variableQuery.singleResult().Value);

		// when
		runtimeService.setVariable(executionId, "myVariable", "testValue4");

		// then
		assertEquals(1, histVariableQuery.count());
		assertEquals(1, detailsQuery.count());
		assertEquals(1, variableQuery.count());
		assertEquals("testValue4", variableQuery.singleResult().Value);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testDeleteHistoricVariableAndDetailsFromCase()
	  {
		// given
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;
		caseService.setVariable(caseInstanceId, "myVariable", 1);
		caseService.setVariable(caseInstanceId, "myVariable", 2);
		caseService.setVariable(caseInstanceId, "myVariable", 3);

		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().caseInstanceId(caseInstanceId).variableInstanceId(variableInstance.Id);
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(3, detailsQuery.count());

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		// then
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(0, detailsQuery.count());
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testDeleteHistoricVariableAndDetailsFromCaseAndSetAgain()
	  {
		// given
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;
		caseService.setVariable(caseInstanceId, "myVariable", 1);
		caseService.setVariable(caseInstanceId, "myVariable", 2);
		caseService.setVariable(caseInstanceId, "myVariable", 3);

		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().caseInstanceId(caseInstanceId).variableInstanceId(variableInstance.Id);
		historyService.deleteHistoricVariableInstance(variableInstance.Id);
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(0, detailsQuery.count());

		// when
		caseService.setVariable(caseInstanceId, "myVariable", 4);

		// then
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(1, detailsQuery.count());
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testDeleteHistoricVariableAndDetailsFromStandaloneTask()
	  {
		// given
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariable(task.Id, "testVariable", "testValue");
		taskService.setVariable(task.Id, "testVariable", "testValue2");
		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().taskId(task.Id).variableInstanceId(variableInstance.Id);
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(2, detailsQuery.count());

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		// then
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(0, detailsQuery.count());

		taskService.deleteTask(task.Id, true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testDeleteHistoricVariableAndDetailsFromStandaloneTaskAndSetAgain()
	  {
		// given
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariable(task.Id, "testVariable", "testValue");
		taskService.setVariable(task.Id, "testVariable", "testValue2");

		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().taskId(task.Id).variableInstanceId(variableInstance.Id);

		historyService.deleteHistoricVariableInstance(variableInstance.Id);
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(0, detailsQuery.count());

		// when
		taskService.setVariable(task.Id, "testVariable", "testValue3");

		// then
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(1, detailsQuery.count());

		taskService.deleteTask(task.Id, true);
	  }

	  public virtual void testDeleteUnknownHistoricVariable()
	  {
		try
		{
		  // when
		  historyService.deleteHistoricVariableInstance("fakeID");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  // then
		  assertTextPresent("No historic variable instance found with id: fakeID", ae.Message);
		}
	  }

	  public virtual void testDeleteHistoricVariableWithNull()
	  {
		try
		{
		  // when
		  historyService.deleteHistoricVariableInstance(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  // then
		  assertTextPresent("variableInstanceId is null", ae.Message);
		}
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteAllHistoricVariablesAndDetails()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		string executionId = processInstance.Id;
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		runtimeService.setVariable(executionId, "myVariable", "testValue1");
		runtimeService.setVariable(executionId, "myVariable", "testValue2");
		runtimeService.setVariable(executionId, "myVariable", "testValue3");
		runtimeService.setVariable(executionId, "mySecondVariable", 5L);
		runtimeService.setVariable(executionId, "mySecondVariable", 7L);

		runtimeService.deleteProcessInstance(executionId, null);
		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());

		HistoricVariableInstanceQuery histVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("myVariable");
		HistoricVariableInstanceQuery secondHistVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("mySecondVariable");
		assertEquals(1, histVariableQuery.count());
		assertEquals(1, secondHistVariableQuery.count());

		string variableInstanceId = histVariableQuery.singleResult().Id;
		string secondVariableInstanceId = secondHistVariableQuery.singleResult().Id;
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(variableInstanceId);
		HistoricDetailQuery secondDetailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(secondVariableInstanceId);
		assertEquals(3, detailsQuery.count());
		assertEquals(2, secondDetailsQuery.count());

		// when
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(executionId);

		// then
		assertEquals(0, histVariableQuery.count());
		assertEquals(0, secondHistVariableQuery.count());
		assertEquals(0, detailsQuery.count());
		assertEquals(0, secondDetailsQuery.count());
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteAllHistoricVariablesAndDetailsOnRunningInstance()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		string executionId = processInstance.Id;
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		runtimeService.setVariable(executionId, "myVariable", "testValue1");
		runtimeService.setVariable(executionId, "myVariable", "testValue2");
		runtimeService.setVariable(executionId, "myVariable", "testValue3");
		runtimeService.setVariable(executionId, "mySecondVariable", "testValue1");
		runtimeService.setVariable(executionId, "mySecondVariable", "testValue2");

		VariableInstanceQuery variableQuery = runtimeService.createVariableInstanceQuery().processInstanceIdIn(executionId).variableName("myVariable");
		VariableInstanceQuery secondVariableQuery = runtimeService.createVariableInstanceQuery().processInstanceIdIn(executionId).variableName("mySecondVariable");
		assertEquals(1L, variableQuery.count());
		assertEquals(1L, secondVariableQuery.count());
		assertEquals("testValue3", variableQuery.singleResult().Value);
		assertEquals("testValue2", secondVariableQuery.singleResult().Value);

		HistoricVariableInstanceQuery histVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("myVariable");
		HistoricVariableInstanceQuery secondHistVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("mySecondVariable");
		assertEquals(1L, histVariableQuery.count());
		assertEquals(1L, secondHistVariableQuery.count());

		string variableInstanceId = histVariableQuery.singleResult().Id;
		string secondVariableInstanceId = secondHistVariableQuery.singleResult().Id;
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(variableInstanceId);
		HistoricDetailQuery secondDetailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(secondVariableInstanceId);
		assertEquals(3L, detailsQuery.count());
		assertEquals(2L, secondDetailsQuery.count());

		// when
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(executionId);

		// then
		HistoricVariableInstanceQuery allHistVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId);
		HistoricDetailQuery allDetailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId);
		assertEquals(0L, histVariableQuery.count());
		assertEquals(0L, secondHistVariableQuery.count());
		assertEquals(0L, allHistVariableQuery.count());
		assertEquals(0L, detailsQuery.count());
		assertEquals(0L, secondDetailsQuery.count());
		assertEquals(0L, allDetailsQuery.count());
		assertEquals(1L, variableQuery.count());
		assertEquals("testValue3", variableQuery.singleResult().Value);
		assertEquals("testValue2", secondVariableQuery.singleResult().Value);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteAllHistoricVariablesAndDetailsOnRunningInstanceAndSetAgain()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		string executionId = processInstance.Id;
		runtimeService.setVariable(executionId, "myVariable", "testValue1");
		runtimeService.setVariable(executionId, "myVariable", "testValue2");
		runtimeService.setVariable(executionId, "mySecondVariable", "testValue1");
		runtimeService.setVariable(executionId, "mySecondVariable", "testValue2");

		historyService.deleteHistoricVariableInstancesByProcessInstanceId(executionId);

		VariableInstanceQuery variableQuery = runtimeService.createVariableInstanceQuery().processInstanceIdIn(executionId).variableName("myVariable");
		VariableInstanceQuery secondVariableQuery = runtimeService.createVariableInstanceQuery().processInstanceIdIn(executionId).variableName("mySecondVariable");
		HistoricVariableInstanceQuery allHistVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId);
		HistoricDetailQuery allDetailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId);
		assertEquals(0L, allHistVariableQuery.count());
		assertEquals(0L, allDetailsQuery.count());
		assertEquals(1L, variableQuery.count());
		assertEquals(1L, secondVariableQuery.count());
		assertEquals("testValue2", variableQuery.singleResult().Value);
		assertEquals("testValue2", secondVariableQuery.singleResult().Value);

		// when
		runtimeService.setVariable(executionId, "myVariable", "testValue3");
		runtimeService.setVariable(executionId, "mySecondVariable", "testValue3");

		// then
		HistoricVariableInstanceQuery histVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("myVariable");
		HistoricVariableInstanceQuery secondHistVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId).variableName("mySecondVariable");
		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(histVariableQuery.singleResult().Id);
		HistoricDetailQuery secondDetailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId).variableInstanceId(secondHistVariableQuery.singleResult().Id);
		assertEquals(1L, histVariableQuery.count());
		assertEquals(1L, secondHistVariableQuery.count());
		assertEquals(2L, allHistVariableQuery.count());
		assertEquals(1L, detailsQuery.count());
		assertEquals(1L, secondDetailsQuery.count());
		assertEquals(2L, allDetailsQuery.count());
		assertEquals(1L, variableQuery.count());
		assertEquals(1L, secondVariableQuery.count());
		assertEquals("testValue3", variableQuery.singleResult().Value);
		assertEquals("testValue3", secondVariableQuery.singleResult().Value);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteAllHistoricVariablesOnEmpty()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		string executionId = processInstance.Id;
		assertEquals(1L, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());

		runtimeService.deleteProcessInstance(executionId, null);
		assertEquals(0L, runtimeService.createProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(1L, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());

		HistoricVariableInstanceQuery histVariableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(executionId);
		assertEquals(0L, histVariableQuery.count());

		HistoricDetailQuery detailsQuery = historyService.createHistoricDetailQuery().processInstanceId(executionId);
		assertEquals(0L, detailsQuery.count());

		// when
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(executionId);

		// then
		assertEquals(0, histVariableQuery.count());
		assertEquals(0, detailsQuery.count());
	  }

	  public virtual void testDeleteAllHistoricVariablesOnUnkownProcessInstance()
	  {
		try
		{
		  // when
		  historyService.deleteHistoricVariableInstancesByProcessInstanceId("fakeID");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  // then
		  assertTextPresent("No historic process instance found with id: fakeID", ae.Message);
		}
	  }

	  public virtual void testDeleteAllHistoricVariablesWithNull()
	  {
		try
		{
		  // when
		  historyService.deleteHistoricVariableInstancesByProcessInstanceId(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  // then
		  assertTextPresent("processInstanceId is null", ae.Message);
		}
	  }

	  protected internal virtual IList<string> prepareHistoricProcesses()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS);

		IList<string> processInstanceIds = new List<string>(Arrays.asList(new string[]{processInstance.Id, processInstance2.Id}));
		runtimeService.deleteProcessInstances(processInstanceIds, null, true, true);

		return processInstanceIds;
	  }
	}

}