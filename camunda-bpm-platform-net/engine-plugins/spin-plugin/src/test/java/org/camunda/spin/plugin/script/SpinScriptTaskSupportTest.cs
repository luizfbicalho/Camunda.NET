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
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpinScriptTaskSupportTest : PluggableProcessEngineTestCase
	{

	  public virtual void testSpinAvailableInGroovy()
	  {
		deployProcess("groovy", "execution.setVariable('name',  S('<test />').name() )\n");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		string var = (string) runtimeService.getVariable(pi.Id, "name");
		assertEquals("test", var);
	  }

	  public virtual void testSpinAvailableInJavascript()
	  {
		deployProcess("javascript", "execution.setVariable('name',  S('<test />').name() )\n");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		string var = (string) runtimeService.getVariable(pi.Id, "name");
		assertEquals("test", var);
	  }

	  public virtual void testSpinAvailableInPython()
	  {
		deployProcess("python", "execution.setVariable('name',  S('<test />').name() )\n");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		string var = (string) runtimeService.getVariable(pi.Id, "name");
		assertEquals("test", var);
	  }

	  public virtual void testSpinAvailableInRuby()
	  {
		deployProcess("ruby", "$execution.setVariable('name',  S('<test />').name() )\n");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		string var = (string) runtimeService.getVariable(pi.Id, "name");
		assertEquals("test", var);
	  }

	  protected internal virtual void deployProcess(string scriptFormat, string scriptText)
	  {
		BpmnModelInstance process = createProcess(scriptFormat, scriptText);
		Deployment deployment = repositoryService.createDeployment().addModelInstance("testProcess.bpmn", process).deploy();
		deploymentId = deployment.Id;
	  }

	  protected internal virtual BpmnModelInstance createProcess(string scriptFormat, string scriptText)
	  {

		return Bpmn.createExecutableProcess("testProcess").startEvent().scriptTask().scriptFormat(scriptFormat).scriptText(scriptText).userTask().endEvent().done();

	  }
	}

}