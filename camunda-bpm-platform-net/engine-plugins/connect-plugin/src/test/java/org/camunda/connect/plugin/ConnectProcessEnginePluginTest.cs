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
namespace org.camunda.connect.plugin
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using BpmnParseException = org.camunda.bpm.engine.BpmnParseException;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using HttpConnector = org.camunda.connect.httpclient.HttpConnector;
	using SoapHttpConnector = org.camunda.connect.httpclient.soap.SoapHttpConnector;
	using TestConnector = org.camunda.connect.plugin.util.TestConnector;
	using Connector = org.camunda.connect.spi.Connector;

	public class ConnectProcessEnginePluginTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();
		TestConnector.responseParameters.Clear();
		TestConnector.requestParameters = null;
	  }

	  public virtual void testConnectorsRegistered()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.connect.spi.Connector<?> http = org.camunda.connect.Connectors.getConnector(org.camunda.connect.httpclient.HttpConnector.ID);
		Connector<object> http = Connectors.getConnector(HttpConnector.ID);
		assertNotNull(http);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.connect.spi.Connector<?> soap = org.camunda.connect.Connectors.getConnector(org.camunda.connect.httpclient.soap.SoapHttpConnector.ID);
		Connector<object> soap = Connectors.getConnector(SoapHttpConnector.ID);
		assertNotNull(soap);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.connect.spi.Connector<?> test = org.camunda.connect.Connectors.getConnector(org.camunda.connect.plugin.util.TestConnector.ID);
		Connector<object> test = Connectors.getConnector(TestConnector.ID);
		assertNotNull(test);
	  }

	  public virtual void testConnectorIdMissing()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorIdMissing.bpmn").deploy();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertFalse(e is BpmnParseException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConnectorIdUnknown()
	  public virtual void testConnectorIdUnknown()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess");
		  fail("Exception expected");
		}
		catch (ConnectorException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConnectorInvoked()
	  public virtual void testConnectorInvoked()
	  {
		string outputParamValue = "someOutputValue";
		string inputVariableValue = "someInputVariableValue";

		TestConnector.responseParameters["someOutputParameter"] = outputParamValue;

		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["someInputVariable"] = inputVariableValue;
		runtimeService.startProcessInstanceByKey("testProcess", vars);

		// validate input parameter
		assertNotNull(TestConnector.requestParameters["reqParam1"]);
		assertEquals(inputVariableValue, TestConnector.requestParameters["reqParam1"]);

		// validate connector output
		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("out1").singleResult();
		assertNotNull(variable);
		assertEquals(outputParamValue, variable.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConnectorWithScriptInputOutputMapping()
	  public virtual void testConnectorWithScriptInputOutputMapping()
	  {
		int x = 3;
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["x"] = x;
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// validate input parameter
		object @in = TestConnector.requestParameters["in"];
		assertNotNull(@in);
		assertEquals(2 * x, @in);

		// validate output parameter
		VariableInstance @out = runtimeService.createVariableInstanceQuery().variableName("out").singleResult();
		assertNotNull(@out);
		assertEquals(3 * x, @out.Value);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConnectorWithSetVariableInOutputMapping()
	  public virtual void testConnectorWithSetVariableInOutputMapping()
	  {
		// given process with set variable on connector in output mapping

		// when start process
		runtimeService.startProcessInstanceByKey("testProcess");

		// then variable x is set and no exception is thrown
		VariableInstance @out = runtimeService.createVariableInstanceQuery().variableName("x").singleResult();
		assertEquals(1, @out.Value);
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorWithThrownExceptionInScriptInputOutputMapping.bpmn")]
	  public virtual void testConnectorBpmnErrorThrownInScriptInputMappingIsHandledByBoundaryEvent()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "in";
		variables["exception"] = new BpmnError("error");
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		//we will only reach the user task if the BPMNError from the script was handled by the boundary event
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("User Task"));
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorWithThrownExceptionInScriptInputOutputMapping.bpmn")]
	  public virtual void testConnectorRuntimeExceptionThrownInScriptInputMappingIsNotHandledByBoundaryEvent()
	  {
		string exceptionMessage = "myException";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "in";
		variables["exception"] = new Exception(exceptionMessage);
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess", variables);
		}
		catch (Exception re)
		{
		  assertThat(re.Message, containsString(exceptionMessage));
		}
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorWithThrownExceptionInScriptInputOutputMapping.bpmn")]
	  public virtual void testConnectorBpmnErrorThrownInScriptOutputMappingIsHandledByBoundaryEvent()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "out";
		variables["exception"] = new BpmnError("error");
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		//we will only reach the user task if the BPMNError from the script was handled by the boundary event
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("User Task"));
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorWithThrownExceptionInScriptInputOutputMapping.bpmn")]
	  public virtual void testConnectorRuntimeExceptionThrownInScriptOutputMappingIsNotHandledByBoundaryEvent()
	  {
		string exceptionMessage = "myException";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "out";
		variables["exception"] = new Exception(exceptionMessage);
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess", variables);
		}
		catch (Exception re)
		{
		  assertThat(re.Message, containsString(exceptionMessage));
		}
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorWithThrownExceptionInScriptResourceInputOutputMapping.bpmn")]
	  public virtual void testConnectorBpmnErrorThrownInScriptResourceInputMappingIsHandledByBoundaryEvent()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "in";
		variables["exception"] = new BpmnError("error");
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		//we will only reach the user task if the BPMNError from the script was handled by the boundary event
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("User Task"));
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorWithThrownExceptionInScriptResourceInputOutputMapping.bpmn")]
	  public virtual void testConnectorRuntimeExceptionThrownInScriptResourceInputMappingIsNotHandledByBoundaryEvent()
	  {
		string exceptionMessage = "myException";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "in";
		variables["exception"] = new Exception(exceptionMessage);
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess", variables);
		}
		catch (Exception re)
		{
		  assertThat(re.Message, containsString(exceptionMessage));
		}
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorWithThrownExceptionInScriptResourceInputOutputMapping.bpmn")]
	  public virtual void testConnectorBpmnErrorThrownInScriptResourceOutputMappingIsHandledByBoundaryEvent()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "out";
		variables["exception"] = new BpmnError("error");
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		//we will only reach the user task if the BPMNError from the script was handled by the boundary event
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("User Task"));
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorWithThrownExceptionInScriptResourceInputOutputMapping.bpmn")]
	  public virtual void testConnectorRuntimeExceptionThrownInScriptResourceOutputMappingIsNotHandledByBoundaryEvent()
	  {
		string exceptionMessage = "myException";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "out";
		variables["exception"] = new Exception(exceptionMessage);
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess", variables);
		}
		catch (Exception re)
		{
		  assertThat(re.Message, containsString(exceptionMessage));
		}
	  }

	  [Deployment(resources:"org/camunda/connect/plugin/ConnectProcessEnginePluginTest.testConnectorBpmnErrorThrownInScriptResourceNoAsyncAfterJobIsCreated.bpmn")]
	  public virtual void testConnectorBpmnErrorThrownInScriptResourceNoAsyncAfterJobIsCreated()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "in";
		variables["exception"] = new BpmnError("error");

		// when
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// then
		// we will only reach the user task if the BPMNError from the script was handled by the boundary event
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("User Task"));

		// no job is created
		assertThat(Convert.ToInt64(managementService.createJobQuery().count()), @is(0l));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFollowingExceptionIsNotHandledByConnector()
	  public virtual void testFollowingExceptionIsNotHandledByConnector()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess");
		}
		catch (Exception re)
		{
		  assertThat(re.Message, containsString("Invalid format"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSendTaskWithConnector()
	  public virtual void testSendTaskWithConnector()
	  {
		string outputParamValue = "someSendTaskOutputValue";
		string inputVariableValue = "someSendTaskInputVariableValue";

		TestConnector.responseParameters["someOutputParameter"] = outputParamValue;

		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["someInputVariable"] = inputVariableValue;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process_sending_with_connector", vars);

		// validate input parameter
		assertNotNull(TestConnector.requestParameters["reqParam1"]);
		assertEquals(inputVariableValue, TestConnector.requestParameters["reqParam1"]);

		// validate connector output
		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("out1").singleResult();
		assertNotNull(variable);
		assertEquals(outputParamValue, variable.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIntermediateMessageThrowEventWithConnector()
	  public virtual void testIntermediateMessageThrowEventWithConnector()
	  {
		string outputParamValue = "someMessageThrowOutputValue";
		string inputVariableValue = "someMessageThrowInputVariableValue";

		TestConnector.responseParameters["someOutputParameter"] = outputParamValue;

		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["someInputVariable"] = inputVariableValue;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process_sending_with_connector", vars);

		// validate input parameter
		assertNotNull(TestConnector.requestParameters["reqParam1"]);
		assertEquals(inputVariableValue, TestConnector.requestParameters["reqParam1"]);

		// validate connector output
		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("out1").singleResult();
		assertNotNull(variable);
		assertEquals(outputParamValue, variable.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageEndEventWithConnector()
	  public virtual void testMessageEndEventWithConnector()
	  {
		string outputParamValue = "someMessageEndOutputValue";
		string inputVariableValue = "someMessageEndInputVariableValue";

		TestConnector.responseParameters["someOutputParameter"] = outputParamValue;

		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["someInputVariable"] = inputVariableValue;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process_sending_with_connector", vars);
		assertProcessEnded(processInstance.Id);

		// validate input parameter
		assertNotNull(TestConnector.requestParameters["reqParam1"]);
		assertEquals(inputVariableValue, TestConnector.requestParameters["reqParam1"]);

		// validate connector output
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().variableName("out1").singleResult();
		assertNotNull(variable);
		assertEquals(outputParamValue, variable.Value);
	  }

	}

}