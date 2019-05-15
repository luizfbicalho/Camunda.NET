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
	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using ProcessApplicationContext = org.camunda.bpm.application.ProcessApplicationContext;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using JsonDataFormatConfigurator = org.camunda.bpm.integrationtest.functional.spin.dataformat.JsonDataFormatConfigurator;
	using JsonSerializable = org.camunda.bpm.integrationtest.functional.spin.dataformat.JsonSerializable;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using DataFormatConfigurator = org.camunda.spin.spi.DataFormatConfigurator;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.application.ProcessApplicationContext.withProcessApplicationContext;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaContextSwitchTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PaContextSwitchTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name : "pa1")]
		public static WebArchive createDeployment1()
		{
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "pa1.war").addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(ProcessApplication1)).addClass(typeof(JsonSerializable)).addClass(typeof(RuntimeServiceDelegate)).addAsResource("org/camunda/bpm/integrationtest/functional/spin/paContextSwitch.bpmn20.xml").addClass(typeof(JsonDataFormatConfigurator)).addAsServiceProvider(typeof(DataFormatConfigurator), typeof(JsonDataFormatConfigurator));

		TestContainer.addSpinJacksonJsonDataFormat(webArchive);

		return webArchive;
		}

	  [Deployment(name : "pa2")]
	  public static WebArchive createDeployment2()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "pa2.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(ProcessApplication2));

		return webArchive;
	  }

	  /// <summary>
	  /// This test ensures that when the <seealso cref="ProcessApplicationContext"/> API is used,
	  /// the context switch is only performed for outer-most command and not if a second, nested
	  /// command is executed; => in nested commands, the engine is already in the correct context
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("pa1") public void testNoContextSwitchOnInnerCommand() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testNoContextSwitchOnInnerCommand()
	  {

		ProcessInstance pi = withProcessApplicationContext(new CallableAnonymousInnerClass(this)
	   , "pa2");

		JsonSerializable expectedJsonSerializable = RuntimeServiceDelegate.createJsonSerializable();
		string expectedJsonString = expectedJsonSerializable.toExpectedJsonString(JsonDataFormatConfigurator.DATE_FORMAT);

		SerializableValue serializedValue = runtimeService.getVariableTyped(pi.Id, RuntimeServiceDelegate.VARIABLE_NAME, false);
		string actualJsonString = serializedValue.ValueSerialized;

		ObjectMapper objectMapper = new ObjectMapper();
		JsonNode actualJsonTree = objectMapper.readTree(actualJsonString);
		JsonNode expectedJsonTree = objectMapper.readTree(expectedJsonString);
		// JsonNode#equals makes a deep comparison
		Assert.assertEquals(expectedJsonTree, actualJsonTree);

	  }

	  private class CallableAnonymousInnerClass : Callable<ProcessInstance>
	  {
		  private readonly PaContextSwitchTest outerInstance;

		  public CallableAnonymousInnerClass(PaContextSwitchTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.runtime.ProcessInstance call() throws Exception
		  public override ProcessInstance call()
		  {
			return outerInstance.runtimeService.startProcessInstanceByKey("process");
		  }

	  }
	}

}