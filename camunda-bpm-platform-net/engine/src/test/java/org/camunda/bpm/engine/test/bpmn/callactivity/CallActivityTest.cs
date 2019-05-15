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
namespace org.camunda.bpm.engine.test.bpmn.callactivity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CallActivityBuilder = org.camunda.bpm.model.bpmn.builder.CallActivityBuilder;
	using CallActivity = org.camunda.bpm.model.bpmn.instance.CallActivity;
	using CamundaIn = org.camunda.bpm.model.bpmn.instance.camunda.CamundaIn;
	using CamundaOut = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOut;

	/// <summary>
	/// @author Joram Barrez
	/// @author Nils Preusker
	/// @author Bernd Ruecker
	/// @author Falko Menge
	/// </summary>
	public class CallActivityTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testCallSimpleSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSimpleSubProcess");

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		// Completing the task continues the process which leads to calling the subprocess
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		// Completing the task in the subprocess, finishes the subprocess
		taskService.complete(taskInSubProcess.Id);
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// Completing this task end the process instance
		taskService.complete(taskAfterSubProcess.Id);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcessParentVariableAccess.bpmn20.xml" })]
	  public virtual void testAccessSuperInstanceVariables()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSimpleSubProcess");

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		// the variable does not yet exist
		assertNull(runtimeService.getVariable(processInstance.Id, "greeting"));

		// completing the task executed the sub process
		taskService.complete(taskBeforeSubProcess.Id);

		// now the variable exists
		assertEquals("hello", runtimeService.getVariable(processInstance.Id, "greeting"));

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/concurrentSubProcessParentVariableAccess.bpmn20.xml" })]
	  public virtual void testAccessSuperInstanceVariablesFromConcurrentExecution()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSimpleSubProcess");

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		// the variable does not yet exist
		assertNull(runtimeService.getVariable(processInstance.Id, "greeting"));

		// completing the task executed the sub process
		taskService.complete(taskBeforeSubProcess.Id);

		// now the variable exists
		assertEquals("hello", runtimeService.getVariable(processInstance.Id, "greeting"));

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcessWithExpressions.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testCallSimpleSubProcessWithExpressions()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSimpleSubProcess");

		// one task in the subprocess should be active after starting the process
		// instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		// Completing the task continues the process which leads to calling the
		// subprocess. The sub process we want to call is passed in as a variable
		// into this task
		taskService.setVariable(taskBeforeSubProcess.Id, "simpleSubProcessExpression", "simpleSubProcess");
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		// Completing the task in the subprocess, finishes the subprocess
		taskService.complete(taskInSubProcess.Id);
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// Completing this task end the process instance
		taskService.complete(taskAfterSubProcess.Id);
		assertProcessEnded(processInstance.Id);
	  }

	  /// <summary>
	  /// Test case for a possible tricky case: reaching the end event of the
	  /// subprocess leads to an end event in the super process instance.
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessEndsSuperProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessEndsSuperProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessEndsSuperProcess");

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskBeforeSubProcess.Name);

		// Completing this task ends the subprocess which leads to the end of the whole process instance
		taskService.complete(taskBeforeSubProcess.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallParallelSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleParallelSubProcess.bpmn20.xml"})]
	  public virtual void testCallParallelSubProcess()
	  {
		runtimeService.startProcessInstanceByKey("callParallelSubProcess");

		// The two tasks in the parallel subprocess should be active
		TaskQuery taskQuery = taskService.createTaskQuery().orderByTaskName().asc();
		IList<Task> tasks = taskQuery.list();
		assertEquals(2, tasks.Count);

		Task taskA = tasks[0];
		Task taskB = tasks[1];
		assertEquals("Task A", taskA.Name);
		assertEquals("Task B", taskB.Name);

		// Completing the first task should not end the subprocess
		taskService.complete(taskA.Id);
		assertEquals(1, taskQuery.list().size());

		// Completing the second task should end the subprocess and end the whole process instance
		taskService.complete(taskB.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSequentialSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcessWithExpressions.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess2.bpmn20.xml"})]
	  public virtual void testCallSequentialSubProcessWithExpressions()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSequentialSubProcess");

		// FIRST sub process calls simpleSubProcess
		// one task in the subprocess should be active after starting the process
		// instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		// Completing the task continues the process which leads to calling the
		// subprocess. The sub process we want to call is passed in as a variable
		// into this task
		taskService.setVariable(taskBeforeSubProcess.Id, "simpleSubProcessExpression", "simpleSubProcess");
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		// Completing the task in the subprocess, finishes the subprocess
		taskService.complete(taskInSubProcess.Id);
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// Completing this task end the process instance
		taskService.complete(taskAfterSubProcess.Id);

		// SECOND sub process calls simpleSubProcess2
		// one task in the subprocess should be active after starting the process
		// instance
		taskQuery = taskService.createTaskQuery();
		taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		// Completing the task continues the process which leads to calling the
		// subprocess. The sub process we want to call is passed in as a variable
		// into this task
		taskService.setVariable(taskBeforeSubProcess.Id, "simpleSubProcessExpression", "simpleSubProcess2");
		taskService.complete(taskBeforeSubProcess.Id);
		taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess 2", taskInSubProcess.Name);

		// Completing the task in the subprocess, finishes the subprocess
		taskService.complete(taskInSubProcess.Id);
		taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// Completing this task end the process instance
		taskService.complete(taskAfterSubProcess.Id);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testTimerOnCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testTimerOnCallActivity()
	  {
		// After process start, the task in the subprocess should be active
		runtimeService.startProcessInstanceByKey("timerOnCallActivity");
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		Job timer = managementService.createJobQuery().singleResult();
		assertNotNull(timer);

		managementService.executeJob(timer.Id);

		Task escalatedTask = taskQuery.singleResult();
		assertEquals("Escalated Task", escalatedTask.Name);

		// Completing the task ends the complete process
		taskService.complete(escalatedTask.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

	  /// <summary>
	  /// Test case for handing over process variables to a sub process
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessDataInputOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessWithDataInputOutput()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["superVariable"] = "Hello from the super process.";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessDataInputOutput", vars);

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskBeforeSubProcess.Name);
		assertEquals("Hello from the super process.", runtimeService.getVariable(taskBeforeSubProcess.ProcessInstanceId, "subVariable"));
		assertEquals("Hello from the super process.", taskService.getVariable(taskBeforeSubProcess.Id, "subVariable"));

		runtimeService.setVariable(taskBeforeSubProcess.ProcessInstanceId, "subVariable", "Hello from sub process.");

		// super variable is unchanged
		assertEquals("Hello from the super process.", runtimeService.getVariable(processInstance.Id, "superVariable"));

		// Completing this task ends the subprocess which leads to a task in the super process
		taskService.complete(taskBeforeSubProcess.Id);

		// one task in the subprocess should be active after starting the process instance
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task in super process", taskAfterSubProcess.Name);
		assertEquals("Hello from sub process.", runtimeService.getVariable(processInstance.Id, "superVariable"));
		assertEquals("Hello from sub process.", taskService.getVariable(taskAfterSubProcess.Id, "superVariable"));

		vars.Clear();
		vars["x"] = new long?(5);

		// Completing this task ends the super process which leads to a task in the super process
		taskService.complete(taskAfterSubProcess.Id, vars);

		// now we are the second time in the sub process but passed variables via expressions
		Task taskInSecondSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSecondSubProcess.Name);
		assertEquals(10l, runtimeService.getVariable(taskInSecondSubProcess.ProcessInstanceId, "y"));
		assertEquals(10l, taskService.getVariable(taskInSecondSubProcess.Id, "y"));

		// Completing this task ends the subprocess which leads to a task in the super process
		taskService.complete(taskInSecondSubProcess.Id);

		// one task in the subprocess should be active after starting the process instance
		Task taskAfterSecondSubProcess = taskQuery.singleResult();
		assertEquals("Task in super process", taskAfterSecondSubProcess.Name);
		assertEquals(15l, runtimeService.getVariable(taskAfterSecondSubProcess.ProcessInstanceId, "z"));
		assertEquals(15l, taskService.getVariable(taskAfterSecondSubProcess.Id, "z"));

		// and end last task in Super process
		taskService.complete(taskAfterSecondSubProcess.Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

	  /// <summary>
	  /// Test case for handing over process variables to a sub process via the typed
	  /// api and passing only certain variables
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessLimitedDataInputOutputTypedApi.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessWithLimitedDataInputOutputTypedApi()
	  {

		TypedValue superVariable = Variables.stringValue(null);
		VariableMap vars = Variables.createVariables();
		vars.putValueTyped("superVariable", superVariable);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessDataInputOutput", vars);

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskInSubProcess = taskQuery.singleResult();
		assertThat(taskInSubProcess.Name, @is("Task in subprocess"));
		assertThat(runtimeService.getVariableTyped(taskInSubProcess.ProcessInstanceId, "subVariable"), @is(superVariable));
		assertThat(taskService.getVariableTyped(taskInSubProcess.Id, "subVariable"), @is(superVariable));

		TypedValue subVariable = Variables.stringValue(null);
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "subVariable", subVariable);

		// super variable is unchanged
		assertThat(runtimeService.getVariableTyped(processInstance.Id, "superVariable"), @is(superVariable));

		// Completing this task ends the subprocess which leads to a task in the super process
		taskService.complete(taskInSubProcess.Id);

		Task taskAfterSubProcess = taskQuery.singleResult();
		assertThat(taskAfterSubProcess.Name, @is("Task in super process"));
		assertThat(runtimeService.getVariableTyped(processInstance.Id, "superVariable"), @is(subVariable));
		assertThat(taskService.getVariableTyped(taskAfterSubProcess.Id, "superVariable"), @is(subVariable));

		// Completing this task ends the super process which leads to a task in the super process
		taskService.complete(taskAfterSubProcess.Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

	  /// <summary>
	  /// Test case for handing over process variables to a sub process via the typed
	  /// api and passing all variables
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessAllDataInputOutputTypedApi.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessWithAllDataInputOutputTypedApi()
	  {

		TypedValue superVariable = Variables.stringValue(null);
		VariableMap vars = Variables.createVariables();
		vars.putValueTyped("superVariable", superVariable);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessDataInputOutput", vars);

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskInSubProcess = taskQuery.singleResult();
		assertThat(taskInSubProcess.Name, @is("Task in subprocess"));
		assertThat(runtimeService.getVariableTyped(taskInSubProcess.ProcessInstanceId, "superVariable"), @is(superVariable));
		assertThat(taskService.getVariableTyped(taskInSubProcess.Id, "superVariable"), @is(superVariable));

		TypedValue subVariable = Variables.stringValue(null);
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "subVariable", subVariable);

		// Completing this task ends the subprocess which leads to a task in the super process
		taskService.complete(taskInSubProcess.Id);

		Task taskAfterSubProcess = taskQuery.singleResult();
		assertThat(taskAfterSubProcess.Name, @is("Task in super process"));
		assertThat(runtimeService.getVariableTyped(processInstance.Id, "subVariable"), @is(subVariable));
		assertThat(taskService.getVariableTyped(taskAfterSubProcess.Id, "superVariable"), @is(superVariable));

		// Completing this task ends the super process which leads to a task in the super process
		taskService.complete(taskAfterSubProcess.Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

	  /// <summary>
	  /// Test case for handing over process variables without target attribute set
	  /// </summary>
	  public virtual void testSubProcessWithDataInputOutputWithoutTarget()
	  {
		string processId = "subProcessDataInputOutputWithoutTarget";

		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(processId).startEvent().callActivity("callActivity").calledElement("simpleSubProcess").userTask().endEvent().done();

		CallActivityBuilder callActivityBuilder = ((CallActivity) modelInstance.getModelElementById("callActivity")).builder();

		// create camunda:in with source but without target
		CamundaIn camundaIn = modelInstance.newInstance(typeof(CamundaIn));
		camundaIn.CamundaSource = "superVariable";
		callActivityBuilder.addExtensionElement(camundaIn);

		deployAndExpectException(modelInstance);
		// set target
		camundaIn.CamundaTarget = "subVariable";

		// create camunda:in with sourceExpression but without target
		camundaIn = modelInstance.newInstance(typeof(CamundaIn));
		camundaIn.CamundaSourceExpression = "${x+5}";
		callActivityBuilder.addExtensionElement(camundaIn);

		deployAndExpectException(modelInstance);
		// set target
		camundaIn.CamundaTarget = "subVariable2";

		// create camunda:out with source but without target
		CamundaOut camundaOut = modelInstance.newInstance(typeof(CamundaOut));
		camundaOut.CamundaSource = "subVariable";
		callActivityBuilder.addExtensionElement(camundaOut);

		deployAndExpectException(modelInstance);
		// set target
		camundaOut.CamundaTarget = "superVariable";

		// create camunda:out with sourceExpression but without target
		camundaOut = modelInstance.newInstance(typeof(CamundaOut));
		camundaOut.CamundaSourceExpression = "${y+1}";
		callActivityBuilder.addExtensionElement(camundaOut);

		deployAndExpectException(modelInstance);
		// set target
		camundaOut.CamundaTarget = "superVariable2";

		try
		{
		  string deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;
		  repositoryService.deleteDeployment(deploymentId, true);
		}
		catch (ProcessEngineException)
		{
		  fail("No exception expected");
		}
	  }

	  /// <summary>
	  /// Test case for handing over a null process variables to a sub process
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessDataInputOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/dataSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessWithNullDataInput()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("subProcessDataInputOutput").Id;

		// the variable named "subVariable" is not set on process instance
		VariableInstance variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("subVariable").singleResult();
		assertNull(variable);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("superVariable").singleResult();
		assertNull(variable);

		// the sub process instance is in the task
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("Task in subprocess", task.Name);

		// the value of "subVariable" is null
		assertNull(taskService.getVariable(task.Id, "subVariable"));

		string subProcessInstanceId = task.ProcessInstanceId;
		assertFalse(processInstanceId.Equals(subProcessInstanceId));

		// the variable "subVariable" is set on the sub process instance
		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(subProcessInstanceId).variableName("subVariable").singleResult();

		assertNotNull(variable);
		assertNull(variable.Value);
		assertEquals("subVariable", variable.Name);
	  }

	  /// <summary>
	  /// Test case for handing over a null process variables to a sub process
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessDataInputOutputAsExpression.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/dataSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessWithNullDataInputAsExpression()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["superVariable"] = null;
		string processInstanceId = runtimeService.startProcessInstanceByKey("subProcessDataInputOutput", @params).Id;

		// the variable named "subVariable" is not set on process instance
		VariableInstance variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("subVariable").singleResult();
		assertNull(variable);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("superVariable").singleResult();
		assertNotNull(variable);
		assertNull(variable.Value);

		// the sub process instance is in the task
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("Task in subprocess", task.Name);

		// the value of "subVariable" is null
		assertNull(taskService.getVariable(task.Id, "subVariable"));

		string subProcessInstanceId = task.ProcessInstanceId;
		assertFalse(processInstanceId.Equals(subProcessInstanceId));

		// the variable "subVariable" is set on the sub process instance
		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(subProcessInstanceId).variableName("subVariable").singleResult();

		assertNotNull(variable);
		assertNull(variable.Value);
		assertEquals("subVariable", variable.Name);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessDataInputOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/dataSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessWithNullDataOutput()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("subProcessDataInputOutput").Id;

		// the variable named "subVariable" is not set on process instance
		VariableInstance variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("subVariable").singleResult();
		assertNull(variable);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("superVariable").singleResult();
		assertNull(variable);

		// the sub process instance is in the task
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("Task in subprocess", task.Name);

		taskService.complete(task.Id);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("subVariable").singleResult();
		assertNull(variable);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("superVariable").singleResult();
		assertNotNull(variable);
		assertNull(variable.Value);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("hisLocalVariable").singleResult();
		assertNotNull(variable);
		assertNull(variable.Value);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessDataInputOutputAsExpression.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/dataSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessWithNullDataOutputAsExpression()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["superVariable"] = null;
		string processInstanceId = runtimeService.startProcessInstanceByKey("subProcessDataInputOutput", @params).Id;

		// the variable named "subVariable" is not set on process instance
		VariableInstance variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("subVariable").singleResult();
		assertNull(variable);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("superVariable").singleResult();
		assertNotNull(variable);
		assertNull(variable.Value);

		// the sub process instance is in the task
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("Task in subprocess", task.Name);

		VariableMap variables = Variables.createVariables().putValue("myLocalVariable", null);
		taskService.complete(task.Id, variables);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("subVariable").singleResult();
		assertNull(variable);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("superVariable").singleResult();
		assertNotNull(variable);
		assertNull(variable.Value);

		variable = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("hisLocalVariable").singleResult();
		assertNotNull(variable);
		assertNull(variable.Value);

	  }

	  private void deployAndExpectException(BpmnModelInstance modelInstance)
	  {
		string deploymentId = null;
		try
		{
		  deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Missing attribute 'target'", e.Message);
		}
		finally
		{
		  if (!string.ReferenceEquals(deploymentId, null))
		  {
			repositoryService.deleteDeployment(deploymentId, true);
		  }
		}
	  }

	  /// <summary>
	  /// Test case for handing over process variables to a sub process
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testTwoSubProcesses.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testTwoSubProcesses()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callTwoSubProcesses");

		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(3, instanceList.Count);

		IList<Task> taskList = taskService.createTaskQuery().list();
		assertNotNull(taskList);
		assertEquals(2, taskList.Count);

		runtimeService.deleteProcessInstance(processInstance.Id, "Test cascading");

		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(0, instanceList.Count);

		taskList = taskService.createTaskQuery().list();
		assertNotNull(taskList);
		assertEquals(0, taskList.Count);
	  }

	  /// <summary>
	  /// Test case for handing all over process variables to a sub process
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessAllDataInputOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessAllDataInputOutput()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["superVariable"] = "Hello from the super process.";
		vars["testVariable"] = "Only a test.";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessAllDataInputOutput", vars);

		// one task in the super process should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);
		assertEquals("Hello from the super process.", runtimeService.getVariable(taskBeforeSubProcess.ProcessInstanceId, "superVariable"));
		assertEquals("Hello from the super process.", taskService.getVariable(taskBeforeSubProcess.Id, "superVariable"));
		assertEquals("Only a test.", runtimeService.getVariable(taskBeforeSubProcess.ProcessInstanceId, "testVariable"));
		assertEquals("Only a test.", taskService.getVariable(taskBeforeSubProcess.Id, "testVariable"));

		taskService.complete(taskBeforeSubProcess.Id);

		// one task in sub process should be active after starting sub process instance
		taskQuery = taskService.createTaskQuery();
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);
		assertEquals("Hello from the super process.", runtimeService.getVariable(taskInSubProcess.ProcessInstanceId, "superVariable"));
		assertEquals("Hello from the super process.", taskService.getVariable(taskInSubProcess.Id, "superVariable"));
		assertEquals("Only a test.", runtimeService.getVariable(taskInSubProcess.ProcessInstanceId, "testVariable"));
		assertEquals("Only a test.", taskService.getVariable(taskInSubProcess.Id, "testVariable"));

		// changed variables in sub process
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "superVariable", "Hello from sub process.");
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "testVariable", "Variable changed in sub process.");

		taskService.complete(taskInSubProcess.Id);

		// task after sub process in super process
		taskQuery = taskService.createTaskQuery();
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// variables are changed after finished sub process
		assertEquals("Hello from sub process.", runtimeService.getVariable(processInstance.Id, "superVariable"));
		assertEquals("Variable changed in sub process.", runtimeService.getVariable(processInstance.Id, "testVariable"));

		taskService.complete(taskAfterSubProcess.Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

	  /// <summary>
	  /// Test case for handing all over process variables to a sub process
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessAllDataInputOutputWithAdditionalInputMapping.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessAllDataInputOutputWithAdditionalInputMapping()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["superVariable"] = "Hello from the super process.";
		vars["testVariable"] = "Only a test.";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessAllDataInputOutput", vars);

		// one task in the super process should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);
		assertEquals("Hello from the super process.", runtimeService.getVariable(taskBeforeSubProcess.ProcessInstanceId, "superVariable"));
		assertEquals("Hello from the super process.", taskService.getVariable(taskBeforeSubProcess.Id, "superVariable"));
		assertEquals("Only a test.", runtimeService.getVariable(taskBeforeSubProcess.ProcessInstanceId, "testVariable"));
		assertEquals("Only a test.", taskService.getVariable(taskBeforeSubProcess.Id, "testVariable"));

		taskService.complete(taskBeforeSubProcess.Id);

		// one task in sub process should be active after starting sub process instance
		taskQuery = taskService.createTaskQuery();
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);
		assertEquals("Hello from the super process.", runtimeService.getVariable(taskInSubProcess.ProcessInstanceId, "superVariable"));
		assertEquals("Hello from the super process.", runtimeService.getVariable(taskInSubProcess.ProcessInstanceId, "subVariable"));
		assertEquals("Hello from the super process.", taskService.getVariable(taskInSubProcess.Id, "superVariable"));
		assertEquals("Only a test.", runtimeService.getVariable(taskInSubProcess.ProcessInstanceId, "testVariable"));
		assertEquals("Only a test.", taskService.getVariable(taskInSubProcess.Id, "testVariable"));

		// changed variables in sub process
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "superVariable", "Hello from sub process.");
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "testVariable", "Variable changed in sub process.");

		taskService.complete(taskInSubProcess.Id);

		// task after sub process in super process
		taskQuery = taskService.createTaskQuery();
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// variables are changed after finished sub process
		assertEquals("Hello from sub process.", runtimeService.getVariable(processInstance.Id, "superVariable"));
		assertEquals("Variable changed in sub process.", runtimeService.getVariable(processInstance.Id, "testVariable"));

		taskService.complete(taskAfterSubProcess.Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

	  /// <summary>
	  /// This testcase verifies that <camunda:out variables="all" /> works also in
	  /// case super process has no variables
	  /// 
	  /// https://app.camunda.com/jira/browse/CAM-1617
	  /// 
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessAllDataInputOutput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessAllDataOutput()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessAllDataInputOutput");

		// one task in the super process should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		taskService.complete(taskBeforeSubProcess.Id);

		// one task in sub process should be active after starting sub process instance
		taskQuery = taskService.createTaskQuery();
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		// add variables to sub process
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "superVariable", "Hello from sub process.");
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "testVariable", "Variable changed in sub process.");

		taskService.complete(taskInSubProcess.Id);

		// task after sub process in super process
		taskQuery = taskService.createTaskQuery();
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// variables are copied to super process instance after sub process instance finishes
		assertEquals("Hello from sub process.", runtimeService.getVariable(processInstance.Id, "superVariable"));
		assertEquals("Variable changed in sub process.", runtimeService.getVariable(processInstance.Id, "testVariable"));

		taskService.complete(taskAfterSubProcess.Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessLocalInputAllVariables.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessLocalInputAllVariables()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessLocalInputAllVariables");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when setting a variable in a process instance
		runtimeService.setVariable(processInstance.Id, "callingProcessVar1", "val1");

		// and executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then only the local variable specified in the io mapping is passed to the called instance
		ProcessInstance calledInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();

		IDictionary<string, object> calledInstanceVariables = runtimeService.getVariables(calledInstance.Id);
		assertEquals(1, calledInstanceVariables.Count);
		assertEquals("val2", calledInstanceVariables["inputParameter"]);

		// when setting a variable in the called process instance
		runtimeService.setVariable(calledInstance.Id, "calledProcessVar1", 42L);

		// and completing it
		Task calledProcessInstanceTask = taskService.createTaskQuery().singleResult();
		taskService.complete(calledProcessInstanceTask.Id);

		// then the call activity output variable has been mapped to the process instance execution
		// and the output mapping variable as well
		IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
		assertEquals(3, callingInstanceVariables.Count);
		assertEquals("val1", callingInstanceVariables["callingProcessVar1"]);
		assertEquals(42L, callingInstanceVariables["calledProcessVar1"]);
		assertEquals(43L, callingInstanceVariables["outputParameter"]);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessLocalInputSingleVariable.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessLocalInputSingleVariable()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessLocalInputSingleVariable");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when setting a variable in a process instance
		runtimeService.setVariable(processInstance.Id, "callingProcessVar1", "val1");

		// and executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then the local variable specified in the io mapping is passed to the called instance
		ProcessInstance calledInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();

		IDictionary<string, object> calledInstanceVariables = runtimeService.getVariables(calledInstance.Id);
		assertEquals(1, calledInstanceVariables.Count);
		assertEquals("val2", calledInstanceVariables["mappedInputParameter"]);

		// when setting a variable in the called process instance
		runtimeService.setVariable(calledInstance.Id, "calledProcessVar1", 42L);

		// and completing it
		Task calledProcessInstanceTask = taskService.createTaskQuery().singleResult();
		taskService.complete(calledProcessInstanceTask.Id);

		// then the call activity output variable has been mapped to the process instance execution
		// and the output mapping variable as well
		IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
		assertEquals(4, callingInstanceVariables.Count);
		assertEquals("val1", callingInstanceVariables["callingProcessVar1"]);
		assertEquals("val2", callingInstanceVariables["mappedInputParameter"]);
		assertEquals(42L, callingInstanceVariables["calledProcessVar1"]);
		assertEquals(43L, callingInstanceVariables["outputParameter"]);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessLocalInputSingleVariableExpression.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessLocalInputSingleVariableExpression()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessLocalInputSingleVariableExpression");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then the local input parameter can be resolved because its source expression variable
		// is defined in the call activity's input mapping
		ProcessInstance calledInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();

		IDictionary<string, object> calledInstanceVariables = runtimeService.getVariables(calledInstance.Id);
		assertEquals(1, calledInstanceVariables.Count);
		assertEquals(43L, calledInstanceVariables["mappedInputParameter"]);

		// and completing it
		Task callActivityTask = taskService.createTaskQuery().singleResult();
		taskService.complete(callActivityTask.Id);

		// and executing a call activity in parameter where the source variable is not mapped by an activity
		// input parameter fails
		Task beforeSecondCallActivityTask = taskService.createTaskQuery().singleResult();
		runtimeService.setVariable(processInstance.Id, "globalVariable", "42");

		try
		{
		  taskService.complete(beforeSecondCallActivityTask.Id);
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot resolve identifier 'globalVariable'", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessLocalOutputAllVariables.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessLocalOutputAllVariables()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessLocalOutputAllVariables");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when setting a variable in a process instance
		runtimeService.setVariable(processInstance.Id, "callingProcessVar1", "val1");

		// and executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then all variables have been mapped into the called instance
		ProcessInstance calledInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();

		IDictionary<string, object> calledInstanceVariables = runtimeService.getVariables(calledInstance.Id);
		assertEquals(2, calledInstanceVariables.Count);
		assertEquals("val1", calledInstanceVariables["callingProcessVar1"]);
		assertEquals("val2", calledInstanceVariables["inputParameter"]);

		// when setting a variable in the called process instance
		runtimeService.setVariable(calledInstance.Id, "calledProcessVar1", 42L);

		// and completing it
		Task calledProcessInstanceTask = taskService.createTaskQuery().singleResult();
		taskService.complete(calledProcessInstanceTask.Id);

		// then only the output mapping variable has been mapped into the calling process instance
		IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
		assertEquals(2, callingInstanceVariables.Count);
		assertEquals("val1", callingInstanceVariables["callingProcessVar1"]);
		assertEquals(43L, callingInstanceVariables["outputParameter"]);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessLocalOutputSingleVariable.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessLocalOutputSingleVariable()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessLocalOutputSingleVariable");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when setting a variable in a process instance
		runtimeService.setVariable(processInstance.Id, "callingProcessVar1", "val1");

		// and executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then all variables have been mapped into the called instance
		ProcessInstance calledInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();

		IDictionary<string, object> calledInstanceVariables = runtimeService.getVariables(calledInstance.Id);
		assertEquals(2, calledInstanceVariables.Count);
		assertEquals("val1", calledInstanceVariables["callingProcessVar1"]);
		assertEquals("val2", calledInstanceVariables["inputParameter"]);

		// when setting a variable in the called process instance
		runtimeService.setVariable(calledInstance.Id, "calledProcessVar1", 42L);

		// and completing it
		Task calledProcessInstanceTask = taskService.createTaskQuery().singleResult();
		taskService.complete(calledProcessInstanceTask.Id);

		// then only the output mapping variable has been mapped into the calling process instance
		IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
		assertEquals(2, callingInstanceVariables.Count);
		assertEquals("val1", callingInstanceVariables["callingProcessVar1"]);
		assertEquals(43L, callingInstanceVariables["outputParameter"]);
	  }

	  /// <summary>
	  /// Test case for handing businessKey to a sub process
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessBusinessKeyInput.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSubProcessBusinessKeyInput()
	  {
		string businessKey = "myBusinessKey";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessBusinessKeyInput", businessKey);

		// one task in the super process should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);
		assertEquals("myBusinessKey", processInstance.BusinessKey);

		taskService.complete(taskBeforeSubProcess.Id);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  // called process started so businesskey should be written in history
		  HistoricProcessInstance hpi = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();
		  assertEquals(businessKey, hpi.BusinessKey);

		  assertEquals(2, historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKey(businessKey).list().size());
		}

		// one task in sub process should be active after starting sub process instance
		taskQuery = taskService.createTaskQuery();
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);
		ProcessInstance subProcessInstance = runtimeService.createProcessInstanceQuery().processInstanceId(taskInSubProcess.ProcessInstanceId).singleResult();
		assertEquals("myBusinessKey", subProcessInstance.BusinessKey);

		taskService.complete(taskInSubProcess.Id);

		// task after sub process in super process
		taskQuery = taskService.createTaskQuery();
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		taskService.complete(taskAfterSubProcess.Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().list().size());

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  HistoricProcessInstance hpi = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(processInstance.Id).finished().singleResult();
		  assertEquals(businessKey, hpi.BusinessKey);

		  assertEquals(2, historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKey(businessKey).finished().list().size());
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcessWithHashExpressions.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testCallSimpleSubProcessWithHashExpressions()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSimpleSubProcess");

		// one task in the subprocess should be active after starting the process
		// instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();
		assertEquals("Task before subprocess", taskBeforeSubProcess.Name);

		// Completing the task continues the process which leads to calling the
		// subprocess. The sub process we want to call is passed in as a variable
		// into this task
		taskService.setVariable(taskBeforeSubProcess.Id, "simpleSubProcessExpression", "simpleSubProcess");
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		// Completing the task in the subprocess, finishes the subprocess
		taskService.complete(taskInSubProcess.Id);
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// Completing this task end the process instance
		taskService.complete(taskAfterSubProcess.Id);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testInterruptingEventSubProcessEventSubscriptions.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/interruptingEventSubProcessEventSubscriptions.bpmn20.xml"})]
	  public virtual void testInterruptingMessageEventSubProcessEventSubscriptionsInsideCallActivity()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callInterruptingEventSubProcess");

		// one task in the call activity subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskInsideCallActivity = taskQuery.singleResult();
		assertEquals("taskBeforeInterruptingEventSubprocess", taskInsideCallActivity.TaskDefinitionKey);

		// we should have no event subscriptions for the parent process
		assertEquals(0, runtimeService.createEventSubscriptionQuery().processInstanceId(processInstance.Id).count());
		// we should have two event subscriptions for the called process instance, one for message and one for signal
		string calledProcessInstanceId = taskInsideCallActivity.ProcessInstanceId;
		EventSubscriptionQuery eventSubscriptionQuery = runtimeService.createEventSubscriptionQuery().processInstanceId(calledProcessInstanceId);
		IList<EventSubscription> subscriptions = eventSubscriptionQuery.list();
		assertEquals(2, subscriptions.Count);

		// start the message interrupting event sub process
		runtimeService.correlateMessage("newMessage");
		Task taskAfterMessageStartEvent = taskQuery.processInstanceId(calledProcessInstanceId).singleResult();
		assertEquals("taskAfterMessageStartEvent", taskAfterMessageStartEvent.TaskDefinitionKey);

		// no subscriptions left
		assertEquals(0, eventSubscriptionQuery.count());

		// Complete the task inside the called process instance
		taskService.complete(taskAfterMessageStartEvent.Id);

		assertProcessEnded(calledProcessInstanceId);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testInterruptingEventSubProcessEventSubscriptions.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/interruptingEventSubProcessEventSubscriptions.bpmn20.xml"})]
	  public virtual void testInterruptingSignalEventSubProcessEventSubscriptionsInsideCallActivity()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callInterruptingEventSubProcess");

		// one task in the call activity subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskInsideCallActivity = taskQuery.singleResult();
		assertEquals("taskBeforeInterruptingEventSubprocess", taskInsideCallActivity.TaskDefinitionKey);

		// we should have no event subscriptions for the parent process
		assertEquals(0, runtimeService.createEventSubscriptionQuery().processInstanceId(processInstance.Id).count());
		// we should have two event subscriptions for the called process instance, one for message and one for signal
		string calledProcessInstanceId = taskInsideCallActivity.ProcessInstanceId;
		EventSubscriptionQuery eventSubscriptionQuery = runtimeService.createEventSubscriptionQuery().processInstanceId(calledProcessInstanceId);
		IList<EventSubscription> subscriptions = eventSubscriptionQuery.list();
		assertEquals(2, subscriptions.Count);

		// start the signal interrupting event sub process
		runtimeService.signalEventReceived("newSignal");
		Task taskAfterSignalStartEvent = taskQuery.processInstanceId(calledProcessInstanceId).singleResult();
		assertEquals("taskAfterSignalStartEvent", taskAfterSignalStartEvent.TaskDefinitionKey);

		// no subscriptions left
		assertEquals(0, eventSubscriptionQuery.count());

		// Complete the task inside the called process instance
		taskService.complete(taskAfterSignalStartEvent.Id);

		assertProcessEnded(calledProcessInstanceId);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testLiteralSourceExpression.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testInputParameterLiteralSourceExpression()
	  {
		runtimeService.startProcessInstanceByKey("process");

		string subInstanceId = runtimeService.createProcessInstanceQuery().processDefinitionKey("simpleSubProcess").singleResult().Id;

		object variable = runtimeService.getVariable(subInstanceId, "inLiteralVariable");
		assertEquals("inLiteralValue", variable);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testLiteralSourceExpression.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testOutputParameterLiteralSourceExpression()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		object variable = runtimeService.getVariable(processInstanceId, "outLiteralVariable");
		assertEquals("outLiteralValue", variable);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessDataOutputOnError.bpmn", "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithError.bpmn" })]
	  public virtual void testSubProcessDataOutputOnError()
	  {
		string variableName = "subVariable";
		object variableValue = "Hello from Subprocess";

		runtimeService.startProcessInstanceByKey("Process_1");
		//first task is the one in the subprocess
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("SubTask"));

		runtimeService.setVariable(task.ProcessInstanceId, variableName, variableValue);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("Task after error"));

		object variable = runtimeService.getVariable(task.ProcessInstanceId, variableName);
		assertThat(variable, @is(notNullValue()));
		assertThat(variable, @is(variableValue));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testSubProcessDataOutputOnThrownError.bpmn", "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithThrownError.bpmn" })]
	  public virtual void testSubProcessDataOutputOnThrownError()
	  {
		string variableName = "subVariable";
		object variableValue = "Hello from Subprocess";

		runtimeService.startProcessInstanceByKey("Process_1");
		//first task is the one in the subprocess
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("SubTask"));

		runtimeService.setVariable(task.ProcessInstanceId, variableName, variableValue);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("Task after error"));

		object variable = runtimeService.getVariable(task.ProcessInstanceId, variableName);
		assertThat(variable, @is(notNullValue()));
		assertThat(variable, @is(variableValue));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testTwoSubProcessesDataOutputOnError.bpmn", "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessCallErrorSubProcess.bpmn", "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithError.bpmn" })]
	  public virtual void testTwoSubProcessesDataOutputOnError()
	  {
		string variableName = "subVariable";
		object variableValue = "Hello from Subprocess";

		runtimeService.startProcessInstanceByKey("Process_1");
		//first task is the one in the subprocess
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("SubTask"));

		runtimeService.setVariable(task.ProcessInstanceId, variableName, variableValue);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("Task after error"));

		object variable = runtimeService.getVariable(task.ProcessInstanceId, variableName);
		//both processes have and out mapping for all, so we want the variable to be propagated to the process with the event handler
		assertThat(variable, @is(notNullValue()));
		assertThat(variable, @is(variableValue));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testTwoSubProcessesLimitedDataOutputOnError.bpmn", "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessCallErrorSubProcessWithLimitedOutMapping.bpmn", "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithError.bpmn" })]
	  public virtual void testTwoSubProcessesLimitedDataOutputOnError()
	  {
		string variableName1 = "subSubVariable1";
		string variableName2 = "subSubVariable2";
		string variableName3 = "subVariable";
		object variableValue = "Hello from Subsubprocess";
		object variableValue2 = "Hello from Subprocess";

		runtimeService.startProcessInstanceByKey("Process_1");

		//task in first subprocess (second process in general)
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("Task"));
		runtimeService.setVariable(task.ProcessInstanceId, variableName3, variableValue2);
		taskService.complete(task.Id);
		//task in the second subprocess (third process in general)
		task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("SubTask"));
		runtimeService.setVariable(task.ProcessInstanceId, variableName1, "foo");
		runtimeService.setVariable(task.ProcessInstanceId, variableName2, variableValue);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("Task after error"));

		//the two subprocess don't pass all their variables, so we check that not all were passed
		object variable = runtimeService.getVariable(task.ProcessInstanceId, variableName2);
		assertThat(variable, @is(notNullValue()));
		assertThat(variable, @is(variableValue));
		variable = runtimeService.getVariable(task.ProcessInstanceId, variableName3);
		assertThat(variable, @is(notNullValue()));
		assertThat(variable, @is(variableValue2));
		variable = runtimeService.getVariable(task.ProcessInstanceId, variableName1);
		assertThat(variable, @is(nullValue()));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityAdvancedTest.testCallProcessByVersionAsExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallCaseByVersionAsExpression()
	  {
		// given

		string bpmnResourceName = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string processDefinitionIdInSecondDeployment = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").deploymentId(secondDeploymentId).singleResult().Id;

		VariableMap variables = Variables.createVariables().putValue("myVersion", 2);

		// when
		runtimeService.startProcessInstanceByKey("process", variables).Id;

		// then
		ProcessInstance subInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").singleResult();
		assertNotNull(subInstance);

		assertEquals(processDefinitionIdInSecondDeployment, subInstance.ProcessDefinitionId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivityAdvancedTest.testCallProcessByVersionAsDelegateExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallCaseByVersionAsDelegateExpression()
	  {
		processEngineConfiguration.Beans["myDelegate"] = new MyVersionDelegate();

		// given
		string bpmnResourceName = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string processDefinitionIdInSecondDeployment = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").deploymentId(secondDeploymentId).singleResult().Id;

		VariableMap variables = Variables.createVariables().putValue("myVersion", 2);

		// when
		runtimeService.startProcessInstanceByKey("process", variables).Id;

		// then
		ProcessInstance subInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").singleResult();
		assertNotNull(subInstance);

		assertEquals(processDefinitionIdInSecondDeployment, subInstance.ProcessDefinitionId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithVersionTag.bpmn20.xml" })]
	  public virtual void testCallProcessByVersionTag()
	  {
		// given
		BpmnModelInstance modelInstance = getModelWithCallActivityVersionTagBinding("ver_tag_1");

		deployment(modelInstance);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// then
		ProcessInstance subInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("subProcess").superProcessInstanceId(processInstance.Id).singleResult();
		assertNotNull(subInstance);

		// clean up
		cleanupDeployments();
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithVersionTag.bpmn20.xml" })]
	  public virtual void testCallProcessByVersionTagAsExpression()
	  {
		// given
		BpmnModelInstance modelInstance = getModelWithCallActivityVersionTagBinding("${versionTagExpr}");

		deployment(modelInstance);

		// when
		VariableMap variables = Variables.createVariables().putValue("versionTagExpr", "ver_tag_1");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		// then
		ProcessInstance subInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("subProcess").superProcessInstanceId(processInstance.Id).singleResult();
		assertNotNull(subInstance);

		// clean up
		cleanupDeployments();
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithVersionTag.bpmn20.xml" })]
	  public virtual void testCallProcessByVersionTagAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myDelegate"] = new MyVersionDelegate();
		BpmnModelInstance modelInstance = getModelWithCallActivityVersionTagBinding("${myDelegate.getVersionTag()}");

		deployment(modelInstance);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// then
		ProcessInstance subInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("subProcess").superProcessInstanceId(processInstance.Id).singleResult();
		assertNotNull(subInstance);

		// clean up
		cleanupDeployments();
	  }

	  public virtual void testCallProcessWithoutVersionTag()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("process").startEvent().callActivity("callActivity").calledElement("subProcess").camundaCalledElementBinding("versionTag").endEvent().done();

		try
		{
		  // when
		  deployment(modelInstance);
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  assertTrue(e.Message.contains("Could not parse BPMN process."));
		  assertTrue(e.Message.contains("Missing attribute 'calledElementVersionTag' when 'calledElementBinding' has value 'versionTag'"));
		}
	  }

	  public virtual void testCallProcessByVersionTagNoneSubprocess()
	  {
		// given
		BpmnModelInstance modelInstance = getModelWithCallActivityVersionTagBinding("ver_tag_1");

		deployment(modelInstance);

		try
		{
		  // when
		  runtimeService.startProcessInstanceByKey("process");
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  assertTrue(e.Message.contains("no processes deployed with key = 'subProcess', versionTag = 'ver_tag_1' and tenant-id = 'null': processDefinition is null"));
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithVersionTag.bpmn20.xml" })]
	  public virtual void testCallProcessByVersionTagTwoSubprocesses()
	  {
		// given
		BpmnModelInstance modelInstance = getModelWithCallActivityVersionTagBinding("ver_tag_1");

		deployment(modelInstance);
		deployment("org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithVersionTag.bpmn20.xml");

		try
		{
		  // when
		  runtimeService.startProcessInstanceByKey("process");
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  assertTrue(e.Message.contains("There are '2' results for a process definition with key 'subProcess', versionTag 'ver_tag_1' and tenant-id '{}'."));
		}

		// clean up
		cleanupDeployments();
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/orderProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/checkCreditProcess.bpmn20.xml" })]
	  public virtual void testOrderProcessWithCallActivity()
	  {
		// After the process has started, the 'verify credit history' task should be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("orderProcess");
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task verifyCreditTask = taskQuery.singleResult();
		assertEquals("Verify credit history", verifyCreditTask.Name);

		// Verify with Query API
		ProcessInstance subProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(pi.Id).singleResult();
		assertNotNull(subProcessInstance);
		assertEquals(pi.Id, runtimeService.createProcessInstanceQuery().subProcessInstanceId(subProcessInstance.Id).singleResult().Id);

		// Completing the task with approval, will end the subprocess and continue the original process
		taskService.complete(verifyCreditTask.Id, CollectionUtil.singletonMap("creditApproved", true));
		Task prepareAndShipTask = taskQuery.singleResult();
		assertEquals("Prepare and Ship", prepareAndShipTask.Name);
	  }

	  /// <summary>
	  /// Test case for checking deletion of process instancess in call activity subprocesses
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testDeleteProcessInstanceInCallActivity()
	  {
		// given
		runtimeService.startProcessInstanceByKey("callSimpleSubProcess");


		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();

		// Completing the task continues the process which leads to calling the subprocess
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();


		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(2, instanceList.Count);


		// when
		// Delete the ProcessInstance in the sub process
		runtimeService.deleteProcessInstance(taskInSubProcess.ProcessInstanceId, "Test upstream deletion");

		// then

		// How many process Instances
		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(0, instanceList.Count);
	  }

	  /// <summary>
	  /// Test case for checking deletion of process instances in call activity subprocesses
	  /// 
	  /// Checks that deletion of process Instance will resepct other process instances in the scope
	  /// and stop its upward deletion propagation will stop at this point
	  /// 
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testTwoSubProcesses.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml"})]
	  public virtual void testSingleDeletionWithTwoSubProcesses()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callTwoSubProcesses");

		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(3, instanceList.Count);

		IList<Task> taskList = taskService.createTaskQuery().list();
		assertNotNull(taskList);
		assertEquals(2, taskList.Count);

		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.ProcessInstanceId);
		assertNotNull(activeActivityIds);
		assertEquals(2, activeActivityIds.Count);

		// when
		runtimeService.deleteProcessInstance(taskList[0].ProcessInstanceId, "Test upstream deletion");

		// then
		// How many process Instances
		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(2, instanceList.Count);

		// How man call activities
		activeActivityIds = runtimeService.getActiveActivityIds(processInstance.ProcessInstanceId);
		assertNotNull(activeActivityIds);
		assertEquals(1, activeActivityIds.Count);
	  }

	  /// <summary>
	  /// Test case for checking deletion of process instances in nested call activity subprocesses
	  /// 
	  /// Checking that nested call activities will propagate upward over multiple nested levels
	  /// 
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testNestedCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testDeleteMultilevelProcessInstanceInCallActivity()
	  {
		// given
		runtimeService.startProcessInstanceByKey("nestedCallActivity");

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();

		// Completing the task continues the process which leads to calling the subprocess
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();

		// Completing the task continues the sub process which leads to calling the deeper subprocess
		taskService.complete(taskInSubProcess.Id);
		Task taskInNestedSubProcess = taskQuery.singleResult();

		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(3, instanceList.Count);

		// when
		// Delete the ProcessInstance in the sub process
		runtimeService.deleteProcessInstance(taskInNestedSubProcess.ProcessInstanceId, "Test cascading upstream deletion");


		// then
		// How many process Instances
		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(0, instanceList.Count);

	  }

	  /// <summary>
	  /// Test case for checking deletion of process instances in nested call activity subprocesses
	  /// 
	  /// The test defines a process waiting on three nested call activities to complete
	  /// 
	  /// At each nested level there is only one process instance, which is waiting on the next level to complete
	  /// 
	  /// When we delete the process instance of the most inner call activity sub process the expected behaviour is that
	  /// the delete will propagate upward and delete all process instances.
	  /// 
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testNestedCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testDoubleNestedCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
	  public virtual void testDeleteDoubleNestedProcessInstanceInCallActivity()
	  {
		// given
		runtimeService.startProcessInstanceByKey("doubleNestedCallActivity");

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();

		// Completing the task continues the process which leads to calling the subprocess
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();

		// Completing the task continues the sub process which leads to calling the deeper subprocess
		taskService.complete(taskInSubProcess.Id);
		Task taskInNestedSubProcess = taskQuery.singleResult();


		// Completing the task continues the sub process which leads to calling the deeper subprocess
		taskService.complete(taskInNestedSubProcess.Id);
		Task taskInDoubleNestedSubProcess = taskQuery.singleResult();


		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(4, instanceList.Count);

		// when
		// Delete the ProcessInstance in the sub process
		runtimeService.deleteProcessInstance(taskInDoubleNestedSubProcess.ProcessInstanceId, "Test cascading upstream deletion");


		// then
		// How many process Instances
		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(0, instanceList.Count);

	  }

	  protected internal virtual BpmnModelInstance getModelWithCallActivityVersionTagBinding(string versionTag)
	  {
		return Bpmn.createExecutableProcess("process").startEvent().callActivity("callActivity").calledElement("subProcess").camundaCalledElementBinding("versionTag").camundaCalledElementVersionTag(versionTag).endEvent().done();
	  }

	  protected internal virtual void cleanupDeployments()
	  {
		IList<org.camunda.bpm.engine.repository.Deployment> deployments = repositoryService.createDeploymentQuery().list();
		foreach (org.camunda.bpm.engine.repository.Deployment currentDeployment in deployments)
		{
		  repositoryService.deleteDeployment(currentDeployment.Id, true);
		}
	  }
	}

}