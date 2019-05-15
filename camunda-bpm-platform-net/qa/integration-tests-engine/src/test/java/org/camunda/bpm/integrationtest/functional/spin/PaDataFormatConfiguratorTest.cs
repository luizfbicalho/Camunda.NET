using System;

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
namespace org.camunda.bpm.integrationtest.functional.spin
{
	using JsonProcessingException = com.fasterxml.jackson.core.JsonProcessingException;
	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using ProcessApplicationContext = org.camunda.bpm.application.ProcessApplicationContext;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using ImplicitObjectValueUpdateHandler = org.camunda.bpm.integrationtest.functional.spin.dataformat.ImplicitObjectValueUpdateHandler;
	using JsonDataFormatConfigurator = org.camunda.bpm.integrationtest.functional.spin.dataformat.JsonDataFormatConfigurator;
	using JsonSerializable = org.camunda.bpm.integrationtest.functional.spin.dataformat.JsonSerializable;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using DataFormatConfigurator = org.camunda.spin.spi.DataFormatConfigurator;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaDataFormatConfiguratorTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PaDataFormatConfiguratorTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "PaDataFormatTest.war").addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(ReferenceStoringProcessApplication)).addAsResource("org/camunda/bpm/integrationtest/oneTaskProcess.bpmn").addAsResource("org/camunda/bpm/integrationtest/functional/spin/implicitProcessVariableUpdate.bpmn").addAsResource("org/camunda/bpm/integrationtest/functional/spin/implicitTaskVariableUpdate.bpmn").addClass(typeof(JsonSerializable)).addClass(typeof(ImplicitObjectValueUpdateHandler)).addClass(typeof(JsonDataFormatConfigurator)).addAsServiceProvider(typeof(DataFormatConfigurator), typeof(JsonDataFormatConfigurator));

		TestContainer.addSpinJacksonJsonDataFormat(webArchive);

		return webArchive;

		}

	  /// <summary>
	  /// Tests that the PA-local data format applies when a variable is set in
	  /// the context of it
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPaLocalFormatApplies() throws com.fasterxml.jackson.core.JsonProcessingException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPaLocalFormatApplies()
	  {

		// given a process instance
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// when setting a variable in the context of a process application
		DateTime date = new DateTime(JsonSerializable.ONE_DAY_IN_MILLIS * 10); // 10th of January 1970
		JsonSerializable jsonSerializable = new JsonSerializable(date);

		try
		{
		  ProcessApplicationContext.CurrentProcessApplication = ReferenceStoringProcessApplication.INSTANCE;
		  runtimeService.setVariable(pi.Id, "jsonSerializable", Variables.objectValue(jsonSerializable).serializationDataFormat(Variables.SerializationDataFormats.JSON).create());
		}
		finally
		{
		  ProcessApplicationContext.clear();
		}

		// then the process-application-local data format has been used to serialize the value
		ObjectValue objectValue = runtimeService.getVariableTyped(pi.Id, "jsonSerializable", false);

		string serializedValue = objectValue.ValueSerialized;
		string expectedSerializedValue = jsonSerializable.toExpectedJsonString(JsonDataFormatConfigurator.DATE_FORMAT);

		ObjectMapper objectMapper = new ObjectMapper();
		JsonNode actualJsonTree = objectMapper.readTree(serializedValue);
		JsonNode expectedJsonTree = objectMapper.readTree(expectedSerializedValue);
		// JsonNode#equals makes a deep comparison
		Assert.assertEquals(expectedJsonTree, actualJsonTree);
	  }

	  /// <summary>
	  /// Tests that the PA-local format does not apply if the value is set outside of the context
	  /// of the process application
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPaLocalFormatDoesNotApply() throws com.fasterxml.jackson.core.JsonProcessingException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPaLocalFormatDoesNotApply()
	  {

		// given a process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// when setting a variable without a process-application cotnext
		DateTime date = new DateTime(JsonSerializable.ONE_DAY_IN_MILLIS * 10); // 10th of January 1970
		JsonSerializable jsonSerializable = new JsonSerializable(date);

		runtimeService.setVariable(pi.Id, "jsonSerializable", Variables.objectValue(jsonSerializable).serializationDataFormat(Variables.SerializationDataFormats.JSON).create());

		// then the global data format is applied
		ObjectValue objectValue = runtimeService.getVariableTyped(pi.Id, "jsonSerializable", false);

		string serializedValue = objectValue.ValueSerialized;
		string expectedSerializedValue = jsonSerializable.toExpectedJsonString();

		ObjectMapper objectMapper = new ObjectMapper();
		JsonNode actualJsonTree = objectMapper.readTree(serializedValue);
		JsonNode expectedJsonTree = objectMapper.readTree(expectedSerializedValue);
		// JsonNode#equals makes a deep comparison
		Assert.assertEquals(expectedJsonTree, actualJsonTree);
	  }

	  /// <summary>
	  /// Tests that an implicit object value update happens in the context of the
	  /// process application.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutionVariableImplicitObjectValueUpdate() throws com.fasterxml.jackson.core.JsonProcessingException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testExecutionVariableImplicitObjectValueUpdate()
	  {

		// given a process instance and a task
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("implicitProcessVariableUpdate");

		// when setting a variable such that the process-application-local dataformat applies
		DateTime date = new DateTime(JsonSerializable.ONE_DAY_IN_MILLIS * 10); // 10th of January 1970
		JsonSerializable jsonSerializable = new JsonSerializable(date);
		try
		{

		  ProcessApplicationContext.CurrentProcessApplication = ReferenceStoringProcessApplication.INSTANCE;
		  runtimeService.setVariable(pi.Id, ImplicitObjectValueUpdateHandler.VARIABLE_NAME, Variables.objectValue(jsonSerializable).serializationDataFormat(Variables.SerializationDataFormats.JSON).create());
		}
		finally
		{
		  ProcessApplicationContext.clear();
		}

		// and triggering an implicit update of the object value variable
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		taskService.complete(task.Id);

		// then the process-application-local format was used for making the update
		ObjectValue objectValue = runtimeService.getVariableTyped(pi.Id, ImplicitObjectValueUpdateHandler.VARIABLE_NAME, false);

		ImplicitObjectValueUpdateHandler.addADay(jsonSerializable);
		string serializedValue = objectValue.ValueSerialized;
		string expectedSerializedValue = jsonSerializable.toExpectedJsonString(JsonDataFormatConfigurator.DATE_FORMAT);

		ObjectMapper objectMapper = new ObjectMapper();
		JsonNode actualJsonTree = objectMapper.readTree(serializedValue);
		JsonNode expectedJsonTree = objectMapper.readTree(expectedSerializedValue);
		// JsonNode#equals makes a deep comparison
		Assert.assertEquals(expectedJsonTree, actualJsonTree);

		// and it is also correct in the history
		HistoricVariableInstance historicObjectValue = historyService.createHistoricVariableInstanceQuery().processInstanceId(pi.Id).variableName(ImplicitObjectValueUpdateHandler.VARIABLE_NAME).disableCustomObjectDeserialization().singleResult();

		serializedValue = ((ObjectValue) historicObjectValue.TypedValue).ValueSerialized;
		actualJsonTree = objectMapper.readTree(serializedValue);
		Assert.assertEquals(expectedJsonTree, actualJsonTree);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskVariableImplicitObjectValueUpdate() throws com.fasterxml.jackson.core.JsonProcessingException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTaskVariableImplicitObjectValueUpdate()
	  {

		// given a process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("implicitTaskVariableUpdate");
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();

		// when setting a variable such that the process-application-local dataformat applies
		DateTime date = new DateTime(JsonSerializable.ONE_DAY_IN_MILLIS * 10); // 10th of January 1970
		JsonSerializable jsonSerializable = new JsonSerializable(date);
		try
		{
		  ProcessApplicationContext.CurrentProcessApplication = ReferenceStoringProcessApplication.INSTANCE;
		  taskService.setVariableLocal(task.Id, ImplicitObjectValueUpdateHandler.VARIABLE_NAME, Variables.objectValue(jsonSerializable).serializationDataFormat(Variables.SerializationDataFormats.JSON).create());
		}
		finally
		{
		  ProcessApplicationContext.clear();
		}

		// and triggering an implicit update of the object value variable
		taskService.setAssignee(task.Id, "foo");

		// then the process-application-local format was used for making the update
		ObjectValue objectValue = taskService.getVariableTyped(task.Id, ImplicitObjectValueUpdateHandler.VARIABLE_NAME, false);

		ImplicitObjectValueUpdateHandler.addADay(jsonSerializable);
		string serializedValue = objectValue.ValueSerialized;
		string expectedSerializedValue = jsonSerializable.toExpectedJsonString(JsonDataFormatConfigurator.DATE_FORMAT);

		ObjectMapper objectMapper = new ObjectMapper();
		JsonNode actualJsonTree = objectMapper.readTree(serializedValue);
		JsonNode expectedJsonTree = objectMapper.readTree(expectedSerializedValue);
		// JsonNode#equals makes a deep comparison
		Assert.assertEquals(expectedJsonTree, actualJsonTree);

		// and it is also correct in the history
		HistoricVariableInstance historicObjectValue = historyService.createHistoricVariableInstanceQuery().processInstanceId(pi.Id).variableName(ImplicitObjectValueUpdateHandler.VARIABLE_NAME).disableCustomObjectDeserialization().singleResult();

		serializedValue = ((ObjectValue) historicObjectValue.TypedValue).ValueSerialized;
		actualJsonTree = objectMapper.readTree(serializedValue);
		Assert.assertEquals(expectedJsonTree, actualJsonTree);
	  }
	}

}