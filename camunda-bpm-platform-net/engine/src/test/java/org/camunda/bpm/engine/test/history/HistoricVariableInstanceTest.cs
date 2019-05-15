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
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using org.camunda.bpm.engine.history;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using org.camunda.bpm.engine.runtime;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using CustomSerializable = org.camunda.bpm.engine.test.api.runtime.util.CustomSerializable;
	using FailingSerializable = org.camunda.bpm.engine.test.api.runtime.util.FailingSerializable;
	using TestPojo = org.camunda.bpm.engine.test.cmmn.decisiontask.TestPojo;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	/// <summary>
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricVariableInstanceTest : PluggableProcessEngineTestCase
	{
		[Deployment(resources:{ "org/camunda/bpm/engine/test/history/orderProcess.bpmn20.xml", "org/camunda/bpm/engine/test/history/checkCreditProcess.bpmn20.xml" })]
		public virtual void testOrderProcessWithCallActivity()
		{
		// After the process has started, the 'verify credit history' task should be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("orderProcess");
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task verifyCreditTask = taskQuery.singleResult();
		assertEquals("Verify credit history", verifyCreditTask.Name);

		// Verify with Query API
		ProcessInstance subProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(pi.Id).singleResult();
		assertNotNull(subProcessInstance);
		assertEquals(pi.Id, runtimeService.createProcessInstanceQuery().subProcessInstanceId(subProcessInstance.Id).singleResult().Id);

		// Completing the task with approval, will end the subprocess and continue the original process
		taskService.complete(verifyCreditTask.Id, CollectionUtil.singletonMap("creditApproved", true));
		Task prepareAndShipTask = taskQuery.singleResult();
		assertEquals("Prepare and Ship", prepareAndShipTask.Name);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimple()
	  public virtual void testSimple()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("myProc");
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task userTask = taskQuery.singleResult();
		assertEquals("userTask1", userTask.Name);

		taskService.complete(userTask.Id, CollectionUtil.singletonMap("myVar", "test789"));

		assertProcessEnded(processInstance.Id);

		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(1, variables.Count);

		HistoricVariableInstanceEntity historicVariable = (HistoricVariableInstanceEntity) variables[0];
		assertEquals("test456", historicVariable.TextValue);

		assertEquals(5, historyService.createHistoricActivityInstanceQuery().count());

		if (FullHistoryEnabled)
		{
		  assertEquals(3, historyService.createHistoricDetailQuery().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleNoWaitState()
	  public virtual void testSimpleNoWaitState()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("myProc");
		assertProcessEnded(processInstance.Id);

		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(1, variables.Count);

		HistoricVariableInstanceEntity historicVariable = (HistoricVariableInstanceEntity) variables[0];
		assertEquals("test456", historicVariable.TextValue);

		assertEquals(4, historyService.createHistoricActivityInstanceQuery().count());

		if (FullHistoryEnabled)
		{
		  assertEquals(2, historyService.createHistoricDetailQuery().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallel()
	  public virtual void testParallel()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("myProc");
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task userTask = taskQuery.singleResult();
		assertEquals("userTask1", userTask.Name);

		taskService.complete(userTask.Id, CollectionUtil.singletonMap("myVar", "test789"));

		assertProcessEnded(processInstance.Id);

		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().orderByVariableName().asc().list();
		assertEquals(2, variables.Count);

		HistoricVariableInstanceEntity historicVariable = (HistoricVariableInstanceEntity) variables[0];
		assertEquals("myVar", historicVariable.Name);
		assertEquals("test789", historicVariable.TextValue);

		HistoricVariableInstanceEntity historicVariable1 = (HistoricVariableInstanceEntity) variables[1];
		assertEquals("myVar1", historicVariable1.Name);
		assertEquals("test456", historicVariable1.TextValue);

		assertEquals(8, historyService.createHistoricActivityInstanceQuery().count());

		if (FullHistoryEnabled)
		{
		  assertEquals(5, historyService.createHistoricDetailQuery().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelNoWaitState()
	  public virtual void testParallelNoWaitState()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("myProc");
		assertProcessEnded(processInstance.Id);

		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(1, variables.Count);

		HistoricVariableInstanceEntity historicVariable = (HistoricVariableInstanceEntity) variables[0];
		assertEquals("test456", historicVariable.TextValue);

		assertEquals(7, historyService.createHistoricActivityInstanceQuery().count());

		if (FullHistoryEnabled)
		{
		  assertEquals(2, historyService.createHistoricDetailQuery().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTwoSubProcessInParallelWithinSubProcess()
	  public virtual void testTwoSubProcessInParallelWithinSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoSubProcessInParallelWithinSubProcess");
		assertProcessEnded(processInstance.Id);

		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().orderByVariableName().asc().list();
		assertEquals(2, variables.Count);

		HistoricVariableInstanceEntity historicVariable = (HistoricVariableInstanceEntity) variables[0];
		assertEquals("myVar", historicVariable.Name);
		assertEquals("test101112", historicVariable.TextValue);
		assertEquals("string", historicVariable.VariableTypeName);
		assertEquals("string", historicVariable.TypeName);

		HistoricVariableInstanceEntity historicVariable1 = (HistoricVariableInstanceEntity) variables[1];
		assertEquals("myVar1", historicVariable1.Name);
		assertEquals("test789", historicVariable1.TextValue);
		assertEquals("string", historicVariable1.VariableTypeName);
		assertEquals("string", historicVariable1.TypeName);

		assertEquals(18, historyService.createHistoricActivityInstanceQuery().count());

		if (FullHistoryEnabled)
		{
		  assertEquals(7, historyService.createHistoricDetailQuery().count());
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testCallSimpleSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/history/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testHistoricVariableInstanceQuery()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSimpleSubProcess");
		assertProcessEnded(processInstance.Id);

		assertEquals(5, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(5, historyService.createHistoricVariableInstanceQuery().list().size());
		assertEquals(5, historyService.createHistoricVariableInstanceQuery().orderByProcessInstanceId().asc().count());
		assertEquals(5, historyService.createHistoricVariableInstanceQuery().orderByProcessInstanceId().asc().list().size());
		assertEquals(5, historyService.createHistoricVariableInstanceQuery().orderByVariableName().asc().count());
		assertEquals(5, historyService.createHistoricVariableInstanceQuery().orderByVariableName().asc().list().size());

		assertEquals(2, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).list().size());
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().variableName("myVar").count());
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().variableName("myVar").list().size());
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().variableNameLike("myVar1").count());
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().variableNameLike("myVar1").list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableNameLike("my\\_Var%").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableNameLike("my\\_Var%").list().size());

		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(5, variables.Count);

		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("myVar", "test123").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("myVar", "test123").list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("myVar1", "test456").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("myVar1", "test456").list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("myVar", "test666").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("myVar", "test666").list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("myVar1", "test666").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("myVar1", "test666").list().size());

		assertEquals(8, historyService.createHistoricActivityInstanceQuery().count());

		if (FullHistoryEnabled)
		{
		  assertEquals(6, historyService.createHistoricDetailQuery().count());
		}

		// non-existing id:
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().variableId("non-existing").count());

		// existing-id
		IList<HistoricVariableInstance> variable = historyService.createHistoricVariableInstanceQuery().listPage(0, 1);
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableId(variable[0].Id).count());

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testCallSubProcessSettingVariableOnStart.bpmn20.xml", "org/camunda/bpm/engine/test/history/subProcessSetVariableOnStart.bpmn20.xml" })]
	  public virtual void testCallSubProcessSettingVariableOnStart()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSubProcess");
		assertProcessEnded(processInstance.Id);

		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());

		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("aVariable", "aValue").count());
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testHistoricProcessVariableOnDeletion()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["testVar"] = "Hallo Christian";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		runtimeService.deleteProcessInstance(processInstance.Id, "deleted");
		assertProcessEnded(processInstance.Id);

		// check that process variable is set even if the process is canceled and not ended normally
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableValueEquals("testVar", "Hallo Christian").count());
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testVariableUpdatesAreLinkedToActivity.bpmn20.xml"}) public void testVariableUpdatesLinkedToActivity() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testVariableUpdatesAreLinkedToActivity.bpmn20.xml"})]
	  public virtual void testVariableUpdatesLinkedToActivity()
	  {
		if (FullHistoryEnabled)
		{
		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("ProcessWithSubProcess");

		  Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		  IDictionary<string, object> variables = new Dictionary<string, object>();
		  variables["test"] = "1";
		  taskService.complete(task.Id, variables);

		  // now we are in the subprocess
		  task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		  variables.Clear();
		  variables["test"] = "2";
		  taskService.complete(task.Id, variables);

		  // now we are ended
		  assertProcessEnded(pi.Id);

		  // check history
		  IList<HistoricDetail> updates = historyService.createHistoricDetailQuery().variableUpdates().list();
		  assertEquals(2, updates.Count);

		  IDictionary<string, HistoricVariableUpdate> updatesMap = new Dictionary<string, HistoricVariableUpdate>();
		  HistoricVariableUpdate update = (HistoricVariableUpdate) updates[0];
		  updatesMap[(string) update.Value] = update;
		  update = (HistoricVariableUpdate) updates[1];
		  updatesMap[(string) update.Value] = update;

		  HistoricVariableUpdate update1 = updatesMap["1"];
		  HistoricVariableUpdate update2 = updatesMap["2"];

		  assertNotNull(update1.ActivityInstanceId);
		  assertNotNull(update1.ExecutionId);
		  HistoricActivityInstance historicActivityInstance1 = historyService.createHistoricActivityInstanceQuery().activityInstanceId(update1.ActivityInstanceId).singleResult();
		  assertEquals(historicActivityInstance1.ExecutionId, update1.ExecutionId);
		  assertEquals("usertask1", historicActivityInstance1.ActivityId);

		  // TODO http://jira.codehaus.org/browse/ACT-1083
		  assertNotNull(update2.ActivityInstanceId);
		  HistoricActivityInstance historicActivityInstance2 = historyService.createHistoricActivityInstanceQuery().activityInstanceId(update2.ActivityInstanceId).singleResult();
		  assertEquals("usertask2", historicActivityInstance2.ActivityId);

		/*
		 * This is OK! The variable is set on the root execution, on a execution never run through the activity, where the process instances
		 * stands when calling the set Variable. But the ActivityId of this flow node is used. So the execution id's doesn't have to be equal.
		 *
		 * execution id: On which execution it was set
		 * activity id: in which activity was the process instance when setting the variable
		 */
		  assertFalse(historicActivityInstance2.ExecutionId.Equals(update2.ExecutionId));
		}
	  }

	  // Test for ACT-1528, which (correctly) reported that deleting any
	  // historic process instance would remove ALL historic variables.
	  // Yes. Real serious bug.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricProcessInstanceDeleteCascadesCorrectly()
	  public virtual void testHistoricProcessInstanceDeleteCascadesCorrectly()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["var1"] = "value1";
		variables["var2"] = "value2";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("myProcess", variables);
		assertNotNull(processInstance);

		variables = new Dictionary<string, object>();
		variables["var3"] = "value3";
		variables["var4"] = "value4";
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("myProcess", variables);
		assertNotNull(processInstance2);

		// check variables
		long count = historyService.createHistoricVariableInstanceQuery().count();
		assertEquals(4, count);

		// delete runtime execution of ONE process instance
		runtimeService.deleteProcessInstance(processInstance.Id, "reason 1");
		historyService.deleteHistoricProcessInstance(processInstance.Id);

		// recheck variables
		// this is a bug: all variables was deleted after delete a history processinstance
		count = historyService.createHistoricVariableInstanceQuery().count();
		assertEquals(2, count);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testParallel.bpmn20.xml"})]
	  public virtual void testHistoricVariableInstanceQueryByTaskIds()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("myProc");

		TaskQuery taskQuery = taskService.createTaskQuery();
		Task userTask = taskQuery.singleResult();
		assertEquals("userTask1", userTask.Name);

		// set local variable on user task
		taskService.setVariableLocal(userTask.Id, "taskVariable", "aCustomValue");

		// complete user task to finish process instance
		taskService.complete(userTask.Id);

		assertProcessEnded(processInstance.Id);

		IList<HistoricTaskInstance> tasks = historyService.createHistoricTaskInstanceQuery().processInstanceId(processInstance.ProcessInstanceId).list();
		assertEquals(1, tasks.Count);

		// check existing variables
		assertEquals(3, historyService.createHistoricVariableInstanceQuery().count());

		// check existing variables for task ID
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().taskIdIn(tasks[0].Id).list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().taskIdIn(tasks[0].Id).count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testParallel.bpmn20.xml"})]
	  public virtual void testHistoricVariableInstanceQueryByProcessIdIn()
	  {
		// given
		IDictionary<string, object> vars = new Dictionary<string, object>();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("myProc",vars);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("myProc",vars);

		// check existing variables for process instance ID
		assertEquals(4, historyService.createHistoricVariableInstanceQuery().processInstanceIdIn(processInstance.ProcessInstanceId,processInstance2.ProcessInstanceId).count());
		assertEquals(4, historyService.createHistoricVariableInstanceQuery().processInstanceIdIn(processInstance.ProcessInstanceId,processInstance2.ProcessInstanceId).list().size());

		//add check with not existing search
		string notExistingSearch = processInstance.ProcessInstanceId + "-notExisting";
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().processInstanceIdIn(notExistingSearch,processInstance2.ProcessInstanceId).count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testParallel.bpmn20.xml"})]
	  public virtual void testHistoricVariableInstanceQueryByInvalidProcessIdIn()
	  {
		// given
		IDictionary<string, object> vars = new Dictionary<string, object>();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("myProc",vars);

		// check existing variables for task ID
		try
		{
		  historyService.createHistoricVariableInstanceQuery().processInstanceIdIn(processInstance.ProcessInstanceId,null);
		  fail("Search by process instance ID was finished");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  historyService.createHistoricVariableInstanceQuery().processInstanceIdIn(null,processInstance.ProcessInstanceId);
		  fail("Search by process instance ID was finished");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricVariableInstanceQueryByExecutionIds()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["myVar"] = "test123";
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().executionIdIn(processInstance1.Id);
		assertEquals(2, query.count());
		IList<HistoricVariableInstance> variableInstances = query.list();
		assertEquals(2, variableInstances.Count);
		foreach (HistoricVariableInstance variableInstance in variableInstances)
		{
		  assertEquals(processInstance1.Id, variableInstance.ExecutionId);
		}

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["myVar"] = "test123";
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		query = historyService.createHistoricVariableInstanceQuery().executionIdIn(processInstance1.Id, processInstance2.Id);
		assertEquals(3, query.list().size());
		assertEquals(3, query.count());
	  }

	  public virtual void testQueryByInvalidExecutionIdIn()
	  {
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().executionIdIn("invalid");
		assertEquals(0, query.count());

		try
		{
		  historyService.createHistoricVariableInstanceQuery().executionIdIn(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  historyService.createHistoricVariableInstanceQuery().executionIdIn((string)null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByInvalidTaskIdIn()
	  {
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().taskIdIn("invalid");
		assertEquals(0, query.count());

		try
		{
		  historyService.createHistoricVariableInstanceQuery().taskIdIn(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  historyService.createHistoricVariableInstanceQuery().taskIdIn((string)null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByActivityInstanceIdIn()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["myVar"] = "test123";
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		query.activityInstanceIdIn(processInstance1.Id);

		assertEquals(2, query.list().size());
		assertEquals(2, query.count());

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["myVar"] = "test123";
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		query.activityInstanceIdIn(processInstance1.Id, processInstance2.Id);

		assertEquals(3, query.list().size());
		assertEquals(3, query.count());
	  }

	  public virtual void testQueryByInvalidActivityInstanceIdIn()
	  {
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		query.taskIdIn("invalid");
		assertEquals(0, query.count());

		try
		{
		  query.taskIdIn(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  query.taskIdIn((string)null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableTypeIn()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["boolVar"] = true;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().variableTypeIn("string");

		// then
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
		assertEquals(query.list().get(0).Name, "stringVar");
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableTypeInWithCapitalLetter()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["boolVar"] = true;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		query.variableTypeIn("Boolean");

		// then
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
		assertEquals(query.list().get(0).Name, "boolVar");
	  }

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
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		query.variableTypeIn("BooLEAN", "string", "Serializable");

		// then
		assertEquals(3, query.list().size());
		assertEquals(3, query.count());
	  }

	  public virtual void testQueryByInvalidVariableTypeIn()
	  {
		// given
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// when
		query.variableTypeIn("invalid");

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

	  public virtual void testBinaryFetchingEnabled()
	  {

		// by default, binary fetching is enabled

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "binaryVariableName";
		taskService.setVariable(newTask.Id, variableName, "some bytes".GetBytes());

		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().variableName(variableName).singleResult();

		assertNotNull(variableInstance.Value);

		taskService.deleteTask(newTask.Id, true);
	  }

	  public virtual void testBinaryFetchingDisabled()
	  {

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "binaryVariableName";
		taskService.setVariable(newTask.Id, variableName, "some bytes".GetBytes());

		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().variableName(variableName).disableBinaryFetching().singleResult();

		assertNull(variableInstance.Value);

		taskService.deleteTask(newTask.Id, true);
	  }

	  [Deployment(resources: "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml")]
	  public virtual void testDisableBinaryFetchingForFileValues()
	  {
		// given
		string fileName = "text.txt";
		string encoding = "crazy-encoding";
		string mimeType = "martini/dry";

		FileValue fileValue = Variables.fileValue(fileName).file("ABC".GetBytes()).encoding(encoding).mimeType(mimeType).create();

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValueTyped("fileVar", fileValue));

		// when enabling binary fetching
		HistoricVariableInstance fileVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// then the binary value is accessible
		assertNotNull(fileVariableInstance.Value);

		// when disabling binary fetching
		fileVariableInstance = historyService.createHistoricVariableInstanceQuery().disableBinaryFetching().singleResult();

		// then the byte value is not fetched
		assertNotNull(fileVariableInstance);
		assertEquals("fileVar", fileVariableInstance.Name);

		assertNull(fileVariableInstance.Value);

		FileValue typedValue = (FileValue) fileVariableInstance.TypedValue;
		assertNull(typedValue.Value);

		// but typed value metadata is accessible
		assertEquals(ValueType.FILE, typedValue.Type);
		assertEquals(fileName, typedValue.Filename);
		assertEquals(encoding, typedValue.Encoding);
		assertEquals(mimeType, typedValue.MimeType);

	  }

	  public virtual void testDisableCustomObjectDeserialization()
	  {
		// given
		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["customSerializable"] = new CustomSerializable();
		variables["failingSerializable"] = new FailingSerializable();
		taskService.setVariables(newTask.Id, variables);

		// when
		IList<HistoricVariableInstance> variableInstances = historyService.createHistoricVariableInstanceQuery().disableCustomObjectDeserialization().list();

		// then
		assertEquals(2, variableInstances.Count);

		foreach (HistoricVariableInstance variableInstance in variableInstances)
		{
		  assertNull(variableInstance.ErrorMessage);

		  ObjectValue typedValue = (ObjectValue) variableInstance.TypedValue;
		  assertNotNull(typedValue);
		  assertFalse(typedValue.Deserialized);
		  // cannot access the deserialized value
		  try
		  {
			typedValue.Value;
		  }
		  catch (System.InvalidOperationException e)
		  {
			assertTextPresent("Object is not deserialized", e.Message);
		  }
		  assertNotNull(typedValue.ValueSerialized);
		}

		taskService.deleteTask(newTask.Id, true);

	  }

	  public virtual void testDisableCustomObjectDeserializationNativeQuery()
	  {
		// given
		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["customSerializable"] = new CustomSerializable();
		variables["failingSerializable"] = new FailingSerializable();
		taskService.setVariables(newTask.Id, variables);

		// when
		IList<HistoricVariableInstance> variableInstances = historyService.createNativeHistoricVariableInstanceQuery().sql("SELECT * from " + managementService.getTableName(typeof(HistoricVariableInstance))).disableCustomObjectDeserialization().list();

		// then
		assertEquals(2, variableInstances.Count);

		foreach (HistoricVariableInstance variableInstance in variableInstances)
		{
		  assertNull(variableInstance.ErrorMessage);

		  ObjectValue typedValue = (ObjectValue) variableInstance.TypedValue;
		  assertNotNull(typedValue);
		  assertFalse(typedValue.Deserialized);
		  // cannot access the deserialized value
		  try
		  {
			typedValue.Value;
		  }
		  catch (System.InvalidOperationException e)
		  {
			assertTextPresent("Object is not deserialized", e.Message);
		  }
		  assertNotNull(typedValue.ValueSerialized);
		}

		taskService.deleteTask(newTask.Id, true);
	  }

	  public virtual void testErrorMessage()
	  {

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "failingSerializable";
		taskService.setVariable(newTask.Id, variableName, new FailingSerializable());

		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().variableName(variableName).singleResult();

		assertNull(variableInstance.Value);
		assertNotNull(variableInstance.ErrorMessage);

		taskService.deleteTask(newTask.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricVariableInstanceRevision()
	  public virtual void testHistoricVariableInstanceRevision()
	  {
		// given:
		// a finished process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		assertProcessEnded(processInstance.Id);

		// when

		// then
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();

		assertNotNull(variable);

		HistoricVariableInstanceEntity variableEntity = (HistoricVariableInstanceEntity) variable;

		// the revision has to be 0
		assertEquals(0, variableEntity.Revision);

		if (FullHistoryEnabled)
		{
		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().orderByVariableRevision().asc().list();

		  foreach (HistoricDetail detail in details)
		  {
			HistoricVariableUpdate variableDetail = (HistoricVariableUpdate) detail;
			assertEquals(0, variableDetail.Revision);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricVariableInstanceRevisionAsync()
	  public virtual void testHistoricVariableInstanceRevisionAsync()
	  {
		// given:
		// a finished process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		executeAvailableJobs();

		// then
		assertProcessEnded(processInstance.Id);

		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();

		assertNotNull(variable);

		HistoricVariableInstanceEntity variableEntity = (HistoricVariableInstanceEntity) variable;

		// the revision has to be 2
		assertEquals(2, variableEntity.Revision);

		if (FullHistoryEnabled)
		{
		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().orderByVariableRevision().asc().list();

		  int i = 0;
		  foreach (HistoricDetail detail in details)
		  {
			HistoricVariableUpdate variableDetail = (HistoricVariableUpdate) detail;
			assertEquals(i, variableDetail.Revision);
			i++;
		  }
		}

	  }

	  /// <summary>
	  /// CAM-3442
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings("unchecked") public void testImplicitVariableUpdate()
	  public virtual void testImplicitVariableUpdate()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new UpdateValueDelegate()));

		IList<string> list = (IList<string>) runtimeService.getVariable(instance.Id, "listVar");
		assertNotNull(list);
		assertEquals(1, list.Count);
		assertEquals(UpdateValueDelegate.NEW_ELEMENT, list[0]);

		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().variableName("listVar").singleResult();

		IList<string> historicList = (IList<string>) historicVariableInstance.Value;
		assertNotNull(historicList);
		assertEquals(1, historicList.Count);
		assertEquals(UpdateValueDelegate.NEW_ELEMENT, historicList[0]);

		if (FullHistoryEnabled)
		{
		  IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(historicVariableInstance.Id).orderPartiallyByOccurrence().asc().list();

		  assertEquals(2, historicDetails.Count);

		  HistoricVariableUpdate update1 = (HistoricVariableUpdate) historicDetails[0];
		  HistoricVariableUpdate update2 = (HistoricVariableUpdate) historicDetails[1];

		  IList<string> value1 = (IList<string>) update1.Value;

		  assertNotNull(value1);
		  assertTrue(value1.Count == 0);

		  IList<string> value2 = (IList<string>) update2.Value;

		  assertNotNull(value2);
		  assertEquals(1, value2.Count);
		  assertEquals(UpdateValueDelegate.NEW_ELEMENT, value2[0]);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testImplicitVariableUpdate.bpmn20.xml")]
	  public virtual void FAILING_testImplicitVariableUpdateActivityInstanceId()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new UpdateValueDelegate()));

		HistoricActivityInstance historicServiceTask = historyService.createHistoricActivityInstanceQuery().activityId("task").singleResult();

		IList<string> list = (IList<string>) runtimeService.getVariable(instance.Id, "listVar");
		assertNotNull(list);
		assertEquals(1, list.Count);
		assertEquals(UpdateValueDelegate.NEW_ELEMENT, list[0]);

		// when
		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().variableName("listVar").singleResult();

		// then
		assertEquals(historicServiceTask.Id, historicVariableInstance.ActivityInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testImplicitVariableUpdate.bpmn20.xml")]
	  public virtual void FAILING_testImplicitVariableUpdateAndReplacementInOneTransaction()
	  {
		// given
		runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new UpdateAndReplaceValueDelegate()));

		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().variableName("listVar").singleResult();

		IList<string> historicList = (IList<string>) historicVariableInstance.Value;
		assertNotNull(historicList);
		assertEquals(0, historicList.Count);

		if (FullHistoryEnabled)
		{
		  IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(historicVariableInstance.Id).orderPartiallyByOccurrence().asc().list();

		  assertEquals(3, historicDetails.Count);

		  HistoricVariableUpdate update1 = (HistoricVariableUpdate) historicDetails[0];
		  HistoricVariableUpdate update2 = (HistoricVariableUpdate) historicDetails[1];
		  HistoricVariableUpdate update3 = (HistoricVariableUpdate) historicDetails[2];

		  IList<string> value1 = (IList<string>) update1.Value;

		  assertNotNull(value1);
		  assertTrue(value1.Count == 0);

		  IList<string> value2 = (IList<string>) update2.Value;

		  assertNotNull(value2);
		  assertEquals(1, value2.Count);
		  assertEquals(UpdateValueDelegate.NEW_ELEMENT, value2[0]);

		  IList<string> value3 = (IList<string>) update3.Value;

		  assertNotNull(value3);
		  assertTrue(value3.Count == 0);
		}
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testImplicitVariableUpdateAndScopeDestroyedInOneTransaction()
	  {
		deployment(Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("task1").camundaExpression("${var.setValue(\"newValue\")}").endEvent("end").done());

		processEngine.RuntimeService.startProcessInstanceByKey("process1", Variables.createVariables().putValue("var", new CustomVar("initialValue")));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HistoricVariableInstance historicVariableInstance = processEngine.getHistoryService().createHistoricVariableInstanceQuery().list().get(0);
		HistoricVariableInstance historicVariableInstance = processEngine.HistoryService.createHistoricVariableInstanceQuery().list().get(0);
		CustomVar var = (CustomVar) historicVariableInstance.TypedValue.Value;

		assertEquals("newValue", var.Value);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<HistoricDetail> historicDetails = processEngine.getHistoryService().createHistoricDetailQuery().orderPartiallyByOccurrence().desc().list();
		IList<HistoricDetail> historicDetails = processEngine.HistoryService.createHistoricDetailQuery().orderPartiallyByOccurrence().desc().list();
		HistoricDetail historicDetail = historicDetails[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CustomVar typedValue = (CustomVar)((HistoricVariableUpdate) historicDetail).getTypedValue().getValue();
		CustomVar typedValue = (CustomVar)((HistoricVariableUpdate) historicDetail).TypedValue.Value;
		assertEquals("newValue", typedValue.Value);
	  }

	  [Serializable]
	  public class CustomVar
	  {
		internal string value;

		public CustomVar(string value)
		{
		  this.value = value;
		}

		public virtual string Value
		{
			get
			{
			  return value;
			}
			set
			{
			  this.value = value;
			}
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNoImplicitUpdateOnHistoricValues()
	  public virtual void testNoImplicitUpdateOnHistoricValues()
	  {
		//given
		runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new UpdateHistoricValueDelegate()));

		// a task before the delegate ensures that the variables have actually been persisted
		// and can be fetched by querying
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// then
		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().variableName("listVar").singleResult();

		IList<string> historicList = (IList<string>) historicVariableInstance.Value;
		assertNotNull(historicList);
		assertEquals(0, historicList.Count);

		if (FullHistoryEnabled)
		{
		  assertEquals(2, historyService.createHistoricDetailQuery().count());

		  IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(historicVariableInstance.Id).list();

		  assertEquals(1, historicDetails.Count);

		  HistoricVariableUpdate update1 = (HistoricVariableUpdate) historicDetails[0];

		  IList<string> value1 = (IList<string>) update1.Value;

		  assertNotNull(value1);
		  assertTrue(value1.Count == 0);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testImplicitVariableUpdate.bpmn20.xml")]
	  public virtual void testImplicitVariableRemoveAndUpdateInOneTransaction()
	  {
		// given
		runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new RemoveAndUpdateValueDelegate()));

		if (FullHistoryEnabled)
		{
		  IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().variableUpdates().orderPartiallyByOccurrence().asc().list();

		  IEnumerator<HistoricDetail> detailsIt = historicDetails.GetEnumerator();
		  while (detailsIt.MoveNext())
		  {
			if (!"listVar".Equals(((HistoricVariableUpdate) detailsIt.Current).VariableName))
			{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			  detailsIt.remove();
			}
		  }

		  // one for creation, one for deletion, none for update
		  assertEquals(2, historicDetails.Count);

		  HistoricVariableUpdate update1 = (HistoricVariableUpdate) historicDetails[0];

		  IList<string> value1 = (IList<string>) update1.Value;

		  assertNotNull(value1);
		  assertTrue(value1.Count == 0);

		  HistoricVariableUpdate update2 = (HistoricVariableUpdate) historicDetails[1];
		  assertNull(update2.Value);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testNoImplicitUpdateOnHistoricValues.bpmn20.xml")]
	  public virtual void testNoImplicitUpdateOnHistoricDetailValues()
	  {
		if (!FullHistoryEnabled)
		{
		  return;
		}

		// given
		runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValue("listVar", new List<string>()).putValue("delegate", new UpdateHistoricDetailValueDelegate()));

		// a task before the delegate ensures that the variables have actually been persisted
		// and can be fetched by querying
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// then
		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().variableName("listVar").singleResult();

		// One for "listvar", one for "delegate"
		assertEquals(2, historyService.createHistoricDetailQuery().count());

		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(historicVariableInstance.Id).list();

		assertEquals(1, historicDetails.Count);

		HistoricVariableUpdate update1 = (HistoricVariableUpdate) historicDetails[0];

		IList<string> value1 = (IList<string>) update1.Value;

		assertNotNull(value1);
		assertTrue(value1.Count == 0);
	  }

	  protected internal virtual bool FullHistoryEnabled
	  {
		  get
		  {
			return processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL);
		  }
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testHistoricVariableInstanceRevision.bpmn20.xml"})]
	  public virtual void testVariableUpdateOrder()
	  {
		// given:
		// a finished process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		assertProcessEnded(processInstance.Id);

		// when

		// then
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(variable);

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variable.Id).orderPartiallyByOccurrence().asc().list();

		  assertEquals(3, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(2, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(3, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricVariableInstanceTest.testHistoricVariableInstanceRevisionAsync.bpmn20.xml"})]
	  public virtual void testVariableUpdateOrderAsync()
	  {
		// given:
		// a finished process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		executeAvailableJobs();

		// then
		assertProcessEnded(processInstance.Id);

		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(variable);

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variable.Id).orderPartiallyByOccurrence().asc().list();

		  assertEquals(3, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(2, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(3, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskVariableUpdateOrder()
	  {
		// given:
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when (1)
		taskService.setVariableLocal(taskId, "myVariable", 1);
		taskService.setVariableLocal(taskId, "myVariable", 2);
		taskService.setVariableLocal(taskId, "myVariable", 3);

		// then (1)
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(variable);

		string variableInstanceId = variable.Id;

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variableInstanceId).orderPartiallyByOccurrence().asc().list();

		  assertEquals(3, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(2, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(3, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
		}

		// when (2)
		taskService.setVariableLocal(taskId, "myVariable", "abc");

		// then (2)
		variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(variable);

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variableInstanceId).orderPartiallyByOccurrence().asc().list();

		  assertEquals(4, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(2, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(3, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);

		  HistoricVariableUpdate fourthUpdate = (HistoricVariableUpdate) details[3];
		  assertEquals("abc", fourthUpdate.Value);
		  assertTrue(((HistoryEvent)fourthUpdate).SequenceCounter > ((HistoryEvent)thirdUpdate).SequenceCounter);
		}

		// when (3)
		taskService.removeVariable(taskId, "myVariable");

		// then (3)
		variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNull(variable);

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variableInstanceId).orderPartiallyByOccurrence().asc().list();

		  assertEquals(5, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(2, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(3, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);

		  HistoricVariableUpdate fourthUpdate = (HistoricVariableUpdate) details[3];
		  assertEquals("abc", fourthUpdate.Value);
		  assertTrue(((HistoryEvent)fourthUpdate).SequenceCounter > ((HistoryEvent)thirdUpdate).SequenceCounter);

		  HistoricVariableUpdate fifthUpdate = (HistoricVariableUpdate) details[4];
		  assertNull(fifthUpdate.Value);
		  assertTrue(((HistoryEvent)fifthUpdate).SequenceCounter > ((HistoryEvent)fourthUpdate).SequenceCounter);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCaseVariableUpdateOrder()
	  {
		// given:
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		// when (1)
		caseService.setVariable(caseInstanceId, "myVariable", 1);
		caseService.setVariable(caseInstanceId, "myVariable", 2);
		caseService.setVariable(caseInstanceId, "myVariable", 3);

		// then (1)
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(variable);

		string variableInstanceId = variable.Id;

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variableInstanceId).orderPartiallyByOccurrence().asc().list();

		  assertEquals(3, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(2, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(3, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
		}

		// when (2)
		caseService.setVariable(caseInstanceId, "myVariable", "abc");

		// then (2)
		variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(variable);

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variableInstanceId).orderPartiallyByOccurrence().asc().list();

		  assertEquals(4, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(2, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(3, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);

		  HistoricVariableUpdate fourthUpdate = (HistoricVariableUpdate) details[3];
		  assertEquals("abc", fourthUpdate.Value);
		  assertTrue(((HistoryEvent)fourthUpdate).SequenceCounter > ((HistoryEvent)thirdUpdate).SequenceCounter);
		}

		// when (3)
		caseService.removeVariable(caseInstanceId, "myVariable");

		// then (3)
		variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNull(variable);

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variableInstanceId).orderPartiallyByOccurrence().asc().list();

		  assertEquals(5, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(2, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(3, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);

		  HistoricVariableUpdate fourthUpdate = (HistoricVariableUpdate) details[3];
		  assertEquals("abc", fourthUpdate.Value);
		  assertTrue(((HistoryEvent)fourthUpdate).SequenceCounter > ((HistoryEvent)thirdUpdate).SequenceCounter);

		  HistoricVariableUpdate fifthUpdate = (HistoricVariableUpdate) details[4];
		  assertNull(fifthUpdate.Value);
		  assertTrue(((HistoryEvent)fifthUpdate).SequenceCounter > ((HistoryEvent)fourthUpdate).SequenceCounter);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSetSameVariableUpdateOrder()
	  {
		// given:
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setVariable(taskId, "myVariable", 1);
		taskService.setVariable(taskId, "myVariable", 1);
		taskService.setVariable(taskId, "myVariable", 2);

		// then
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(variable);

		string variableInstanceId = variable.Id;

		if (FullHistoryEnabled)
		{

		  IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableInstanceId(variableInstanceId).orderPartiallyByOccurrence().asc().list();

		  assertEquals(3, details.Count);

		  HistoricVariableUpdate firstUpdate = (HistoricVariableUpdate) details[0];
		  assertEquals(1, firstUpdate.Value);

		  HistoricVariableUpdate secondUpdate = (HistoricVariableUpdate) details[1];
		  assertEquals(1, secondUpdate.Value);
		  assertTrue(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

		  HistoricVariableUpdate thirdUpdate = (HistoricVariableUpdate) details[2];
		  assertEquals(2, thirdUpdate.Value);
		  assertTrue(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
		}

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessDefinitionProperty()
	  {
		// given
		string key = "oneTaskProcess";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(key);

		string processInstanceId = processInstance.Id;
		string taskId = taskService.createTaskQuery().singleResult().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "aValue");
		taskService.setVariableLocal(taskId, "aLocalVariable", "anotherValue");

		// when (1)
		HistoricVariableInstance instance = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceId).variableName("aVariable").singleResult();

		// then (1)
		assertNotNull(instance.ProcessDefinitionKey);
		assertEquals(key, instance.ProcessDefinitionKey);

		assertNotNull(instance.ProcessDefinitionId);
		assertEquals(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

		assertNull(instance.CaseDefinitionKey);
		assertNull(instance.CaseDefinitionId);

		// when (2)
		instance = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceId).variableName("aLocalVariable").singleResult();

		// then (2)
		assertNotNull(instance.ProcessDefinitionKey);
		assertEquals(key, instance.ProcessDefinitionKey);

		assertNotNull(instance.ProcessDefinitionId);
		assertEquals(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

		assertNull(instance.CaseDefinitionKey);
		assertNull(instance.CaseDefinitionId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testCaseDefinitionProperty()
	  {
		// given
		string key = "oneTaskCase";
		CaseInstance caseInstance = caseService.createCaseInstanceByKey(key);

		string caseInstanceId = caseInstance.Id;

		caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;
		string taskId = taskService.createTaskQuery().singleResult().Id;

		caseService.setVariable(caseInstanceId, "aVariable", "aValue");
		taskService.setVariableLocal(taskId, "aLocalVariable", "anotherValue");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).variableName("aVariable").singleResult();
		assertNotNull(variable);

		// when (1)
		HistoricVariableInstance instance = historyService.createHistoricVariableInstanceQuery().caseInstanceId(caseInstanceId).variableName("aVariable").singleResult();

		// then (1)
		assertCaseVariable(key, caseInstance, instance);

		// when (2)
		instance = historyService.createHistoricVariableInstanceQuery().caseInstanceId(caseInstanceId).variableName("aLocalVariable").singleResult();

		// then (2)
		assertCaseVariable(key, caseInstance, instance);

		// when (3)
		instance = historyService.createHistoricVariableInstanceQuery().caseInstanceId(caseInstanceId).variableId(variable.Id).singleResult();

		// then (4)
		assertNotNull(instance);
		assertCaseVariable(key, caseInstance, instance);
	  }

	  protected internal virtual void assertCaseVariable(string key, CaseInstance caseInstance, HistoricVariableInstance instance)
	  {
		assertNotNull(instance.CaseDefinitionKey);
		assertEquals(key, instance.CaseDefinitionKey);

		assertNotNull(instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);

		assertNull(instance.ProcessDefinitionKey);
		assertNull(instance.ProcessDefinitionId);
	  }

	  public virtual void testStandaloneTaskDefinitionProperties()
	  {
		// given
		string taskId = "myTask";
		Task task = taskService.newTask(taskId);
		taskService.saveTask(task);

		taskService.setVariable(taskId, "aVariable", "anotherValue");

		// when (1)
		HistoricVariableInstance instance = historyService.createHistoricVariableInstanceQuery().taskIdIn(taskId).variableName("aVariable").singleResult();

		// then (1)
		assertNull(instance.ProcessDefinitionKey);
		assertNull(instance.ProcessDefinitionId);
		assertNull(instance.CaseDefinitionKey);
		assertNull(instance.CaseDefinitionId);

		taskService.deleteTask(taskId, true);
	  }

	  public virtual void testTaskIdProperty()
	  {
		// given
		string taskId = "myTask";
		Task task = taskService.newTask(taskId);
		taskService.saveTask(task);

		taskService.setVariable(taskId, "aVariable", "anotherValue");

		// when
		HistoricVariableInstance instance = historyService.createHistoricVariableInstanceQuery().taskIdIn(taskId).variableName("aVariable").singleResult();

		// then
		assertEquals(taskId, instance.TaskId);

		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinParallelGatewayLocalVariableOnLastJoiningExecution()
	  public virtual void testJoinParallelGatewayLocalVariableOnLastJoiningExecution()
	  {
		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());

		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(historicVariable);
		assertEquals("testVar", historicVariable.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedJoinParallelGatewayLocalVariableOnLastJoiningExecution()
	  public virtual void testNestedJoinParallelGatewayLocalVariableOnLastJoiningExecution()
	  {
		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());

		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(historicVariable);
		assertEquals("testVar", historicVariable.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinInclusiveGatewayLocalVariableOnLastJoiningExecution()
	  public virtual void testJoinInclusiveGatewayLocalVariableOnLastJoiningExecution()
	  {
		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());

		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(historicVariable);
		assertEquals("testVar", historicVariable.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedJoinInclusiveGatewayLocalVariableOnLastJoiningExecution()
	  public virtual void testNestedJoinInclusiveGatewayLocalVariableOnLastJoiningExecution()
	  {
		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());

		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(historicVariable);
		assertEquals("testVar", historicVariable.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkParallelGatewayTreeCompaction()
	  public virtual void testForkParallelGatewayTreeCompaction()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();

		// when
		runtimeService.setVariableLocal(task2Execution.Id, "foo", "bar");
		taskService.complete(task1.Id);

		// then
		assertEquals(1, runtimeService.createVariableInstanceQuery().count());

		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(historicVariable);
		assertEquals("foo", historicVariable.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedForkParallelGatewayTreeCompaction()
	  public virtual void testNestedForkParallelGatewayTreeCompaction()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();

		// when
		runtimeService.setVariableLocal(task2Execution.Id, "foo", "bar");
		taskService.complete(task1.Id);

		// then
		assertEquals(1, runtimeService.createVariableInstanceQuery().count());

		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertNotNull(historicVariable);
		assertEquals("foo", historicVariable.Name);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testQueryByCaseActivityId()
	  {
		// given
		caseService.createCaseInstanceByKey("oneTaskCase", Variables.putValue("foo", "bar"));

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		caseService.setVariableLocal(caseExecution.Id, "bar", "foo");

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().caseActivityIdIn("PI_HumanTask_1");

		// then
		assertEquals(1, query.count());
		assertEquals("bar", query.singleResult().Name);
		assertEquals("foo", query.singleResult().Value);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn")]
	  public virtual void testQueryByCaseActivityIds()
	  {
		// given
		caseService.createCaseInstanceByKey("twoTaskCase");

		CaseExecution caseExecution1 = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		caseService.setVariableLocal(caseExecution1.Id, "foo", "bar");

		CaseExecution caseExecution2 = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2").singleResult();
		caseService.setVariableLocal(caseExecution2.Id, "bar", "foo");

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().caseActivityIdIn("PI_HumanTask_1", "PI_HumanTask_2");

		// then
		assertEquals(2, query.count());
	  }

	  public virtual void testQueryByInvalidCaseActivityIds()
	  {
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		query.caseActivityIdIn("invalid");
		assertEquals(0, query.count());

		try
		{
		  query.caseActivityIdIn(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (NullValueException)
		{
		}

		try
		{
		  query.caseActivityIdIn((string)null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (NullValueException)
		{
		}

		try
		{
		  string[] values = new string[] {"a", null, "b"};
		  query.caseActivityIdIn(values);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (NullValueException)
		{
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSetVariableInSubProcessStartEventWithEndListener() throws Exception
	  public virtual void testSetVariableInSubProcessStartEventWithEndListener()
	  {
		//given
		BpmnModelInstance topProcess = Bpmn.createExecutableProcess("topProcess").startEvent().callActivity().calledElement("subProcess").camundaIn("executionListenerCounter","executionListenerCounter").endEvent().done();

		BpmnModelInstance subProcess = Bpmn.createExecutableProcess("subProcess").startEvent().camundaAsyncBefore().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, "org.camunda.bpm.engine.test.history.SubProcessActivityStartListener").endEvent().done();
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", topProcess).addModelInstance("subProcess.bpmn", subProcess).deploy();

		//when
		runtimeService.startProcessInstanceByKey("topProcess", Variables.createVariables().putValue("executionListenerCounter",1));
		managementService.executeJob(managementService.createJobQuery().active().singleResult().Id);

		//then
		assertThat(historyService.createHistoricVariableInstanceQuery().count(), @is(3L));
		repositoryService.deleteDeployment(deployment.Id,true);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSetVariableInEndListenerOfAsyncStartEvent() throws Exception
	  public virtual void testSetVariableInEndListenerOfAsyncStartEvent()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance subProcess = Bpmn.createExecutableProcess("process").startEvent().camundaAsyncBefore().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SubProcessActivityStartListener).FullName).endEvent().done();

		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", subProcess).deploy();

		//when
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("executionListenerCounter",1));
		managementService.executeJob(managementService.createJobQuery().active().singleResult().Id);

		//then
		assertThat(historyService.createHistoricVariableInstanceQuery().count(), @is(2L));
		repositoryService.deleteDeployment(deployment.Id,true);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSetVariableInStartListenerOfAsyncStartEvent() throws Exception
	  public virtual void testSetVariableInStartListenerOfAsyncStartEvent()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance subProcess = Bpmn.createExecutableProcess("process").startEvent().camundaAsyncBefore().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(SubProcessActivityStartListener).FullName).endEvent().done();

		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", subProcess).deploy();

		//when
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("executionListenerCounter",1));
		managementService.executeJob(managementService.createJobQuery().active().singleResult().Id);

		//then
		assertThat(historyService.createHistoricVariableInstanceQuery().count(), @is(2L));
		repositoryService.deleteDeployment(deployment.Id,true);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testAsyncStartEvent.bpmn20.xml")]
	  public virtual void testAsyncStartEventHistory()
	  {
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  runtimeService.startProcessInstanceByKey("asyncStartEvent");

		  HistoricProcessInstance historicInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		  Assert.assertNotNull(historicInstance);
		  Assert.assertNotNull(historicInstance.StartTime);

		  HistoricActivityInstance historicStartEvent = historyService.createHistoricActivityInstanceQuery().singleResult();
		  Assert.assertNull(historicStartEvent);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testAsyncStartEvent.bpmn20.xml")]
	  public virtual void testAsyncStartEventVariableHistory()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		string processInstanceId = runtimeService.startProcessInstanceByKey("asyncStartEvent", variables).Id;

		VariableInstance variableFoo = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variableFoo);
		assertEquals("foo", variableFoo.Name);
		assertEquals("bar", variableFoo.Value);

		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		executeAvailableJobs();

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		// assert process instance is ended
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		  assertNotNull(variable);
		  assertEquals("foo", variable.Name);
		  assertEquals("bar", variable.Value);
		  assertEquals(processInstanceId, variable.ActivityInstanceId);

		  if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		  {

			string startEventId = historyService.createHistoricActivityInstanceQuery().activityId("startEvent").singleResult().Id;

			HistoricDetail historicDetail = historyService.createHistoricDetailQuery().singleResult();

			assertEquals(startEventId, historicDetail.ActivityInstanceId);
		  }
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testMultipleAsyncStartEvents.bpmn20.xml"})]
	  public virtual void testMultipleAsyncStartEventsVariableHistory()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		runtimeService.correlateMessage("newInvoiceMessage", new Dictionary<string, object>(), variables);

		VariableInstance variableFoo = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variableFoo);
		assertEquals("foo", variableFoo.Name);
		assertEquals("bar", variableFoo.Value);

		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		executeAvailableJobs();

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		// assert process instance is ended
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{

		  string processInstanceId = historyService.createHistoricProcessInstanceQuery().singleResult().Id;

		  HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		  assertNotNull(variable);
		  assertEquals("foo", variable.Name);
		  assertEquals("bar", variable.Value);
		  assertEquals(processInstanceId, variable.ActivityInstanceId);

		  if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		  {

			string theStartActivityInstanceId = historyService.createHistoricActivityInstanceQuery().activityId("messageStartEvent").singleResult().Id;

			HistoricDetail historicDetail = historyService.createHistoricDetailQuery().singleResult();

			assertEquals(theStartActivityInstanceId, historicDetail.ActivityInstanceId);

		  }
		}
	  }

	  public virtual void testAsyncStartEventWithAddedVariable()
	  {
		// given a process definition with asynchronous start event
		deployment(Bpmn.createExecutableProcess("testProcess").startEvent().camundaAsyncBefore().endEvent().done());

		// when create an instance with a variable
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", Variables.putValue("var1", "foo"));

		// and add a variable before the instance is created
		runtimeService.setVariable(processInstance.Id, "var2", "bar");

		executeAvailableJobs();

		assertProcessEnded(processInstance.Id);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{

		  // then the history contains one entry for each variable
		  HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		  assertEquals(query.count(), 2);

		  HistoricVariableInstance firstVariable = query.variableName("var1").singleResult();
		  assertNotNull(firstVariable);
		  assertEquals(firstVariable.Value, "foo");
		  assertNotNull(firstVariable.ActivityInstanceId);

		  HistoricVariableInstance secondVariable = query.variableName("var2").singleResult();
		  assertNotNull(secondVariable);
		  assertEquals(secondVariable.Value, "bar");
		  assertNotNull(secondVariable.ActivityInstanceId);
		}
	  }


	  public virtual void testAsyncStartEventWithChangedVariable()
	  {
		// given a process definition with asynchronous start event
		deployment(Bpmn.createExecutableProcess("testProcess").startEvent().camundaAsyncBefore().endEvent().done());

		// when create an instance with a variable
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", Variables.putValue("var", "foo"));

		// and update this variable before the instance is created
		runtimeService.setVariable(processInstance.Id, "var", "bar");

		executeAvailableJobs();

		assertProcessEnded(processInstance.Id);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{

		  // then the history contains only one entry for the latest update (value = "bar")
		  // - the entry for the initial value (value = "foo") is lost because of current limitations
		  HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		  assertEquals(query.count(), 1);

		  HistoricVariableInstance variable = query.singleResult();
		  assertEquals(variable.Value, "bar");
		  assertNotNull(variable.ActivityInstanceId);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testAsyncStartEvent.bpmn20.xml")]
	  public virtual void testSubmitForm()
	  {

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("asyncStartEvent").singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["foo"] = "bar";

		formService.submitStartForm(processDefinitionId, properties);

		VariableInstance variableFoo = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variableFoo);
		assertEquals("foo", variableFoo.Name);
		assertEquals("bar", variableFoo.Value);

		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		executeAvailableJobs();

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		// assert process instance is ended
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{

		  string processInstanceId = historyService.createHistoricProcessInstanceQuery().singleResult().Id;

		  HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		  assertNotNull(variable);
		  assertEquals("foo", variable.Name);
		  assertEquals("bar", variable.Value);
		  assertEquals(processInstanceId, variable.ActivityInstanceId);

		  if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		  {

			string theStartActivityInstanceId = historyService.createHistoricActivityInstanceQuery().activityId("startEvent").singleResult().Id;

			HistoricFormField historicFormUpdate = (HistoricFormField) historyService.createHistoricDetailQuery().formFields().singleResult();

			assertNotNull(historicFormUpdate);
			assertEquals("bar", historicFormUpdate.FieldValue);

			HistoricVariableUpdate historicVariableUpdate = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().singleResult();

			assertNotNull(historicVariableUpdate);
			assertEquals(theStartActivityInstanceId, historicVariableUpdate.ActivityInstanceId);
			assertEquals("bar", historicVariableUpdate.Value);

		  }
		}
	  }

	  /// <summary>
	  /// CAM-2828
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testAsyncStartEvent.bpmn20.xml")]
	  public virtual void FAILING_testSubmitFormHistoricUpdates()
	  {

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("asyncStartEvent").singleResult().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["foo"] = "bar";

		formService.submitStartForm(processDefinitionId, properties);
		executeAvailableJobs();

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{

		  string theStartActivityInstanceId = historyService.createHistoricActivityInstanceQuery().activityId("startEvent").singleResult().Id;

		  HistoricDetail historicFormUpdate = historyService.createHistoricDetailQuery().formFields().singleResult();

		  assertNotNull(historicFormUpdate);
		  assertEquals(theStartActivityInstanceId, historicFormUpdate.ActivityInstanceId);

		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml"})]
	  public virtual void testSetDifferentStates()
	  {
		//given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoTasksProcess", Variables.createVariables().putValue("initial", "foo"));
		Task task = taskService.createTaskQuery().singleResult();
		taskService.setVariables(task.Id, Variables.createVariables().putValue("bar", "abc"));
		taskService.complete(task.Id);

		//when
		runtimeService.removeVariable(processInstance.Id, "bar");

		//then
		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().includeDeleted().list();
		Assert.assertEquals(2, variables.Count);

		int createdCounter = 0;
		int deletedCounter = 0;

		foreach (HistoricVariableInstance variable in variables)
		{
		  if (variable.Name.Equals("initial"))
		  {
			Assert.assertEquals(HistoricVariableInstance_Fields.STATE_CREATED, variable.State);
			createdCounter += 1;
		  }
		  else if (variable.Name.Equals("bar"))
		  {
			Assert.assertEquals(HistoricVariableInstance_Fields.STATE_DELETED, variable.State);
			deletedCounter += 1;
		  }
		}

		Assert.assertEquals(1, createdCounter);
		Assert.assertEquals(1, deletedCounter);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml"})]
	  public virtual void testQueryNotIncludeDeleted()
	  {
		//given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoTasksProcess", Variables.createVariables().putValue("initial", "foo"));
		Task task = taskService.createTaskQuery().singleResult();
		taskService.setVariables(task.Id, Variables.createVariables().putValue("bar", "abc"));
		taskService.complete(task.Id);

		//when
		runtimeService.removeVariable(processInstance.Id, "bar");

		//then
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertEquals(HistoricVariableInstance_Fields.STATE_CREATED, variable.State);
		assertEquals("initial", variable.Name);
		assertEquals("foo", variable.Value);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessDefinitionId()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoTasksProcess", Variables.createVariables().putValue("initial", "foo"));

		// when
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();

		// then
		assertNotNull(variable);
		assertEquals("initial", variable.Name);
		assertEquals("foo", variable.Value);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessDefinitionKey()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoTasksProcess", Variables.createVariables().putValue("initial", "foo"));

		// when
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().processDefinitionKey("twoTasksProcess").singleResult();

		// then
		assertNotNull(variable);
		assertEquals("initial", variable.Name);
		assertEquals("foo", variable.Value);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessDefinitionKeyTwoInstances()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoTasksProcess", Variables.createVariables().putValue("initial", "foo").putValue("vegie", "cucumber"));
		runtimeService.startProcessInstanceByKey("twoTasksProcess", Variables.createVariables().putValue("initial", "bar").putValue("fruit", "marakuia"));

		// when
		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().processDefinitionKey("twoTasksProcess").list();

		// then
		assertNotNull(variables);
		assertEquals(4, variables.Count);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessDefinitionKeyTwoDefinitions()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("initial", "bar"));
		runtimeService.startProcessInstanceByKey("twoTasksProcess", Variables.createVariables().putValue("initial", "foo"));

		// when
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().processDefinitionKey("twoTasksProcess").singleResult();

		// then
		assertNotNull(variable);
		assertEquals("initial", variable.Name);
		assertEquals("foo", variable.Value);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessInstanceIdAndVariableId()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("initial", "bar"));

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("initial").singleResult();
		assertNotNull(variable);

		// when
		HistoricVariableInstance historyVariable = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableId(variable.Id).singleResult();

		// then
		assertNotNull(historyVariable);
		assertEquals("initial", historyVariable.Name);
		assertEquals("bar", historyVariable.Value);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testVariableCreateTime() throws java.text.ParseException
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testVariableCreateTime()
	  {
		// given
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");
		DateTime fixedDate = sdf.parse("01/01/2001 01:01:01.000");
		ClockUtil.CurrentTime = fixedDate;
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		// when
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// then
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertEquals(fixedDate, variable.CreateTime);

		// clean up
		ClockUtil.CurrentTime = DateTime.Now;
	  }
	}

}