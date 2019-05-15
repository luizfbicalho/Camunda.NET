using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.createVariables;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.objectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.anyOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.collection.IsCollectionWithSize.hasSize;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoricDetailVariableInstanceUpdateEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using SimpleSerializableBean = org.camunda.bpm.engine.test.api.runtime.util.SimpleSerializableBean;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using RecordedEvent = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener.RecordedEvent;
	using RecorderTaskListener = org.camunda.bpm.engine.test.bpmn.tasklistener.util.RecorderTaskListener;
	using SerializableVariable = org.camunda.bpm.engine.test.history.SerializableVariable;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using TestExecutionListener = org.camunda.bpm.engine.test.util.TestExecutionListener;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// </summary>
	public class RuntimeServiceTest
	{
		private bool InstanceFieldsInitialized = false;

		public RuntimeServiceTest()
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


	  public const string TESTING_INSTANCE_DELETION = "testing instance deletion";
	  public const string A_STREAM = "aStream";

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JavaSerializationFormatEnabled = true;
			return configuration;
		  }
	  }
	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private RuntimeService runtimeService;
	  private TaskService taskService;
	  private ManagementService managementService;
	  private RepositoryService repositoryService;
	  private HistoryService historyService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		managementService = engineRule.ManagementService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartProcessInstanceByKeyNullKey()
	  public virtual void testStartProcessInstanceByKeyNullKey()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartProcessInstanceByKeyUnexistingKey()
	  public virtual void testStartProcessInstanceByKeyUnexistingKey()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("unexistingkey");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("no processes deployed with key", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartProcessInstanceByIdNullId()
	  public virtual void testStartProcessInstanceByIdNullId()
	  {
		try
		{
		  runtimeService.startProcessInstanceById(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartProcessInstanceByIdUnexistingId()
	  public virtual void testStartProcessInstanceByIdUnexistingId()
	  {
		try
		{
		  runtimeService.startProcessInstanceById("unexistingId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("no deployed process definition found with id", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testStartProcessInstanceByIdNullVariables()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStartProcessInstanceByIdNullVariables()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", (IDictionary<string, object>) null);
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void startProcessInstanceWithBusinessKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// by key
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "123");
		assertNotNull(processInstance);
		assertEquals("123", processInstance.BusinessKey);
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());

		// by key with variables
		processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "456", CollectionUtil.singletonMap("var", "value"));
		assertNotNull(processInstance);
		assertEquals(2, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());
		assertEquals("var", runtimeService.getVariable(processInstance.Id, "var"));

		// by id
		processInstance = runtimeService.startProcessInstanceById(processDefinition.Id, "789");
		assertNotNull(processInstance);
		assertEquals(3, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());

		// by id with variables
		processInstance = runtimeService.startProcessInstanceById(processDefinition.Id, "101123", CollectionUtil.singletonMap("var", "value2"));
		assertNotNull(processInstance);
		assertEquals(4, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());
		assertEquals("var", runtimeService.getVariable(processInstance.Id, "var"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstance()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());

		runtimeService.deleteProcessInstance(processInstance.Id, TESTING_INSTANCE_DELETION);
		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());

		// test that the delete reason of the process instance shows up as delete reason of the task in history
		// ACT-848
		if (!ProcessEngineConfiguration.HISTORY_NONE.Equals(processEngineConfiguration.History))
		{

		  HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().processInstanceId(processInstance.Id).singleResult();

		  assertEquals(TESTING_INSTANCE_DELETION, historicTaskInstance.DeleteReason);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstances()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstances()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// if we skip the custom listeners,
		runtimeService.deleteProcessInstances(Arrays.asList(processInstance.Id,processInstance2.Id), null, false, false);

		assertThat(runtimeService.createProcessInstanceQuery().count(),@is(0l));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testDeleteProcessInstanceWithListeners()
	  public virtual void testDeleteProcessInstanceWithListeners()
	  {
		RecorderExecutionListener.clear();

		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGatewayScopeTasks");

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, "");

		// then
		IList<RecorderExecutionListener.RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(10, recordedEvents.Count);

		ISet<RecorderExecutionListener.RecordedEvent> startEvents = new HashSet<RecorderExecutionListener.RecordedEvent>();
		ISet<RecorderExecutionListener.RecordedEvent> endEvents = new HashSet<RecorderExecutionListener.RecordedEvent>();
		foreach (RecorderExecutionListener.RecordedEvent @event in recordedEvents)
		{
		  if (@event.EventName.Equals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START))
		  {
			startEvents.Add(@event);
		  }
		  else if (@event.EventName.Equals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END))
		  {
			endEvents.Add(@event);
		  }
		}

		assertThat(startEvents, hasSize(5));
		assertThat(endEvents, hasSize(5));
		foreach (RecorderExecutionListener.RecordedEvent startEvent in startEvents)
		{
		  assertThat(startEvent.ActivityId, @is(anyOf(equalTo("innerTask1"), equalTo("innerTask2"), equalTo("outerTask"), equalTo("subProcess"), equalTo("theStart"))));
		  foreach (RecorderExecutionListener.RecordedEvent endEvent in endEvents)
		  {
			if (startEvent.ActivityId.Equals(endEvent.ActivityId))
			{
			  assertThat(startEvent.ActivityInstanceId, @is(endEvent.ActivityInstanceId));
			  assertThat(startEvent.ExecutionId, @is(endEvent.ExecutionId));
			}
		  }
		}
		foreach (RecorderExecutionListener.RecordedEvent recordedEvent in endEvents)
		{
		  assertThat(recordedEvent.ActivityId, @is(anyOf(equalTo("innerTask1"), equalTo("innerTask2"), equalTo("outerTask"), equalTo("subProcess"), nullValue())));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstanceSkipCustomListenersEnsureHistoryWritten()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstanceSkipCustomListenersEnsureHistoryWritten()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// if we skip the custom listeners,
		runtimeService.deleteProcessInstance(processInstance.Id, null, true);

		// buit-in listeners are still invoked and thus history is written
		if (!ProcessEngineConfiguration.HISTORY_NONE.Equals(processEngineConfiguration.History))
		{
		  // verify that all historic activity instances are ended
		  IList<HistoricActivityInstance> hais = historyService.createHistoricActivityInstanceQuery().list();
		  foreach (HistoricActivityInstance hai in hais)
		  {
			assertNotNull(hai.EndTime);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testDeleteProcessInstanceSkipCustomListeners()
	  public virtual void testDeleteProcessInstanceSkipCustomListeners()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// if we do not skip the custom listeners,
		runtimeService.deleteProcessInstance(processInstance.Id, null, false);
		// the custom listener is invoked
		assertTrue(TestExecutionListener.collectedEvents.Count == 1);
		TestExecutionListener.reset();

		processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// if we DO skip the custom listeners,
		runtimeService.deleteProcessInstance(processInstance.Id, null, true);
		// the custom listener is not invoked
		assertTrue(TestExecutionListener.collectedEvents.Count == 0);
		TestExecutionListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testDeleteProcessInstanceSkipCustomListenersScope()
	  public virtual void testDeleteProcessInstanceSkipCustomListenersScope()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// if we do not skip the custom listeners,
		runtimeService.deleteProcessInstance(processInstance.Id, null, false);
		// the custom listener is invoked
		assertTrue(TestExecutionListener.collectedEvents.Count == 1);
		TestExecutionListener.reset();

		processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// if we DO skip the custom listeners,
		runtimeService.deleteProcessInstance(processInstance.Id, null, true);
		// the custom listener is not invoked
		assertTrue(TestExecutionListener.collectedEvents.Count == 0);
		TestExecutionListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testDeleteProcessInstanceSkipCustomTaskListeners()
	  public virtual void testDeleteProcessInstanceSkipCustomTaskListeners()
	  {

		// given a process instance
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// and an empty task listener invocation storage
		RecorderTaskListener.clear();

		// if we do not skip the custom listeners
		runtimeService.deleteProcessInstance(instance.Id, null, false);

		// then the the custom listener is invoked
		assertEquals(1, RecorderTaskListener.RecordedEvents.Count);
		assertEquals(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE, RecorderTaskListener.RecordedEvents[0].Event);

		// if we do skip the custom listeners
		instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		RecorderTaskListener.clear();

		runtimeService.deleteProcessInstance(instance.Id, null, true);

		// then the the custom listener is not invoked
		assertTrue(RecorderTaskListener.RecordedEvents.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcessWithIoMappings.bpmn20.xml" }) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Test public void testDeleteProcessInstanceSkipIoMappings()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcessWithIoMappings.bpmn20.xml" }), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testDeleteProcessInstanceSkipIoMappings()
	  {

		// given a process instance
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("ioMappingProcess");

		// when the process instance is deleted and we do skip the io mappings
		runtimeService.deleteProcessInstance(instance.Id, null, false, true, true);

		// then
		testRule.assertProcessEnded(instance.Id);
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().processInstanceId(instance.Id).list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("inputMappingExecuted").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcessWithIoMappings.bpmn20.xml" }) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Test public void testDeleteProcessInstanceWithoutSkipIoMappings()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcessWithIoMappings.bpmn20.xml" }), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testDeleteProcessInstanceWithoutSkipIoMappings()
	  {

		// given a process instance
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("ioMappingProcess");

		// when the process instance is deleted and we do not skip the io mappings
		runtimeService.deleteProcessInstance(instance.Id, null, false, true, false);

		// then
		testRule.assertProcessEnded(instance.Id);
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().processInstanceId(instance.Id).list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("inputMappingExecuted").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("outputMappingExecuted").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testCascadingDeleteSubprocessInstanceSkipIoMappings.Calling.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testCascadingDeleteSubprocessInstanceSkipIoMappings.Called.bpmn20.xml" }) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Test public void testCascadingDeleteSubprocessInstanceSkipIoMappings()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testCascadingDeleteSubprocessInstanceSkipIoMappings.Calling.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testCascadingDeleteSubprocessInstanceSkipIoMappings.Called.bpmn20.xml" }), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testCascadingDeleteSubprocessInstanceSkipIoMappings()
	  {

		// given a process instance
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("callingProcess");

		ProcessInstance instance2 = runtimeService.createProcessInstanceQuery().superProcessInstanceId(instance.Id).singleResult();

		// when the process instance is deleted and we do skip the io mappings
		runtimeService.deleteProcessInstance(instance.Id, "test_purposes", false, true, true);

		// then
		testRule.assertProcessEnded(instance.Id);
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().processInstanceId(instance2.Id).list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("inputMappingExecuted").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testCascadingDeleteSubprocessInstanceSkipIoMappings.Calling.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testCascadingDeleteSubprocessInstanceSkipIoMappings.Called.bpmn20.xml" }) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Test public void testCascadingDeleteSubprocessInstanceWithoutSkipIoMappings()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testCascadingDeleteSubprocessInstanceSkipIoMappings.Calling.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testCascadingDeleteSubprocessInstanceSkipIoMappings.Called.bpmn20.xml" }), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testCascadingDeleteSubprocessInstanceWithoutSkipIoMappings()
	  {

		// given a process instance
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("callingProcess");

		ProcessInstance instance2 = runtimeService.createProcessInstanceQuery().superProcessInstanceId(instance.Id).singleResult();

		// when the process instance is deleted and we do not skip the io mappings
		runtimeService.deleteProcessInstance(instance.Id, "test_purposes", false, true, false);

		// then
		testRule.assertProcessEnded(instance.Id);
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().processInstanceId(instance2.Id).list().size());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("inputMappingExecuted").count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("outputMappingExecuted").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstanceNullReason()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstanceNullReason()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());

		// Deleting without a reason should be possible
		runtimeService.deleteProcessInstance(processInstance.Id, null);
		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());
	  }

	  /// <summary>
	  /// CAM-8005 - StackOverflowError must not happen.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstancesManyParallelSubprocesses()
	  public virtual void testDeleteProcessInstancesManyParallelSubprocesses()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance multiInstanceWithSubprocess = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("multiInstanceWithSubprocess").startEvent().subProcess().embeddedSubProcess().startEvent().userTask("userTask").endEvent().subProcessDone().multiInstance().cardinality("300").multiInstanceDone().endEvent().done();
		BpmnModelInstance multiInstanceWithSubprocess = Bpmn.createExecutableProcess("multiInstanceWithSubprocess").startEvent().subProcess().embeddedSubProcess().startEvent().userTask("userTask").endEvent().subProcessDone().multiInstance().cardinality("300").multiInstanceDone().endEvent().done();

		testRule.deploy(multiInstanceWithSubprocess);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("multiInstanceWithSubprocess");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("multiInstanceWithSubprocess");

		runtimeService.deleteProcessInstance(processInstance.Id, "some reason");
		assertEquals(0, runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceWithFake()
	  public virtual void testDeleteProcessInstanceWithFake()
	  {
		try
		{
		  runtimeService.deleteProcessInstance("aFake", null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("No process instance found for id", ae.Message);
		  assertTrue(ae is BadUserRequestException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceIfExistsWithFake()
	  public virtual void testDeleteProcessInstanceIfExistsWithFake()
	  {
		  runtimeService.deleteProcessInstanceIfExists("aFake", null, false, false, false, false);
		  //don't expect exception
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesWithFake()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesWithFake()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		try
		{
		  runtimeService.deleteProcessInstances(Arrays.asList(instance.Id, "aFake"), "test", false, false, false);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException)
		{
		  //expected
		  assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesIfExistsWithFake()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesIfExistsWithFake()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		runtimeService.deleteProcessInstancesIfExists(Arrays.asList(instance.Id, "aFake"), "test", false, false, false);
		//dont't expect exception, existing instances are deleted
		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceNullId()
	  public virtual void testDeleteProcessInstanceNullId()
	  {
		try
		{
		  runtimeService.deleteProcessInstance(null, "test null id delete");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("processInstanceId is null", ae.Message);
		  assertTrue(ae is BadUserRequestException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testDeleteProcessInstanceWithActiveCompensation()
	  public virtual void testDeleteProcessInstanceWithActiveCompensation()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("compensationProcess");

		Task innerTask = taskService.createTaskQuery().singleResult();
		taskService.complete(innerTask.Id);

		Task afterSubProcessTask = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterSubprocess", afterSubProcessTask.TaskDefinitionKey);
		taskService.complete(afterSubProcessTask.Id);

		// when
		// there are two compensation tasks
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("outerAfterBoundaryTask").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("innerAfterBoundaryTask").count());

		// when the process instance is deleted
		runtimeService.deleteProcessInstance(instance.Id, "");

		// then
		testRule.assertProcessEnded(instance.Id);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testDeleteProcessInstanceWithVariableOnScopeAndConcurrentExecution()
	  public virtual void testDeleteProcessInstanceWithVariableOnScopeAndConcurrentExecution()
	  {

		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task").execute();

		IList<Execution> executions = runtimeService.createExecutionQuery().list();

		foreach (Execution execution in executions)
		{
		  runtimeService.setVariableLocal(execution.Id, "foo", "bar");
		}

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		testRule.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl.HISTORY_AUDIT) public void testDeleteCalledSubprocess()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl.HISTORY_AUDIT)]
	  public virtual void testDeleteCalledSubprocess()
	  {

		// given
		BpmnModelInstance callingInstance = ProcessModels.newModel("oneTaskProcess").startEvent().callActivity().calledElement("called").endEvent().done();

		BpmnModelInstance calledInstance = ProcessModels.newModel("called").startEvent().userTask().endEvent().done();

		testRule.deploy(callingInstance, calledInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").getProcessInstanceId();
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").ProcessInstanceId;

		string subprocessId = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("called").singleResult().Id;

		runtimeService.deleteProcessInstance(subprocessId, TESTING_INSTANCE_DELETION);

		assertEquals(TESTING_INSTANCE_DELETION, historyService.createHistoricProcessInstanceQuery().processInstanceId(subprocessId).singleResult().DeleteReason);
		assertEquals(TESTING_INSTANCE_DELETION, historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult().DeleteReason);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testFindActiveActivityIds()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testFindActiveActivityIds()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<string> activities = runtimeService.getActiveActivityIds(processInstance.Id);
		assertNotNull(activities);
		assertEquals(1, activities.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindActiveActivityIdsUnexistingExecututionId()
	  public virtual void testFindActiveActivityIdsUnexistingExecututionId()
	  {
		try
		{
		  runtimeService.getActiveActivityIds("unexistingExecutionId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindActiveActivityIdsNullExecututionId()
	  public virtual void testFindActiveActivityIdsNullExecututionId()
	  {
		try
		{
		  runtimeService.getActiveActivityIds(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

	  /// <summary>
	  /// Testcase to reproduce ACT-950 (https://jira.codehaus.org/browse/ACT-950)
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testFindActiveActivityIdProcessWithErrorEventAndSubProcess()
	  public virtual void testFindActiveActivityIdProcessWithErrorEventAndSubProcess()
	  {
		ProcessInstance processInstance = engineRule.ProcessEngine.RuntimeService.startProcessInstanceByKey("errorEventSubprocess");

		IList<string> activeActivities = runtimeService.getActiveActivityIds(processInstance.Id);
		assertEquals(3, activeActivities.Count);

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		Task parallelUserTask = null;
		foreach (Task task in tasks)
		{
		  if (!task.Name.Equals("ParallelUserTask") && !task.Name.Equals("MainUserTask"))
		  {
			fail("Expected: <ParallelUserTask> or <MainUserTask> but was <" + task.Name + ">.");
		  }
		  if (task.Name.Equals("ParallelUserTask"))
		  {
			parallelUserTask = task;
		  }
		}
		assertNotNull(parallelUserTask);

		taskService.complete(parallelUserTask.Id);

		Execution execution = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).activityId("subprocess1WaitBeforeError").singleResult();
		runtimeService.signal(execution.Id);

		activeActivities = runtimeService.getActiveActivityIds(processInstance.Id);
		assertEquals(2, activeActivities.Count);

		tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		Task beforeErrorUserTask = null;
		foreach (Task task in tasks)
		{
		  if (!task.Name.Equals("BeforeError") && !task.Name.Equals("MainUserTask"))
		  {
			fail("Expected: <BeforeError> or <MainUserTask> but was <" + task.Name + ">.");
		  }
		  if (task.Name.Equals("BeforeError"))
		  {
			beforeErrorUserTask = task;
		  }
		}
		assertNotNull(beforeErrorUserTask);

		taskService.complete(beforeErrorUserTask.Id);

		activeActivities = runtimeService.getActiveActivityIds(processInstance.Id);
		assertEquals(2, activeActivities.Count);

		tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		Task afterErrorUserTask = null;
		foreach (Task task in tasks)
		{
		  if (!task.Name.Equals("AfterError") && !task.Name.Equals("MainUserTask"))
		  {
			fail("Expected: <AfterError> or <MainUserTask> but was <" + task.Name + ">.");
		  }
		  if (task.Name.Equals("AfterError"))
		  {
			afterErrorUserTask = task;
		  }
		}
		assertNotNull(afterErrorUserTask);

		taskService.complete(afterErrorUserTask.Id);

		tasks = taskService.createTaskQuery().list();
		assertEquals(1, tasks.Count);
		assertEquals("MainUserTask", tasks[0].Name);

		activeActivities = runtimeService.getActiveActivityIds(processInstance.Id);
		assertEquals(1, activeActivities.Count);
		assertEquals("MainUserTask", activeActivities[0]);

		taskService.complete(tasks[0].Id);

		testRule.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalUnexistingExecututionId()
	  public virtual void testSignalUnexistingExecututionId()
	  {
		try
		{
		  runtimeService.signal("unexistingExecutionId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
		  assertTrue(ae is BadUserRequestException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalNullExecutionId()
	  public virtual void testSignalNullExecutionId()
	  {
		try
		{
		  runtimeService.signal(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		  assertTrue(ae is BadUserRequestException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSignalWithProcessVariables()
	  public virtual void testSignalWithProcessVariables()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testSignalWithProcessVariables");
		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["variable"] = "value";

		// signal the execution while passing in the variables
		runtimeService.signal(processInstance.Id, processVariables);

		IDictionary<string, object> variables = runtimeService.getVariables(processInstance.Id);
		assertEquals(variables, processVariables);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testSignalWithProcessVariables.bpmn20.xml"}) @Test public void testSignalWithSignalNameAndData()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testSignalWithProcessVariables.bpmn20.xml"})]
	  public virtual void testSignalWithSignalNameAndData()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testSignalWithProcessVariables");
		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["variable"] = "value";

		// signal the execution while passing in the variables
		runtimeService.signal(processInstance.Id, "dummySignalName", "SignalData", processVariables);

		IDictionary<string, object> variables = runtimeService.getVariables(processInstance.Id);
		assertEquals(variables, processVariables);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testSignalWithProcessVariables.bpmn20.xml"}) @Test public void testSignalWithoutSignalNameAndData()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testSignalWithProcessVariables.bpmn20.xml"})]
	  public virtual void testSignalWithoutSignalNameAndData()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testSignalWithProcessVariables");
		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["variable"] = "value";

		// signal the execution while passing in the variables
		runtimeService.signal(processInstance.Id, null, null, processVariables);

		IDictionary<string, object> variables = runtimeService.getVariables(processInstance.Id);
		assertEquals(processVariables, variables);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSignalInactiveExecution()
	  public virtual void testSignalInactiveExecution()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("testSignalInactiveExecution");

		// there exist two executions: the inactive parent (the process instance) and the child that actually waits in the receive task
		try
		{
		  runtimeService.signal(instance.Id);
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  testRule.assertTextPresent("cannot signal execution " + instance.Id + ": it has no current activity", e.Message);
		}
		catch (Exception)
		{
		  fail("Signalling an inactive execution that has no activity should result in a ProcessEngineException");
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesUnexistingExecutionId()
	  public virtual void testGetVariablesUnexistingExecutionId()
	  {
		try
		{
		  runtimeService.getVariables("unexistingExecutionId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesNullExecutionId()
	  public virtual void testGetVariablesNullExecutionId()
	  {
		try
		{
		  runtimeService.getVariables(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableUnexistingExecutionId()
	  public virtual void testGetVariableUnexistingExecutionId()
	  {
		try
		{
		  runtimeService.getVariables("unexistingExecutionId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableNullExecutionId()
	  public virtual void testGetVariableNullExecutionId()
	  {
		try
		{
		  runtimeService.getVariables(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariableUnexistingVariableName()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariableUnexistingVariableName()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		object variableValue = runtimeService.getVariable(processInstance.Id, "unexistingVariable");
		assertNull(variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableUnexistingExecutionId()
	  public virtual void testSetVariableUnexistingExecutionId()
	  {
		try
		{
		  runtimeService.setVariable("unexistingExecutionId", "variableName", "value");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableNullExecutionId()
	  public virtual void testSetVariableNullExecutionId()
	  {
		try
		{
		  runtimeService.setVariable(null, "variableName", "variableValue");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testSetVariableNullVariableName()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSetVariableNullVariableName()
	  {
		try
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		  runtimeService.setVariable(processInstance.Id, null, "variableValue");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("variableName is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testSetVariables()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSetVariables()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariables(processInstance.Id, vars);

		assertEquals("value1", runtimeService.getVariable(processInstance.Id, "variable1"));
		assertEquals("value2", runtimeService.getVariable(processInstance.Id, "variable2"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariablesTyped()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesTyped()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);
		VariableMap variablesTyped = runtimeService.getVariablesTyped(processInstance.Id);
		assertEquals(vars, variablesTyped);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariablesTypedDeserialize()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesTypedDeserialize()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("broken", Variables.serializedObjectValue("broken").serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName("unexisting").create()));

		// this works
		VariableMap variablesTyped = runtimeService.getVariablesTyped(processInstance.Id, false);
		assertNotNull(variablesTyped.getValueTyped("broken"));
		variablesTyped = runtimeService.getVariablesTyped(processInstance.Id, Arrays.asList("broken"), false);
		assertNotNull(variablesTyped.getValueTyped("broken"));

		// this does not
		try
		{
		  runtimeService.getVariablesTyped(processInstance.Id);
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Cannot deserialize object", e.Message);
		}

		// this does not
		try
		{
		  runtimeService.getVariablesTyped(processInstance.Id, Arrays.asList("broken"), true);
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Cannot deserialize object", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariablesLocalTyped()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesLocalTyped()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);
		VariableMap variablesTyped = runtimeService.getVariablesLocalTyped(processInstance.Id);
		assertEquals(vars, variablesTyped);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariablesLocalTypedDeserialize()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesLocalTypedDeserialize()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("broken", Variables.serializedObjectValue("broken").serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName("unexisting").create()));

		// this works
		VariableMap variablesTyped = runtimeService.getVariablesLocalTyped(processInstance.Id, false);
		assertNotNull(variablesTyped.getValueTyped("broken"));
		variablesTyped = runtimeService.getVariablesLocalTyped(processInstance.Id, Arrays.asList("broken"), false);
		assertNotNull(variablesTyped.getValueTyped("broken"));

		// this does not
		try
		{
		  runtimeService.getVariablesLocalTyped(processInstance.Id);
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Cannot deserialize object", e.Message);
		}

		// this does not
		try
		{
		  runtimeService.getVariablesLocalTyped(processInstance.Id, Arrays.asList("broken"), true);
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Cannot deserialize object", e.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testSetVariablesUnexistingExecutionId()
	  public virtual void testSetVariablesUnexistingExecutionId()
	  {
		try
		{
		  runtimeService.setVariables("unexistingexecution", Collections.EMPTY_MAP);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("execution unexistingexecution doesn't exist", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testSetVariablesNullExecutionId()
	  public virtual void testSetVariablesNullExecutionId()
	  {
		try
		{
		  runtimeService.setVariables(null, Collections.EMPTY_MAP);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

	  private void checkHistoricVariableUpdateEntity(string variableName, string processInstanceId)
	  {
		if (processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
		{
		  bool deletedVariableUpdateFound = false;

		  IList<HistoricDetail> resultSet = historyService.createHistoricDetailQuery().processInstanceId(processInstanceId).list();
		  foreach (HistoricDetail currentHistoricDetail in resultSet)
		  {
			assertTrue(currentHistoricDetail is HistoricDetailVariableInstanceUpdateEntity);
			HistoricDetailVariableInstanceUpdateEntity historicVariableUpdate = (HistoricDetailVariableInstanceUpdateEntity) currentHistoricDetail;

			if (historicVariableUpdate.Name.Equals(variableName))
			{
			  if (historicVariableUpdate.Value == null)
			  {
				if (deletedVariableUpdateFound)
				{
				  fail("Mismatch: A HistoricVariableUpdateEntity with a null value already found");
				}
				else
				{
				  deletedVariableUpdateFound = true;
				}
			  }
			}
		  }

		  assertTrue(deletedVariableUpdateFound);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testRemoveVariable()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testRemoveVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariables(processInstance.Id, vars);

		runtimeService.removeVariable(processInstance.Id, "variable1");

		assertNull(runtimeService.getVariable(processInstance.Id, "variable1"));
		assertNull(runtimeService.getVariableLocal(processInstance.Id, "variable1"));
		assertEquals("value2", runtimeService.getVariable(processInstance.Id, "variable2"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"}) @Test public void testRemoveVariableInParentScope()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"})]
	  public virtual void testRemoveVariableInParentScope()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startSimpleSubProcess", vars);
		Task currentTask = taskService.createTaskQuery().singleResult();

		runtimeService.removeVariable(currentTask.ExecutionId, "variable1");

		assertNull(runtimeService.getVariable(processInstance.Id, "variable1"));
		assertEquals("value2", runtimeService.getVariable(processInstance.Id, "variable2"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveVariableNullExecutionId()
	  public virtual void testRemoveVariableNullExecutionId()
	  {
		try
		{
		  runtimeService.removeVariable(null, "variable");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testRemoveVariableLocal()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testRemoveVariableLocal()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);
		runtimeService.removeVariableLocal(processInstance.Id, "variable1");

		assertNull(runtimeService.getVariable(processInstance.Id, "variable1"));
		assertNull(runtimeService.getVariableLocal(processInstance.Id, "variable1"));
		assertEquals("value2", runtimeService.getVariable(processInstance.Id, "variable2"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"}) @Test public void testRemoveVariableLocalWithParentScope()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"})]
	  public virtual void testRemoveVariableLocalWithParentScope()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startSimpleSubProcess", vars);
		Task currentTask = taskService.createTaskQuery().singleResult();
		runtimeService.setVariableLocal(currentTask.ExecutionId, "localVariable", "local value");

		assertEquals("local value", runtimeService.getVariableLocal(currentTask.ExecutionId, "localVariable"));

		runtimeService.removeVariableLocal(currentTask.ExecutionId, "localVariable");

		assertNull(runtimeService.getVariable(currentTask.ExecutionId, "localVariable"));
		assertNull(runtimeService.getVariableLocal(currentTask.ExecutionId, "localVariable"));

		assertEquals("value1", runtimeService.getVariable(processInstance.Id, "variable1"));
		assertEquals("value2", runtimeService.getVariable(processInstance.Id, "variable2"));

		assertEquals("value1", runtimeService.getVariable(currentTask.ExecutionId, "variable1"));
		assertEquals("value2", runtimeService.getVariable(currentTask.ExecutionId, "variable2"));

		checkHistoricVariableUpdateEntity("localVariable", processInstance.Id);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveLocalVariableNullExecutionId()
	  public virtual void testRemoveLocalVariableNullExecutionId()
	  {
		try
		{
		  runtimeService.removeVariableLocal(null, "variable");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testRemoveVariables()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testRemoveVariables()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);
		runtimeService.setVariable(processInstance.Id, "variable3", "value3");

		runtimeService.removeVariables(processInstance.Id, vars.Keys);

		assertNull(runtimeService.getVariable(processInstance.Id, "variable1"));
		assertNull(runtimeService.getVariableLocal(processInstance.Id, "variable1"));
		assertNull(runtimeService.getVariable(processInstance.Id, "variable2"));
		assertNull(runtimeService.getVariableLocal(processInstance.Id, "variable2"));

		assertEquals("value3", runtimeService.getVariable(processInstance.Id, "variable3"));
		assertEquals("value3", runtimeService.getVariableLocal(processInstance.Id, "variable3"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
		checkHistoricVariableUpdateEntity("variable2", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"}) @Test public void testRemoveVariablesWithParentScope()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"})]
	  public virtual void testRemoveVariablesWithParentScope()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startSimpleSubProcess", vars);
		runtimeService.setVariable(processInstance.Id, "variable3", "value3");

		Task currentTask = taskService.createTaskQuery().singleResult();

		runtimeService.removeVariables(currentTask.ExecutionId, vars.Keys);

		assertNull(runtimeService.getVariable(processInstance.Id, "variable1"));
		assertNull(runtimeService.getVariableLocal(processInstance.Id, "variable1"));
		assertNull(runtimeService.getVariable(processInstance.Id, "variable2"));
		assertNull(runtimeService.getVariableLocal(processInstance.Id, "variable2"));

		assertEquals("value3", runtimeService.getVariable(processInstance.Id, "variable3"));
		assertEquals("value3", runtimeService.getVariableLocal(processInstance.Id, "variable3"));

		assertNull(runtimeService.getVariable(currentTask.ExecutionId, "variable1"));
		assertNull(runtimeService.getVariable(currentTask.ExecutionId, "variable2"));

		assertEquals("value3", runtimeService.getVariable(currentTask.ExecutionId, "variable3"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
		checkHistoricVariableUpdateEntity("variable2", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testRemoveVariablesNullExecutionId()
	  public virtual void testRemoveVariablesNullExecutionId()
	  {
		try
		{
		  runtimeService.removeVariables(null, Collections.EMPTY_LIST);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"}) @Test public void testRemoveVariablesLocalWithParentScope()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"})]
	  public virtual void testRemoveVariablesLocalWithParentScope()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startSimpleSubProcess", vars);

		Task currentTask = taskService.createTaskQuery().singleResult();
		IDictionary<string, object> varsToDelete = new Dictionary<string, object>();
		varsToDelete["variable3"] = "value3";
		varsToDelete["variable4"] = "value4";
		varsToDelete["variable5"] = "value5";
		runtimeService.setVariablesLocal(currentTask.ExecutionId, varsToDelete);
		runtimeService.setVariableLocal(currentTask.ExecutionId, "variable6", "value6");

		assertEquals("value3", runtimeService.getVariable(currentTask.ExecutionId, "variable3"));
		assertEquals("value3", runtimeService.getVariableLocal(currentTask.ExecutionId, "variable3"));
		assertEquals("value4", runtimeService.getVariable(currentTask.ExecutionId, "variable4"));
		assertEquals("value4", runtimeService.getVariableLocal(currentTask.ExecutionId, "variable4"));
		assertEquals("value5", runtimeService.getVariable(currentTask.ExecutionId, "variable5"));
		assertEquals("value5", runtimeService.getVariableLocal(currentTask.ExecutionId, "variable5"));
		assertEquals("value6", runtimeService.getVariable(currentTask.ExecutionId, "variable6"));
		assertEquals("value6", runtimeService.getVariableLocal(currentTask.ExecutionId, "variable6"));

		runtimeService.removeVariablesLocal(currentTask.ExecutionId, varsToDelete.Keys);

		assertEquals("value1", runtimeService.getVariable(currentTask.ExecutionId, "variable1"));
		assertEquals("value2", runtimeService.getVariable(currentTask.ExecutionId, "variable2"));

		assertNull(runtimeService.getVariable(currentTask.ExecutionId, "variable3"));
		assertNull(runtimeService.getVariableLocal(currentTask.ExecutionId, "variable3"));
		assertNull(runtimeService.getVariable(currentTask.ExecutionId, "variable4"));
		assertNull(runtimeService.getVariableLocal(currentTask.ExecutionId, "variable4"));
		assertNull(runtimeService.getVariable(currentTask.ExecutionId, "variable5"));
		assertNull(runtimeService.getVariableLocal(currentTask.ExecutionId, "variable5"));

		assertEquals("value6", runtimeService.getVariable(currentTask.ExecutionId, "variable6"));
		assertEquals("value6", runtimeService.getVariableLocal(currentTask.ExecutionId, "variable6"));

		checkHistoricVariableUpdateEntity("variable3", processInstance.Id);
		checkHistoricVariableUpdateEntity("variable4", processInstance.Id);
		checkHistoricVariableUpdateEntity("variable5", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testRemoveVariablesLocalNullExecutionId()
	  public virtual void testRemoveVariablesLocalNullExecutionId()
	  {
		try
		{
		  runtimeService.removeVariablesLocal(null, Collections.EMPTY_LIST);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("executionId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testUpdateVariables()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testUpdateVariables()
	  {
		IDictionary<string, object> modifications = new Dictionary<string, object>();
		modifications["variable1"] = "value1";
		modifications["variable2"] = "value2";

		IList<string> deletions = new List<string>();
		deletions.Add("variable1");

		IDictionary<string, object> initialVariables = new Dictionary<string, object>();
		initialVariables["variable1"] = "initialValue";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", initialVariables);
		((RuntimeServiceImpl) runtimeService).updateVariables(processInstance.Id, modifications, deletions);

		assertNull(runtimeService.getVariable(processInstance.Id, "variable1"));
		assertEquals("value2", runtimeService.getVariable(processInstance.Id, "variable2"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"}) @Test public void testUpdateVariablesLocal()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"})]
	  public virtual void testUpdateVariablesLocal()
	  {
		IDictionary<string, object> globalVars = new Dictionary<string, object>();
		globalVars["variable4"] = "value4";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startSimpleSubProcess", globalVars);

		Task currentTask = taskService.createTaskQuery().singleResult();
		IDictionary<string, object> localVars = new Dictionary<string, object>();
		localVars["variable1"] = "value1";
		localVars["variable2"] = "value2";
		localVars["variable3"] = "value3";
		runtimeService.setVariablesLocal(currentTask.ExecutionId, localVars);

		IDictionary<string, object> modifications = new Dictionary<string, object>();
		modifications["variable1"] = "anotherValue1";
		modifications["variable2"] = "anotherValue2";

		IList<string> deletions = new List<string>();
		deletions.Add("variable2");
		deletions.Add("variable3");
		deletions.Add("variable4");

		((RuntimeServiceImpl) runtimeService).updateVariablesLocal(currentTask.ExecutionId, modifications, deletions);

		assertEquals("anotherValue1", runtimeService.getVariable(currentTask.ExecutionId, "variable1"));
		assertNull(runtimeService.getVariable(currentTask.ExecutionId, "variable2"));
		assertNull(runtimeService.getVariable(currentTask.ExecutionId, "variable3"));
		assertEquals("value4", runtimeService.getVariable(processInstance.Id, "variable4"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchAlertSignal.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchPanicSignal.bpmn20.xml" }) @Test public void testSignalEventReceived()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchAlertSignal.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchPanicSignal.bpmn20.xml" })]
	  public virtual void testSignalEventReceived()
	  {

		//////  test  signalEventReceived(String)

		startSignalCatchProcesses();
		// 12, because the signal catch is a scope
		assertEquals(12, runtimeService.createExecutionQuery().count());
		runtimeService.signalEventReceived("alert");
		assertEquals(6, runtimeService.createExecutionQuery().count());
		runtimeService.signalEventReceived("panic");
		assertEquals(0, runtimeService.createExecutionQuery().count());

		//////  test  signalEventReceived(String, String)
		startSignalCatchProcesses();

		// signal the executions one at a time:
		for (int executions = 3; executions > 0; executions--)
		{
		  IList<Execution> page = runtimeService.createExecutionQuery().signalEventSubscriptionName("alert").listPage(0, 1);
		  runtimeService.signalEventReceived("alert", page[0].Id);

		  assertEquals(executions - 1, runtimeService.createExecutionQuery().signalEventSubscriptionName("alert").count());
		}

		for (int executions = 3; executions > 0; executions--)
		{
		  IList<Execution> page = runtimeService.createExecutionQuery().signalEventSubscriptionName("panic").listPage(0, 1);
		  runtimeService.signalEventReceived("panic", page[0].Id);

		  assertEquals(executions - 1, runtimeService.createExecutionQuery().signalEventSubscriptionName("panic").count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchAlertMessage.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchPanicMessage.bpmn20.xml" }) @Test public void testMessageEventReceived()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchAlertMessage.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchPanicMessage.bpmn20.xml" })]
	  public virtual void testMessageEventReceived()
	  {

		startMessageCatchProcesses();
		// 12, because the signal catch is a scope
		assertEquals(12, runtimeService.createExecutionQuery().count());

		// signal the executions one at a time:
		for (int executions = 3; executions > 0; executions--)
		{
		  IList<Execution> page = runtimeService.createExecutionQuery().messageEventSubscriptionName("alert").listPage(0, 1);
		  runtimeService.messageEventReceived("alert", page[0].Id);

		  assertEquals(executions - 1, runtimeService.createExecutionQuery().messageEventSubscriptionName("alert").count());
		}

		for (int executions = 3; executions > 0; executions--)
		{
		  IList<Execution> page = runtimeService.createExecutionQuery().messageEventSubscriptionName("panic").listPage(0, 1);
		  runtimeService.messageEventReceived("panic", page[0].Id);

		  assertEquals(executions - 1, runtimeService.createExecutionQuery().messageEventSubscriptionName("panic").count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalEventReceivedNonExistingExecution()
	 public virtual void testSignalEventReceivedNonExistingExecution()
	 {
	   try
	   {
		 runtimeService.signalEventReceived("alert", "nonexistingExecution");
		 fail("exeception expected");
	   }
	   catch (ProcessEngineException e)
	   {
		 // this is good
		 assertTrue(e.Message.contains("Cannot find execution with id 'nonexistingExecution'"));
	   }
	 }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEventReceivedNonExistingExecution()
	 public virtual void testMessageEventReceivedNonExistingExecution()
	 {
	   try
	   {
		 runtimeService.messageEventReceived("alert", "nonexistingExecution");
		 fail("exeception expected");
	   }
	   catch (ProcessEngineException e)
	   {
		 // this is good
		 assertTrue(e.Message.contains("Execution with id 'nonexistingExecution' does not have a subscription to a message event with name 'alert'"));
	   }
	 }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchAlertSignal.bpmn20.xml" }) @Test public void testExecutionWaitingForDifferentSignal()
	 [Deployment(resources:{ "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.catchAlertSignal.bpmn20.xml" })]
	 public virtual void testExecutionWaitingForDifferentSignal()
	 {
	   runtimeService.startProcessInstanceByKey("catchAlertSignal");
	   Execution execution = runtimeService.createExecutionQuery().signalEventSubscriptionName("alert").singleResult();
	   try
	   {
		 runtimeService.signalEventReceived("bogusSignal", execution.Id);
		 fail("exeception expected");
	   }
	   catch (ProcessEngineException e)
	   {
		 // this is good
		 assertTrue(e.Message.contains("has not subscribed to a signal event with name 'bogusSignal'"));
	   }
	 }

	  private void startSignalCatchProcesses()
	  {
		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("catchAlertSignal");
		  runtimeService.startProcessInstanceByKey("catchPanicSignal");
		}
	  }

	  private void startMessageCatchProcesses()
	  {
		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("catchAlertMessage");
		  runtimeService.startProcessInstanceByKey("catchPanicMessage");
		}
	  }

	  // getActivityInstance Tests //////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityInstanceForNonExistingProcessInstanceId()
	  public virtual void testActivityInstanceForNonExistingProcessInstanceId()
	  {
		assertNull(runtimeService.getActivityInstance("some-nonexisting-id"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityInstanceForNullProcessInstanceId()
	  public virtual void testActivityInstanceForNullProcessInstanceId()
	  {
		try
		{
		  runtimeService.getActivityInstance(null);
		  fail("PEE expected!");
		}
		catch (ProcessEngineException engineException)
		{
		  assertTrue(engineException.Message.contains("processInstanceId is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testActivityInstancePopulated()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testActivityInstancePopulated()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "business-key");

		// validate properties of root
		ActivityInstance rootActInstance = runtimeService.getActivityInstance(processInstance.Id);
		assertEquals(processInstance.Id, rootActInstance.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, rootActInstance.ProcessDefinitionId);
		assertEquals(processInstance.Id, rootActInstance.ProcessInstanceId);
		assertTrue(rootActInstance.ExecutionIds[0].Equals(processInstance.Id));
		assertEquals(rootActInstance.ProcessDefinitionId, rootActInstance.ActivityId);
		assertNull(rootActInstance.ParentActivityInstanceId);
		assertEquals("processDefinition", rootActInstance.ActivityType);

		// validate properties of child:
		Task task = taskService.createTaskQuery().singleResult();
		ActivityInstance childActivityInstance = rootActInstance.ChildActivityInstances[0];
		assertEquals(processInstance.Id, childActivityInstance.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, childActivityInstance.ProcessDefinitionId);
		assertEquals(processInstance.Id, childActivityInstance.ProcessInstanceId);
		assertTrue(childActivityInstance.ExecutionIds[0].Equals(task.ExecutionId));
		assertEquals("theTask", childActivityInstance.ActivityId);
		assertEquals(rootActInstance.Id, childActivityInstance.ParentActivityInstanceId);
		assertEquals("userTask", childActivityInstance.ActivityType);
		assertNotNull(childActivityInstance.ChildActivityInstances);
		assertNotNull(childActivityInstance.ChildTransitionInstances);
		assertEquals(0, childActivityInstance.ChildActivityInstances.Length);
		assertEquals(0, childActivityInstance.ChildTransitionInstances.Length);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForAsyncBeforeTask()
	  public virtual void testActivityInstanceTreeForAsyncBeforeTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("theTask").done());

		TransitionInstance asyncBeforeTransitionInstance = tree.ChildTransitionInstances[0];
		assertEquals(processInstance.Id, asyncBeforeTransitionInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForConcurrentAsyncBeforeTask()
	  public virtual void testActivityInstanceTreeForConcurrentAsyncBeforeTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("concurrentTasksProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").transition("asyncTask").done());

		TransitionInstance asyncBeforeTransitionInstance = tree.ChildTransitionInstances[0];
		string asyncExecutionId = managementService.createJobQuery().singleResult().ExecutionId;
		assertEquals(asyncExecutionId, asyncBeforeTransitionInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForAsyncBeforeStartEvent()
	  public virtual void testActivityInstanceTreeForAsyncBeforeStartEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("theStart").done());

		TransitionInstance asyncBeforeTransitionInstance = tree.ChildTransitionInstances[0];
		assertEquals(processInstance.Id, asyncBeforeTransitionInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForAsyncAfterTask()
	  public virtual void testActivityInstanceTreeForAsyncAfterTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);


		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("theTask").done());

		TransitionInstance asyncAfterTransitionInstance = tree.ChildTransitionInstances[0];
		assertEquals(processInstance.Id, asyncAfterTransitionInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForConcurrentAsyncAfterTask()
	  public virtual void testActivityInstanceTreeForConcurrentAsyncAfterTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("concurrentTasksProcess");

		Task asyncTask = taskService.createTaskQuery().taskDefinitionKey("asyncTask").singleResult();
		assertNotNull(asyncTask);
		taskService.complete(asyncTask.Id);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").transition("asyncTask").done());

		TransitionInstance asyncBeforeTransitionInstance = tree.ChildTransitionInstances[0];
		string asyncExecutionId = managementService.createJobQuery().singleResult().ExecutionId;
		assertEquals(asyncExecutionId, asyncBeforeTransitionInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForAsyncAfterEndEvent()
	  public virtual void testActivityInstanceTreeForAsyncAfterEndEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("asyncEndEventProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("theEnd").done());

		TransitionInstance asyncAfterTransitionInstance = tree.ChildTransitionInstances[0];
		assertEquals(processInstance.Id, asyncAfterTransitionInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForNestedAsyncBeforeTask()
	  public virtual void testActivityInstanceTreeForNestedAsyncBeforeTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").transition("theTask").done());

		TransitionInstance asyncBeforeTransitionInstance = tree.ChildActivityInstances[0].ChildTransitionInstances[0];
		string asyncExecutionId = managementService.createJobQuery().singleResult().ExecutionId;
		assertEquals(asyncExecutionId, asyncBeforeTransitionInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForNestedAsyncBeforeStartEvent()
	  public virtual void testActivityInstanceTreeForNestedAsyncBeforeStartEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").transition("theSubProcessStart").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForNestedAsyncAfterTask()
	  public virtual void testActivityInstanceTreeForNestedAsyncAfterTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);


		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").transition("theTask").done());

		TransitionInstance asyncAfterTransitionInstance = tree.ChildActivityInstances[0].ChildTransitionInstances[0];
		string asyncExecutionId = managementService.createJobQuery().singleResult().ExecutionId;
		assertEquals(asyncExecutionId, asyncAfterTransitionInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceTreeForNestedAsyncAfterEndEvent()
	  public virtual void testActivityInstanceTreeForNestedAsyncAfterEndEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("asyncEndEventProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").transition("theSubProcessEnd").done());

		TransitionInstance asyncAfterTransitionInstance = tree.ChildActivityInstances[0].ChildTransitionInstances[0];
		string asyncExecutionId = managementService.createJobQuery().singleResult().ExecutionId;
		assertEquals(asyncExecutionId, asyncAfterTransitionInstance.ExecutionId);
	  }

	  /// <summary>
	  /// Test for CAM-3572
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testActivityInstanceForConcurrentSubprocess()
	  public virtual void testActivityInstanceForConcurrentSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("concurrentSubProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertNotNull(tree);

		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testGetActivityInstancesForActivity()
	  public virtual void testGetActivityInstancesForActivity()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSubprocess");
		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().singleResult();

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		// then
		ActivityInstance[] processActivityInstances = tree.getActivityInstances(definition.Id);
		assertEquals(1, processActivityInstances.Length);
		assertEquals(tree.Id, processActivityInstances[0].Id);
		assertEquals(definition.Id, processActivityInstances[0].ActivityId);

		assertActivityInstances(tree.getActivityInstances("subProcess#multiInstanceBody"), 1, "subProcess#multiInstanceBody");
		assertActivityInstances(tree.getActivityInstances("subProcess"), 3, "subProcess");
		assertActivityInstances(tree.getActivityInstances("innerTask"), 3, "innerTask");

		ActivityInstance subProcessInstance = tree.ChildActivityInstances[0].ChildActivityInstances[0];
		assertActivityInstances(subProcessInstance.getActivityInstances("subProcess"), 1, "subProcess");

		ActivityInstance[] childInstances = subProcessInstance.getActivityInstances("innerTask");
		assertEquals(1, childInstances.Length);
		assertEquals(subProcessInstance.ChildActivityInstances[0].Id, childInstances[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testGetActivityInstancesForActivity.bpmn20.xml") @Test public void testGetInvalidActivityInstancesForActivity()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testGetActivityInstancesForActivity.bpmn20.xml")]
	  public virtual void testGetInvalidActivityInstancesForActivity()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSubprocess");

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		try
		{
		  tree.getActivityInstances(null);
		  fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testGetActivityInstancesForActivity.bpmn20.xml") @Test public void testGetActivityInstancesForNonExistingActivity()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testGetActivityInstancesForActivity.bpmn20.xml")]
	  public virtual void testGetActivityInstancesForNonExistingActivity()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSubprocess");

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		ActivityInstance[] instances = tree.getActivityInstances("aNonExistingActivityId");
		assertNotNull(instances);
		assertEquals(0, instances.Length);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testGetTransitionInstancesForActivity()
	  public virtual void testGetTransitionInstancesForActivity()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSubprocess");

		// complete one async task
		Job job = managementService.createJobQuery().listPage(0, 1).get(0);
		managementService.executeJob(job.Id);
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		// then
		assertEquals(0, tree.getTransitionInstances("subProcess").Length);
		TransitionInstance[] asyncBeforeInstances = tree.getTransitionInstances("innerTask");
		assertEquals(2, asyncBeforeInstances.Length);

		assertEquals("innerTask", asyncBeforeInstances[0].ActivityId);
		assertEquals("innerTask", asyncBeforeInstances[1].ActivityId);
		assertFalse(asyncBeforeInstances[0].Id.Equals(asyncBeforeInstances[1].Id));

		TransitionInstance[] asyncEndEventInstances = tree.getTransitionInstances("theSubProcessEnd");
		assertEquals(1, asyncEndEventInstances.Length);
		assertEquals("theSubProcessEnd", asyncEndEventInstances[0].ActivityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testGetTransitionInstancesForActivity.bpmn20.xml") @Test public void testGetInvalidTransitionInstancesForActivity()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testGetTransitionInstancesForActivity.bpmn20.xml")]
	  public virtual void testGetInvalidTransitionInstancesForActivity()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSubprocess");

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		try
		{
		  tree.getTransitionInstances(null);
		  fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testGetTransitionInstancesForActivity.bpmn20.xml") @Test public void testGetTransitionInstancesForNonExistingActivity()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testGetTransitionInstancesForActivity.bpmn20.xml")]
	  public virtual void testGetTransitionInstancesForNonExistingActivity()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSubprocess");

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		TransitionInstance[] instances = tree.getTransitionInstances("aNonExistingActivityId");
		assertNotNull(instances);
		assertEquals(0, instances.Length);
	  }


	  protected internal virtual void assertActivityInstances(ActivityInstance[] instances, int expectedAmount, string expectedActivityId)
	  {
		assertEquals(expectedAmount, instances.Length);

		ISet<string> instanceIds = new HashSet<string>();

		foreach (ActivityInstance instance in instances)
		{
		  assertEquals(expectedActivityId, instance.ActivityId);
		  instanceIds.Add(instance.Id);
		}

		// ensure that all instances are unique
		assertEquals(expectedAmount, instanceIds.Count);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") @Test public void testChangeVariableType()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testChangeVariableType()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		DummySerializable dummy = new DummySerializable();
		runtimeService.setVariable(instance.Id, "dummy", dummy);

		runtimeService.setVariable(instance.Id, "dummy", 47);

		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();

		assertEquals(47, variableInstance.Value);
		assertEquals(ValueType.INTEGER.Name, variableInstance.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") @Test public void testStartByKeyWithCaseInstanceId()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testStartByKeyWithCaseInstanceId()
	  {
		string caseInstanceId = "aCaseInstanceId";

		ProcessInstance firstInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", null, caseInstanceId);

		assertEquals(caseInstanceId, firstInstance.CaseInstanceId);

		// load process instance from db
		firstInstance = runtimeService.createProcessInstanceQuery().processInstanceId(firstInstance.Id).singleResult();

		assertNotNull(firstInstance);

		assertEquals(caseInstanceId, firstInstance.CaseInstanceId);

		// the second possibility to start a process instance /////////////////////////////////////////////

		ProcessInstance secondInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", null, caseInstanceId, null);

		assertEquals(caseInstanceId, secondInstance.CaseInstanceId);

		// load process instance from db
		secondInstance = runtimeService.createProcessInstanceQuery().processInstanceId(secondInstance.Id).singleResult();

		assertNotNull(secondInstance);

		assertEquals(caseInstanceId, secondInstance.CaseInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") @Test public void testStartByIdWithCaseInstanceId()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testStartByIdWithCaseInstanceId()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult().Id;

		string caseInstanceId = "aCaseInstanceId";
		ProcessInstance firstInstance = runtimeService.startProcessInstanceById(processDefinitionId, null, caseInstanceId);

		assertEquals(caseInstanceId, firstInstance.CaseInstanceId);

		// load process instance from db
		firstInstance = runtimeService.createProcessInstanceQuery().processInstanceId(firstInstance.Id).singleResult();

		assertNotNull(firstInstance);

		assertEquals(caseInstanceId, firstInstance.CaseInstanceId);

		// the second possibility to start a process instance /////////////////////////////////////////////

		ProcessInstance secondInstance = runtimeService.startProcessInstanceById(processDefinitionId, null, caseInstanceId, null);

		assertEquals(caseInstanceId, secondInstance.CaseInstanceId);

		// load process instance from db
		secondInstance = runtimeService.createProcessInstanceQuery().processInstanceId(secondInstance.Id).singleResult();

		assertNotNull(secondInstance);

		assertEquals(caseInstanceId, secondInstance.CaseInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") @Test public void testSetAbstractNumberValueFails()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testSetAbstractNumberValueFails()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValueTyped("var", Variables.numberValue(42)));
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  testRule.assertTextPresentIgnoreCase("cannot serialize value of abstract type number", e.Message);
		}

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		try
		{
		  runtimeService.setVariable(processInstance.Id, "var", Variables.numberValue(42));
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  testRule.assertTextPresentIgnoreCase("cannot serialize value of abstract type number", e.Message);
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/messageStartEvent.bpmn20.xml") @Test public void testStartProcessInstanceByMessageWithEarlierVersionOfProcessDefinition()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/messageStartEvent.bpmn20.xml")]
	  public virtual void testStartProcessInstanceByMessageWithEarlierVersionOfProcessDefinition()
	  {
		  string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/runtime/messageStartEvent_version2.bpmn20.xml").deploy().Id;
		  ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(1).singleResult();

		  ProcessInstance processInstance = runtimeService.startProcessInstanceByMessageAndProcessDefinitionId("startMessage", processDefinition.Id);

		  assertThat(processInstance, @is(notNullValue()));
		  assertThat(processInstance.ProcessDefinitionId, @is(processDefinition.Id));

		  // clean up
		  repositoryService.deleteDeployment(deploymentId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/messageStartEvent.bpmn20.xml") @Test public void testStartProcessInstanceByMessageWithLastVersionOfProcessDefinition()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/messageStartEvent.bpmn20.xml")]
	  public virtual void testStartProcessInstanceByMessageWithLastVersionOfProcessDefinition()
	  {
		  string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/runtime/messageStartEvent_version2.bpmn20.xml").deploy().Id;
		  ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().latestVersion().singleResult();

		  ProcessInstance processInstance = runtimeService.startProcessInstanceByMessageAndProcessDefinitionId("newStartMessage", processDefinition.Id);

		  assertThat(processInstance, @is(notNullValue()));
		  assertThat(processInstance.ProcessDefinitionId, @is(processDefinition.Id));

		  // clean up
		  repositoryService.deleteDeployment(deploymentId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/messageStartEvent.bpmn20.xml") @Test public void testStartProcessInstanceByMessageWithNonExistingMessageStartEvent()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/messageStartEvent.bpmn20.xml")]
	  public virtual void testStartProcessInstanceByMessageWithNonExistingMessageStartEvent()
	  {
		  string deploymentId = null;
		  try
		  {
			 deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/runtime/messageStartEvent_version2.bpmn20.xml").deploy().Id;
			 ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(1).singleResult();

			 runtimeService.startProcessInstanceByMessageAndProcessDefinitionId("newStartMessage", processDefinition.Id);

			 fail("exeception expected");
		  }
		 catch (ProcessEngineException e)
		 {
			 assertThat(e.Message, containsString("Cannot correlate message 'newStartMessage'"));
		 }
		 finally
		 {
			 // clean up
			 if (!string.ReferenceEquals(deploymentId, null))
			 {
				 repositoryService.deleteDeployment(deploymentId, true);
			 }
		 }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testActivityInstanceActivityNameProperty()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testActivityInstanceActivityNameProperty()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		ActivityInstance[] activityInstances = tree.getActivityInstances("theTask");
		assertEquals(1, activityInstances.Length);

		ActivityInstance task = activityInstances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityName);
		assertEquals("my task", task.ActivityName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testTransitionInstanceActivityNamePropertyBeforeTask()
	  public virtual void testTransitionInstanceActivityNamePropertyBeforeTask()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		TransitionInstance[] instances = tree.getTransitionInstances("firstServiceTask");
		TransitionInstance task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityName);
		assertEquals("First Service Task", task.ActivityName);

		instances = tree.getTransitionInstances("secondServiceTask");
		task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityName);
		assertEquals("Second Service Task", task.ActivityName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testTransitionInstanceActivityNamePropertyBeforeTask.bpmn20.xml") @Test public void testTransitionInstanceActivityTypePropertyBeforeTask()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testTransitionInstanceActivityNamePropertyBeforeTask.bpmn20.xml")]
	  public virtual void testTransitionInstanceActivityTypePropertyBeforeTask()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		TransitionInstance[] instances = tree.getTransitionInstances("firstServiceTask");
		TransitionInstance task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityType);
		assertEquals("serviceTask", task.ActivityType);

		instances = tree.getTransitionInstances("secondServiceTask");
		task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityType);
		assertEquals("serviceTask", task.ActivityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testTransitionInstanceActivityNamePropertyAfterTask()
	  public virtual void testTransitionInstanceActivityNamePropertyAfterTask()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		TransitionInstance[] instances = tree.getTransitionInstances("firstServiceTask");
		TransitionInstance task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityName);
		assertEquals("First Service Task", task.ActivityName);

		instances = tree.getTransitionInstances("secondServiceTask");
		task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityName);
		assertEquals("Second Service Task", task.ActivityName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testTransitionInstanceActivityNamePropertyAfterTask.bpmn20.xml") @Test public void testTransitionInstanceActivityTypePropertyAfterTask()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testTransitionInstanceActivityNamePropertyAfterTask.bpmn20.xml")]
	  public virtual void testTransitionInstanceActivityTypePropertyAfterTask()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		TransitionInstance[] instances = tree.getTransitionInstances("firstServiceTask");
		TransitionInstance task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityType);
		assertEquals("serviceTask", task.ActivityType);

		instances = tree.getTransitionInstances("secondServiceTask");
		task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityType);
		assertEquals("serviceTask", task.ActivityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testTransitionInstanceActivityNamePropertyBeforeStartEvent()
	  public virtual void testTransitionInstanceActivityNamePropertyBeforeStartEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		TransitionInstance[] instances = tree.getTransitionInstances("start");
		TransitionInstance task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityName);
		assertEquals("The Start Event", task.ActivityName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testTransitionInstanceActivityNamePropertyBeforeStartEvent.bpmn20.xml") @Test public void testTransitionInstanceActivityTypePropertyBeforeStartEvent()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testTransitionInstanceActivityNamePropertyBeforeStartEvent.bpmn20.xml")]
	  public virtual void testTransitionInstanceActivityTypePropertyBeforeStartEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		TransitionInstance[] instances = tree.getTransitionInstances("start");
		TransitionInstance task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityType);
		assertEquals("startEvent", task.ActivityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testTransitionInstanceActivityNamePropertyAfterStartEvent()
	  public virtual void testTransitionInstanceActivityNamePropertyAfterStartEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		TransitionInstance[] instances = tree.getTransitionInstances("start");
		TransitionInstance task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityName);
		assertEquals("The Start Event", task.ActivityName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testTransitionInstanceActivityNamePropertyAfterStartEvent.bpmn20.xml") @Test public void testTransitionInstanceActivityTypePropertyAfterStartEvent()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testTransitionInstanceActivityNamePropertyAfterStartEvent.bpmn20.xml")]
	  public virtual void testTransitionInstanceActivityTypePropertyAfterStartEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		// then
		TransitionInstance[] instances = tree.getTransitionInstances("start");
		TransitionInstance task = instances[0];
		assertNotNull(task);
		assertNotNull(task.ActivityType);
		assertEquals("startEvent", task.ActivityType);
	  }

	  //Test for a bug: when the process engine is rebooted the
	  // cache is cleaned and the deployed process definition is
	  // removed from the process cache. This led to problems because
	  // the id wasnt fetched from the DB after a redeploy.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartProcessInstanceByIdAfterReboot()
	  public virtual void testStartProcessInstanceByIdAfterReboot()
	  {

		// In case this test is run in a test suite, previous engines might
		// have been initialized and cached.  First we close the
		// existing process engines to make sure that the db is clean
		// and that there are no existing process engines involved.
		ProcessEngines.destroy();

		// Creating the DB schema (without building a process engine)
		ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneInMemProcessEngineConfiguration();
		processEngineConfiguration.ProcessEngineName = "reboot-test-schema";
		processEngineConfiguration.JdbcUrl = "jdbc:h2:mem:activiti-reboot-test;DB_CLOSE_DELAY=1000";
		ProcessEngine schemaProcessEngine = processEngineConfiguration.buildProcessEngine();

		// Create process engine and deploy test process
		ProcessEngine processEngine = (new StandaloneProcessEngineConfiguration()).setProcessEngineName("reboot-test").setDatabaseSchemaUpdate(ProcessEngineConfiguration.DB_SCHEMA_UPDATE_FALSE).setJdbcUrl("jdbc:h2:mem:activiti-reboot-test;DB_CLOSE_DELAY=1000").setJobExecutorActivate(false).buildProcessEngine();

		processEngine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml").deploy();
		  // verify existence of process definition
		IList<ProcessDefinition> processDefinitions = processEngine.RepositoryService.createProcessDefinitionQuery().list();

		assertEquals(1, processDefinitions.Count);

		// Start a new Process instance
		ProcessInstance processInstance = processEngine.RuntimeService.startProcessInstanceById(processDefinitions[0].Id);
		string processInstanceId = processInstance.Id;
		assertNotNull(processInstance);

		// Close the process engine
		processEngine.close();
		assertNotNull(processEngine.RuntimeService);

		// Reboot the process engine
		processEngine = (new StandaloneProcessEngineConfiguration()).setProcessEngineName("reboot-test").setDatabaseSchemaUpdate(ProcessEngineConfiguration.DB_SCHEMA_UPDATE_FALSE).setJdbcUrl("jdbc:h2:mem:activiti-reboot-test;DB_CLOSE_DELAY=1000").setJobExecutorActivate(false).buildProcessEngine();

		// Check if the existing process instance is still alive
		processInstance = processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		assertNotNull(processInstance);

		// Complete the task.  That will end the process instance
		TaskService taskService = processEngine.TaskService;
		Task task = taskService.createTaskQuery().list().get(0);
		taskService.complete(task.Id);

		// Check if the process instance has really ended.  This means that the process definition has
		// re-loaded into the process definition cache
		processInstance = processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();
		assertNull(processInstance);

		// Extra check to see if a new process instance can be started as well
		processInstance = processEngine.RuntimeService.startProcessInstanceById(processDefinitions[0].Id);
		assertNotNull(processInstance);

		// close the process engine
		processEngine.close();

		// Cleanup schema
		schemaProcessEngine.close();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testVariableScope()
	  public virtual void testVariableScope()
	  {

		// After starting the process, the task in the subprocess should be active
		IDictionary<string, object> varMap = new Dictionary<string, object>();
		varMap["test"] = "test";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("simpleSubProcess", varMap);
		Task subProcessTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task in subprocess", subProcessTask.Name);

		// get variables for execution id user task, should return the new value of variable test --> test2
		assertEquals("test2", runtimeService.getVariable(subProcessTask.ExecutionId, "test"));
		assertEquals("test2", runtimeService.getVariables(subProcessTask.ExecutionId)["test"]);

		// get variables for process instance id, should return the initial value of variable test --> test
		assertEquals("test", runtimeService.getVariable(pi.Id, "test"));
		assertEquals("test", runtimeService.getVariables(pi.Id)["test"]);

		runtimeService.setVariableLocal(subProcessTask.ExecutionId, "test", "test3");

		// get variables for execution id user task, should return the new value of variable test --> test3
		assertEquals("test3", runtimeService.getVariable(subProcessTask.ExecutionId, "test"));
		assertEquals("test3", runtimeService.getVariables(subProcessTask.ExecutionId)["test"]);

		// get variables for process instance id, should still return the initial value of variable test --> test
		assertEquals("test", runtimeService.getVariable(pi.Id, "test"));
		assertEquals("test", runtimeService.getVariables(pi.Id)["test"]);

		runtimeService.setVariable(pi.Id, "test", "test4");

		// get variables for execution id user task, should return the old value of variable test --> test3
		assertEquals("test3", runtimeService.getVariable(subProcessTask.ExecutionId, "test"));
		assertEquals("test3", runtimeService.getVariables(subProcessTask.ExecutionId)["test"]);

		// get variables for process instance id, should also return the initial value of variable test --> test4
		assertEquals("test4", runtimeService.getVariable(pi.Id, "test"));
		assertEquals("test4", runtimeService.getVariables(pi.Id)["test"]);

		// After completing the task in the subprocess,
		// the subprocess scope is destroyed and the complete process ends
		taskService.complete(subProcessTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testBasicVariableOperations()
	  public virtual void testBasicVariableOperations()
	  {

		DateTime now = DateTime.Now;
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");
		sbyte[] bytes = "somebytes".GetBytes();
		sbyte[] streamBytes = "morebytes".GetBytes();

		// Start process instance with different types of variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 928374L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "coca-cola";
		variables["dateVar"] = now;
		variables["nullVar"] = null;
		variables["serializableVar"] = serializable;
		variables["bytesVar"] = bytes;
		variables["byteStreamVar"] = new MemoryStream(streamBytes);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskAssigneeProcess", variables);

		variables = runtimeService.getVariables(processInstance.Id);
		assertEquals("coca-cola", variables["stringVar"]);
		assertEquals(928374L, variables["longVar"]);
		assertEquals((short) 123, variables["shortVar"]);
		assertEquals(1234, variables["integerVar"]);
		assertEquals(now, variables["dateVar"]);
		assertEquals(null, variables["nullVar"]);
		assertEquals(serializable, variables["serializableVar"]);
		assertTrue(Arrays.Equals(bytes, (sbyte[]) variables["bytesVar"]));
		assertTrue(Arrays.Equals(streamBytes, (sbyte[]) variables["byteStreamVar"]));
		assertEquals(9, variables.Count);

		// Set all existing variables values to null
		runtimeService.setVariable(processInstance.Id, "longVar", null);
		runtimeService.setVariable(processInstance.Id, "shortVar", null);
		runtimeService.setVariable(processInstance.Id, "integerVar", null);
		runtimeService.setVariable(processInstance.Id, "stringVar", null);
		runtimeService.setVariable(processInstance.Id, "dateVar", null);
		runtimeService.setVariable(processInstance.Id, "nullVar", null);
		runtimeService.setVariable(processInstance.Id, "serializableVar", null);
		runtimeService.setVariable(processInstance.Id, "bytesVar", null);
		runtimeService.setVariable(processInstance.Id, "byteStreamVar", null);

		variables = runtimeService.getVariables(processInstance.Id);
		assertEquals(null, variables["longVar"]);
		assertEquals(null, variables["shortVar"]);
		assertEquals(null, variables["integerVar"]);
		assertEquals(null, variables["stringVar"]);
		assertEquals(null, variables["dateVar"]);
		assertEquals(null, variables["nullVar"]);
		assertEquals(null, variables["serializableVar"]);
		assertEquals(null, variables["bytesVar"]);
		assertEquals(null, variables["byteStreamVar"]);
		assertEquals(9, variables.Count);

		// Update existing variable values again, and add a new variable
		runtimeService.setVariable(processInstance.Id, "new var", "hi");
		runtimeService.setVariable(processInstance.Id, "longVar", 9987L);
		runtimeService.setVariable(processInstance.Id, "shortVar", (short) 456);
		runtimeService.setVariable(processInstance.Id, "integerVar", 4567);
		runtimeService.setVariable(processInstance.Id, "stringVar", "colgate");
		runtimeService.setVariable(processInstance.Id, "dateVar", now);
		runtimeService.setVariable(processInstance.Id, "serializableVar", serializable);
		runtimeService.setVariable(processInstance.Id, "bytesVar", bytes);
		runtimeService.setVariable(processInstance.Id, "byteStreamVar", new MemoryStream(streamBytes));

		variables = runtimeService.getVariables(processInstance.Id);
		assertEquals("hi", variables["new var"]);
		assertEquals(9987L, variables["longVar"]);
		assertEquals((short)456, variables["shortVar"]);
		assertEquals(4567, variables["integerVar"]);
		assertEquals("colgate", variables["stringVar"]);
		assertEquals(now, variables["dateVar"]);
		assertEquals(null, variables["nullVar"]);
		assertEquals(serializable, variables["serializableVar"]);
		assertTrue(Arrays.Equals(bytes, (sbyte[]) variables["bytesVar"]));
		assertTrue(Arrays.Equals(streamBytes, (sbyte[]) variables["byteStreamVar"]));
		assertEquals(10, variables.Count);

		ICollection<string> varFilter = new List<string>(2);
		varFilter.Add("stringVar");
		varFilter.Add("integerVar");

		IDictionary<string, object> filteredVariables = runtimeService.getVariables(processInstance.Id, varFilter);
		assertEquals(2, filteredVariables.Count);
		assertTrue(filteredVariables.ContainsKey("stringVar"));
		assertTrue(filteredVariables.ContainsKey("integerVar"));

		// Try setting the value of the variable that was initially created with value 'null'
		runtimeService.setVariable(processInstance.Id, "nullVar", "a value");
		object newValue = runtimeService.getVariable(processInstance.Id, "nullVar");
		assertNotNull(newValue);
		assertEquals("a value", newValue);

		// Try setting the value of the serializableVar to an integer value
		runtimeService.setVariable(processInstance.Id, "serializableVar", 100);
		variables = runtimeService.getVariables(processInstance.Id);
		assertEquals(100, variables["serializableVar"]);

		// Try setting the value of the serializableVar back to a serializable value
		runtimeService.setVariable(processInstance.Id, "serializableVar", serializable);
		variables = runtimeService.getVariables(processInstance.Id);
		assertEquals(serializable, variables["serializableVar"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testBasicVariableOperations.bpmn20.xml"}) @Test public void testOnlyChangeType()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testBasicVariableOperations.bpmn20.xml"})]
	  public virtual void testOnlyChangeType()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = 1234;
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("taskAssigneeProcess", variables);

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableName("aVariable");

		VariableInstance variable = query.singleResult();
		assertEquals(ValueType.INTEGER.Name, variable.TypeName);

		runtimeService.setVariable(pi.Id, "aVariable", 1234L);
		variable = query.singleResult();
		assertEquals(ValueType.LONG.Name, variable.TypeName);

		runtimeService.setVariable(pi.Id, "aVariable", (short)1234);
		variable = query.singleResult();
		assertEquals(ValueType.SHORT.Name, variable.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testBasicVariableOperations.bpmn20.xml"}) @Test public void testChangeTypeFromSerializableUsingApi()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testBasicVariableOperations.bpmn20.xml"})]
	  public virtual void testChangeTypeFromSerializableUsingApi()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = new SerializableVariable("foo");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("taskAssigneeProcess", variables);

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableName("aVariable");

		VariableInstance variable = query.singleResult();
		assertEquals(ValueType.OBJECT.Name, variable.TypeName);

		runtimeService.setVariable(pi.Id, "aVariable", null);
		variable = query.singleResult();
		assertEquals(ValueType.NULL.Name, variable.TypeName);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testChangeSerializableInsideEngine()
	  public virtual void testChangeSerializableInsideEngine()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		Task task = taskService.createTaskQuery().singleResult();

		SerializableVariable var = (SerializableVariable) taskService.getVariable(task.Id, "variableName");
		assertNotNull(var);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testBasicVariableOperations.bpmn20.xml"}) @Test public void testChangeToSerializableUsingApi()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testBasicVariableOperations.bpmn20.xml"})]
	  public virtual void testChangeToSerializableUsingApi()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "test";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskAssigneeProcess", variables);

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableName("aVariable");

		VariableInstance variable = query.singleResult();
		assertEquals(ValueType.STRING.Name, variable.TypeName);

		runtimeService.setVariable(processInstance.Id, "aVariable", new SerializableVariable("foo"));
		variable = query.singleResult();
		assertEquals(ValueType.OBJECT.Name, variable.TypeName);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testGetVariableInstancesFromVariableScope()
	  public virtual void testGetVariableInstancesFromVariableScope()
	  {

		VariableMap variables = createVariables().putValue("anIntegerVariable", 1234).putValue("anObjectValue", objectValue(new SimpleSerializableBean(10)).serializationDataFormat(Variables.SerializationDataFormats.JAVA)).putValue("anUntypedObjectValue", new SimpleSerializableBean(30));

		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// assertions are part of the java delegate AssertVariableInstancesDelegate
		// only there we can access the VariableScope methods
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testSetVariableInScope.bpmn20.xml") @Test public void testSetVariableInScopeExplicitUpdate()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testSetVariableInScope.bpmn20.xml")]
	  public virtual void testSetVariableInScopeExplicitUpdate()
	  {
		// when a process instance is started and the task after the subprocess reached
		runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("shouldExplicitlyUpdateVariable", true));

		// then there should be only the "shouldExplicitlyUpdateVariable" variable
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variableInstance);
		assertEquals("shouldExplicitlyUpdateVariable", variableInstance.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testSetVariableInScope.bpmn20.xml") @Test public void testSetVariableInScopeImplicitUpdate()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/RuntimeServiceTest.testSetVariableInScope.bpmn20.xml")]
	  public virtual void testSetVariableInScopeImplicitUpdate()
	  {
		// when a process instance is started and the task after the subprocess reached
		runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("shouldExplicitlyUpdateVariable", true));

		// then there should be only the "shouldExplicitlyUpdateVariable" variable
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variableInstance);
		assertEquals("shouldExplicitlyUpdateVariable", variableInstance.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testUpdateVariableInProcessWithoutWaitstate()
	  public virtual void testUpdateVariableInProcessWithoutWaitstate()
	  {
		// when a process instance is started
		runtimeService.startProcessInstanceByKey("oneScriptTaskProcess", Collections.singletonMap<string, object>("var", new SimpleSerializableBean(10)));

		// then it should succeeds successfully
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNull(processInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSetUpdateAndDeleteComplexVariable()
	  public virtual void testSetUpdateAndDeleteComplexVariable()
	  {
		// when a process instance is started
		runtimeService.startProcessInstanceByKey("oneUserTaskProcess", Collections.singletonMap<string, object>("var", new SimpleSerializableBean(10)));

		// then it should wait at the user task
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(processInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testRollback()
	  public virtual void testRollback()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("RollbackProcess");

		  fail("Starting the process instance should throw an exception");

		}
		catch (Exception e)
		{
		  assertEquals("Buzzz", e.Message);
		}

		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/trivial.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/rollbackAfterSubProcess.bpmn20.xml"}) @Test public void testRollbackAfterSubProcess()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/trivial.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/rollbackAfterSubProcess.bpmn20.xml"})]
	  public virtual void testRollbackAfterSubProcess()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("RollbackAfterSubProcess");

		  fail("Starting the process instance should throw an exception");

		}
		catch (Exception e)
		{
		  assertEquals("Buzzz", e.Message);
		}

		assertEquals(0, runtimeService.createExecutionQuery().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetActivityInstanceForCompletedInstanceInDelegate()
	  public virtual void testGetActivityInstanceForCompletedInstanceInDelegate()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance deletingProcess = Bpmn.createExecutableProcess("process1").startEvent().userTask().serviceTask().camundaClass(typeof(DeleteInstanceDelegate).FullName).userTask().endEvent().done();
		BpmnModelInstance processToDelete = Bpmn.createExecutableProcess("process2").startEvent().userTask().endEvent().done();

		testRule.deploy(deletingProcess, processToDelete);

		ProcessInstance instanceToDelete = runtimeService.startProcessInstanceByKey("process2");
		ProcessInstance deletingInstance = runtimeService.startProcessInstanceByKey("process1", Variables.createVariables().putValue("instanceToComplete", instanceToDelete.Id));

		Task deleteTrigger = taskService.createTaskQuery().processInstanceId(deletingInstance.Id).singleResult();

		// when
		taskService.complete(deleteTrigger.Id);

		// then
		bool activityInstanceNull = (bool?) runtimeService.getVariable(deletingInstance.Id, "activityInstanceNull").Value;
		assertTrue(activityInstanceNull);
	  }

	  public class DeleteInstanceDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  RuntimeService runtimeService = execution.ProcessEngineServices.RuntimeService;
		  TaskService taskService = execution.ProcessEngineServices.TaskService;

		  string instanceToDelete = (string) execution.getVariable("instanceToComplete");
		  Task taskToTrigger = taskService.createTaskQuery().processInstanceId(instanceToDelete).singleResult();
		  taskService.complete(taskToTrigger.Id);

		  ActivityInstance activityInstance = runtimeService.getActivityInstance(instanceToDelete);
		  execution.setVariable("activityInstanceNull", activityInstance == null);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Test public void testDeleteProcessInstanceWithSubprocessInstances()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testDeleteProcessInstanceWithSubprocessInstances()
	  {
		// given a process instance with subprocesses
		BpmnModelInstance calling = prepareComplexProcess("A", "B", "A");

		BpmnModelInstance calledA = prepareSimpleProcess("A");
		BpmnModelInstance calledB = prepareSimpleProcess("B");

		testRule.deploy(calling, calledA, calledB);

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("calling");
		IList<ProcessInstance> subInstances = runtimeService.createProcessInstanceQuery().superProcessInstanceId(instance.Id).list();

		// when the process instance is deleted and we do not skip sub processes
		string id = instance.Id;
		runtimeService.deleteProcessInstance(id, "test_purposes", false, true, false, false);

		// then
		testRule.assertProcessEnded(id);

		foreach (ProcessInstance subInstance in subInstances)
		{
		  testRule.assertProcessEnded(subInstance.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Test public void testDeleteProcessInstanceWithoutSubprocessInstances()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testDeleteProcessInstanceWithoutSubprocessInstances()
	  {
		// given a process instance with subprocesses
		BpmnModelInstance calling = prepareComplexProcess("A", "B", "C");

		BpmnModelInstance calledA = prepareSimpleProcess("A");
		BpmnModelInstance calledB = prepareSimpleProcess("B");
		BpmnModelInstance calledC = prepareSimpleProcess("C");

		testRule.deploy(calling, calledA, calledB, calledC);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("calling");
		IList<ProcessInstance> subInstances = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).list();

		// when the process instance is deleted and we do skip sub processes
		string id = processInstance.Id;
		runtimeService.deleteProcessInstance(id, "test_purposes", false, true, false, true);

		// then
		testRule.assertProcessEnded(id);

		foreach (ProcessInstance subInstance in subInstances)
		{
		  testRule.assertProcessNotEnded(subInstance.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstancesWithoutSubprocessInstances()
	  public virtual void testDeleteProcessInstancesWithoutSubprocessInstances()
	  {
		// given a process instance with subprocess
		string callingProcessKey = "calling";
		string calledProcessKey = "called";
		BpmnModelInstance calling = prepareCallingProcess(callingProcessKey, calledProcessKey);
		BpmnModelInstance called = prepareSimpleProcess(calledProcessKey);
		testRule.deploy(calling, called);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(callingProcessKey);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey(callingProcessKey);

		IList<ProcessInstance> subprocessList = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).list();
		((IList<ProcessInstance>)subprocessList).AddRange(runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance2.Id).list());

		// when
		runtimeService.deleteProcessInstances(Arrays.asList(processInstance.Id, processInstance2.Id), null, false, false, true);

		// then
		testRule.assertProcessEnded(processInstance.Id);
		testRule.assertProcessEnded(processInstance2.Id);

		foreach (ProcessInstance instance in subprocessList)
		{
		  testRule.assertProcessNotEnded(instance.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstancesWithSubprocessInstances()
	  public virtual void testDeleteProcessInstancesWithSubprocessInstances()
	  {
		// given a process instance with subprocess
		string callingProcessKey = "calling";
		string calledProcessKey = "called";
		BpmnModelInstance calling = prepareCallingProcess(callingProcessKey, calledProcessKey);
		BpmnModelInstance called = prepareSimpleProcess(calledProcessKey);
		testRule.deploy(calling, called);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(callingProcessKey);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey(callingProcessKey);

		IList<ProcessInstance> subprocessList = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).list();
		((IList<ProcessInstance>)subprocessList).AddRange(runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance2.Id).list());

		// when
		runtimeService.deleteProcessInstances(Arrays.asList(processInstance.Id, processInstance2.Id), null, false, false, false);

		// then
		testRule.assertProcessEnded(processInstance.Id);
		testRule.assertProcessEnded(processInstance2.Id);

		foreach (ProcessInstance subprocess in subprocessList)
		{
		  testRule.assertProcessEnded(subprocess.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testGetVariablesByEmptyList()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesByEmptyList()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId, new List<string>());

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testGetVariablesTypedByEmptyList()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesTypedByEmptyList()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesTyped(processInstanceId, new List<string>(), false);

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testGetVariablesLocalByEmptyList()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesLocalByEmptyList()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocal(processInstanceId, new List<string>());

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testGetVariablesLocalTypedByEmptyList()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesLocalTypedByEmptyList()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		IDictionary<string, object> variables = runtimeService.getVariablesLocalTyped(processInstanceId, new List<string>(), false);

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

	  private BpmnModelInstance prepareComplexProcess(string calledProcessA, string calledProcessB, string calledProcessC)
	  {
		BpmnModelInstance calling = Bpmn.createExecutableProcess("calling").startEvent().parallelGateway("fork1").subProcess().embeddedSubProcess().startEvent().parallelGateway("fork2").callActivity("callingA").calledElement(calledProcessA).endEvent("endA").moveToNode("fork2").callActivity("callingB").calledElement(calledProcessB).endEvent().subProcessDone().moveToNode("fork1").callActivity("callingC").calledElement(calledProcessC).endEvent().done();
		return calling;
	  }

	  private BpmnModelInstance prepareSimpleProcess(string name)
	  {
		BpmnModelInstance calledA = Bpmn.createExecutableProcess(name).startEvent().userTask("Task" + name).endEvent().done();
		return calledA;
	  }

	  private BpmnModelInstance prepareCallingProcess(string callingProcess, string calledProcess)
	  {
		BpmnModelInstance calling = Bpmn.createExecutableProcess(callingProcess).startEvent().callActivity().calledElement(calledProcess).endEvent().done();
		return calling;
	  }
	}

}