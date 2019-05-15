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
namespace org.camunda.bpm.engine.test.standalone.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricFormField = org.camunda.bpm.engine.history.HistoricFormField;
	using HistoricFormProperty = org.camunda.bpm.engine.history.HistoricFormProperty;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using SubmitStartFormCmd = org.camunda.bpm.engine.impl.cmd.SubmitStartFormCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using DummySerializable = org.camunda.bpm.engine.test.api.runtime.DummySerializable;
	using CustomSerializable = org.camunda.bpm.engine.test.api.runtime.util.CustomSerializable;
	using FailingSerializable = org.camunda.bpm.engine.test.api.runtime.util.FailingSerializable;
	using SerializableVariable = org.camunda.bpm.engine.test.history.SerializableVariable;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class FullHistoryTest
	{
		private bool InstanceFieldsInitialized = false;

		public FullHistoryTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal TaskService taskService;
	  protected internal FormService formService;
	  protected internal RepositoryService repositoryService;
	  protected internal CaseService caseService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		taskService = engineRule.TaskService;
		formService = engineRule.FormService;
		repositoryService = engineRule.RepositoryService;
		caseService = engineRule.CaseService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testVariableUpdates()
	  public virtual void testVariableUpdates()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["number"] = "one";
		variables["character"] = "a";
		variables["bytes"] = ":-(".GetBytes();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("receiveTask", variables);
		runtimeService.setVariable(processInstance.Id, "number", "two");
		runtimeService.setVariable(processInstance.Id, "bytes", ":-)".GetBytes());

		// Start-task should be added to history
		HistoricActivityInstance historicStartEvent = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).activityId("theStart").singleResult();
		assertNotNull(historicStartEvent);

		HistoricActivityInstance waitStateActivity = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).activityId("waitState").singleResult();
		assertNotNull(waitStateActivity);

		HistoricActivityInstance serviceTaskActivity = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).activityId("serviceTask").singleResult();
		assertNotNull(serviceTaskActivity);

		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().orderByVariableName().asc().orderByVariableRevision().asc().list();

		assertEquals(10, historicDetails.Count);

		HistoricVariableUpdate historicVariableUpdate = (HistoricVariableUpdate) historicDetails[0];
		assertEquals("bytes", historicVariableUpdate.VariableName);
		assertEquals(":-(", StringHelper.NewString((sbyte[])historicVariableUpdate.Value));
		assertEquals(0, historicVariableUpdate.Revision);
		assertEquals(historicStartEvent.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable is updated when process was in waitstate
		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[1];
		assertEquals("bytes", historicVariableUpdate.VariableName);
		assertEquals(":-)", StringHelper.NewString((sbyte[])historicVariableUpdate.Value));
		assertEquals(1, historicVariableUpdate.Revision);
		assertEquals(waitStateActivity.Id, historicVariableUpdate.ActivityInstanceId);

		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[2];
		assertEquals("character", historicVariableUpdate.VariableName);
		assertEquals("a", historicVariableUpdate.Value);
		assertEquals(0, historicVariableUpdate.Revision);
		assertEquals(historicStartEvent.Id, historicVariableUpdate.ActivityInstanceId);

		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[3];
		assertEquals("number", historicVariableUpdate.VariableName);
		assertEquals("one", historicVariableUpdate.Value);
		assertEquals(0, historicVariableUpdate.Revision);
		assertEquals(historicStartEvent.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable is updated when process was in waitstate
		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[4];
		assertEquals("number", historicVariableUpdate.VariableName);
		assertEquals("two", historicVariableUpdate.Value);
		assertEquals(1, historicVariableUpdate.Revision);
		assertEquals(waitStateActivity.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable set from process-start execution listener
		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[5];
		assertEquals("zVar1", historicVariableUpdate.VariableName);
		assertEquals("Event: start", historicVariableUpdate.Value);
		assertEquals(0, historicVariableUpdate.Revision);
		assertEquals(historicStartEvent.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable set from transition take execution listener
		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[6];
		assertEquals("zVar2", historicVariableUpdate.VariableName);
		assertEquals("Event: take", historicVariableUpdate.Value);
		assertEquals(0, historicVariableUpdate.Revision);
		assertNull(historicVariableUpdate.ActivityInstanceId);

		// Variable set from activity start execution listener on the servicetask
		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[7];
		assertEquals("zVar3", historicVariableUpdate.VariableName);
		assertEquals("Event: start", historicVariableUpdate.Value);
		assertEquals(0, historicVariableUpdate.Revision);
		assertEquals(serviceTaskActivity.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable set from activity end execution listener on the servicetask
		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[8];
		assertEquals("zVar4", historicVariableUpdate.VariableName);
		assertEquals("Event: end", historicVariableUpdate.Value);
		assertEquals(0, historicVariableUpdate.Revision);
		assertEquals(serviceTaskActivity.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable set from service-task
		historicVariableUpdate = (HistoricVariableUpdate) historicDetails[9];
		assertEquals("zzz", historicVariableUpdate.VariableName);
		assertEquals(123456789L, historicVariableUpdate.Value);
		assertEquals(0, historicVariableUpdate.Revision);
		assertEquals(serviceTaskActivity.Id, historicVariableUpdate.ActivityInstanceId);

		// trigger receive task
		runtimeService.signal(processInstance.Id);
		testHelper.assertProcessEnded(processInstance.Id);

		// check for historic process variables set
		HistoricVariableInstanceQuery historicProcessVariableQuery = historyService.createHistoricVariableInstanceQuery().orderByVariableName().asc();

		assertEquals(8, historicProcessVariableQuery.count());

		IList<HistoricVariableInstance> historicVariables = historicProcessVariableQuery.list();

		// Variable status when process is finished
		HistoricVariableInstance historicVariable = historicVariables[0];
		assertEquals("bytes", historicVariable.VariableName);
		assertEquals(":-)", StringHelper.NewString((sbyte[])historicVariable.Value));

		historicVariable = historicVariables[1];
		assertEquals("character", historicVariable.VariableName);
		assertEquals("a", historicVariable.Value);

		historicVariable = historicVariables[2];
		assertEquals("number", historicVariable.VariableName);
		assertEquals("two", historicVariable.Value);

		historicVariable = historicVariables[3];
		assertEquals("zVar1", historicVariable.VariableName);
		assertEquals("Event: start", historicVariable.Value);

		historicVariable = historicVariables[4];
		assertEquals("zVar2", historicVariable.VariableName);
		assertEquals("Event: take", historicVariable.Value);

		historicVariable = historicVariables[5];
		assertEquals("zVar3", historicVariable.VariableName);
		assertEquals("Event: start", historicVariable.Value);

		historicVariable = historicVariables[6];
		assertEquals("zVar4", historicVariable.VariableName);
		assertEquals("Event: end", historicVariable.Value);

		historicVariable = historicVariables[7];
		assertEquals("zzz", historicVariable.VariableName);
		assertEquals(123456789L, historicVariable.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources="org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testVariableUpdates.bpmn20.xml") public void testHistoricVariableInstanceQuery()
	  [Deployment(resources:"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testVariableUpdates.bpmn20.xml")]
	  public virtual void testHistoricVariableInstanceQuery()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["process"] = "one";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("receiveTask", variables);
		runtimeService.signal(processInstance.ProcessInstanceId);

		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("process").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("process", "one").count());

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["process"] = "two";
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("receiveTask", variables2);
		runtimeService.signal(processInstance2.ProcessInstanceId);

		assertEquals(2, historyService.createHistoricVariableInstanceQuery().variableName("process").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("process", "one").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("process", "two").count());

		HistoricVariableInstance historicProcessVariable = historyService.createHistoricVariableInstanceQuery().variableValueEquals("process", "one").singleResult();
		assertEquals("process", historicProcessVariable.VariableName);
		assertEquals("one", historicProcessVariable.Value);
		assertEquals(ValueType.STRING.Name, historicProcessVariable.VariableTypeName);
		assertEquals(ValueType.STRING.Name, historicProcessVariable.TypeName);
		assertEquals(historicProcessVariable.Value, historicProcessVariable.TypedValue.Value);
		assertEquals(historicProcessVariable.TypeName, historicProcessVariable.TypedValue.Type.Name);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["long"] = 1000l;
		variables3["double"] = 25.43d;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("receiveTask", variables3);
		runtimeService.signal(processInstance3.ProcessInstanceId);

		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("long").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("long", 1000l).count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("double").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableValueEquals("double", 25.43d).count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricVariableUpdatesAllTypes() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricVariableUpdatesAllTypes()
	  {

		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss SSS");
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "initial value";

		DateTime startedDate = sdf.parse("01/01/2001 01:23:45 000");

		// In the javaDelegate, the current time is manipulated
		DateTime updatedDate = sdf.parse("01/01/2001 01:23:46 000");

		ClockUtil.CurrentTime = startedDate;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricVariableUpdateProcess", variables);

		IList<HistoricDetail> details = historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).orderByVariableName().asc().orderByTime().asc().list();

		// 8 variable updates should be present, one performed when starting process
		// the other 7 are set in VariableSetter serviceTask
		assertEquals(9, details.Count);

		// Since we order by varName, first entry should be aVariable update from startTask
		HistoricVariableUpdate startVarUpdate = (HistoricVariableUpdate) details[0];
		assertEquals("aVariable", startVarUpdate.VariableName);
		assertEquals("initial value", startVarUpdate.Value);
		assertEquals(0, startVarUpdate.Revision);
		assertEquals(processInstance.Id, startVarUpdate.ProcessInstanceId);
		// Date should the the one set when starting
		assertEquals(startedDate, startVarUpdate.Time);

		HistoricVariableUpdate updatedStringVariable = (HistoricVariableUpdate) details[1];
		assertEquals("aVariable", updatedStringVariable.VariableName);
		assertEquals("updated value", updatedStringVariable.Value);
		assertEquals(processInstance.Id, updatedStringVariable.ProcessInstanceId);
		// Date should be the updated date
		assertEquals(updatedDate, updatedStringVariable.Time);

		HistoricVariableUpdate intVariable = (HistoricVariableUpdate) details[2];
		assertEquals("bVariable", intVariable.VariableName);
		assertEquals(123, intVariable.Value);
		assertEquals(processInstance.Id, intVariable.ProcessInstanceId);
		assertEquals(updatedDate, intVariable.Time);

		HistoricVariableUpdate longVariable = (HistoricVariableUpdate) details[3];
		assertEquals("cVariable", longVariable.VariableName);
		assertEquals(12345L, longVariable.Value);
		assertEquals(processInstance.Id, longVariable.ProcessInstanceId);
		assertEquals(updatedDate, longVariable.Time);

		HistoricVariableUpdate doubleVariable = (HistoricVariableUpdate) details[4];
		assertEquals("dVariable", doubleVariable.VariableName);
		assertEquals(1234.567, doubleVariable.Value);
		assertEquals(processInstance.Id, doubleVariable.ProcessInstanceId);
		assertEquals(updatedDate, doubleVariable.Time);

		HistoricVariableUpdate shortVariable = (HistoricVariableUpdate) details[5];
		assertEquals("eVariable", shortVariable.VariableName);
		assertEquals((short)12, shortVariable.Value);
		assertEquals(processInstance.Id, shortVariable.ProcessInstanceId);
		assertEquals(updatedDate, shortVariable.Time);

		HistoricVariableUpdate dateVariable = (HistoricVariableUpdate) details[6];
		assertEquals("fVariable", dateVariable.VariableName);
		assertEquals(sdf.parse("01/01/2001 01:23:45 678"), dateVariable.Value);
		assertEquals(processInstance.Id, dateVariable.ProcessInstanceId);
		assertEquals(updatedDate, dateVariable.Time);

		HistoricVariableUpdate serializableVariable = (HistoricVariableUpdate) details[7];
		assertEquals("gVariable", serializableVariable.VariableName);
		assertEquals(new SerializableVariable("hello hello"), serializableVariable.Value);
		assertEquals(processInstance.Id, serializableVariable.ProcessInstanceId);
		assertEquals(updatedDate, serializableVariable.Time);

		HistoricVariableUpdate byteArrayVariable = (HistoricVariableUpdate) details[8];
		assertEquals("hVariable", byteArrayVariable.VariableName);
		assertEquals(";-)", StringHelper.NewString((sbyte[])byteArrayVariable.Value));
		assertEquals(processInstance.Id, byteArrayVariable.ProcessInstanceId);
		assertEquals(updatedDate, byteArrayVariable.Time);

		// end process instance
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(1, tasks.Count);
		taskService.complete(tasks[0].Id);
		testHelper.assertProcessEnded(processInstance.Id);

		// check for historic process variables set
		HistoricVariableInstanceQuery historicProcessVariableQuery = historyService.createHistoricVariableInstanceQuery().orderByVariableName().asc();

		assertEquals(8, historicProcessVariableQuery.count());

		IList<HistoricVariableInstance> historicVariables = historicProcessVariableQuery.list();

	 // Variable status when process is finished
		HistoricVariableInstance historicVariable = historicVariables[0];
		assertEquals("aVariable", historicVariable.VariableName);
		assertEquals("updated value", historicVariable.Value);
		assertEquals(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[1];
		assertEquals("bVariable", historicVariable.VariableName);
		assertEquals(123, historicVariable.Value);
		assertEquals(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[2];
		assertEquals("cVariable", historicVariable.VariableName);
		assertEquals(12345L, historicVariable.Value);
		assertEquals(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[3];
		assertEquals("dVariable", historicVariable.VariableName);
		assertEquals(1234.567, historicVariable.Value);
		assertEquals(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[4];
		assertEquals("eVariable", historicVariable.VariableName);
		assertEquals((short) 12, historicVariable.Value);
		assertEquals(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[5];
		assertEquals("fVariable", historicVariable.VariableName);
		assertEquals(sdf.parse("01/01/2001 01:23:45 678"), historicVariable.Value);
		assertEquals(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[6];
		assertEquals("gVariable", historicVariable.VariableName);
		assertEquals(new SerializableVariable("hello hello"), historicVariable.Value);
		assertEquals(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[7];
		assertEquals("hVariable", historicVariable.VariableName);
		assertEquals(";-)", ";-)", StringHelper.NewString((sbyte[])historicVariable.Value));
		assertEquals(processInstance.Id, historicVariable.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricFormProperties() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricFormProperties()
	  {
		DateTime startedDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss SSS")).parse("01/01/2001 01:23:46 000");

		ClockUtil.CurrentTime = startedDate;

		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["formProp1"] = "Activiti rocks";
		formProperties["formProp2"] = "12345";

		ProcessDefinition procDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("historicFormPropertiesProcess").singleResult();

		ProcessInstance processInstance = formService.submitStartFormData(procDef.Id, formProperties);

		// Submit form-properties on the created task
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);

		// Out execution only has a single activity waiting, the task
		IList<string> activityIds = runtimeService.getActiveActivityIds(task.ExecutionId);
		assertNotNull(activityIds);
		assertEquals(1, activityIds.Count);

		string taskActivityId = activityIds[0];

		// Submit form properties
		formProperties = new Dictionary<string, string>();
		formProperties["formProp3"] = "Activiti still rocks!!!";
		formProperties["formProp4"] = "54321";
		formService.submitTaskFormData(task.Id, formProperties);

		// 4 historic form properties should be created. 2 when process started, 2 when task completed
		IList<HistoricDetail> props = historyService.createHistoricDetailQuery().formProperties().processInstanceId(processInstance.Id).orderByFormPropertyId().asc().list();

		HistoricFormProperty historicProperty1 = (HistoricFormProperty) props[0];
		assertEquals("formProp1", historicProperty1.PropertyId);
		assertEquals("Activiti rocks", historicProperty1.PropertyValue);
		assertEquals(startedDate, historicProperty1.Time);
		assertEquals(processInstance.Id, historicProperty1.ProcessInstanceId);
		assertNull(historicProperty1.TaskId);

		assertNotNull(historicProperty1.ActivityInstanceId);
		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityInstanceId(historicProperty1.ActivityInstanceId).singleResult();
		assertNotNull(historicActivityInstance);
		assertEquals("start", historicActivityInstance.ActivityId);

		HistoricFormProperty historicProperty2 = (HistoricFormProperty) props[1];
		assertEquals("formProp2", historicProperty2.PropertyId);
		assertEquals("12345", historicProperty2.PropertyValue);
		assertEquals(startedDate, historicProperty2.Time);
		assertEquals(processInstance.Id, historicProperty2.ProcessInstanceId);
		assertNull(historicProperty2.TaskId);

		assertNotNull(historicProperty2.ActivityInstanceId);
		historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityInstanceId(historicProperty2.ActivityInstanceId).singleResult();
		assertNotNull(historicActivityInstance);
		assertEquals("start", historicActivityInstance.ActivityId);

		HistoricFormProperty historicProperty3 = (HistoricFormProperty) props[2];
		assertEquals("formProp3", historicProperty3.PropertyId);
		assertEquals("Activiti still rocks!!!", historicProperty3.PropertyValue);
		assertEquals(startedDate, historicProperty3.Time);
		assertEquals(processInstance.Id, historicProperty3.ProcessInstanceId);
		string activityInstanceId = historicProperty3.ActivityInstanceId;
		historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityInstanceId(activityInstanceId).singleResult();
		assertNotNull(historicActivityInstance);
		assertEquals(taskActivityId, historicActivityInstance.ActivityId);
		assertNotNull(historicProperty3.TaskId);

		HistoricFormProperty historicProperty4 = (HistoricFormProperty) props[3];
		assertEquals("formProp4", historicProperty4.PropertyId);
		assertEquals("54321", historicProperty4.PropertyValue);
		assertEquals(startedDate, historicProperty4.Time);
		assertEquals(processInstance.Id, historicProperty4.ProcessInstanceId);
		activityInstanceId = historicProperty4.ActivityInstanceId;
		historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityInstanceId(activityInstanceId).singleResult();
		assertNotNull(historicActivityInstance);
		assertEquals(taskActivityId, historicActivityInstance.ActivityId);
		assertNotNull(historicProperty4.TaskId);

		assertEquals(4, props.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"}) public void testHistoricVariableQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricVariableQuery()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "activiti rocks!";
		variables["longVar"] = 12345L;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// Query on activity-instance, activity instance null will return all vars set when starting process
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().activityInstanceId(null).count());
		assertEquals(0, historyService.createHistoricDetailQuery().variableUpdates().activityInstanceId("unexisting").count());

		// Query on process-instance
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).count());
		assertEquals(0, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId("unexisting").count());

		// Query both process-instance and activity-instance
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().activityInstanceId(null).processInstanceId(processInstance.Id).count());

		// end process instance
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(1, tasks.Count);
		taskService.complete(tasks[0].Id);
		testHelper.assertProcessEnded(processInstance.Id);

		assertEquals(2, historyService.createHistoricVariableInstanceQuery().count());

		// Query on process-instance
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().processInstanceId("unexisting").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"}) public void testHistoricVariableQueryExcludeTaskRelatedDetails() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricVariableQueryExcludeTaskRelatedDetails()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "activiti rocks!";
		variables["longVar"] = 12345L;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// Set a local task-variable
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);
		taskService.setVariableLocal(task.Id, "taskVar", "It is I, le Variable");

		// Query on process-instance
		assertEquals(3, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).count());

		// Query on process-instance, excluding task-details
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).excludeTaskDetails().count());

		// Check task-id precedence on excluding task-details
		assertEquals(1, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).excludeTaskDetails().taskId(task.Id).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"}) public void testHistoricFormPropertiesQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricFormPropertiesQuery()
	  {
		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["stringVar"] = "activiti rocks!";
		formProperties["longVar"] = "12345";

		ProcessDefinition procDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult();
		ProcessInstance processInstance = formService.submitStartFormData(procDef.Id, formProperties);

		// Query on activity-instance, activity instance null will return all vars set when starting process
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().activityInstanceId(null).count());
		assertEquals(0, historyService.createHistoricDetailQuery().formProperties().activityInstanceId("unexisting").count());

		// Query on process-instance
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().processInstanceId(processInstance.Id).count());
		assertEquals(0, historyService.createHistoricDetailQuery().formProperties().processInstanceId("unexisting").count());

		// Complete the task by submitting the task properties
		Task task = taskService.createTaskQuery().singleResult();
		formProperties = new Dictionary<string, string>();
		formProperties["taskVar"] = "task form property";
		formService.submitTaskFormData(task.Id, formProperties);

		assertEquals(3, historyService.createHistoricDetailQuery().formProperties().processInstanceId(processInstance.Id).count());
		assertEquals(0, historyService.createHistoricDetailQuery().formProperties().processInstanceId("unexisting").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"}) public void testHistoricVariableQuerySorting() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricVariableQuerySorting()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "activiti rocks!";
		variables["longVar"] = 12345L;

		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByProcessInstanceId().asc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByTime().asc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableName().asc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableRevision().asc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableType().asc().count());

		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByProcessInstanceId().desc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByTime().desc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableName().desc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableRevision().desc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableType().desc().count());

		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByProcessInstanceId().asc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByTime().asc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableName().asc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableRevision().asc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableType().asc().list().size());

		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByProcessInstanceId().desc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByTime().desc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableName().desc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableRevision().desc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().orderByVariableType().desc().list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"}) public void testHistoricFormPropertySorting() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricFormPropertySorting()
	  {

		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["stringVar"] = "activiti rocks!";
		formProperties["longVar"] = "12345";

		ProcessDefinition procDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult();
		formService.submitStartFormData(procDef.Id, formProperties);

		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByProcessInstanceId().asc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByTime().asc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByFormPropertyId().asc().count());

		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByProcessInstanceId().desc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByTime().desc().count());
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByFormPropertyId().desc().count());

		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByProcessInstanceId().asc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByTime().asc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByFormPropertyId().asc().list().size());

		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByProcessInstanceId().desc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByTime().desc().list().size());
		assertEquals(2, historyService.createHistoricDetailQuery().formProperties().orderByFormPropertyId().desc().list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricDetailQueryMixed() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricDetailQueryMixed()
	  {

		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["formProp1"] = "activiti rocks!";
		formProperties["formProp2"] = "12345";

		ProcessDefinition procDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("historicDetailMixed").singleResult();
		ProcessInstance processInstance = formService.submitStartFormData(procDef.Id, formProperties);

		IList<HistoricDetail> details = historyService.createHistoricDetailQuery().processInstanceId(processInstance.Id).orderByVariableName().asc().list();

		assertEquals(4, details.Count);

		assertTrue(details[0] is HistoricFormProperty);
		HistoricFormProperty formProp1 = (HistoricFormProperty) details[0];
		assertEquals("formProp1", formProp1.PropertyId);
		assertEquals("activiti rocks!", formProp1.PropertyValue);

		assertTrue(details[1] is HistoricFormProperty);
		HistoricFormProperty formProp2 = (HistoricFormProperty) details[1];
		assertEquals("formProp2", formProp2.PropertyId);
		assertEquals("12345", formProp2.PropertyValue);


		assertTrue(details[2] is HistoricVariableUpdate);
		HistoricVariableUpdate varUpdate1 = (HistoricVariableUpdate) details[2];
		assertEquals("variable1", varUpdate1.VariableName);
		assertEquals("activiti rocks!", varUpdate1.Value);


		// This variable should be of type LONG since this is defined in the process-definition
		assertTrue(details[3] is HistoricVariableUpdate);
		HistoricVariableUpdate varUpdate2 = (HistoricVariableUpdate) details[3];
		assertEquals("variable2", varUpdate2.VariableName);
		assertEquals(12345L, varUpdate2.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDetailQueryInvalidSorting() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricDetailQueryInvalidSorting()
	  {
		try
		{
		  historyService.createHistoricDetailQuery().asc().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricDetailQuery().desc().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricDetailQuery().orderByProcessInstanceId().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricDetailQuery().orderByTime().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricDetailQuery().orderByVariableName().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricDetailQuery().orderByVariableRevision().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricDetailQuery().orderByVariableType().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricTaskInstanceVariableUpdates()
	  public virtual void testHistoricTaskInstanceVariableUpdates()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest").Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		runtimeService.setVariable(processInstanceId, "deadline", "yesterday");

		taskService.setVariableLocal(taskId, "bucket", "23c");
		taskService.setVariableLocal(taskId, "mop", "37i");

		taskService.complete(taskId);

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().count());

		IList<HistoricDetail> historicTaskVariableUpdates = historyService.createHistoricDetailQuery().taskId(taskId).variableUpdates().orderByVariableName().asc().list();

		assertEquals(2, historicTaskVariableUpdates.Count);

		historyService.deleteHistoricTaskInstance(taskId);

		// Check if the variable updates have been removed as well
		historicTaskVariableUpdates = historyService.createHistoricDetailQuery().taskId(taskId).variableUpdates().orderByVariableName().asc().list();

		 assertEquals(0, historicTaskVariableUpdates.Count);
	  }

	  // ACT-592
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testSetVariableOnProcessInstanceWithTimer()
	  public virtual void testSetVariableOnProcessInstanceWithTimer()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("timerVariablesProcess");
		runtimeService.setVariable(processInstance.Id, "myVar", 123456L);
		assertEquals(123456L, runtimeService.getVariable(processInstance.Id, "myVar"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testDeleteHistoricProcessInstance()
	  public virtual void testDeleteHistoricProcessInstance()
	  {
		// Start process-instance with some variables set
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["processVar"] = 123L;
		vars["anotherProcessVar"] = new DummySerializable();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest", vars);
		assertNotNull(processInstance);

		// Set 2 task properties
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		taskService.setVariableLocal(task.Id, "taskVar", 45678);
		taskService.setVariableLocal(task.Id, "anotherTaskVar", "value");

		// Finish the task, this end the process-instance
		taskService.complete(task.Id);

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(3, historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(4, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(4, historyService.createHistoricDetailQuery().processInstanceId(processInstance.Id).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processInstanceId(processInstance.Id).count());

		// Delete the historic process-instance
		historyService.deleteHistoricProcessInstance(processInstance.Id);

		// Verify no traces are left in the history tables
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(0, historyService.createHistoricDetailQuery().processInstanceId(processInstance.Id).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processInstanceId(processInstance.Id).count());

		try
		{
		  // Delete the historic process-instance, which is still running
		  historyService.deleteHistoricProcessInstance("unexisting");
		  fail("Exception expected when deleting process-instance that is still running");
		}
		catch (ProcessEngineException ae)
		{
		  // Expected exception
		  Assert.assertThat(ae.Message, CoreMatchers.containsString("No historic process instance found with id: unexisting"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testDeleteRunningHistoricProcessInstance()
	  public virtual void testDeleteRunningHistoricProcessInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest");
		assertNotNull(processInstance);

		try
		{
		  // Delete the historic process-instance, which is still running
		  historyService.deleteHistoricProcessInstance(processInstance.Id);
		  fail("Exception expected when deleting process-instance that is still running");
		}
		catch (ProcessEngineException ae)
		{
		  // Expected exception
		  Assert.assertThat(ae.Message, CoreMatchers.containsString("Process instance is still running, cannot delete historic process instance"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testDeleteCachedHistoricDetails()
	  public virtual void testDeleteCachedHistoricDetails()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().getId();
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;


		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, processDefinitionId));

		// the historic process instance should still be there
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().count());

		// the historic details should be deleted
		assertEquals(0, historyService.createHistoricDetailQuery().count());
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly FullHistoryTest outerInstance;

		  private string processDefinitionId;

		  public CommandAnonymousInnerClass(FullHistoryTest outerInstance, string processDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionId = processDefinitionId;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			IDictionary<string, object> formProperties = new Dictionary<string, object>();
			formProperties["formProp1"] = "value1";

			ProcessInstance processInstance = (new SubmitStartFormCmd(processDefinitionId, null, formProperties)).execute(commandContext);

			// two historic details should be in cache: one form property and one variable update
			commandContext.HistoricDetailManager.deleteHistoricDetailsByProcessInstanceIds(Arrays.asList(processInstance.Id));
			return null;
		  }
	  }

	  /// <summary>
	  /// Test created to validate ACT-621 fix.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricFormPropertiesOnReEnteringActivity()
	  public virtual void testHistoricFormPropertiesOnReEnteringActivity()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["comeBack"] = true;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricFormPropertiesProcess", variables);
		assertNotNull(processInstance);

		// Submit form on task
		IDictionary<string, string> data = new Dictionary<string, string>();
		data["formProp1"] = "Property value";

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		formService.submitTaskFormData(task.Id, data);

		// Historic property should be available
		IList<HistoricDetail> details = historyService.createHistoricDetailQuery().formProperties().processInstanceId(processInstance.Id).list();
		assertNotNull(details);
		assertEquals(1, details.Count);

		// Task should be active in the same activity as the previous one
		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		formService.submitTaskFormData(task.Id, data);

		details = historyService.createHistoricDetailQuery().formProperties().processInstanceId(processInstance.Id).list();
		assertNotNull(details);
		assertEquals(2, details.Count);

		// Should have 2 different historic activity instance ID's, with the same activityId
		Assert.assertNotSame(details[0].ActivityInstanceId, details[1].ActivityInstanceId);

		HistoricActivityInstance historicActInst1 = historyService.createHistoricActivityInstanceQuery().activityInstanceId(details[0].ActivityInstanceId).singleResult();

		HistoricActivityInstance historicActInst2 = historyService.createHistoricActivityInstanceQuery().activityInstanceId(details[1].ActivityInstanceId).singleResult();

		assertEquals(historicActInst1.ActivityId, historicActInst2.ActivityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricTaskInstanceQueryTaskVariableValueEquals() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricTaskInstanceQueryTaskVariableValueEquals()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Set some variables on the task
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		taskService.setVariablesLocal(task.Id, variables);

		// Validate all variable-updates are present in DB
		assertEquals(7, historyService.createHistoricDetailQuery().variableUpdates().taskId(task.Id).count());

		// Query Historic task instances based on variable
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("longVar", 12345L).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("shortVar", (short) 123).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("integerVar",1234).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("stringVar","stringValue").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("booleanVar", true).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("dateVar", date).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("nullVar", null).count());

		// Update the variables
		variables["longVar"] = 67890L;
		variables["shortVar"] = (short) 456;
		variables["integerVar"] = 5678;
		variables["stringVar"] = "updatedStringValue";
		variables["booleanVar"] = false;
		DateTime otherCal = new DateTime();
		otherCal.AddDays(1);
		DateTime otherDate = otherCal;
		variables["dateVar"] = otherDate;
		variables["nullVar"] = null;

		taskService.setVariablesLocal(task.Id, variables);

		// Validate all variable-updates are present in DB
		assertEquals(14, historyService.createHistoricDetailQuery().variableUpdates().taskId(task.Id).count());

		// Previous values should NOT match
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("longVar", 12345L).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("shortVar", (short) 123).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("integerVar",1234).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("stringVar","stringValue").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("booleanVar", true).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("dateVar", date).count());

		// New values should match
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("longVar", 67890L).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("shortVar", (short) 456).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("integerVar",5678).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("stringVar","updatedStringValue").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("booleanVar", false).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("dateVar", otherDate).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("nullVar", null).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testHistoricTaskInstanceQueryTaskVariableValueEqualsOverwriteType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testHistoricTaskInstanceQueryTaskVariableValueEqualsOverwriteType()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Set a long variable on a task
		taskService.setVariableLocal(task.Id, "var", 12345L);

		// Validate all variable-updates are present in DB
		assertEquals(1, historyService.createHistoricDetailQuery().variableUpdates().taskId(task.Id).count());

		// Query Historic task instances based on variable
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("var", 12345L).count());

		// Update the variables to an int variable
		taskService.setVariableLocal(task.Id, "var", 12345);

		// Validate all variable-updates are present in DB
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().taskId(task.Id).count());

		// The previous long value should not match
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("var", 12345L).count());

		// The previous int value should not match
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("var", 12345).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricTaskInstanceQueryVariableInParallelBranch() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricTaskInstanceQueryVariableInParallelBranch()
	  {
		runtimeService.startProcessInstanceByKey("parallelGateway");

		// when there are two process variables of the same name but different types
		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		runtimeService.setVariableLocal(task1Execution.Id, "var", 12345L);
		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		runtimeService.setVariableLocal(task2Execution.Id, "var", 12345);

		// then the task query should be able to filter by both variables and return both tasks
		assertEquals(2, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", 12345).count());
		assertEquals(2, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", 12345L).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testHistoricTaskInstanceQueryVariableInParallelBranch.bpmn20.xml") public void testHistoricTaskInstanceQueryVariableOfSameTypeInParallelBranch() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : "org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testHistoricTaskInstanceQueryVariableInParallelBranch.bpmn20.xml")]
	  public virtual void testHistoricTaskInstanceQueryVariableOfSameTypeInParallelBranch()
	  {
		runtimeService.startProcessInstanceByKey("parallelGateway");

		// when there are two process variables of the same name but different types
		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		runtimeService.setVariableLocal(task1Execution.Id, "var", 12345L);
		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		runtimeService.setVariableLocal(task2Execution.Id, "var", 45678L);

		// then the task query should be able to filter by both variables and return both tasks
		assertEquals(2, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", 12345L).count());
		assertEquals(2, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", 45678L).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricTaskInstanceQueryProcessVariableValueEquals() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricTaskInstanceQueryProcessVariableValueEquals()
	  {
		// Set some variables on the process instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest", variables);
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Validate all variable-updates are present in DB
		assertEquals(7, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).count());

		// Query Historic task instances based on process variable
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("longVar", 12345L).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("shortVar", (short) 123).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("integerVar",1234).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("stringVar","stringValue").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("booleanVar", true).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("dateVar", date).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("nullVar", null).count());

		// Update the variables
		variables["longVar"] = 67890L;
		variables["shortVar"] = (short) 456;
		variables["integerVar"] = 5678;
		variables["stringVar"] = "updatedStringValue";
		variables["booleanVar"] = false;
		DateTime otherCal = new DateTime();
		otherCal.AddDays(1);
		DateTime otherDate = otherCal;
		variables["dateVar"] = otherDate;
		variables["nullVar"] = null;

		runtimeService.setVariables(processInstance.Id, variables);

		// Validate all variable-updates are present in DB
		assertEquals(14, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).count());

		// Previous values should NOT match
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("longVar", 12345L).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("shortVar", (short) 123).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("integerVar",1234).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("stringVar","stringValue").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("booleanVar", true).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("dateVar", date).count());

		// New values should match
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("longVar", 67890L).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("shortVar", (short) 456).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("integerVar",5678).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("stringVar","updatedStringValue").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("booleanVar", false).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("dateVar", otherDate).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("nullVar", null).count());

		// Set a task-variables, shouldn't affect the process-variable matches
		taskService.setVariableLocal(task.Id, "longVar", 9999L);
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("longVar", 9999L).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("longVar", 67890L).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricProcessInstanceVariableValueEquals() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricProcessInstanceVariableValueEquals()
	  {
		// Set some variables on the process instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricProcessInstanceTest", variables);

		// Validate all variable-updates are present in DB
		assertEquals(7, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testHistoricProcessInstanceVariableValueEquals.bpmn20.xml"}) public void testHistoricProcessInstanceVariableValueNotEquals() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testHistoricProcessInstanceVariableValueEquals.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceVariableValueNotEquals()
	  {
		// Set some variables on the process instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		DateTime otherCal = new DateTime();
		otherCal.AddDays(1);
		DateTime otherDate = otherCal;
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricProcessInstanceTest", variables);

		// Validate all variable-updates are present in DB
		assertEquals(7, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).count());

		// Query Historic process instances based on process variable, shouldn't match
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("longVar", 12345L).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("shortVar", (short) 123).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("integerVar",1234).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("stringVar","stringValue").count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("booleanVar", true).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("dateVar", date).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueNotEquals("nullVar", null).count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testHistoricProcessInstanceVariableValueEquals.bpmn20.xml"}) public void testHistoricProcessInstanceVariableValueLessThanAndGreaterThan() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testHistoricProcessInstanceVariableValueEquals.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceVariableValueLessThanAndGreaterThan()
	  {
		// Set some variables on the process instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("HistoricProcessInstanceTest", variables);

		// Validate all variable-updates are present in DB
		assertEquals(1, historyService.createHistoricDetailQuery().variableUpdates().processInstanceId(processInstance.Id).count());

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().variableValueGreaterThan("longVar", 12345L).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testVariableUpdatesAreLinkedToActivity.bpmn20.xml"}) public void testVariableUpdatesLinkedToActivity() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/standalone/history/FullHistoryTest.testVariableUpdatesAreLinkedToActivity.bpmn20.xml"})]
	  public virtual void testVariableUpdatesLinkedToActivity()
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
		testHelper.assertProcessEnded(pi.Id);

		// check history
		IList<HistoricDetail> updates = historyService.createHistoricDetailQuery().variableUpdates().list();
		assertEquals(2, updates.Count);

		IDictionary<string, HistoricVariableUpdate> updatesMap = new Dictionary<string, HistoricVariableUpdate>();
		HistoricVariableUpdate update = (HistoricVariableUpdate) updates[0];
		updatesMap[(string)update.Value] = update;
		update = (HistoricVariableUpdate) updates[1];
		updatesMap[(string)update.Value] = update;

		HistoricVariableUpdate update1 = updatesMap["1"];
		HistoricVariableUpdate update2 = updatesMap["2"];

		assertNotNull(update1.ActivityInstanceId);
		assertNotNull(update1.ExecutionId);
		HistoricActivityInstance historicActivityInstance1 = historyService.createHistoricActivityInstanceQuery().activityInstanceId(update1.ActivityInstanceId).singleResult();
		assertEquals(historicActivityInstance1.ExecutionId, update1.ExecutionId);
		assertEquals("usertask1", historicActivityInstance1.ActivityId);

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"}) public void testHistoricDetailQueryByVariableInstanceId() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricDetailQueryByVariableInstanceId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["testVar"] = "testValue";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", @params);

		HistoricVariableInstance testVariable = historyService.createHistoricVariableInstanceQuery().variableName("testVar").singleResult();

		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		query.variableInstanceId(testVariable.Id);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDetailQueryByInvalidVariableInstanceId()
	  public virtual void testHistoricDetailQueryByInvalidVariableInstanceId()
	  {
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		query.variableInstanceId("invalid");
		assertEquals(0, query.count());

		try
		{
		  query.variableInstanceId(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  query.variableInstanceId((string)null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testHistoricDetailActivityInstanceIdForInactiveScopeExecution()
	  public virtual void testHistoricDetailActivityInstanceIdForInactiveScopeExecution()
	  {

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		runtimeService.setVariable(pi.Id, "foo", "bar");

		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().singleResult();
		assertNotNull(historicDetail.ActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDetailQueryById()
	  public virtual void testHistoricDetailQueryById()
	  {

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "someName";
		string variableValue = "someValue";
		taskService.setVariable(newTask.Id, variableName, variableValue);

		HistoricDetail result = historyService.createHistoricDetailQuery().singleResult();

		HistoricDetail resultById = historyService.createHistoricDetailQuery().detailId(result.Id).singleResult();
		assertNotNull(resultById);
		assertEquals(result.Id, resultById.Id);
		assertEquals(variableName, ((HistoricVariableUpdate)resultById).VariableName);
		assertEquals(variableValue, ((HistoricVariableUpdate)resultById).Value);
		assertEquals(ValueType.STRING.Name, ((HistoricVariableUpdate)resultById).VariableTypeName);
		assertEquals(ValueType.STRING.Name, ((HistoricVariableUpdate)resultById).TypeName);

		taskService.deleteTask(newTask.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDetailQueryByNonExistingId()
	  public virtual void testHistoricDetailQueryByNonExistingId()
	  {

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "someName";
		string variableValue = "someValue";
		taskService.setVariable(newTask.Id, variableName, variableValue);

		HistoricDetail result = historyService.createHistoricDetailQuery().detailId("non-existing").singleResult();
		assertNull(result);

		taskService.deleteTask(newTask.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryFetchingEnabled()
	  public virtual void testBinaryFetchingEnabled()
	  {

		// by default, binary fetching is enabled

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "binaryVariableName";
		taskService.setVariable(newTask.Id, variableName, "some bytes".GetBytes());

		HistoricDetail result = historyService.createHistoricDetailQuery().variableUpdates().singleResult();

		assertNotNull(((HistoricVariableUpdate)result).Value);

		taskService.deleteTask(newTask.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryFetchingDisabled()
	  public virtual void testBinaryFetchingDisabled()
	  {

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "binaryVariableName";
		taskService.setVariable(newTask.Id, variableName, "some bytes".GetBytes());

		HistoricDetail result = historyService.createHistoricDetailQuery().disableBinaryFetching().variableUpdates().singleResult();

		assertNull(((HistoricVariableUpdate)result).Value);

		taskService.deleteTask(newTask.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources= "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml") public void testDisableBinaryFetchingForFileValues()
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
		HistoricVariableUpdate fileVariableInstance = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().singleResult();

		// then the binary value is accessible
		assertNotNull(fileVariableInstance.Value);

		// when disabling binary fetching
		fileVariableInstance = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().disableBinaryFetching().singleResult();

		// then the byte value is not fetched
		assertNotNull(fileVariableInstance);
		assertEquals("fileVar", fileVariableInstance.VariableName);

		assertNull(fileVariableInstance.Value);

		FileValue typedValue = (FileValue) fileVariableInstance.TypedValue;
		assertNull(typedValue.Value);

		// but typed value metadata is accessible
		assertEquals(ValueType.FILE, typedValue.Type);
		assertEquals(fileName, typedValue.Filename);
		assertEquals(encoding, typedValue.Encoding);
		assertEquals(mimeType, typedValue.MimeType);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableCustomObjectDeserialization()
	  public virtual void testDisableCustomObjectDeserialization()
	  {

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["customSerializable"] = new CustomSerializable();
		variables["failingSerializable"] = new FailingSerializable();
		taskService.setVariables(newTask.Id, variables);

		IList<HistoricDetail> results = historyService.createHistoricDetailQuery().disableBinaryFetching().disableCustomObjectDeserialization().variableUpdates().list();

		// both variables are not deserialized, but their serialized values are available
		assertEquals(2, results.Count);

		foreach (HistoricDetail update in results)
		{
		  HistoricVariableUpdate variableUpdate = (HistoricVariableUpdate) update;
		  assertNull(variableUpdate.ErrorMessage);

		  ObjectValue typedValue = (ObjectValue) variableUpdate.TypedValue;
		  assertNotNull(typedValue);
		  assertFalse(typedValue.Deserialized);
		  // cannot access the deserialized value
		  try
		  {
			typedValue.Value;
		  }
		  catch (System.InvalidOperationException e)
		  {
			Assert.assertThat(e.Message, CoreMatchers.containsString("Object is not deserialized"));
		  }
		  assertNotNull(typedValue.ValueSerialized);
		}

		taskService.deleteTask(newTask.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorMessage()
	  public virtual void testErrorMessage()
	  {

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "failingSerializable";
		taskService.setVariable(newTask.Id, variableName, new FailingSerializable());

		HistoricDetail result = historyService.createHistoricDetailQuery().disableBinaryFetching().variableUpdates().singleResult();

		assertNull(((HistoricVariableUpdate)result).Value);
		assertNotNull(((HistoricVariableUpdate)result).ErrorMessage);

		taskService.deleteTask(newTask.Id, true);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableInstance()
	  public virtual void testVariableInstance()
	  {

		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);

		string variableName = "someName";
		string variableValue = "someValue";
		taskService.setVariable(newTask.Id, variableName, variableValue);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variable);

		HistoricVariableUpdate result = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().singleResult();
		assertNotNull(result);

		assertEquals(variable.Id, result.VariableInstanceId);

		taskService.deleteTask(newTask.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testHistoricVariableUpdateProcessDefinitionProperty()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testHistoricVariableUpdateProcessDefinitionProperty()
	  {
		// given
		string key = "oneTaskProcess";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(key);

		string processInstanceId = processInstance.Id;
		string taskId = taskService.createTaskQuery().singleResult().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "aValue");
		taskService.setVariableLocal(taskId, "aLocalVariable", "anotherValue");

		string firstVariable = runtimeService.createVariableInstanceQuery().variableName("aVariable").singleResult().Id;

		string secondVariable = runtimeService.createVariableInstanceQuery().variableName("aLocalVariable").singleResult().Id;

		// when (1)
		HistoricVariableUpdate instance = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(firstVariable).singleResult();

		// then (1)
		assertNotNull(instance.ProcessDefinitionKey);
		assertEquals(key, instance.ProcessDefinitionKey);

		assertNotNull(instance.ProcessDefinitionId);
		assertEquals(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

		assertNull(instance.CaseDefinitionKey);
		assertNull(instance.CaseDefinitionId);

		// when (2)
		instance = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(secondVariable).singleResult();

		// then (2)
		assertNotNull(instance.ProcessDefinitionKey);
		assertEquals(key, instance.ProcessDefinitionKey);

		assertNotNull(instance.ProcessDefinitionId);
		assertEquals(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

		assertNull(instance.CaseDefinitionKey);
		assertNull(instance.CaseDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn") public void testHistoricVariableUpdateCaseDefinitionProperty()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testHistoricVariableUpdateCaseDefinitionProperty()
	  {
		// given
		string key = "oneTaskCase";
		CaseInstance caseInstance = caseService.createCaseInstanceByKey(key);

		string caseInstanceId = caseInstance.Id;

		string humanTask = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;
		string taskId = taskService.createTaskQuery().singleResult().Id;

		caseService.setVariable(caseInstanceId, "aVariable", "aValue");
		taskService.setVariableLocal(taskId, "aLocalVariable", "anotherValue");

		string firstVariable = runtimeService.createVariableInstanceQuery().variableName("aVariable").singleResult().Id;

		string secondVariable = runtimeService.createVariableInstanceQuery().variableName("aLocalVariable").singleResult().Id;


		// when (1)
		HistoricVariableUpdate instance = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(firstVariable).singleResult();

		// then (1)
		assertNotNull(instance.CaseDefinitionKey);
		assertEquals(key, instance.CaseDefinitionKey);

		assertNotNull(instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);

		assertNull(instance.ProcessDefinitionKey);
		assertNull(instance.ProcessDefinitionId);

		// when (2)
		instance = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(secondVariable).singleResult();

		// then (2)
		assertNotNull(instance.CaseDefinitionKey);
		assertEquals(key, instance.CaseDefinitionKey);

		assertNotNull(instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);

		assertNull(instance.ProcessDefinitionKey);
		assertNull(instance.ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableUpdateStandaloneTaskDefinitionProperties()
	  public virtual void testHistoricVariableUpdateStandaloneTaskDefinitionProperties()
	  {
		// given
		string taskId = "myTask";
		Task task = taskService.newTask(taskId);
		taskService.saveTask(task);

		taskService.setVariable(taskId, "aVariable", "anotherValue");

		string firstVariable = runtimeService.createVariableInstanceQuery().variableName("aVariable").singleResult().Id;

		// when
		HistoricVariableUpdate instance = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(firstVariable).singleResult();

		// then
		assertNull(instance.ProcessDefinitionKey);
		assertNull(instance.ProcessDefinitionId);
		assertNull(instance.CaseDefinitionKey);
		assertNull(instance.CaseDefinitionId);

		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testHistoricFormFieldProcessDefinitionProperty()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testHistoricFormFieldProcessDefinitionProperty()
	  {
		// given
		string key = "oneTaskProcess";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(key);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		formService.submitTaskForm(taskId, Variables.createVariables().putValue("aVariable", "aValue"));

		// when
		HistoricFormField instance = (HistoricFormField) historyService.createHistoricDetailQuery().formFields().singleResult();

		// then
		assertNotNull(instance.ProcessDefinitionKey);
		assertEquals(key, instance.ProcessDefinitionKey);

		assertNotNull(instance.ProcessDefinitionId);
		assertEquals(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

		assertNull(instance.CaseDefinitionKey);
		assertNull(instance.CaseDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testDeleteProcessInstanceSkipCustomListener()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testDeleteProcessInstanceSkipCustomListener()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		runtimeService.deleteProcessInstance(processInstanceId, null, true);

		// then
		HistoricProcessInstance instance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();
		assertNotNull(instance);

		assertEquals(processInstanceId, instance.Id);
		assertNotNull(instance.EndTime);
	  }

	}

}