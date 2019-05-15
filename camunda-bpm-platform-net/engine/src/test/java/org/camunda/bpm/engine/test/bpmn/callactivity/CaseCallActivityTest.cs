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
namespace org.camunda.bpm.engine.test.bpmn.callactivity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using CaseDefinitionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseDefinitionNotFoundException;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseCallActivityTest : CmmnProcessEngineTestCase
	{

	  protected internal readonly string PROCESS_DEFINITION_KEY = "process";
	  protected internal readonly string ONE_TASK_CASE = "oneTaskCase";
	  protected internal readonly string CALL_ACTIVITY_ID = "callActivity";
	  protected internal readonly string USER_TASK_ID = "userTask";
	  protected internal readonly string HUMAN_TASK_ID = "PI_HumanTask_1";

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseAsConstant()
	  {
		// given
		// a deployed process definition and case definition

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);

		// complete
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsExpressionStartsWithDollar.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseAsExpressionStartsWithDollar()
	  {
		// given
		// a deployed process definition and case definition

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, Variables.createVariables().putValue(ONE_TASK_CASE, ONE_TASK_CASE)).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);

		// complete
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsExpressionStartsWithHash.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseAsExpressionStartsWithHash()
	  {
		// given
		// a deployed process definition and case definition

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, Variables.createVariables().putValue(ONE_TASK_CASE, ONE_TASK_CASE)).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);

		// complete
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseWithCompositeExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseWithCompositeExpression()
	  {
		// given
		// a deployed process definition and case definition

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);

		// complete
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallLatestCase.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallLatestCase()
	  {
		// given
		string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

		string deploymentId = repositoryService.createDeployment().addClasspathResource(cmmnResourceName).deploy().Id;

		assertEquals(2, repositoryService.createCaseDefinitionQuery().count());

		string latestCaseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).latestVersion().singleResult().Id;

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);
		assertEquals(latestCaseDefinitionId, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseByDeployment.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseByDeployment()
	  {
		// given

		string firstDeploymentId = repositoryService.createDeploymentQuery().singleResult().Id;

		string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
		string deploymentId = repositoryService.createDeployment().addClasspathResource(cmmnResourceName).deploy().Id;

		assertEquals(2, repositoryService.createCaseDefinitionQuery().count());

		string caseDefinitionIdInSameDeployment = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).deploymentId(firstDeploymentId).singleResult().Id;

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);
		assertEquals(caseDefinitionIdInSameDeployment, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseByVersion.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseByVersion()
	  {
		// given

		string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(cmmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(cmmnResourceName).deploy().Id;

		assertEquals(3, repositoryService.createCaseDefinitionQuery().count());

		string caseDefinitionIdInSecondDeployment = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).deploymentId(secondDeploymentId).singleResult().Id;

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);
		assertEquals(caseDefinitionIdInSecondDeployment, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseByVersionAsExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseByVersionAsExpression()
	  {
		// given

		string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(cmmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(cmmnResourceName).deploy().Id;

		assertEquals(3, repositoryService.createCaseDefinitionQuery().count());

		string caseDefinitionIdInSecondDeployment = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).deploymentId(secondDeploymentId).singleResult().Id;

		VariableMap variables = Variables.createVariables().putValue("myVersion", 2);

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, variables).Id;

		// then
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(caseDefinitionIdInSecondDeployment, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml" })]
	  public virtual void testCaseNotFound()
	  {
		// given

		try
		{
		  // when
		  startProcessInstanceByKey(PROCESS_DEFINITION_KEY);
		  fail("It should not be possible to start a not existing case instance.");
		}
		catch (CaseDefinitionNotFoundException)
		{
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputBusinessKey.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputBusinessKey()
	  {
		// given
		string businessKey = "myBusinessKey";

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, null, businessKey).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);
		assertEquals(businessKey, subCaseInstance.BusinessKey);

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputDifferentBusinessKey.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputDifferentBusinessKey()
	  {
		// given
		string myBusinessKey = "myBusinessKey";
		string myOwnBusinessKey = "myOwnBusinessKey";

		VariableMap variables = Variables.createVariables().putValue(myOwnBusinessKey, myOwnBusinessKey);

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, variables, myBusinessKey).Id;

		// then
		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		assertEquals(callActivityId, subCaseInstance.SuperExecutionId);
		assertEquals(myOwnBusinessKey, subCaseInstance.BusinessKey);

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSource.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSource()
	  {
		// given

		VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999).putValue("aThirdVariable", "def");

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

		// then
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSourceDifferentTarget.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSourceDifferentTarget()
	  {
		// given

		VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999).putValue("aThirdVariable", "def");

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

		// then
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSource.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSourceNullValue()
	  {
		// given

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		// then
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;

		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }

		  assertNull(variable.Value);
		}

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSourceExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSourceExpression()
	  {
		// given
		VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999);

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

		// then
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
			assertEquals((long)1000, variable.Value);

		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSourceAsCompositeExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSourceAsCompositeExpression()
	  {
		// given
		VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999);

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

		// then
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("Prefixabc", variable.Value);

		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals("Prefix" + (long)1000, variable.Value);

		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputAll.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputAll()
	  {
		// given
		VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999);

		// when
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

		// then
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCompleteCase.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCompleteCase()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then
		Task userTask = queryTaskByActivityId(USER_TASK_ID);
		assertNotNull(userTask);

		Execution callActivity = queryExecutionByActivityId(CALL_ACTIVITY_ID);
		assertNull(callActivity);

		// complete ////////////////////////////////////////////////////////

		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		taskService.complete(userTask.Id);
		assertCaseEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSource.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputSource()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).setVariable("aThirdVariable", "def").execute();

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(superProcessInstanceId).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
		taskService.complete(taskId);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSourceDifferentTarget.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputSourceDifferentTarget()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(superProcessInstanceId).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
		taskService.complete(taskId);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSource.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputSourceNullValue()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(superProcessInstanceId).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }

		  assertNull(variable.Value);
		}

		// complete ////////////////////////////////////////////////////////
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
		taskService.complete(taskId);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSourceExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputSourceExpression()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(superProcessInstanceId).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
		taskService.complete(taskId);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSourceAsCompositeExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputSourceAsCompositeExpression()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(superProcessInstanceId).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

		foreach (VariableInstance variable in variables)
		{
		  string name = variable.Name;
		  if ("aVariable".Equals(name))
		  {
			assertEquals("aVariable", name);
			assertEquals("Prefixabc", variable.Value);
		  }
		  else if ("anotherVariable".Equals(name))
		  {
			assertEquals("anotherVariable", name);
			assertEquals("Prefix" + (long) 1000, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
		taskService.complete(taskId);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputAll.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputAll()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(superProcessInstanceId).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
		taskService.complete(taskId);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputAll.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputVariablesShouldNotExistAnymore()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

		VariableMap parameters = Variables.createVariables().putValue("aVariable", "xyz").putValue("anotherVariable", 123);

		runtimeService.setVariablesLocal(callActivityId, parameters);

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then

		// the variables has been deleted
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(superProcessInstanceId).list();

		assertTrue(variables.Count == 0);

		// complete ////////////////////////////////////////////////////////
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
		taskService.complete(taskId);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testVariablesRoundtrip.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testVariablesRoundtrip()
	  {
		// given
		VariableMap parameters = Variables.createVariables().putValue("aVariable", "xyz").putValue("anotherVariable", 999);

		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 1000).execute();

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when
		complete(humanTaskId);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(superProcessInstanceId).list();

		assertFalse(variables.Count == 0);
		assertEquals(2, variables.Count);

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
			assertEquals(1000, variable.Value);
		  }
		  else
		  {
			fail("Found an unexpected variable: '" + name + "'");
		  }
		}

		// complete ////////////////////////////////////////////////////////
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
		taskService.complete(taskId);
		assertProcessEnded(superProcessInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputAll.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testCallCaseOutputAllVariablesTypedToProcess()
	  {
		startProcessInstanceByKey("process");
		CaseInstance caseInstance = queryOneTaskCaseInstance();
		string variableName = "foo";
		string variableName2 = "null";
		TypedValue variableValue = Variables.stringValue("bar");
		TypedValue variableValue2 = Variables.integerValue(null);
		caseService.withCaseExecution(caseInstance.Id).setVariable(variableName, variableValue).setVariable(variableName2, variableValue2).execute();
		complete(caseInstance.Id);

		Task task = taskService.createTaskQuery().singleResult();
		TypedValue value = runtimeService.getVariableTyped(task.ProcessInstanceId, variableName);
		assertThat(value, @is(variableValue));
		value = runtimeService.getVariableTyped(task.ProcessInstanceId, variableName2);
		assertThat(value, @is(variableValue2));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testDeleteProcessInstance()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		// when
		runtimeService.deleteProcessInstance(superProcessInstanceId, null);

		// then
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		CaseInstance subCaseInstance = queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		// complete ////////////////////////////////////////////////////////
		terminate(subCaseInstanceId);
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testSuspendProcessInstance()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

		// when (1)
		runtimeService.suspendProcessInstanceById(superProcessInstanceId);

		// then
		Execution superProcessInstance = queryExecutionById(superProcessInstanceId);
		assertNotNull(superProcessInstance);
		assertTrue(superProcessInstance.Suspended);

		CaseInstance subCaseInstance = queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		try
		{
		  // when
		  complete(humanTaskId);
		  fail("The super process instance is suspended.");
		}
		catch (Exception)
		{
		  // expected
		}

		// complete ////////////////////////////////////////////////////////
		runtimeService.activateProcessInstanceById(superProcessInstanceId);

		complete(humanTaskId);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstanceId);
		assertProcessEnded(superProcessInstanceId);

		repositoryService.deleteDeployment(deploymentId, true);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testTerminateSubCaseInstance()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		// when
		terminate(subCaseInstanceId);

		// then
		CmmnExecution subCaseInstance = (CmmnExecution) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Terminated);

		Execution callActivity = queryExecutionByActivityId(CALL_ACTIVITY_ID);
		assertNotNull(callActivity);

		// complete ////////////////////////////////////////////////////////

		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		runtimeService.deleteProcessInstance(superProcessInstanceId, null);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testSuspendSubCaseInstance()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		// when
		suspend(subCaseInstanceId);

		// then
		CmmnExecution subCaseInstance = (CmmnExecution) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Suspended);

		Execution callActivity = queryExecutionByActivityId(CALL_ACTIVITY_ID);
		assertNotNull(callActivity);

		// complete ////////////////////////////////////////////////////////

		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		runtimeService.deleteProcessInstance(superProcessInstanceId, null);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCompletionOfCaseWithTwoTasks.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn" })]
	  public virtual void testCompletionOfTwoHumanTasks()
	  {
		// given
		string superProcessInstanceId = startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		// when (1)
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
		manualStart(humanTaskId);
		complete(humanTaskId);

		// then (1)

		assertEquals(0, taskService.createTaskQuery().count());

		// when (2)
		humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_2").Id;
		manualStart(humanTaskId);
		complete(humanTaskId);

		// then (2)
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		assertEquals(superProcessInstanceId, task.ProcessInstanceId);
		assertEquals("userTask", task.TaskDefinitionKey);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalInputAllVariables.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testSubProcessLocalInputAllVariables()
	  {
		ProcessInstance processInstance = startProcessInstanceByKey("subProcessLocalInputAllVariables");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when setting a variable in a process instance
		runtimeService.setVariable(processInstance.Id, "callingProcessVar1", "val1");

		// and executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then only the local variable specified in the io mapping is passed to the called instance
		CaseExecutionEntity calledInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(calledInstance);

		IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
		assertEquals(1, calledInstanceVariables.Count);
		assertEquals("val2", calledInstanceVariables["inputParameter"]);

		// when setting a variable in the called instance
		caseService.setVariable(calledInstance.Id, "calledCaseVar1", 42L);

		// and completing it
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
		complete(humanTaskId);

		// then the call activity output variable has been mapped to the process instance execution
		// and the output mapping variable as well
		IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
		assertEquals(3, callingInstanceVariables.Count);
		assertEquals("val1", callingInstanceVariables["callingProcessVar1"]);
		assertEquals(42L, callingInstanceVariables["calledCaseVar1"]);
		assertEquals(43L, callingInstanceVariables["outputParameter"]);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalInputSingleVariable.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testSubProcessLocalInputSingleVariable()
	  {
		ProcessInstance processInstance = startProcessInstanceByKey("subProcessLocalInputSingleVariable");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when setting a variable in a process instance
		runtimeService.setVariable(processInstance.Id, "callingProcessVar1", "val1");

		// and executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then the local variable specified in the io mapping is passed to the called instance
		CaseExecutionEntity calledInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(calledInstance);

		IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
		assertEquals(1, calledInstanceVariables.Count);
		assertEquals("val2", calledInstanceVariables["mappedInputParameter"]);

		// when setting a variable in the called instance
		caseService.setVariable(calledInstance.Id, "calledCaseVar1", 42L);

		// and completing it
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
		complete(humanTaskId);

		// then the call activity output variable has been mapped to the process instance execution
		// and the output mapping variable as well
		IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
		assertEquals(4, callingInstanceVariables.Count);
		assertEquals("val1", callingInstanceVariables["callingProcessVar1"]);
		assertEquals("val2", callingInstanceVariables["mappedInputParameter"]);
		assertEquals(42L, callingInstanceVariables["calledCaseVar1"]);
		assertEquals(43L, callingInstanceVariables["outputParameter"]);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalInputSingleVariableExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testSubProcessLocalInputSingleVariableExpression()
	  {
		ProcessInstance processInstance = startProcessInstanceByKey("subProcessLocalInputSingleVariableExpression");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then the local input parameter can be resolved because its source expression variable
		// is defined in the call activity's input mapping
		CaseExecutionEntity calledInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(calledInstance);

		IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
		assertEquals(1, calledInstanceVariables.Count);
		assertEquals(43L, calledInstanceVariables["mappedInputParameter"]);

		// and completing it
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
		complete(humanTaskId);

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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalOutputAllVariables.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testSubProcessLocalOutputAllVariables()
	  {
		ProcessInstance processInstance = startProcessInstanceByKey("subProcessLocalOutputAllVariables");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when setting a variable in a process instance
		runtimeService.setVariable(processInstance.Id, "callingProcessVar1", "val1");

		// and executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then all variables have been mapped into the called instance
		CaseExecutionEntity calledInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(calledInstance);

		IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
		assertEquals(2, calledInstanceVariables.Count);
		assertEquals("val1", calledInstanceVariables["callingProcessVar1"]);
		assertEquals("val2", calledInstanceVariables["inputParameter"]);

		// when setting a variable in the called instance
		caseService.setVariable(calledInstance.Id, "calledCaseVar1", 42L);

		// and completing it
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
		manualStart(humanTaskId);
		complete(humanTaskId);

		// then only the output mapping variable has been mapped into the calling process instance
		IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
		assertEquals(2, callingInstanceVariables.Count);
		assertEquals("val1", callingInstanceVariables["callingProcessVar1"]);
		assertEquals(43L, callingInstanceVariables["outputParameter"]);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalOutputSingleVariable.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testSubProcessLocalOutputSingleVariable()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessLocalOutputSingleVariable");
		Task beforeCallActivityTask = taskService.createTaskQuery().singleResult();

		// when setting a variable in a process instance
		runtimeService.setVariable(processInstance.Id, "callingProcessVar1", "val1");

		// and executing the call activity
		taskService.complete(beforeCallActivityTask.Id);

		// then all variables have been mapped into the called instance
		CaseExecutionEntity calledInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(calledInstance);

		IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
		assertEquals(2, calledInstanceVariables.Count);
		assertEquals("val1", calledInstanceVariables["callingProcessVar1"]);
		assertEquals("val2", calledInstanceVariables["inputParameter"]);

		// when setting a variable in the called instance
		caseService.setVariable(calledInstance.Id, "calledCaseVar1", 42L);

		// and completing it
		string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
		complete(humanTaskId);

		// then only the output mapping variable has been mapped into the calling process instance
		IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
		assertEquals(2, callingInstanceVariables.Count);
		assertEquals("val1", callingInstanceVariables["callingProcessVar1"]);
		assertEquals(43L, callingInstanceVariables["outputParameter"]);
	  }

	  protected internal virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey)
	  {
		return startProcessInstanceByKey(processDefinitionKey, null);
	  }

	  protected internal virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, IDictionary<string, object> variables)
	  {
		return startProcessInstanceByKey(processDefinitionKey, variables, null);
	  }

	  protected internal virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, IDictionary<string, object> variables, string businessKey)
	  {
		return runtimeService.startProcessInstanceByKey(processDefinitionKey, businessKey, variables);
	  }

	  protected internal override CaseExecution queryCaseExecutionById(string id)
	  {
		return caseService.createCaseExecutionQuery().caseExecutionId(id).singleResult();
	  }

	  protected internal override CaseExecution queryCaseExecutionByActivityId(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).singleResult();
	  }

	  protected internal virtual CaseInstance queryOneTaskCaseInstance()
	  {
		return caseService.createCaseInstanceQuery().caseDefinitionKey(ONE_TASK_CASE).singleResult();
	  }

	  protected internal virtual Execution queryExecutionById(string id)
	  {
		return runtimeService.createExecutionQuery().executionId(id).singleResult();
	  }

	  protected internal virtual Execution queryExecutionByActivityId(string activityId)
	  {
		return runtimeService.createExecutionQuery().activityId(activityId).singleResult();
	  }

	  protected internal virtual Task queryTaskByActivityId(string activityId)
	  {
		return taskService.createTaskQuery().taskDefinitionKey(activityId).singleResult();
	  }

	}

}