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

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using MessageCorrelationResult = org.camunda.bpm.engine.runtime.MessageCorrelationResult;
	using MessageCorrelationResultType = org.camunda.bpm.engine.runtime.MessageCorrelationResultType;
	using MessageCorrelationResultWithVariables = org.camunda.bpm.engine.runtime.MessageCorrelationResultWithVariables;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using FailingJavaSerializable = org.camunda.bpm.engine.test.api.variables.FailingJavaSerializable;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
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

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class MessageCorrelationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MessageCorrelationTest()
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
	  private RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testCatchingMessageEventCorrelation()
	  public virtual void testCatchingMessageEventCorrelation()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		variables = new Dictionary<>();
		variables["aKey"] = "anotherValue";
		runtimeService.startProcessInstanceByKey("process", variables);

		string messageName = "newInvoiceMessage";
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		correlationKeys["aKey"] = "aValue";
		IDictionary<string, object> messagePayload = new Dictionary<string, object>();
		messagePayload["aNewKey"] = "aNewVariable";

		runtimeService.correlateMessage(messageName, correlationKeys, messagePayload);

		long uncorrelatedExecutions = runtimeService.createExecutionQuery().processVariableValueEquals("aKey", "anotherValue").messageEventSubscriptionName("newInvoiceMessage").count();
		assertEquals(1, uncorrelatedExecutions);

		// the execution that has been correlated should have advanced
		long correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").processVariableValueEquals("aKey", "aValue").processVariableValueEquals("aNewKey", "aNewVariable").count();
		assertEquals(1, correlatedExecutions);

		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// this time: use the builder ////////////////

		variables = new Dictionary<>();
		variables["aKey"] = "aValue";
		processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		// use the fluent builder
		runtimeService.createMessageCorrelation(messageName).processInstanceVariableEquals("aKey", "aValue").setVariable("aNewKey", "aNewVariable").correlate();

		uncorrelatedExecutions = runtimeService.createExecutionQuery().processVariableValueEquals("aKey", "anotherValue").messageEventSubscriptionName("newInvoiceMessage").count();
		assertEquals(1, uncorrelatedExecutions);

		// the execution that has been correlated should have advanced
		correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").processVariableValueEquals("aKey", "aValue").processVariableValueEquals("aNewKey", "aNewVariable").count();
		assertEquals(1, correlatedExecutions);

		runtimeService.deleteProcessInstance(processInstance.Id, null);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testOneMatchinProcessInstanceUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testOneMatchinProcessInstanceUsingFluentCorrelateAll()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		runtimeService.startProcessInstanceByKey("process", variables);

		variables = new Dictionary<>();
		variables["aKey"] = "anotherValue";
		runtimeService.startProcessInstanceByKey("process", variables);

		string messageName = "newInvoiceMessage";

		// use the fluent builder: correlate to first started process instance
		runtimeService.createMessageCorrelation(messageName).processInstanceVariableEquals("aKey", "aValue").setVariable("aNewKey", "aNewVariable").correlateAll();

		// there exists an uncorrelated executions (the second process instance)
		long uncorrelatedExecutions = runtimeService.createExecutionQuery().processVariableValueEquals("aKey", "anotherValue").messageEventSubscriptionName("newInvoiceMessage").count();
		assertEquals(1, uncorrelatedExecutions);

		// the execution that has been correlated should have advanced
		long correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").processVariableValueEquals("aKey", "aValue").processVariableValueEquals("aNewKey", "aNewVariable").count();
		assertEquals(1, correlatedExecutions);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testTwoMatchingProcessInstancesCorrelation()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testTwoMatchingProcessInstancesCorrelation()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		runtimeService.startProcessInstanceByKey("process", variables);

		variables = new Dictionary<>();
		variables["aKey"] = "aValue";
		runtimeService.startProcessInstanceByKey("process", variables);

		string messageName = "newInvoiceMessage";
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		correlationKeys["aKey"] = "aValue";

		try
		{
		  runtimeService.correlateMessage(messageName, correlationKeys);
		  fail("Expected an Exception");
		}
		catch (MismatchingMessageCorrelationException e)
		{
		  testRule.assertTextPresent("2 executions match the correlation keys", e.Message);
		}

		// fluent builder fails as well
		try
		{
		  runtimeService.createMessageCorrelation(messageName).processInstanceVariableEquals("aKey", "aValue").correlate();
		  fail("Expected an Exception");
		}
		catch (MismatchingMessageCorrelationException e)
		{
		  testRule.assertTextPresent("2 executions match the correlation keys", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testTwoMatchingProcessInstancesUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testTwoMatchingProcessInstancesUsingFluentCorrelateAll()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		runtimeService.startProcessInstanceByKey("process", variables);

		variables = new Dictionary<>();
		variables["aKey"] = "aValue";
		runtimeService.startProcessInstanceByKey("process", variables);

		string messageName = "newInvoiceMessage";
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		correlationKeys["aKey"] = "aValue";

		// fluent builder multiple should not fail
		runtimeService.createMessageCorrelation(messageName).processInstanceVariableEquals("aKey", "aValue").setVariable("aNewKey", "aNewVariable").correlateAll();

		long uncorrelatedExecutions = runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoiceMessage").count();
		assertEquals(0, uncorrelatedExecutions);

		// the executions that has been correlated should have advanced
		long correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").processVariableValueEquals("aKey", "aValue").processVariableValueEquals("aNewKey", "aNewVariable").count();
		assertEquals(2, correlatedExecutions);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testExecutionCorrelationByBusinessKey()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testExecutionCorrelationByBusinessKey()
	  {
		string businessKey = "aBusinessKey";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", businessKey);
		runtimeService.correlateMessage("newInvoiceMessage", businessKey);

		// the execution that has been correlated should have advanced
		long correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").count();
		assertEquals(1, correlatedExecutions);

		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// use fluent builder //////////////////////

		processInstance = runtimeService.startProcessInstanceByKey("process", businessKey);
		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceBusinessKey(businessKey).correlate();

		// the execution that has been correlated should have advanced
		correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").count();
		assertEquals(1, correlatedExecutions);

		runtimeService.deleteProcessInstance(processInstance.Id, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testExecutionCorrelationByBusinessKeyUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testExecutionCorrelationByBusinessKeyUsingFluentCorrelateAll()
	  {
		string businessKey = "aBusinessKey";
		runtimeService.startProcessInstanceByKey("process", businessKey);
		runtimeService.startProcessInstanceByKey("process", businessKey);

		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceBusinessKey(businessKey).correlateAll();

		// the executions that has been correlated should be in the task
		long correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").count();
		assertEquals(2, correlatedExecutions);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testMessageCorrelateAllResultListWithResultTypeExecution()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageCorrelateAllResultListWithResultTypeExecution()
	  {
		//given
		ProcessInstance procInstance1 = runtimeService.startProcessInstanceByKey("process");
		ProcessInstance procInstance2 = runtimeService.startProcessInstanceByKey("process");

		//when correlated all with result
		IList<MessageCorrelationResult> resultList = runtimeService.createMessageCorrelation("newInvoiceMessage").correlateAllWithResult();

		assertEquals(2, resultList.Count);
		//then result should contains executions on which messages was correlated
		foreach (MessageCorrelationResult result in resultList)
		{
		  assertNotNull(result);
		  assertEquals(MessageCorrelationResultType.Execution, result.ResultType);
		  assertTrue(procInstance1.Id.Equals(result.Execution.ProcessInstanceId, StringComparison.OrdinalIgnoreCase) || procInstance2.Id.Equals(result.Execution.ProcessInstanceId, StringComparison.OrdinalIgnoreCase));
		  ExecutionEntity entity = (ExecutionEntity) result.Execution;
		  assertEquals("messageCatch", entity.ActivityId);
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageCorrelateAllResultListWithResultTypeProcessDefinition()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageCorrelateAllResultListWithResultTypeProcessDefinition()
	  {
		//when correlated all with result
		IList<MessageCorrelationResult> resultList = runtimeService.createMessageCorrelation("newInvoiceMessage").correlateAllWithResult();

		assertEquals(1, resultList.Count);
		//then result should contains process definitions and start event activity ids on which messages was correlated
		foreach (MessageCorrelationResult result in resultList)
		{
		  checkProcessDefinitionMessageCorrelationResult(result, "theStart", "messageStartEvent");
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testExecutionCorrelationByBusinessKeyWithVariables()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testExecutionCorrelationByBusinessKeyWithVariables()
	  {
		string businessKey = "aBusinessKey";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", businessKey);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		runtimeService.correlateMessage("newInvoiceMessage", businessKey, variables);

		// the execution that has been correlated should have advanced
		long correlatedExecutions = runtimeService.createExecutionQuery().processVariableValueEquals("aKey", "aValue").count();
		assertEquals(1, correlatedExecutions);

		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// use fluent builder /////////////////////////

		processInstance = runtimeService.startProcessInstanceByKey("process", businessKey);

		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceBusinessKey(businessKey).setVariable("aKey", "aValue").correlate();

		// the execution that has been correlated should have advanced
		correlatedExecutions = runtimeService.createExecutionQuery().processVariableValueEquals("aKey", "aValue").count();
		assertEquals(1, correlatedExecutions);

		runtimeService.deleteProcessInstance(processInstance.Id, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testExecutionCorrelationByBusinessKeyWithVariablesUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testExecutionCorrelationByBusinessKeyWithVariablesUsingFluentCorrelateAll()
	  {
		string businessKey = "aBusinessKey";

		runtimeService.startProcessInstanceByKey("process", businessKey);
		runtimeService.startProcessInstanceByKey("process", businessKey);

		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceBusinessKey(businessKey).setVariable("aKey", "aValue").correlateAll();

		// the executions that has been correlated should have advanced
		long correlatedExecutions = runtimeService.createExecutionQuery().processVariableValueEquals("aKey", "aValue").count();
		assertEquals(2, correlatedExecutions);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testExecutionCorrelationSetSerializedVariableValue() throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testExecutionCorrelationSetSerializedVariableValue()
	  {

		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

		// then it is not possible to deserialize the object
		try
		{
		  (new ObjectInputStream(new MemoryStream(baos.toByteArray()))).readObject();
		}
		catch (Exception e)
		{
		  testRule.assertTextPresent("Exception while deserializing object.", e.Message);
		}

		// but it can be set as a variable:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("var", Variables.serializedObjectValue(serializedObject).objectTypeName(typeof(FailingJavaSerializable).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create()).correlate();

		// then
		ObjectValue variableTyped = runtimeService.getVariableTyped(processInstance.Id, "var", false);
		assertNotNull(variableTyped);
		assertFalse(variableTyped.Deserialized);
		assertEquals(serializedObject, variableTyped.ValueSerialized);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
		assertEquals(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testExecutionCorrelationSetSerializedVariableValues() throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testExecutionCorrelationSetSerializedVariableValues()
	  {

		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

		// then it is not possible to deserialize the object
		try
		{
		  (new ObjectInputStream(new MemoryStream(baos.toByteArray()))).readObject();
		}
		catch (Exception e)
		{
		  testRule.assertTextPresent("Exception while deserializing object.", e.Message);
		}

		// but it can be set as a variable:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariables(Variables.createVariables().putValueTyped("var", Variables.serializedObjectValue(serializedObject).objectTypeName(typeof(FailingJavaSerializable).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create())).correlate();

		// then
		ObjectValue variableTyped = runtimeService.getVariableTyped(processInstance.Id, "var", false);
		assertNotNull(variableTyped);
		assertFalse(variableTyped.Deserialized);
		assertEquals(serializedObject, variableTyped.ValueSerialized);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
		assertEquals(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testMessageStartEventCorrelation()
	  public virtual void testMessageStartEventCorrelation()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";

		runtimeService.correlateMessage("newInvoiceMessage", new Dictionary<string, object>(), variables);

		long instances = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("aKey", "aValue").count();
		assertEquals(1, instances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageStartEventCorrelationUsingFluentCorrelateStartMessage()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageStartEventCorrelationUsingFluentCorrelateStartMessage()
	  {

		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("aKey", "aValue").correlateStartMessage();

		long instances = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("aKey", "aValue").count();
		assertEquals(1, instances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageStartEventCorrelationUsingFluentCorrelateSingle()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageStartEventCorrelationUsingFluentCorrelateSingle()
	  {

		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("aKey", "aValue").correlate();

		long instances = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("aKey", "aValue").count();
		assertEquals(1, instances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageStartEventCorrelationUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageStartEventCorrelationUsingFluentCorrelateAll()
	  {

		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("aKey", "aValue").correlateAll();

		long instances = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("aKey", "aValue").count();
		assertEquals(1, instances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml"}) @Test public void testMessageStartEventCorrelationWithBusinessKey()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml"})]
	  public virtual void testMessageStartEventCorrelationWithBusinessKey()
	  {
		const string businessKey = "aBusinessKey";

		runtimeService.correlateMessage("newInvoiceMessage", businessKey);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(processInstance);
		assertEquals(businessKey, processInstance.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml"}) @Test public void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateStartMessage()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml"})]
	  public virtual void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateStartMessage()
	  {
		const string businessKey = "aBusinessKey";

		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceBusinessKey(businessKey).correlateStartMessage();

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(processInstance);
		assertEquals(businessKey, processInstance.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml"}) @Test public void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateSingle()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml"})]
	  public virtual void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateSingle()
	  {
		const string businessKey = "aBusinessKey";

		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceBusinessKey(businessKey).correlate();

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(processInstance);
		assertEquals(businessKey, processInstance.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml"}) @Test public void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateAll()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml"})]
	  public virtual void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateAll()
	  {
		const string businessKey = "aBusinessKey";

		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceBusinessKey(businessKey).correlateAll();

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(processInstance);
		assertEquals(businessKey, processInstance.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageStartEventCorrelationSetSerializedVariableValue() throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageStartEventCorrelationSetSerializedVariableValue()
	  {

		// when
		FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

		// then it is not possible to deserialize the object
		try
		{
		  (new ObjectInputStream(new MemoryStream(baos.toByteArray()))).readObject();
		}
		catch (Exception e)
		{
		  testRule.assertTextPresent("Exception while deserializing object.", e.Message);
		}

		// but it can be set as a variable:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("var", Variables.serializedObjectValue(serializedObject).objectTypeName(typeof(FailingJavaSerializable).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create()).correlate();

		// then
		ProcessInstance startedInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(startedInstance);

		ObjectValue variableTyped = runtimeService.getVariableTyped(startedInstance.Id, "var", false);
		assertNotNull(variableTyped);
		assertFalse(variableTyped.Deserialized);
		assertEquals(serializedObject, variableTyped.ValueSerialized);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
		assertEquals(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageStartEventCorrelationSetSerializedVariableValues() throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageStartEventCorrelationSetSerializedVariableValues()
	  {

		// when
		FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

		// then it is not possible to deserialize the object
		try
		{
		  (new ObjectInputStream(new MemoryStream(baos.toByteArray()))).readObject();
		}
		catch (Exception e)
		{
		  testRule.assertTextPresent("Exception while deserializing object.", e.Message);
		}

		// but it can be set as a variable:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariables(Variables.createVariables().putValueTyped("var", Variables.serializedObjectValue(serializedObject).objectTypeName(typeof(FailingJavaSerializable).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create())).correlate();

		// then
		ProcessInstance startedInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(startedInstance);

		ObjectValue variableTyped = runtimeService.getVariableTyped(startedInstance.Id, "var", false);
		assertNotNull(variableTyped);
		assertFalse(variableTyped.Deserialized);
		assertEquals(serializedObject, variableTyped.ValueSerialized);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
		assertEquals(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateStartMessage()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateStartMessage()
	  {

		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariables(Variables.createVariables().putValue("var1", "a").putValue("var2", "b")).correlateStartMessage();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("var1", "a").variableValueEquals("var2", "b");
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateSingleMessage()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateSingleMessage()
	  {

		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariables(Variables.createVariables().putValue("var1", "a").putValue("var2", "b")).correlate();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("var1", "a").variableValueEquals("var2", "b");
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateAll()
	  {

		runtimeService.createMessageCorrelation("newInvoiceMessage").setVariables(Variables.createVariables().putValue("var1", "a").putValue("var2", "b")).correlateAll();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("var1", "a").variableValueEquals("var2", "b");
		assertEquals(1, query.count());
	  }

	  /// <summary>
	  /// this test assures the right start event is selected
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testMultipleMessageStartEventsCorrelation()
	  public virtual void testMultipleMessageStartEventsCorrelation()
	  {

		runtimeService.correlateMessage("someMessage");
		// verify the right start event was selected:
		Task task = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task);
		assertNull(taskService.createTaskQuery().taskDefinitionKey("task2").singleResult());
		taskService.complete(task.Id);

		runtimeService.correlateMessage("someOtherMessage");
		// verify the right start event was selected:
		task = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task);
		assertNull(taskService.createTaskQuery().taskDefinitionKey("task1").singleResult());
		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMultipleMessageStartEventsCorrelation.bpmn20.xml"}) @Test public void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateStartMessage()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMultipleMessageStartEventsCorrelation.bpmn20.xml"})]
	  public virtual void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateStartMessage()
	  {

		runtimeService.createMessageCorrelation("someMessage").correlateStartMessage();
		// verify the right start event was selected:
		Task task = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task);
		assertNull(taskService.createTaskQuery().taskDefinitionKey("task2").singleResult());
		taskService.complete(task.Id);

		runtimeService.createMessageCorrelation("someOtherMessage").correlateStartMessage();
		// verify the right start event was selected:
		task = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task);
		assertNull(taskService.createTaskQuery().taskDefinitionKey("task1").singleResult());
		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMultipleMessageStartEventsCorrelation.bpmn20.xml"}) @Test public void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateSingle()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMultipleMessageStartEventsCorrelation.bpmn20.xml"})]
	  public virtual void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateSingle()
	  {

		runtimeService.createMessageCorrelation("someMessage").correlate();
		// verify the right start event was selected:
		Task task = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task);
		assertNull(taskService.createTaskQuery().taskDefinitionKey("task2").singleResult());
		taskService.complete(task.Id);

		runtimeService.createMessageCorrelation("someOtherMessage").correlate();
		// verify the right start event was selected:
		task = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task);
		assertNull(taskService.createTaskQuery().taskDefinitionKey("task1").singleResult());
		taskService.complete(task.Id);
	  }

	  /// <summary>
	  /// this test assures the right start event is selected
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMultipleMessageStartEventsCorrelation.bpmn20.xml"}) @Test public void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateAll()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMultipleMessageStartEventsCorrelation.bpmn20.xml"})]
	  public virtual void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateAll()
	  {

		runtimeService.createMessageCorrelation("someMessage").correlateAll();
		// verify the right start event was selected:
		Task task = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task);
		assertNull(taskService.createTaskQuery().taskDefinitionKey("task2").singleResult());
		taskService.complete(task.Id);

		runtimeService.createMessageCorrelation("someOtherMessage").correlateAll();
		// verify the right start event was selected:
		task = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task);
		assertNull(taskService.createTaskQuery().taskDefinitionKey("task1").singleResult());
		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testMatchingStartEventAndExecution()
	  public virtual void testMatchingStartEventAndExecution()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		assertNotNull(runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoiceMessage").singleResult());
		// correlate message -> this will trigger the execution
		runtimeService.correlateMessage("newInvoiceMessage");
		assertNull(runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoiceMessage").singleResult());

		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// fluent builder //////////////////////

		processInstance = runtimeService.startProcessInstanceByKey("process");

		assertNotNull(runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoiceMessage").singleResult());
		// correlate message -> this will trigger the execution
		runtimeService.createMessageCorrelation("newInvoiceMessage").correlate();
		assertNull(runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoiceMessage").singleResult());

		runtimeService.deleteProcessInstance(processInstance.Id, null);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMatchingStartEventAndExecution.bpmn20.xml"}) @Test public void testMessageCorrelationResultWithResultTypeProcessDefinition()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMatchingStartEventAndExecution.bpmn20.xml"})]
	  public virtual void testMessageCorrelationResultWithResultTypeProcessDefinition()
	  {
		//given
		string msgName = "newInvoiceMessage";

		//when
		//correlate message with result
		MessageCorrelationResult result = runtimeService.createMessageCorrelation(msgName).correlateWithResult();

		//then
		//message correlation result contains information from receiver
		checkProcessDefinitionMessageCorrelationResult(result, "theStart", "process");
	  }

	  protected internal virtual void checkProcessDefinitionMessageCorrelationResult(MessageCorrelationResult result, string startActivityId, string processDefinitionId)
	  {
		assertNotNull(result);
		assertNotNull(result.ProcessInstance.Id);
		assertEquals(MessageCorrelationResultType.ProcessDefinition, result.ResultType);
		assertTrue(result.ProcessInstance.ProcessDefinitionId.Contains(processDefinitionId));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMatchingStartEventAndExecution.bpmn20.xml"}) @Test public void testMessageCorrelationResultWithResultTypeExecution()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMatchingStartEventAndExecution.bpmn20.xml"})]
	  public virtual void testMessageCorrelationResultWithResultTypeExecution()
	  {
		//given
		string msgName = "newInvoiceMessage";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		assertNotNull(runtimeService.createExecutionQuery().messageEventSubscriptionName(msgName).singleResult());

		//when
		//correlate message with result
		MessageCorrelationResult result = runtimeService.createMessageCorrelation(msgName).correlateWithResult();

		//then
		//message correlation result contains information from receiver
		checkExecutionMessageCorrelationResult(result, processInstance, "messageCatch");
	  }

	  protected internal virtual void checkExecutionMessageCorrelationResult(MessageCorrelationResult result, ProcessInstance processInstance, string activityId)
	  {
		assertNotNull(result);
		assertEquals(MessageCorrelationResultType.Execution, result.ResultType);
		assertEquals(processInstance.Id, result.Execution.ProcessInstanceId);
		ExecutionEntity entity = (ExecutionEntity) result.Execution;
		assertEquals(activityId, entity.ActivityId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMatchingStartEventAndExecution.bpmn20.xml"}) @Test public void testMatchingStartEventAndExecutionUsingFluentCorrelateAll()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMatchingStartEventAndExecution.bpmn20.xml"})]
	  public virtual void testMatchingStartEventAndExecutionUsingFluentCorrelateAll()
	  {
		runtimeService.startProcessInstanceByKey("process");
		runtimeService.startProcessInstanceByKey("process");

		assertEquals(2, runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoiceMessage").count());
		// correlate message -> this will trigger the executions AND start a new process instance
		runtimeService.createMessageCorrelation("newInvoiceMessage").correlateAll();
		assertNotNull(runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoiceMessage").singleResult());

		assertEquals(3, runtimeService.createProcessInstanceQuery().count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMatchingStartEventAndExecution.bpmn20.xml"}) @Test public void testMatchingStartEventAndExecutionCorrelateAllWithResult()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMatchingStartEventAndExecution.bpmn20.xml"})]
	  public virtual void testMatchingStartEventAndExecutionCorrelateAllWithResult()
	  {
		//given
		ProcessInstance procInstance1 = runtimeService.startProcessInstanceByKey("process");
		ProcessInstance procInstance2 = runtimeService.startProcessInstanceByKey("process");

		//when correlated all with result
		IList<MessageCorrelationResult> resultList = runtimeService.createMessageCorrelation("newInvoiceMessage").correlateAllWithResult();

		//then result should contains three entries
		//two of type execution und one of type process definition
		assertEquals(3, resultList.Count);
		int executionResultCount = 0;
		int procDefResultCount = 0;
		foreach (MessageCorrelationResult result in resultList)
		{
		  if (result.ResultType.Equals(MessageCorrelationResultType.Execution))
		  {
			assertNotNull(result);
			assertEquals(MessageCorrelationResultType.Execution, result.ResultType);
			assertTrue(procInstance1.Id.Equals(result.Execution.ProcessInstanceId, StringComparison.OrdinalIgnoreCase) || procInstance2.Id.Equals(result.Execution.ProcessInstanceId, StringComparison.OrdinalIgnoreCase));
			ExecutionEntity entity = (ExecutionEntity) result.Execution;
			assertEquals("messageCatch", entity.ActivityId);
			executionResultCount++;
		  }
		  else
		  {
			checkProcessDefinitionMessageCorrelationResult(result, "theStart", "process");
			procDefResultCount++;
		  }
		}
		assertEquals(2, executionResultCount);
		assertEquals(1, procDefResultCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageStartEventCorrelationWithNonMatchingDefinition()
	  public virtual void testMessageStartEventCorrelationWithNonMatchingDefinition()
	  {
		try
		{
		  runtimeService.correlateMessage("aMessageName");
		  fail("Expect an Exception");
		}
		catch (MismatchingMessageCorrelationException e)
		{
		  testRule.assertTextPresent("Cannot correlate message", e.Message);
		}

		// fluent builder //////////////////

		try
		{
		  runtimeService.createMessageCorrelation("aMessageName").correlate();
		  fail("Expect an Exception");
		}
		catch (MismatchingMessageCorrelationException e)
		{
		  testRule.assertTextPresent("Cannot correlate message", e.Message);
		}

		// fluent builder with multiple correlation //////////////////
		// This should not fail
		runtimeService.createMessageCorrelation("aMessageName").correlateAll();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByBusinessKeyAndVariables()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByBusinessKeyAndVariables()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", "aBusinessKey", variables);

		variables = new Dictionary<>();
		variables["aKey"] = "aValue";
		runtimeService.startProcessInstanceByKey("process", "anotherBusinessKey", variables);

		string messageName = "newInvoiceMessage";
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		correlationKeys["aKey"] = "aValue";

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["aProcessVariable"] = "aVariableValue";
		runtimeService.correlateMessage(messageName, "aBusinessKey", correlationKeys, processVariables);

		Execution correlatedExecution = runtimeService.createExecutionQuery().activityId("task").processVariableValueEquals("aProcessVariable", "aVariableValue").singleResult();

		assertNotNull(correlatedExecution);

		ProcessInstance correlatedProcessInstance = runtimeService.createProcessInstanceQuery().processInstanceId(correlatedExecution.ProcessInstanceId).singleResult();

		assertEquals("aBusinessKey", correlatedProcessInstance.BusinessKey);

		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// fluent builder /////////////////////////////

		variables = new Dictionary<>();
		variables["aKey"] = "aValue";
		processInstance = runtimeService.startProcessInstanceByKey("process", "aBusinessKey", variables);

		runtimeService.createMessageCorrelation(messageName).processInstanceBusinessKey("aBusinessKey").processInstanceVariableEquals("aKey", "aValue").setVariable("aProcessVariable", "aVariableValue").correlate();

		correlatedExecution = runtimeService.createExecutionQuery().activityId("task").processVariableValueEquals("aProcessVariable", "aVariableValue").singleResult();

		assertNotNull(correlatedExecution);

		correlatedProcessInstance = runtimeService.createProcessInstanceQuery().processInstanceId(correlatedExecution.ProcessInstanceId).singleResult();

		assertEquals("aBusinessKey", correlatedProcessInstance.BusinessKey);

		runtimeService.deleteProcessInstance(processInstance.Id, null);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByBusinessKeyAndVariablesUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByBusinessKeyAndVariablesUsingFluentCorrelateAll()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		runtimeService.startProcessInstanceByKey("process", "aBusinessKey", variables);
		runtimeService.startProcessInstanceByKey("process", "aBusinessKey", variables);

		string messageName = "newInvoiceMessage";
		runtimeService.createMessageCorrelation(messageName).processInstanceBusinessKey("aBusinessKey").processInstanceVariableEquals("aKey", "aValue").setVariable("aProcessVariable", "aVariableValue").correlateAll();

		IList<Execution> correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").processVariableValueEquals("aProcessVariable", "aVariableValue").list();

		assertEquals(2, correlatedExecutions.Count);

		// Instance 1
		Execution correlatedExecution = correlatedExecutions[0];
		ProcessInstance correlatedProcessInstance = runtimeService.createProcessInstanceQuery().processInstanceId(correlatedExecution.ProcessInstanceId).singleResult();

		assertEquals("aBusinessKey", correlatedProcessInstance.BusinessKey);

		// Instance 2
		correlatedExecution = correlatedExecutions[1];
		correlatedProcessInstance = runtimeService.createProcessInstanceQuery().processInstanceId(correlatedExecution.ProcessInstanceId).singleResult();

		assertEquals("aBusinessKey", correlatedProcessInstance.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByProcessInstanceId()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByProcessInstanceId()
	  {

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("process");

		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("process");

		// correlation with only the name is ambiguous:
		try
		{
		  runtimeService.createMessageCorrelation("aMessageName").correlate();
		  fail("Expect an Exception");
		}
		catch (MismatchingMessageCorrelationException e)
		{
		  testRule.assertTextPresent("Cannot correlate message", e.Message);
		}

		// use process instance id as well
		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceId(processInstance1.Id).correlate();

		Execution correlatedExecution = runtimeService.createExecutionQuery().activityId("task").processInstanceId(processInstance1.Id).singleResult();
		assertNotNull(correlatedExecution);

		Execution uncorrelatedExecution = runtimeService.createExecutionQuery().activityId("task").processInstanceId(processInstance2.Id).singleResult();
		assertNull(uncorrelatedExecution);

		runtimeService.deleteProcessInstance(processInstance1.Id, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByProcessInstanceIdUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByProcessInstanceIdUsingFluentCorrelateAll()
	  {
		// correlate by name
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("process");

		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("process");

		// correlation with only the name is ambiguous:
		runtimeService.createMessageCorrelation("aMessageName").correlateAll();

		assertEquals(0, runtimeService.createExecutionQuery().activityId("task").count());

		// correlate process instance id
		processInstance1 = runtimeService.startProcessInstanceByKey("process");

		processInstance2 = runtimeService.startProcessInstanceByKey("process");

		// use process instance id as well
		runtimeService.createMessageCorrelation("newInvoiceMessage").processInstanceId(processInstance1.Id).correlateAll();

		Execution correlatedExecution = runtimeService.createExecutionQuery().activityId("task").processInstanceId(processInstance1.Id).singleResult();
		assertNotNull(correlatedExecution);

		Execution uncorrelatedExecution = runtimeService.createExecutionQuery().activityId("task").processInstanceId(processInstance2.Id).singleResult();
		assertNull(uncorrelatedExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByBusinessKeyAndNullVariableUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByBusinessKeyAndNullVariableUsingFluentCorrelateAll()
	  {
		runtimeService.startProcessInstanceByKey("process", "aBusinessKey");

		string messageName = "newInvoiceMessage";

		try
		{
		  runtimeService.createMessageCorrelation(messageName).processInstanceBusinessKey("aBusinessKey").setVariable(null, "aVariableValue").correlateAll();
		  fail("Variable name is null");
		}
		catch (Exception e)
		{
		  assertTrue(e is ProcessEngineException);
		  testRule.assertTextPresent("null", e.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByBusinessKeyAndNullVariableEqualsUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByBusinessKeyAndNullVariableEqualsUsingFluentCorrelateAll()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		runtimeService.startProcessInstanceByKey("process", "aBusinessKey", variables);

		string messageName = "newInvoiceMessage";

		try
		{
		  runtimeService.createMessageCorrelation(messageName).processInstanceBusinessKey("aBusinessKey").processInstanceVariableEquals(null, "bar").correlateAll();
		  fail("Variable name is null");
		}
		catch (Exception e)
		{
		  assertTrue(e is ProcessEngineException);
		  testRule.assertTextPresent("null", e.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByBusinessKeyAndNullVariablesUsingFluentCorrelateAll()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByBusinessKeyAndNullVariablesUsingFluentCorrelateAll()
	  {
		runtimeService.startProcessInstanceByKey("process", "aBusinessKey");

		string messageName = "newInvoiceMessage";

		runtimeService.createMessageCorrelation(messageName).processInstanceBusinessKey("aBusinessKey").setVariables(null).setVariable("foo", "bar").correlateAll();

		IList<Execution> correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").processVariableValueEquals("foo", "bar").list();

		assertFalse(correlatedExecutions.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByVariablesOnly()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByVariablesOnly()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variable"] = "value1";
		runtimeService.startProcessInstanceByKey("process", variables);

		variables["variable"] = "value2";
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process", variables);

		runtimeService.correlateMessage(null, variables);

		IList<Execution> correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").list();

		assertEquals(1, correlatedExecutions.Count);
		assertEquals(instance.Id, correlatedExecutions[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByBusinessKey()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByBusinessKey()
	  {
		runtimeService.startProcessInstanceByKey("process", "businessKey1");
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process", "businessKey2");

		runtimeService.correlateMessage(null, "businessKey2");

		IList<Execution> correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").list();

		assertEquals(1, correlatedExecutions.Count);
		assertEquals(instance.Id, correlatedExecutions[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationByProcessInstanceIdOnly()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationByProcessInstanceIdOnly()
	  {
		runtimeService.startProcessInstanceByKey("process");
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");

		runtimeService.createMessageCorrelation(null).processInstanceId(instance.Id).correlate();

		IList<Execution> correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").list();

		assertEquals(1, correlatedExecutions.Count);
		assertEquals(instance.Id, correlatedExecutions[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationWithoutMessageNameFluent()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationWithoutMessageNameFluent()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variable"] = "value1";
		runtimeService.startProcessInstanceByKey("process", variables);

		variables["variable"] = "value2";
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process", variables);

		runtimeService.createMessageCorrelation(null).processInstanceVariableEquals("variable", "value2").correlate();

		IList<Execution> correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").list();

		assertEquals(1, correlatedExecutions.Count);
		assertEquals(instance.Id, correlatedExecutions[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCorrelateAllWithoutMessage.bpmn20.xml"}) @Test public void testCorrelateAllWithoutMessage()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCorrelateAllWithoutMessage.bpmn20.xml"})]
	  public virtual void testCorrelateAllWithoutMessage()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variable"] = "value1";
		runtimeService.startProcessInstanceByKey("process", variables);
		runtimeService.startProcessInstanceByKey("secondProcess", variables);

		variables["variable"] = "value2";
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("process", variables);
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("secondProcess", variables);

		runtimeService.createMessageCorrelation(null).processInstanceVariableEquals("variable", "value2").correlateAll();

		IList<Execution> correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").orderByProcessDefinitionKey().asc().list();

		assertEquals(2, correlatedExecutions.Count);
		assertEquals(instance1.Id, correlatedExecutions[0].Id);
		assertEquals(instance2.Id, correlatedExecutions[1].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testCorrelationWithoutMessageDoesNotMatchStartEvent()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationWithoutMessageDoesNotMatchStartEvent()
	  {
		try
		{
		  runtimeService.createMessageCorrelation(null).processInstanceVariableEquals("variable", "value2").correlate();
		  fail("exception expected");
		}
		catch (MismatchingMessageCorrelationException)
		{
		  // expected
		}

		IList<Execution> correlatedExecutions = runtimeService.createExecutionQuery().activityId("task").list();

		assertTrue(correlatedExecutions.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelationWithoutCorrelationPropertiesFails()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelationWithoutCorrelationPropertiesFails()
	  {

		runtimeService.startProcessInstanceByKey("process");

		try
		{
		  runtimeService.createMessageCorrelation(null).correlate();
		  fail("expected exception");
		}
		catch (NullValueException)
		{
		  // expected
		}

		try
		{
		  runtimeService.correlateMessage(null);
		  fail("expected exception");
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/twoBoundaryEventSubscriptions.bpmn20.xml") @Test public void testCorrelationToExecutionWithMultipleSubscriptionsFails()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/twoBoundaryEventSubscriptions.bpmn20.xml")]
	  public virtual void testCorrelationToExecutionWithMultipleSubscriptionsFails()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");

		try
		{
		  runtimeService.createMessageCorrelation(null).processInstanceId(instance.Id).correlate();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // note: this does not expect a MismatchingCorrelationException since the exception
		  // is only raised in the MessageEventReceivedCmd. Otherwise, this would require explicit checking in the
		  // correlation handler that a matched execution without message name has exactly one message (now it checks for
		  // at least one message)

		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testSuspendedProcessInstance()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testSuspendedProcessInstance()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		string processInstance = runtimeService.startProcessInstanceByKey("process", variables).Id;

		// suspend process instance
		runtimeService.suspendProcessInstanceById(processInstance);

		string messageName = "newInvoiceMessage";
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		correlationKeys["aKey"] = "aValue";

		try
		{
		  runtimeService.correlateMessage(messageName, correlationKeys);
		  fail("It should not be possible to correlate a message to a suspended process instance.");
		}
		catch (MismatchingMessageCorrelationException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testOneMatchingAndOneSuspendedProcessInstance()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testOneMatchingAndOneSuspendedProcessInstance()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";
		string firstProcessInstance = runtimeService.startProcessInstanceByKey("process", variables).Id;

		variables = new Dictionary<>();
		variables["aKey"] = "aValue";
		string secondProcessInstance = runtimeService.startProcessInstanceByKey("process", variables).Id;

		// suspend second process instance
		runtimeService.suspendProcessInstanceById(secondProcessInstance);

		string messageName = "newInvoiceMessage";
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		correlationKeys["aKey"] = "aValue";

		IDictionary<string, object> messagePayload = new Dictionary<string, object>();
		messagePayload["aNewKey"] = "aNewVariable";

		runtimeService.correlateMessage(messageName, correlationKeys, messagePayload);

		// there exists an uncorrelated executions (the second process instance)
		long uncorrelatedExecutions = runtimeService.createExecutionQuery().processInstanceId(secondProcessInstance).processVariableValueEquals("aKey", "aValue").messageEventSubscriptionName("newInvoiceMessage").count();
		assertEquals(1, uncorrelatedExecutions);

		// the execution that has been correlated should have advanced
		long correlatedExecutions = runtimeService.createExecutionQuery().processInstanceId(firstProcessInstance).activityId("task").processVariableValueEquals("aKey", "aValue").processVariableValueEquals("aNewKey", "aNewVariable").count();
		assertEquals(1, correlatedExecutions);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testSuspendedProcessDefinition()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testSuspendedProcessDefinition()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		repositoryService.suspendProcessDefinitionById(processDefinitionId);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aKey"] = "aValue";

		try
		{
		  runtimeService.correlateMessage("newInvoiceMessage", new Dictionary<string, object>(), variables);
		  fail("It should not be possible to correlate a message to a suspended process definition.");
		}
		catch (MismatchingMessageCorrelationException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelateMessageStartEventWithProcessDefinitionId()
	  public virtual void testCorrelateMessageStartEventWithProcessDefinitionId()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().message("a").userTask().endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().message("b").userTask().endEvent().done());

		ProcessDefinition firstProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(1).singleResult();
		ProcessDefinition secondProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(2).singleResult();

		runtimeService.createMessageCorrelation("a").processDefinitionId(firstProcessDefinition.Id).processInstanceBusinessKey("first").correlateStartMessage();

		runtimeService.createMessageCorrelation("b").processDefinitionId(secondProcessDefinition.Id).processInstanceBusinessKey("second").correlateStartMessage();

		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("first").processDefinitionId(firstProcessDefinition.Id).count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("second").processDefinitionId(secondProcessDefinition.Id).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailCorrelateMessageStartEventWithWrongProcessDefinitionId()
	  public virtual void testFailCorrelateMessageStartEventWithWrongProcessDefinitionId()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().message("a").userTask().endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().message("b").userTask().endEvent().done());

		ProcessDefinition latestProcessDefinition = repositoryService.createProcessDefinitionQuery().latestVersion().singleResult();

		try
		{
		  runtimeService.createMessageCorrelation("a").processDefinitionId(latestProcessDefinition.Id).correlateStartMessage();

		  fail("expected exception");
		}
		catch (MismatchingMessageCorrelationException e)
		{
		  testRule.assertTextPresent("Cannot correlate message 'a'", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailCorrelateMessageStartEventWithNonExistingProcessDefinitionId()
	  public virtual void testFailCorrelateMessageStartEventWithNonExistingProcessDefinitionId()
	  {
		try
		{
		  runtimeService.createMessageCorrelation("a").processDefinitionId("not existing").correlateStartMessage();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("no deployed process definition found", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailCorrelateMessageWithProcessDefinitionId()
	  public virtual void testFailCorrelateMessageWithProcessDefinitionId()
	  {
		try
		{
		  runtimeService.createMessageCorrelation("a").processDefinitionId("id").correlate();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  testRule.assertTextPresent("Cannot specify a process definition id", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailCorrelateMessagesWithProcessDefinitionId()
	  public virtual void testFailCorrelateMessagesWithProcessDefinitionId()
	  {
		try
		{
		  runtimeService.createMessageCorrelation("a").processDefinitionId("id").correlateAll();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  testRule.assertTextPresent("Cannot specify a process definition id", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailCorrelateMessageStartEventWithCorrelationVariable()
	  public virtual void testFailCorrelateMessageStartEventWithCorrelationVariable()
	  {
		try
		{
		  runtimeService.createMessageCorrelation("a").processInstanceVariableEquals("var", "value").correlateStartMessage();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  testRule.assertTextPresent("Cannot specify correlation variables ", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailCorrelateMessageStartEventWithCorrelationVariables()
	  public virtual void testFailCorrelateMessageStartEventWithCorrelationVariables()
	  {
		try
		{
		  runtimeService.createMessageCorrelation("a").processInstanceVariablesEqual(Variables.createVariables().putValue("var1", "b").putValue("var2", "c")).correlateStartMessage();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  testRule.assertTextPresent("Cannot specify correlation variables ", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelationWithResultBySettingLocalVariables()
	  public virtual void testCorrelationWithResultBySettingLocalVariables()
	  {
		// given
		string outputVarName = "localVar";
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().intermediateCatchEvent("message_1").message("1").camundaOutputParameter(outputVarName, "${testLocalVar}").userTask("UserTask_1").endEvent().done();

		testRule.deploy(model);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["processInstanceVar"] = "processInstanceVarValue";
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		IDictionary<string, object> messageLocalPayload = new Dictionary<string, object>();
		string outpuValue = "outputValue";
		string localVarName = "testLocalVar";
		messageLocalPayload[localVarName] = outpuValue;

		// when
		MessageCorrelationResultWithVariables messageCorrelationResult = runtimeService.createMessageCorrelation("1").setVariablesLocal(messageLocalPayload).correlateWithResultAndVariables(true);

		// then
		checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "message_1");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).variableName(outputVarName).singleResult();
		assertNotNull(variable);
		assertEquals(outpuValue, variable.Value);
		assertEquals(processInstance.Id, variable.ExecutionId);

		VariableInstance variableNonExisting = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).variableName(localVarName).singleResult();
		assertNull(variableNonExisting);

		VariableMap variablesInReturn = messageCorrelationResult.Variables;
		assertEquals(variable.TypedValue, variablesInReturn.getValueTyped(outputVarName));
		assertEquals("processInstanceVarValue", variablesInReturn.getValue("processInstanceVar", typeof(string)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelationBySettingLocalVariables()
	  public virtual void testCorrelationBySettingLocalVariables()
	  {
		// given
		string outputVarName = "localVar";
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().intermediateCatchEvent("message_1").message("1").camundaOutputParameter(outputVarName, "${testLocalVar}").userTask("UserTask_1").endEvent().done();

		testRule.deploy(model);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["processInstanceVar"] = "processInstanceVarValue";
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		IDictionary<string, object> messageLocalPayload = new Dictionary<string, object>();
		string outpuValue = "outputValue";
		string localVarName = "testLocalVar";
		messageLocalPayload[localVarName] = outpuValue;

		// when
		runtimeService.createMessageCorrelation("1").setVariablesLocal(messageLocalPayload).correlate();

		// then
		VariableInstance variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).variableName(outputVarName).singleResult();
		assertNotNull(variable);
		assertEquals(outpuValue, variable.Value);
		assertEquals(processInstance.Id, variable.ExecutionId);

		VariableInstance variableNonExisting = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).variableName(localVarName).singleResult();
		assertNull(variableNonExisting);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.waitForMessageProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.sendMessageProcess.bpmn20.xml" }) @Test public void testCorrelateWithResultTwoTimesInSameTransaction()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.waitForMessageProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.sendMessageProcess.bpmn20.xml" })]
	  public virtual void testCorrelateWithResultTwoTimesInSameTransaction()
	  {
		// start process that waits for message
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["correlationKey"] = "someCorrelationKey";
		ProcessInstance messageWaitProcess = runtimeService.startProcessInstanceByKey("waitForMessageProcess", variables);

		Execution waitingProcess = runtimeService.createExecutionQuery().executionId(messageWaitProcess.ProcessInstanceId).singleResult();
		Assert.assertNotNull(waitingProcess);

		thrown.expect(typeof(MismatchingMessageCorrelationException));
		thrown.expectMessage("Cannot correlate message 'waitForCorrelationKeyMessage'");

		// start process that sends two messages with the same correlationKey
		VariableMap switchScenarioFlag = Variables.createVariables().putValue("allFlag", false);
		runtimeService.startProcessInstanceByKey("sendMessageProcess", switchScenarioFlag);

		// waiting process must be finished
		waitingProcess = runtimeService.createExecutionQuery().executionId(messageWaitProcess.ProcessInstanceId).singleResult();
		Assert.assertNull(waitingProcess);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.waitForMessageProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.sendMessageProcess.bpmn20.xml" }) @Test public void testCorrelateAllWithResultTwoTimesInSameTransaction()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.waitForMessageProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.sendMessageProcess.bpmn20.xml" })]
	  public virtual void testCorrelateAllWithResultTwoTimesInSameTransaction()
	  {
		// start process that waits for message
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["correlationKey"] = "someCorrelationKey";
		ProcessInstance messageWaitProcess = runtimeService.startProcessInstanceByKey("waitForMessageProcess", variables);

		Execution waitingProcess = runtimeService.createExecutionQuery().executionId(messageWaitProcess.ProcessInstanceId).singleResult();
		Assert.assertNotNull(waitingProcess);

		// start process that sends two messages with the same correlationKey
		VariableMap switchScenarioFlag = Variables.createVariables().putValue("allFlag", true);
		runtimeService.startProcessInstanceByKey("sendMessageProcess", switchScenarioFlag);

		// waiting process must be finished
		waitingProcess = runtimeService.createExecutionQuery().executionId(messageWaitProcess.ProcessInstanceId).singleResult();
		Assert.assertNull(waitingProcess);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Ignore("CAM-10198") public void testMessageStartEventCorrelationWithLocalVariables()
	  public virtual void testMessageStartEventCorrelationWithLocalVariables()
	  {
		// given
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().message("1").userTask("userTask1").endEvent().done();

		testRule.deploy(model);

		IDictionary<string, object> messagePayload = new Dictionary<string, object>();
		string outpuValue = "outputValue";
		string localVarName = "testLocalVar";
		messagePayload[localVarName] = outpuValue;

		// when
		MessageCorrelationResult result = runtimeService.createMessageCorrelation("1").setVariablesLocal(messagePayload).correlateWithResult();

		// then
		assertNotNull(result);
		assertEquals(MessageCorrelationResultType.ProcessDefinition, result.ResultType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageStartEventCorrelationWithVariablesInResult()
	  public virtual void testMessageStartEventCorrelationWithVariablesInResult()
	  {
		// given
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().message("1").userTask("UserTask_1").endEvent().done();

		testRule.deploy(model);

		// when
		MessageCorrelationResultWithVariables result = runtimeService.createMessageCorrelation("1").setVariable("foo", "bar").correlateWithResultAndVariables(true);

		// then
		assertNotNull(result);
		assertEquals(MessageCorrelationResultType.ProcessDefinition, result.ResultType);
		assertEquals("bar", result.Variables.getValue("foo", typeof(string)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml") @Test public void testCorrelateAllWithResultVariables()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testCatchingMessageEventCorrelation.bpmn20.xml")]
	  public virtual void testCorrelateAllWithResultVariables()
	  {
		//given
		ProcessInstance procInstance1 = runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("var1", "foo"));
		ProcessInstance procInstance2 = runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("var2", "bar"));

		//when correlated all with result and variables
		IList<MessageCorrelationResultWithVariables> resultList = runtimeService.createMessageCorrelation("newInvoiceMessage").correlateAllWithResultAndVariables(true);

		assertEquals(2, resultList.Count);
		//then result should contains executions on which messages was correlated
		foreach (MessageCorrelationResultWithVariables result in resultList)
		{
		  assertNotNull(result);
		  assertEquals(MessageCorrelationResultType.Execution, result.ResultType);
		  ExecutionEntity execution = (ExecutionEntity) result.Execution;
		  VariableMap variables = result.Variables;
		  assertEquals(1, variables.size());
		  if (procInstance1.Id.Equals(execution.ProcessInstanceId, StringComparison.OrdinalIgnoreCase))
		  {
			assertEquals("foo", variables.getValue("var1", typeof(string)));
		  }
		  else if (procInstance2.Id.Equals(execution.ProcessInstanceId, StringComparison.OrdinalIgnoreCase))
		  {
			assertEquals("bar", variables.getValue("var2", typeof(string)));
		  }
		  else
		  {
			fail("Only those process instances should exist");
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelationWithModifiedVariablesInResult()
	  public virtual void testCorrelationWithModifiedVariablesInResult()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().intermediateCatchEvent("Message_1").message("1").serviceTask().camundaClass(typeof(ChangeVariableDelegate).FullName).userTask("UserTask_1").endEvent().done();

		testRule.deploy(model);

		runtimeService.startProcessInstanceByKey("Process_1", Variables.createVariables().putValue("a", 40).putValue("b", 2));

		// when
		MessageCorrelationResultWithVariables result = runtimeService.createMessageCorrelation("1").correlateWithResultAndVariables(true);

		// then
		assertNotNull(result);
		assertEquals(MessageCorrelationResultType.Execution, result.ResultType);
		assertEquals(3, result.Variables.size());
		assertEquals("foo", result.Variables.get("a"));
		assertEquals(2, result.Variables.get("b"));
		assertEquals(42, result.Variables.get("sum"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelateWithVariablesInReturnShouldDeserializeObjectValue()
	  public virtual void testCorrelateWithVariablesInReturnShouldDeserializeObjectValue()
	  {
		// given
		BpmnModelInstance model = Bpmn.createExecutableProcess("process").startEvent().intermediateCatchEvent("Message_1").message("1").userTask("UserTask_1").endEvent().done();

		testRule.deploy(model);

		ObjectValue value = Variables.objectValue("value").create();
		VariableMap variables = Variables.createVariables().putValue("var", value);

		runtimeService.startProcessInstanceByKey("process", variables);

		// when
		MessageCorrelationResultWithVariables result = runtimeService.createMessageCorrelation("1").correlateWithResultAndVariables(true);

		// then
		VariableMap resultVariables = result.Variables;

		ObjectValue returnedValue = resultVariables.getValueTyped("var");
		assertThat(returnedValue.Deserialized).True;
		assertThat(returnedValue.Value).isEqualTo("value");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelateWithVariablesInReturnShouldNotDeserializeObjectValue()
	  public virtual void testCorrelateWithVariablesInReturnShouldNotDeserializeObjectValue()
	  {
		// given
		BpmnModelInstance model = Bpmn.createExecutableProcess("process").startEvent().intermediateCatchEvent("Message_1").message("1").userTask("UserTask_1").endEvent().done();

		testRule.deploy(model);

		ObjectValue value = Variables.objectValue("value").create();
		VariableMap variables = Variables.createVariables().putValue("var", value);

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process", variables);
		string serializedValue = ((ObjectValue) runtimeService.getVariableTyped(instance.Id, "var")).ValueSerialized;

		// when
		MessageCorrelationResultWithVariables result = runtimeService.createMessageCorrelation("1").correlateWithResultAndVariables(false);

		// then
		VariableMap resultVariables = result.Variables;

		ObjectValue returnedValue = resultVariables.getValueTyped("var");
		assertThat(returnedValue.Deserialized).False;
		assertThat(returnedValue.ValueSerialized).isEqualTo(serializedValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelateAllWithVariablesInReturnShouldDeserializeObjectValue()
	  public virtual void testCorrelateAllWithVariablesInReturnShouldDeserializeObjectValue()
	  {
		// given
		BpmnModelInstance model = Bpmn.createExecutableProcess("process").startEvent().intermediateCatchEvent("Message_1").message("1").userTask("UserTask_1").endEvent().done();

		testRule.deploy(model);

		ObjectValue value = Variables.objectValue("value").create();
		VariableMap variables = Variables.createVariables().putValue("var", value);

		runtimeService.startProcessInstanceByKey("process", variables);

		// when
		IList<MessageCorrelationResultWithVariables> result = runtimeService.createMessageCorrelation("1").correlateAllWithResultAndVariables(true);

		// then
		assertThat(result).hasSize(1);

		VariableMap resultVariables = result[0].Variables;

		ObjectValue returnedValue = resultVariables.getValueTyped("var");
		assertThat(returnedValue.Deserialized).True;
		assertThat(returnedValue.Value).isEqualTo("value");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelateAllWithVariablesInReturnShouldNotDeserializeObjectValue()
	  public virtual void testCorrelateAllWithVariablesInReturnShouldNotDeserializeObjectValue()
	  {
		// given
		BpmnModelInstance model = Bpmn.createExecutableProcess("process").startEvent().intermediateCatchEvent("Message_1").message("1").userTask("UserTask_1").endEvent().done();

		testRule.deploy(model);

		ObjectValue value = Variables.objectValue("value").create();
		VariableMap variables = Variables.createVariables().putValue("var", value);

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process", variables);
		string serializedValue = ((ObjectValue) runtimeService.getVariableTyped(instance.Id, "var")).ValueSerialized;

		// when
		IList<MessageCorrelationResultWithVariables> result = runtimeService.createMessageCorrelation("1").correlateAllWithResultAndVariables(false);

		// then
		assertThat(result).hasSize(1);

		VariableMap resultVariables = result[0].Variables;

		ObjectValue returnedValue = resultVariables.getValueTyped("var");
		assertThat(returnedValue.Deserialized).False;
		assertThat(returnedValue.ValueSerialized).isEqualTo(serializedValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartMessageOnlyFlag()
	  public virtual void testStartMessageOnlyFlag()
	  {
		deployTwoVersionsWithStartMessageEvent();

		ProcessDefinition firstProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(1).singleResult();
		ProcessDefinition secondProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(2).singleResult();

		runtimeService.createMessageCorrelation("a").startMessageOnly().processDefinitionId(firstProcessDefinition.Id).processInstanceBusinessKey("first").correlate();

		runtimeService.createMessageCorrelation("a").startMessageOnly().processDefinitionId(secondProcessDefinition.Id).processInstanceBusinessKey("second").correlate();

		assertTwoInstancesAreStarted(firstProcessDefinition, secondProcessDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartMessageOnlyFlagAll()
	  public virtual void testStartMessageOnlyFlagAll()
	  {
		deployTwoVersionsWithStartMessageEvent();

		ProcessDefinition firstProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(1).singleResult();
		ProcessDefinition secondProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(2).singleResult();

		runtimeService.createMessageCorrelation("a").startMessageOnly().processDefinitionId(firstProcessDefinition.Id).processInstanceBusinessKey("first").correlateAll();

		runtimeService.createMessageCorrelation("a").startMessageOnly().processDefinitionId(secondProcessDefinition.Id).processInstanceBusinessKey("second").correlateAll();

		assertTwoInstancesAreStarted(firstProcessDefinition, secondProcessDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testStartMessageOnlyFlagWithResult()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testStartMessageOnlyFlagWithResult()
	  {
		MessageCorrelationResult result = runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("aKey", "aValue").startMessageOnly().correlateWithResult();

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("aKey", "aValue");
		assertThat(processInstanceQuery.count()).isEqualTo(1);
		assertThat(result.ProcessInstance.Id).isEqualTo(processInstanceQuery.singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testStartMessageOnlyFlagWithVariablesInResult()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testStartMessageOnlyFlagWithVariablesInResult()
	  {

		MessageCorrelationResultWithVariables result = runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("aKey", "aValue").startMessageOnly().correlateWithResultAndVariables(false);

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("aKey", "aValue");
		assertThat(processInstanceQuery.count()).isEqualTo(1);
		assertThat(result.Variables.size()).isEqualTo(1);
		assertThat(result.Variables.getValueTyped("aKey").Value).isEqualTo("aValue");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testStartMessageOnlyFlagAllWithResult()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testStartMessageOnlyFlagAllWithResult()
	  {
		IList<MessageCorrelationResult> result = runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("aKey", "aValue").startMessageOnly().correlateAllWithResult();

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("aKey", "aValue");
		assertThat(processInstanceQuery.count()).isEqualTo(1);
		assertThat(result[0].ProcessInstance.Id).isEqualTo(processInstanceQuery.singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml") @Test public void testStartMessageOnlyFlagAllWithVariablesInResult()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/MessageCorrelationTest.testMessageStartEventCorrelation.bpmn20.xml")]
	  public virtual void testStartMessageOnlyFlagAllWithVariablesInResult()
	  {

		IList<MessageCorrelationResultWithVariables> results = runtimeService.createMessageCorrelation("newInvoiceMessage").setVariable("aKey", "aValue").startMessageOnly().correlateAllWithResultAndVariables(false);

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("messageStartEvent").variableValueEquals("aKey", "aValue");
		assertThat(processInstanceQuery.count()).isEqualTo(1);
		MessageCorrelationResultWithVariables result = results[0];
		assertThat(result.Variables.size()).isEqualTo(1);
		assertThat(result.Variables.getValueTyped("aKey").Value).isEqualTo("aValue");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailStartMessageOnlyFlagWithCorrelationVariable()
	  public virtual void testFailStartMessageOnlyFlagWithCorrelationVariable()
	  {
		try
		{
		  runtimeService.createMessageCorrelation("a").startMessageOnly().processInstanceVariableEquals("var", "value").correlate();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  testRule.assertTextPresent("Cannot specify correlation variables ", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailStartMessageOnlyFlagWithCorrelationVariables()
	  public virtual void testFailStartMessageOnlyFlagWithCorrelationVariables()
	  {
		try
		{
		  runtimeService.createMessageCorrelation("a").startMessageOnly().processInstanceVariablesEqual(Variables.createVariables().putValue("var1", "b").putValue("var2", "c")).correlateAll();

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  testRule.assertTextPresent("Cannot specify correlation variables ", e.Message);
		}
	  }

	  protected internal virtual void deployTwoVersionsWithStartMessageEvent()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().message("a").userTask("ut1").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().message("a").userTask("ut2").endEvent().done());
	  }

	  protected internal virtual void assertTwoInstancesAreStarted(ProcessDefinition firstProcessDefinition, ProcessDefinition secondProcessDefinition)
	  {
		assertThat(runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("first").processDefinitionId(firstProcessDefinition.Id).count()).isEqualTo(1);
		assertThat(runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("second").processDefinitionId(secondProcessDefinition.Id).count()).isEqualTo(1);
	  }

	  public class ChangeVariableDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  int? a = (int?) execution.getVariable("a");
		  int? b = (int?) execution.getVariable("b");
		  execution.setVariable("sum", a + b);
		  execution.setVariable("a", "foo");
		}
	  }
	}

}