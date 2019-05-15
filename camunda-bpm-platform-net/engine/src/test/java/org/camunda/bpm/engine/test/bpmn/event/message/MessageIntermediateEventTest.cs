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
namespace org.camunda.bpm.engine.test.bpmn.@event.message
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using FailingJavaSerializable = org.camunda.bpm.engine.test.api.variables.FailingJavaSerializable;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
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
	/// @author Daniel Meyer
	/// @author Nico Rehwaldt
	/// </summary>
	public class MessageIntermediateEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MessageIntermediateEventTest()
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
//ORIGINAL LINE: @Deployment @Test public void testSingleIntermediateMessageEvent()
	  public virtual void testSingleIntermediateMessageEvent()
	  {

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(pi.Id);
		assertNotNull(activeActivityIds);
		assertEquals(1, activeActivityIds.Count);
		assertTrue(activeActivityIds.Contains("messageCatch"));

		string messageName = "newInvoiceMessage";
		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName(messageName).singleResult();

		assertNotNull(execution);

		runtimeService.messageEventReceived(messageName, execution.Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testConcurrentIntermediateMessageEvent()
	  public virtual void testConcurrentIntermediateMessageEvent()
	  {

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(pi.Id);
		assertNotNull(activeActivityIds);
		assertEquals(2, activeActivityIds.Count);
		assertTrue(activeActivityIds.Contains("messageCatch1"));
		assertTrue(activeActivityIds.Contains("messageCatch2"));

		string messageName = "newInvoiceMessage";
		IList<Execution> executions = runtimeService.createExecutionQuery().messageEventSubscriptionName(messageName).list();

		assertNotNull(executions);
		assertEquals(2, executions.Count);

		runtimeService.messageEventReceived(messageName, executions[0].Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertNull(task);

		runtimeService.messageEventReceived(messageName, executions[1].Id);

		task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateMessageEventRedeployment()
	  public virtual void testIntermediateMessageEventRedeployment()
	  {

		// deploy version 1
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageIntermediateEventTest.testSingleIntermediateMessageEvent.bpmn20.xml").deploy();
		// now there is one process deployed
		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(pi.Id);
		assertNotNull(activeActivityIds);
		assertEquals(1, activeActivityIds.Count);
		assertTrue(activeActivityIds.Contains("messageCatch"));

		// deploy version 2
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageIntermediateEventTest.testSingleIntermediateMessageEvent.bpmn20.xml").deploy();

		// now there are two versions deployed:
		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());

		// assert process is still waiting in message event:
		activeActivityIds = runtimeService.getActiveActivityIds(pi.Id);
		assertNotNull(activeActivityIds);
		assertEquals(1, activeActivityIds.Count);
		assertTrue(activeActivityIds.Contains("messageCatch"));

		// delete both versions:
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyMessageNameFails()
	  public virtual void testEmptyMessageNameFails()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageIntermediateEventTest.testEmptyMessageNameFails.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("Cannot have a message event subscription with an empty or missing name"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/event/message/MessageIntermediateEventTest.testSingleIntermediateMessageEvent.bpmn20.xml") @Test public void testSetSerializedVariableValues() throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/message/MessageIntermediateEventTest.testSingleIntermediateMessageEvent.bpmn20.xml")]
	  public virtual void testSetSerializedVariableValues()
	  {

		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		EventSubscription messageEventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();

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

		// but it can be set as a variable when delivering a message:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.messageEventReceived("newInvoiceMessage", messageEventSubscription.ExecutionId, Variables.createVariables().putValueTyped("var", Variables.serializedObjectValue(serializedObject).objectTypeName(typeof(FailingJavaSerializable).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create()));

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
//ORIGINAL LINE: @Deployment @Test public void testExpressionInSingleIntermediateMessageEvent()
	  public virtual void testExpressionInSingleIntermediateMessageEvent()
	  {

		// given
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";

		// when
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process", variables);
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(pi.Id);
		assertNotNull(activeActivityIds);
		assertEquals(1, activeActivityIds.Count);
		assertTrue(activeActivityIds.Contains("messageCatch"));

		// then
		string messageName = "newInvoiceMessage-bar";
		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName(messageName).singleResult();
		assertNotNull(execution);

		runtimeService.messageEventReceived(messageName, execution.Id);
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);
	  }

	}

}