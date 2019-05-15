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
namespace org.camunda.bpm.engine.test.cmmn.processtask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessTaskTest : CmmnProcessEngineTestCase
	{

	  protected internal readonly string PROCESS_TASK = "PI_ProcessTask_1";
	  protected internal readonly string ONE_PROCESS_TASK_CASE = "oneProcessTaskCase";

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallProcessAsConstant()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testCallProcessAsExpressionStartsWithDollar.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallProcessAsExpressionStartsWithDollar()
	  {
		// given
		// a deployed case definition
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue("process", "oneTaskProcess")).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testCallProcessAsExpressionStartsWithHash.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallProcessAsExpressionStartsWithHash()
	  {
		// given
		// a deployed case definition
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue("process", "oneTaskProcess")).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testCallLatestProcess.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallLatestProcess()
	  {
		// given
		string bpmnResourceName = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";

		string deploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());

		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// latest process definition
		string latestProcessDefinitionId = repositoryService.createProcessDefinitionQuery().latestVersion().singleResult().Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);
		// it is associated with the latest process definition
		assertEquals(latestProcessDefinitionId, processInstance.ProcessDefinitionId);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testCallProcessByDeployment.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallProcessByDeployment()
	  {
		// given

		string firstDeploymentId = repositoryService.createDeploymentQuery().singleResult().Id;

		string bpmnResourceName = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
		string deploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());

		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// latest process definition
		string processDefinitionIdInSameDeployment = repositoryService.createProcessDefinitionQuery().deploymentId(firstDeploymentId).singleResult().Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);
		// it is associated with the correct process definition
		assertEquals(processDefinitionIdInSameDeployment, processInstance.ProcessDefinitionId);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testCallProcessByVersion.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallProcessByVersion()
	  {
		// given

		string bpmnResourceName = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		assertEquals(3, repositoryService.createProcessDefinitionQuery().count());

		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// latest process definition
		string processDefinitionIdInSecondDeployment = repositoryService.createProcessDefinitionQuery().deploymentId(secondDeploymentId).singleResult().Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);
		// it is associated with the correct process definition
		assertEquals(processDefinitionIdInSecondDeployment, processInstance.ProcessDefinitionId);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testCallProcessByVersionAsExpressionStartsWithDollar.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallProcessByVersionAsExpressionStartsWithDollar()
	  {
		// given

		string bpmnResourceName = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		assertEquals(3, repositoryService.createProcessDefinitionQuery().count());

		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue("myVersion", 2)).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// latest process definition
		string processDefinitionIdInSecondDeployment = repositoryService.createProcessDefinitionQuery().deploymentId(secondDeploymentId).singleResult().Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);
		// it is associated with the correct process definition
		assertEquals(processDefinitionIdInSecondDeployment, processInstance.ProcessDefinitionId);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testCallProcessByVersionAsExpressionStartsWithHash.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCallProcessByVersionAsExpressionStartsWithHash()
	  {
		// given

		string bpmnResourceName = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		assertEquals(3, repositoryService.createProcessDefinitionQuery().count());

		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue("myVersion", 2)).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// latest process definition
		string processDefinitionIdInSecondDeployment = repositoryService.createProcessDefinitionQuery().deploymentId(secondDeploymentId).singleResult().Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);
		// it is associated with the correct process definition
		assertEquals(processDefinitionIdInSecondDeployment, processInstance.ProcessDefinitionId);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputBusinessKey.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputBusinessKey()
	  {
		// given
		string businessKey = "myBusinessKey";
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, businessKey).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);
		// the business key has been set
		assertEquals(businessKey, processInstance.BusinessKey);

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputDifferentBusinessKey.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputDifferentBusinessKey()
	  {
		// given
		string businessKey = "myBusinessKey";
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, businessKey).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// when
		caseService.withCaseExecution(processTaskId).setVariable("myOwnBusinessKey", "myOwnBusinessKey").manualStart();

		// then

		// there exists a process instance
		ExecutionEntity processInstance = (ExecutionEntity) queryProcessInstance();
		assertNotNull(processInstance);

		// the case instance id is set on called process instance
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
		// the super case execution id is equals the processTaskId
		assertEquals(processTaskId, processInstance.SuperCaseExecutionId);
		// the business key has been set
		assertEquals("myOwnBusinessKey", processInstance.BusinessKey);
		assertFalse(businessKey.Equals(processInstance.BusinessKey));

		TaskEntity task = (TaskEntity) queryTask();

		// the case instance id has been also set on the task
		assertEquals(caseInstanceId, task.CaseInstanceId);
		// the case execution id should be null
		assertNull(task.CaseExecutionId);

		// complete ////////////////////////////////////////////////////////

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputSource.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputSource()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999).putValue("aThirdVariable", "def")).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then

		// there exists a process instance
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		taskService.complete(queryTask().Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputSourceDifferentTarget.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputSourceDifferentTarget()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// when
		caseService.withCaseExecution(processTaskId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).setVariable("aThirdVariable", "def").manualStart();

		// then

		// there exists a process instance
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("myVariable".Equals(name))
		  {
			assertEquals("myVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("myAnotherVariable".Equals(name))
		  {
			assertEquals("myAnotherVariable", name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		taskService.complete(queryTask().Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputSource.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputSourceNullValue()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then

		// there exists a process instance
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;

		  if (!"aVariable".Equals(name) && !"anotherVariable".Equals(name))
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }

		  assertNull(variable.Value);
		}

		// complete ////////////////////////////////////////////////////////

		taskService.complete(queryTask().Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputSourceExpression.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputSourceExpression()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999)).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then

		// there exists a process instance
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals((long) 1000, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		taskService.complete(queryTask().Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputAll.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputAll()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999)).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then

		// there exists a process instance
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		taskService.complete(queryTask().Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputAllLocal.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputAllLocal()
	  {
		// given
		createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// when
		caseService.withCaseExecution(caseTaskId).setVariable("aVariable", "abc").setVariableLocal("aLocalVariable", "def").manualStart();

		// then only the local variable is mapped to the sub process instance
		ProcessInstance subProcessInstance = queryProcessInstance();

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(subProcessInstance.Id).list();

		assertEquals(1, variables.Count);
		assertEquals("aLocalVariable", variables[0].Name);
	  }


	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputOverlapping.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputOverlapping()
	  {
		// specifics should override "all"
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// when
		caseService.withCaseExecution(processTaskId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).manualStart();

		// then

		// there exists a process instance
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals((long) 1000, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		taskService.complete(queryTask().Id);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testProcessNotFound()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).manualStart();
		  fail("It should not be possible to start a process instance.");
		}
		catch (ProcessEngineException)
		{
		}

		// complete //////////////////////////////////////////////////////////

		terminate(caseInstanceId);
		close(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCompleteSimpleProcess()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		Task task = queryTask();

		// when
		taskService.complete(task.Id);

		// then

		// the process instance has been completed
		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		// the case execution associated with the
		// process task has been completed
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK);
		assertNull(processTask);

		// complete ////////////////////////////////////////////////////////

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputSource.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testOutputSource()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		string processInstanceId = queryProcessInstance().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "abc");
		runtimeService.setVariable(processInstanceId, "anotherVariable", 999);
		runtimeService.setVariable(processInstanceId, "aThirdVariable", "def");

		string taskId = queryTask().Id;

		// when
		// should also complete process instance
		taskService.complete(taskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputSourceDifferentTarget.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testOutputSourceDifferentTarget()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		string processInstanceId = queryProcessInstance().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "abc");
		runtimeService.setVariable(processInstanceId, "anotherVariable", 999);

		string taskId = queryTask().Id;

		// when
		// should also complete process instance
		taskService.complete(taskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("myVariable".Equals(name))
		  {
			assertEquals("myVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("myAnotherVariable".Equals(name))
		  {
			assertEquals("myAnotherVariable", name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputSource.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testOutputSourceNullValue()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		string processInstanceId = queryProcessInstance().Id;

		string taskId = queryTask().Id;

		// when
		// should also complete process instance
		taskService.complete(taskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if (!"aVariable".Equals(name) && !"anotherVariable".Equals(name))
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }

		  assertNull(variable.Value);
		}

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputSourceExpression.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testOutputSourceExpression()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		string processInstanceId = queryProcessInstance().Id;

		string taskId = queryTask().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "abc");
		runtimeService.setVariable(processInstanceId, "anotherVariable", 999);

		// when
		// should also complete process instance
		taskService.complete(taskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals((long) 1000, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputAll.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testOutputAll()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		string processInstanceId = queryProcessInstance().Id;

		string taskId = queryTask().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "abc");
		runtimeService.setVariable(processInstanceId, "anotherVariable", 999);

		// when
		// should also complete process instance
		taskService.complete(taskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputOverlapping.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testOutputOverlapping()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		string processInstanceId = queryProcessInstance().Id;

		string taskId = queryTask().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "abc");
		runtimeService.setVariable(processInstanceId, "anotherVariable", 999);

		// when
		// should also complete process instance
		taskService.complete(taskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals((long) 1000, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputAllWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testOutputVariablesShouldNotExistAnymore()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		caseService.withCaseExecution(processTaskId).setVariableLocal("aVariable", "xyz").setVariableLocal("anotherVariable", 123).manualStart();

		string processInstanceId = queryProcessInstance().Id;

		string taskId = queryTask().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "abc");
		runtimeService.setVariable(processInstanceId, "anotherVariable", 999);

		// when
		// should also complete process instance
		taskService.complete(taskId);

		// then

		// the variables has been deleted
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertTrue(variables.Count == 0);

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testVariablesRoundtrip.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testVariablesRoundtrip()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		caseService.withCaseExecution(processTaskId).setVariable("aVariable", "xyz").setVariable("anotherVariable", 123);

		string processInstanceId = queryProcessInstance().Id;

		string taskId = queryTask().Id;

		runtimeService.setVariable(processInstanceId, "aVariable", "abc");
		runtimeService.setVariable(processInstanceId, "anotherVariable", 999);

		// when
		// should also complete process instance
		taskService.complete(taskId);

		// then

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertEquals(2, variables.Count);
		assertFalse(variables[0].Name.Equals(variables[1].Name));

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("abc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testInputOutputAll.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputOutputAllTypedVariables()
	  {
		string variableName = "aVariable";
		string variableName2 = "anotherVariable";
		string variableName3 = "theThirdVariable";
		TypedValue variableValue = Variables.stringValue("abc");
		TypedValue variableValue2 = Variables.longValue(null);

		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue(variableName, variableValue).putValue(variableName2, variableValue2)).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		string processInstanceId = queryProcessInstance().Id;

		TypedValue value = runtimeService.getVariableTyped(processInstanceId, variableName);
		assertThat(value, @is(variableValue));
		value = runtimeService.getVariableTyped(processInstanceId, variableName2);
		assertThat(value, @is(variableValue2));

		string taskId = queryTask().Id;

		TypedValue variableValue3 = Variables.integerValue(1);
		runtimeService.setVariable(processInstanceId, variableName3, variableValue3);

		// should also complete process instance
		taskService.complete(taskId);

		value = caseService.getVariableTyped(caseInstanceId, variableName3);

		assertThat(value, @is(variableValue3));

		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testVariablesRoundtrip.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInputOutputLimitedTypedVariables()
	  {
		string variableName = "aVariable";
		string variableName2 = "anotherVariable";
		TypedValue caseVariableValue = Variables.stringValue("abc");
		TypedValue caseVariableValue2 = Variables.integerValue(null);

		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE, Variables.createVariables().putValue(variableName, caseVariableValue).putValue(variableName2, caseVariableValue2)).Id;

		string processInstanceId = queryProcessInstance().Id;
		TypedValue value = runtimeService.getVariableTyped(processInstanceId, variableName);
		assertThat(value, @is(caseVariableValue));
		value = runtimeService.getVariableTyped(processInstanceId, variableName2);
		assertThat(value, @is(caseVariableValue2));


		TypedValue processVariableValue = Variables.stringValue("cba");
		TypedValue processVariableValue2 = Variables.booleanValue(null);
		runtimeService.setVariable(processInstanceId, variableName, processVariableValue);
		runtimeService.setVariable(processInstanceId, variableName2, processVariableValue2);

		// should also complete process instance
		taskService.complete(queryTask().Id);

		value = caseService.getVariableTyped(caseInstanceId, variableName);
		assertThat(value, @is(processVariableValue));
		value = caseService.getVariableTyped(caseInstanceId, variableName2);
		assertThat(value, @is(processVariableValue2));
		// complete ////////////////////////////////////////////////////////

		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCompleteProcessTask()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).complete();
		  fail("It should not be possible to complete a process task, while the process instance is running.");
		}
		catch (NotAllowedException)
		{
		}

		// complete ////////////////////////////////////////////////////////

		string processInstanceId = queryProcessInstance().Id;

		string taskId = queryTask().Id;

		taskService.complete(taskId);
		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCompleteProcessTaskAfterTerminateSubProcessInstance()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		string processInstanceId = queryProcessInstance().Id;

		runtimeService.deleteProcessInstance(processInstanceId, null);

		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).complete();
		  fail("It should not be possible to complete a process task");
		}
		catch (Exception)
		{
		}

		// complete ////////////////////////////////////////////////////////

		terminate(caseInstanceId);
		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTerminateProcessTask()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// when
		// terminate process task
		terminate(processTaskId);

		// then

		// the process instance is still running
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		// complete ////////////////////////////////////////////////////////

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTerminateSubProcessInstance()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		string processInstanceId = queryProcessInstance().Id;

		// when
		runtimeService.deleteProcessInstance(processInstanceId, null);

		// then
		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		// the case execution associated with the process task
		// does still exist and is active.
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK);

		assertNotNull(processTask);

		assertTrue(processTask.Active);

		// complete ////////////////////////////////////////////////////////

		terminate(caseInstanceId);
		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testSuspendProcessTask()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// when
		// suspend process task
		suspend(processTaskId);

		// then
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);
		assertFalse(processInstance.Suspended);

		Task task = queryTask();
		assertNotNull(task);
		assertFalse(task.Suspended);

		// complete ////////////////////////////////////////////////////////

		resume(processTaskId);
		terminate(caseInstanceId);
		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

		taskService.complete(task.Id);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testSuspendSubProcessInstance()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		string processInstanceId = queryProcessInstance().Id;

		// when
		// suspend sub process instance
		runtimeService.suspendProcessInstanceById(processInstanceId);

		// then
		ProcessInstance processInstance = queryProcessInstance();
		assertTrue(processInstance.Suspended);

		// the case execution associated with the process task
		// is still active
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK);
		assertTrue(processTask.Active);

		// complete ////////////////////////////////////////////////////////

		runtimeService.activateProcessInstanceById(processInstanceId);

		string taskId = queryTask().Id;
		taskService.complete(taskId);
		assertProcessEnded(processInstanceId);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testResumeProcessTask()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		suspend(processTaskId);

		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK);
		assertFalse(processTask.Active);

		ProcessInstance processInstance = queryProcessInstance();
		assertFalse(processInstance.Suspended);

		// when
		resume(processTaskId);

		// then
		processInstance = queryProcessInstance();
		assertFalse(processInstance.Suspended);

		processTask = queryCaseExecutionByActivityId(PROCESS_TASK);
		assertTrue(processTask.Active);

		// complete ////////////////////////////////////////////////////////

		string taskId = queryTask().Id;
		taskService.complete(taskId);
		assertProcessEnded(processInstance.Id);

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testNonBlockingProcessTask.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testNonBlockingProcessTask()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		// then
		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		Task task = queryTask();
		assertNotNull(task);

		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK);
		assertNull(processTask);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

		// complete ////////////////////////////////////////////////////////

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);

		string taskId = queryTask().Id;
		taskService.complete(taskId);
		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testProcessInstanceCompletesInOneGo.cmmn", "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testProcessInstanceCompletesInOneGo.bpmn20.xml" })]
	  public virtual void testProcessInstanceCompletesInOneGo()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		// then
		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK);
		assertNull(processTask);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

		// complete ////////////////////////////////////////////////////////

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testNonBlockingProcessTaskAndProcessInstanceCompletesInOneGo.cmmn", "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testProcessInstanceCompletesInOneGo.bpmn20.xml" })]
	  public virtual void testNonBlockingProcessTaskAndProcessInstanceCompletesInOneGo()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		// then
		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK);
		assertNull(processTask);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

		// complete ////////////////////////////////////////////////////////

		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testStartProcessInstanceAsync.cmmn", "org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testStartProcessInstanceAsync.bpmn20.xml" })]
	  public virtual void testStartProcessInstanceAsync()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK).Id;

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		ProcessInstance processInstance = queryProcessInstance();
		assertNotNull(processInstance);

		// complete ////////////////////////////////////////////////////////

		managementService.executeJob(job.Id);
		close(caseInstanceId);
		assertCaseEnded(caseInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testActivityType()
	  {
		// given
		createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;

		// when
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK);

		// then
		assertEquals("processTask", processTask.ActivityType);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputAll.cmmn", "org/camunda/bpm/engine/test/cmmn/processtask/subProcessWithError.bpmn"})]
	  public virtual void testOutputWhenErrorOccurs()
	  {
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		Task task = queryTask();
		assertThat(task.Name, @is("SubTask"));
		string variableName = "foo";
		object variableValue = "bar";
		runtimeService.setVariable(task.ProcessInstanceId, variableName, variableValue);
		taskService.complete(task.Id);

		object variable = caseService.getVariable(caseInstanceId, variableName);
		assertThat(variable, @is(notNullValue()));
		assertThat(variable, @is(variableValue));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/processtask/ProcessTaskTest.testOutputAll.cmmn", "org/camunda/bpm/engine/test/cmmn/processtask/subProcessWithThrownError.bpmn"})]
	  public virtual void testOutputWhenThrownBpmnErrorOccurs()
	  {
		string caseInstanceId = createCaseInstanceByKey(ONE_PROCESS_TASK_CASE).Id;
		Task task = queryTask();
		assertThat(task.Name, @is("SubTask"));
		string variableName = "foo";
		object variableValue = "bar";
		runtimeService.setVariable(task.ProcessInstanceId, variableName, variableValue);
		taskService.complete(task.Id);

		object variable = caseService.getVariable(caseInstanceId, variableName);
		assertThat(variable, @is(notNullValue()));
		assertThat(variable, @is(variableValue));
	  }

	  protected internal virtual ProcessInstance queryProcessInstance()
	  {
		return runtimeService.createProcessInstanceQuery().singleResult();
	  }

	  protected internal virtual Task queryTask()
	  {
		return taskService.createTaskQuery().singleResult();
	  }


	}

}