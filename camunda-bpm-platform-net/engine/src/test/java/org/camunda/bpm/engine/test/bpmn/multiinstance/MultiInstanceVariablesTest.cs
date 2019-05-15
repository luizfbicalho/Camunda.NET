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
namespace org.camunda.bpm.engine.test.bpmn.multiinstance
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior.NUMBER_OF_INSTANCES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Task = org.camunda.bpm.engine.task.Task;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CallActivityBuilder = org.camunda.bpm.model.bpmn.builder.CallActivityBuilder;
	using CallActivity = org.camunda.bpm.model.bpmn.instance.CallActivity;
	using CamundaIn = org.camunda.bpm.model.bpmn.instance.camunda.CamundaIn;
	using CamundaOut = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOut;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class MultiInstanceVariablesTest
	{

	  public const string ALL = "all";
	  public const string SUB_PROCESS_ID = "testProcess";
	  public const string PROCESS_ID = "process";
	  public const string CALL_ACTIVITY = "callActivity";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultiInstanceWithAllInOutMapping()
	  public virtual void testMultiInstanceWithAllInOutMapping()
	  {
		BpmnModelInstance modelInstance = BpmnModelInstance;

		CallActivityBuilder callActivityBuilder = ((CallActivity) modelInstance.getModelElementById(CALL_ACTIVITY)).builder();

		addAllIn(modelInstance, callActivityBuilder);

		addAllOut(modelInstance, callActivityBuilder);

		BpmnModelInstance testProcess = BpmnSubProcessModelInstance;

		deployAndStartProcess(modelInstance, testProcess);
		assertThat(engineRule.RuntimeService.createExecutionQuery().processDefinitionKey(SUB_PROCESS_ID).list().size(),@is(2));

		IList<Task> tasks = engineRule.TaskService.createTaskQuery().active().list();
		foreach (Task task in tasks)
		{
		  engineRule.TaskService.setVariable(task.Id,NUMBER_OF_INSTANCES,"3");
		  engineRule.TaskService.complete(task.Id);
		}

		assertThat(engineRule.RuntimeService.createExecutionQuery().processDefinitionKey(SUB_PROCESS_ID).list().size(),@is(0));
		assertThat(engineRule.RuntimeService.createExecutionQuery().activityId(CALL_ACTIVITY).list().size(),@is(0));
	  }

	  protected internal virtual void addAllOut(BpmnModelInstance modelInstance, CallActivityBuilder callActivityBuilder)
	  {
		CamundaOut camundaOut = modelInstance.newInstance(typeof(CamundaOut));
		camundaOut.CamundaVariables = ALL;
		callActivityBuilder.addExtensionElement(camundaOut);
	  }

	  protected internal virtual void addAllIn(BpmnModelInstance modelInstance, CallActivityBuilder callActivityBuilder)
	  {
		CamundaIn camundaIn = modelInstance.newInstance(typeof(CamundaIn));
		camundaIn.CamundaVariables = ALL;
		callActivityBuilder.addExtensionElement(camundaIn);
	  }

	  protected internal virtual void deployAndStartProcess(BpmnModelInstance modelInstance, BpmnModelInstance testProcess)
	  {
		engineRule.manageDeployment(engineRule.RepositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy());
		engineRule.manageDeployment(engineRule.RepositoryService.createDeployment().addModelInstance("testProcess.bpmn", testProcess).deploy());
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_ID);
	  }

	  protected internal virtual BpmnModelInstance BpmnModelInstance
	  {
		  get
		  {
			return Bpmn.createExecutableProcess(PROCESS_ID).startEvent().callActivity(CALL_ACTIVITY).calledElement(SUB_PROCESS_ID).multiInstance().cardinality("2").multiInstanceDone().endEvent().done();
		  }
	  }

	  protected internal virtual BpmnModelInstance BpmnSubProcessModelInstance
	  {
		  get
		  {
			return Bpmn.createExecutableProcess(SUB_PROCESS_ID).startEvent().userTask("userTask").endEvent().done();
		  }
	  }

	}

}