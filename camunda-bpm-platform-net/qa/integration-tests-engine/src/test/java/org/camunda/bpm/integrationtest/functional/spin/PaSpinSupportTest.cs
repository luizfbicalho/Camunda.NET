using System.Collections;
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
namespace org.camunda.bpm.integrationtest.functional.spin
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.Spin.JSON;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.Spin.XML;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using SpinIoUtil = org.camunda.spin.impl.util.SpinIoUtil;
	using SpinJsonNode = org.camunda.spin.json.SpinJsonNode;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// <para>Smoketest Make sure camunda spin can be used in a process application </para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaSpinSupportTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PaSpinSupportTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{
		return initWebArchiveDeployment().addAsResource("org/camunda/bpm/integrationtest/oneTaskProcess.bpmn").addAsResource("org/camunda/bpm/integrationtest/functional/spin/jackson146.json");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void spinShouldBeAvailable()
	  public virtual void spinShouldBeAvailable()
	  {
		Assert.assertEquals("someXml", XML("<someXml />").xPath("/someXml").element().name());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void spinCanBeUsedForVariableSerialization()
	  public virtual void spinCanBeUsedForVariableSerialization()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", Variables.createVariables().putValue("serializedObject", serializedObjectValue("{\"foo\": \"bar\"}").serializationDataFormat("application/json").objectTypeName(typeof(Hashtable).FullName)));

		ObjectValue objectValue = runtimeService.getVariableTyped(pi.Id, "serializedObject", true);

		Dictionary<string, string> expected = new Dictionary<string, string>();
		expected["foo"] = "bar";

		Assert.assertEquals(expected, objectValue.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void spinPluginShouldBeRegistered()
	  public virtual void spinPluginShouldBeRegistered()
	  {

		IList<ProcessEnginePlugin> processEnginePlugins = processEngineConfiguration.ProcessEnginePlugins;

		bool spinPluginFound = false;

		foreach (ProcessEnginePlugin plugin in processEnginePlugins)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  if (plugin.GetType().FullName.Contains("Spin"))
		  {
			spinPluginFound = true;
			break;
		  }
		}

		Assert.assertTrue(spinPluginFound);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJacksonBug146()
	  public virtual void testJacksonBug146()
	  {
		Stream resourceAsStream = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/integrationtest/functional/spin/jackson146.json");
		string jackson146 = SpinIoUtil.inputStreamAsString(resourceAsStream);

		// this should not fail
		SpinJsonNode node = JSON(jackson146);

		// file has 4000 characters in length a
		// 20 characters per repeated JSON object
		assertEquals(200, node.prop("abcdef").elements().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJacksonBug146AsVariable()
	  public virtual void testJacksonBug146AsVariable()
	  {
		Stream resourceAsStream = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/integrationtest/functional/spin/jackson146.json");
		string jackson146 = SpinIoUtil.inputStreamAsString(resourceAsStream);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", Variables.createVariables().putValue("jackson146", serializedObjectValue(jackson146).serializationDataFormat("application/json").objectTypeName(typeof(Hashtable).FullName)));

		// file has 4000 characters in length a
		// 20 characters per repeated JSON object
		ObjectValue objectValue = runtimeService.getVariableTyped(pi.Id, "jackson146", true);
		Dictionary<string, IList<object>> map = (Dictionary<string, IList<object>>) objectValue.Value;

		assertEquals(200, map["abcdef"].Count);
	  }

	}

}