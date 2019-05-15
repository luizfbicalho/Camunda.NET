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
namespace org.camunda.spin.plugin.script
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpinScriptTaskSupportWithAutoStoreScriptVariablesTest : PluggableProcessEngineTestCase
	{

	  protected internal static string TEST_SCRIPT = "var_s = S('{}')\n" +
											"var_xml = XML('<root/>')\n" +
											"var_json = JSON('{}')\n";

	  protected internal ProcessInstance processInstance;

	  public virtual void setUp()
	  {
		processEngineConfiguration.AutoStoreScriptVariables = true;
	  }

	  public virtual void tearDown()
	  {
		processEngineConfiguration.AutoStoreScriptVariables = false;
	  }

	  public virtual void testSpinInternalVariablesNotExportedGroovyScriptTask()
	  {
		string importXML = "XML = org.camunda.spin.Spin.&XML\n";
		string importJSON = "JSON = org.camunda.spin.Spin.&JSON\n";

		string script = importXML + importJSON + TEST_SCRIPT;

		deployProcess("groovy", script);

		startProcess();
		checkVariables("foo", "var_s", "var_xml", "var_json");
		continueProcess();
		checkVariables("foo", "var_s", "var_xml", "var_json");
	  }

	  public virtual void testSpinInternalVariablesNotExportedByJavascriptScriptTask()
	  {
		string importXML = "var XML = org.camunda.spin.Spin.XML;\n";
		string importJSON = "var JSON = org.camunda.spin.Spin.JSON;\n";

		string script = importXML + importJSON + TEST_SCRIPT;

		deployProcess("javascript", script);

		startProcess();
		checkVariables("foo", "var_s", "var_xml", "var_json");
		continueProcess();
		checkVariables("foo", "var_s", "var_xml", "var_json");
	  }

	  public virtual void testSpinInternalVariablesNotExportedByPythonScriptTask()
	  {
		string importXML = "import org.camunda.spin.Spin.XML as XML;\n";
		string importJSON = "import org.camunda.spin.Spin.JSON as JSON;\n";

		string script = importXML + importJSON + TEST_SCRIPT;

		deployProcess("python", script);

		startProcess();
		checkVariables("foo", "var_s", "var_xml", "var_json");
		continueProcess();
		checkVariables("foo", "var_s", "var_xml", "var_json");
	  }

	  public virtual void testSpinInternalVariablesNotExportedByRubyScriptTask()
	  {
		string importXML = "def XML(*args)\n\torg.camunda.spin.Spin.XML(*args)\nend\n";
		string importJSON = "def JSON(*args)\n\torg.camunda.spin.Spin.JSON(*args)\nend\n";

		string script = importXML + importJSON + TEST_SCRIPT;

		deployProcess("ruby", script);

		startProcess();
		checkVariables("foo");
		continueProcess();
		checkVariables("foo");
	  }

	  protected internal virtual void startProcess()
	  {
		VariableMap variables = Variables.putValue("foo", "bar");
		processInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);
	  }

	  protected internal virtual void continueProcess()
	  {
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);
	  }

	  protected internal virtual void checkVariables(params string[] expectedVariables)
	  {
		IDictionary<string, object> variables = runtimeService.getVariables(processInstance.Id);

		assertFalse(variables.ContainsKey("S"));
		assertFalse(variables.ContainsKey("XML"));
		assertFalse(variables.ContainsKey("JSON"));

		foreach (string expectedVariable in expectedVariables)
		{
		  assertTrue(variables.ContainsKey(expectedVariable));
		}

		assertEquals(expectedVariables.Length, variables.Count);
	  }

	  protected internal virtual void deployProcess(string scriptFormat, string scriptText)
	  {
		BpmnModelInstance process = createProcess(scriptFormat, scriptText);
		Deployment deployment = repositoryService.createDeployment().addModelInstance("testProcess.bpmn", process).addString("testScript.txt", scriptText).deploy();
		deploymentId = deployment.Id;
	  }

	  protected internal virtual BpmnModelInstance createProcess(string scriptFormat, string scriptText)
	  {

		return Bpmn.createExecutableProcess("testProcess").startEvent().scriptTask().scriptFormat(scriptFormat).scriptText(scriptText).userTask().scriptTask().scriptFormat(scriptFormat).camundaResource("deployment://testScript.txt").userTask().endEvent().done();

	  }
	}

}