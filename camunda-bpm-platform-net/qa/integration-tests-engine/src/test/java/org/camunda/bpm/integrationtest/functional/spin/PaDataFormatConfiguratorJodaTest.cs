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
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using JodaJsonDataFormatConfigurator = org.camunda.bpm.integrationtest.functional.spin.dataformat.JodaJsonDataFormatConfigurator;
	using JodaJsonSerializable = org.camunda.bpm.integrationtest.functional.spin.dataformat.JodaJsonSerializable;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using DataFormatConfigurator = org.camunda.spin.spi.DataFormatConfigurator;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using DateTime = org.joda.time.DateTime;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaDataFormatConfiguratorJodaTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PaDataFormatConfiguratorJodaTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "PaDataFormatTest.war").addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(ReferenceStoringProcessApplication)).addAsResource("org/camunda/bpm/integrationtest/oneTaskProcess.bpmn").addClass(typeof(JodaJsonSerializable)).addClass(typeof(JodaJsonDataFormatConfigurator)).addAsServiceProvider(typeof(DataFormatConfigurator), typeof(JodaJsonDataFormatConfigurator));

		TestContainer.addSpinJacksonJsonDataFormat(webArchive);
		TestContainer.addJodaTimeJacksonModule(webArchive);

		return webArchive;

		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPaLocalJodaConfiguration() throws com.fasterxml.jackson.core.JsonProcessingException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPaLocalJodaConfiguration()
	  {
		// given a process instance
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// when setting a variable in the context of a process application
		DateTime date = new DateTime(JodaJsonSerializable.ONE_DAY_IN_MILLIS * 10); // 10th of January 1970
		JodaJsonSerializable jsonSerializable = new JodaJsonSerializable(new DateTime(date.Ticks));

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
		string expectedSerializedValue = jsonSerializable.toExpectedJsonString();

		ObjectMapper objectMapper = new ObjectMapper();
		JsonNode actualJsonTree = objectMapper.readTree(serializedValue);
		JsonNode expectedJsonTree = objectMapper.readTree(expectedSerializedValue);
		// JsonNode#equals makes a deep comparison
		Assert.assertEquals(expectedJsonTree, actualJsonTree);
	  }

	}

}