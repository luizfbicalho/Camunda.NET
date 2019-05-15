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
namespace org.camunda.bpm.engine.test.bpmn.scripttask
{

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ExternalScriptTaskTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultExternalScript()
	  public virtual void testDefaultExternalScript()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultExternalScriptAsVariable()
	  public virtual void testDefaultExternalScriptAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "org/camunda/bpm/engine/test/bpmn/scripttask/greeting.py";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/scripttask/ExternalScriptTaskTest.testDefaultExternalScriptAsVariable.bpmn20.xml"})]
	  public virtual void testDefaultExternalScriptAsNonExistingVariable()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("process");
		  fail("Process variable 'scriptPath' not defined");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("Cannot resolve identifier 'scriptPath'", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultExternalScriptAsBean()
	  public virtual void testDefaultExternalScriptAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptResourceBean"] = new ScriptResourceBean();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptInClasspath()
	  public virtual void testScriptInClasspath()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptInClasspathAsVariable()
	  public virtual void testScriptInClasspathAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "classpath://org/camunda/bpm/engine/test/bpmn/scripttask/greeting.py";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptInClasspathAsBean()
	  public virtual void testScriptInClasspathAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptResourceBean"] = new ScriptResourceBean();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptNotFoundInClasspath()
	  public virtual void testScriptNotFoundInClasspath()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("process");
		  fail("Resource does not exist in classpath");
		}
		catch (NotFoundException e)
		{
		  assertTextPresentIgnoreCase("unable to find resource at path classpath://org/camunda/bpm/engine/test/bpmn/scripttask/notexisting.py", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/scripttask/ExternalScriptTaskTest.testScriptInDeployment.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/scripttask/greeting.py" })]
	  public virtual void testScriptInDeployment()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/scripttask/ExternalScriptTaskTest.testScriptInDeployment.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/scripttask/greeting.py" })]
	  public virtual void testScriptInDeploymentAfterCacheWasCleaned()
	  {
		processEngineConfiguration.DeploymentCache.discardProcessDefinitionCache();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/scripttask/ExternalScriptTaskTest.testScriptInDeploymentAsVariable.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/scripttask/greeting.py" })]
	  public virtual void testScriptInDeploymentAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "deployment://org/camunda/bpm/engine/test/bpmn/scripttask/greeting.py";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/scripttask/ExternalScriptTaskTest.testScriptInDeploymentAsBean.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/scripttask/greeting.py" })]
	  public virtual void testScriptInDeploymentAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptResourceBean"] = new ScriptResourceBean();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		string greeting = (string) runtimeService.getVariable(processInstance.Id, "greeting");
		assertNotNull(greeting);
		assertEquals("Greetings camunda BPM speaking", greeting);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptNotFoundInDeployment()
	  public virtual void testScriptNotFoundInDeployment()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("process");
		  fail("Resource does not exist in classpath");
		}
		catch (NotFoundException e)
		{
		  assertTextPresentIgnoreCase("unable to find resource at path deployment://org/camunda/bpm/engine/test/bpmn/scripttask/notexisting.py", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNotExistingImport()
	  public virtual void testNotExistingImport()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("process");
		  fail("Should fail during script compilation");
		}
		catch (ScriptCompilationException e)
		{
		  assertTextPresentIgnoreCase("import unknown", e.Message);
		}
	  }

	}

}