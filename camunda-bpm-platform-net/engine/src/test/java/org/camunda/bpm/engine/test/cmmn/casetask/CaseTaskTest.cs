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
namespace org.camunda.bpm.engine.test.cmmn.casetask
{

	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseTaskTest : CmmnProcessEngineTestCase
	{

	  protected internal readonly string CASE_TASK = "PI_CaseTask_1";
	  protected internal readonly string ONE_CASE_TASK_CASE = "oneCaseTaskCase";
	  protected internal readonly string ONE_TASK_CASE = "oneTaskCase";

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseAsConstant()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		// then
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testCallCaseAsExpressionStartsWithDollar.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseAsExpressionStartsWithDollar()
	  {
		// given
		// a deployed case definition
		VariableMap vars = new VariableMapImpl();
		vars.putValue("oneTaskCase", ONE_TASK_CASE);
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE, vars).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testCallCaseAsExpressionStartsWithHash.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseAsExpressionStartsWithHash()
	  {
		// given
		// a deployed case definition
		VariableMap vars = new VariableMapImpl();
		vars.putValue("oneTaskCase", ONE_TASK_CASE);
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE, vars).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);
	  }

	  /// <summary>
	  /// assert on default behaviour - remove manual activation
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testCallLatestCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallLatestCase()
	  {
		// given
		string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

		string deploymentId = repositoryService.createDeployment().addClasspathResource(cmmnResourceName).deploy().Id;

		assertEquals(3, repositoryService.createCaseDefinitionQuery().count());

		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		string latestCaseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).latestVersion().singleResult().Id;

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);
		assertEquals(latestCaseDefinitionId, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  /// <summary>
	  /// default behaviour of manual activation changed - remove manual activation
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testCallCaseByDeployment.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseByDeployment()
	  {
		// given

		string firstDeploymentId = repositoryService.createDeploymentQuery().singleResult().Id;

		string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
		string deploymentId = repositoryService.createDeployment().addClasspathResource(cmmnResourceName).deploy().Id;

		assertEquals(3, repositoryService.createCaseDefinitionQuery().count());

		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		string caseDefinitionIdInSameDeployment = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).deploymentId(firstDeploymentId).singleResult().Id;

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);
		assertEquals(caseDefinitionIdInSameDeployment, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  /// <summary>
	  /// assertions on completion - take manual activation out
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testCallCaseByVersion.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseByVersion()
	  {
		// given

		string bpmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		assertEquals(4, repositoryService.createCaseDefinitionQuery().count());

		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		string caseDefinitionIdInSecondDeployment = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).deploymentId(secondDeploymentId).singleResult().Id;

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);
		assertEquals(caseDefinitionIdInSecondDeployment, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testCallCaseByVersionAsExpressionStartsWithDollar.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseByVersionAsExpressionStartsWithDollar()
	  {
		// given

		string bpmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		assertEquals(4, repositoryService.createCaseDefinitionQuery().count());

		VariableMap vars = new VariableMapImpl();
		vars.putValue("myVersion", 2);
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE,vars).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		string caseDefinitionIdInSecondDeployment = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).deploymentId(secondDeploymentId).singleResult().Id;

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);
		assertEquals(caseDefinitionIdInSecondDeployment, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testCallCaseByVersionAsExpressionStartsWithHash.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCallCaseByVersionAsExpressionStartsWithHash()
	  {
		// given

		string bpmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		string thirdDeploymentId = repositoryService.createDeployment().addClasspathResource(bpmnResourceName).deploy().Id;

		assertEquals(4, repositoryService.createCaseDefinitionQuery().count());

		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		string caseDefinitionIdInSecondDeployment = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).deploymentId(secondDeploymentId).singleResult().Id;

		// when
		caseService.withCaseExecution(caseTaskId).setVariable("myVersion", 2).manualStart();

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);
		assertEquals(caseDefinitionIdInSecondDeployment, subCaseInstance.CaseDefinitionId);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

		repositoryService.deleteDeployment(secondDeploymentId, true);
		repositoryService.deleteDeployment(thirdDeploymentId, true);
	  }

	  /// <summary>
	  /// assertion on default behaviour - remove manual activation
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testInputBusinessKey.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputBusinessKey()
	  {
		// given
		string businessKey = "myBusinessKey";
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE, businessKey).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);
		assertEquals(businessKey, subCaseInstance.BusinessKey);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// variable passed in manual activation - change process definition
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testInputDifferentBusinessKey.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputDifferentBusinessKey()
	  {
		// given
		string businessKey = "myBusinessKey";
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE, businessKey).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		// when
		caseService.withCaseExecution(caseTaskId).setVariable("myOwnBusinessKey", "myOwnBusinessKey").manualStart();

		// then

		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		string superCaseExecutionId = subCaseInstance.SuperCaseExecutionId;
		CaseExecution superCaseExecution = queryCaseExecutionById(superCaseExecutionId);

		assertEquals(caseTaskId, superCaseExecutionId);
		assertEquals(superCaseInstanceId, superCaseExecution.CaseInstanceId);
		assertEquals("myOwnBusinessKey", subCaseInstance.BusinessKey);

		// complete ////////////////////////////////////////////////////////

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assertion on variables which are set on manual start - change process definition
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testInputSourceWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSource()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		// when
		caseService.withCaseExecution(caseTaskId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).setVariable("aThirdVariable", "def").manualStart();

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

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// default manual activation behaviour changed - remove manual activation statement
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testInputSourceDifferentTarget.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSourceDifferentTarget()
	  {
		// given
		VariableMap vars = new VariableMapImpl();
		vars.putValue("aVariable", "abc");
		vars.putValue("anotherVariable", 999);
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE, vars).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

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

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assertion on default execution - take manual start out
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testInputSource.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSourceNullValue()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

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

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);
	  }

	  /// <summary>
	  /// Default manual activation changed - add variables to case instantiation, remove manual activation
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testInputSourceExpression.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputSourceExpression()
	  {
		// given
		VariableMap vars = new VariableMapImpl();
		vars.putValue("aVariable", "abc");
		vars.putValue("anotherVariable", 999);
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE,vars).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

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

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testInputAll.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputAll()
	  {
		// given
		VariableMap vars = new VariableMapImpl();
		vars.putValue("aVariable", "abc");
		vars.putValue("anotherVariable", 999);
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE, vars).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

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

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertCaseEnded(subCaseInstance.Id);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assert on variable defined during manual start - change process definition
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testInputAllLocal.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testInputAllLocal()
	  {
		// given
		createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		// when
		caseService.withCaseExecution(caseTaskId).setVariable("aVariable", "abc").setVariableLocal("aLocalVariable", "def").manualStart();

		// then only the local variable is mapped to the subCaseInstance
		CaseInstance subCaseInstance = queryOneTaskCaseInstance();

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id).list();

		assertEquals(1, variables.Count);
		assertEquals("aLocalVariable", variables[0].Name);
	  }

	  /// <summary>
	  /// assertion on manual activation operation - change process definition
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testCaseNotFound()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).manualStart();
		  fail("It should not be possible to start a not existing case instance.");
		}
		catch (NotFoundException)
		{
		}

		// complete //////////////////////////////////////////////////////////

		caseService.withCaseExecution(caseTaskId).disable();
		close(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assertion on completion - remove manual start
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCompleteSimpleCase()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;
		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then

		CaseExecution caseTask = queryCaseExecutionByActivityId("PI_CaseTask_1");
		assertNull(caseTask);

		// complete ////////////////////////////////////////////////////////

		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// subprocess manual start with variables - change process definition
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testOutputSource.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testOutputSource()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).setVariable("aThirdVariable", "def").execute();

		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		caseService.withCaseExecution(humanTaskId).manualStart();

		caseService.withCaseExecution(humanTaskId).complete();

		// when
		caseService.withCaseExecution(subCaseInstanceId).close();

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(superCaseInstanceId).list();

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

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// default behaviour of manual activation changed - remove manual activation
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testOutputSourceDifferentTarget.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputSourceDifferentTarget()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		caseService.withCaseExecution(humanTaskId).complete();

		// when
		caseService.withCaseExecution(subCaseInstanceId).close();

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(superCaseInstanceId).list();

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

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assertion on default behaviour - remove manual activations
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testOutputSource.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testOutputSourceNullValue()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;
		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		caseService.withCaseExecution(humanTaskId).complete();

		// when
		caseService.withCaseExecution(subCaseInstanceId).close();

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(superCaseInstanceId).list();

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

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assertion on variables - change process definition
	  /// manual start on case not needed enaymore and therefore removed
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testOutputSourceExpression.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testOutputSourceExpression()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		caseService.withCaseExecution(humanTaskId).manualStart();

		caseService.withCaseExecution(humanTaskId).complete();

		// when
		caseService.withCaseExecution(subCaseInstanceId).close();

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(superCaseInstanceId).list();

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

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// since assertion happens on variables, changing oneTaskCase definition to have manual activation,
	  /// case task behaviour changed, so manual activation is taken out
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testOutputAll.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testOutputAll()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		caseService.withCaseExecution(humanTaskId).manualStart();

		caseService.withCaseExecution(humanTaskId).complete();

		// when
		caseService.withCaseExecution(subCaseInstanceId).close();

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(superCaseInstanceId).list();

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

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testOutputVariablesShouldNotExistAnymore.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testOutputVariablesShouldNotExistAnymore()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		caseService.withCaseExecution(caseTaskId).setVariableLocal("aVariable", "xyz").setVariableLocal("anotherVariable", 123).manualStart();

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		caseService.withCaseExecution(humanTaskId).manualStart();

		caseService.withCaseExecution(humanTaskId).complete();

		// when
		caseService.withCaseExecution(subCaseInstanceId).close();

		// then

		// the variables has been deleted
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(superCaseInstanceId).list();

		assertTrue(variables.Count == 0);

		// complete ////////////////////////////////////////////////////////

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assertion on variables - change subprocess definition
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testVariablesRoundtrip.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testVariablesRoundtrip()
	  {
		// given

		VariableMap vars = new VariableMapImpl();
		vars.putValue("aVariable", "xyz");
		vars.putValue("anotherVariable", 123);
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE, vars).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		caseService.withCaseExecution(subCaseInstanceId).setVariable("aVariable", "abc").setVariable("anotherVariable", 999).execute();

		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		caseService.withCaseExecution(humanTaskId).manualStart();

		caseService.withCaseExecution(humanTaskId).complete();

		// when
		caseService.withCaseExecution(subCaseInstanceId).close();

		// then

		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(superCaseInstanceId).list();

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

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// Default behaviour changed, so manual start is taken out
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCompleteCaseTask()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).complete();
		  fail("It should not be possible to complete a case task, while the case instance is active.");
		}
		catch (NotAllowedException)
		{
		}


		// complete ////////////////////////////////////////////////////////

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;
		terminate(subCaseInstanceId);
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assert on default behaviour - remove manual activation
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testTerminateCaseTask()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		CaseInstance subCaseInstance = queryOneTaskCaseInstance();
		assertTrue(subCaseInstance.Active);

		// when
		terminate(caseTaskId);

		subCaseInstance = queryOneTaskCaseInstance();
		assertTrue(subCaseInstance.Active);

		// complete ////////////////////////////////////////////////////////

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;
		terminate(subCaseInstanceId);
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// removed manual start as it is handled by default behaviour
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testTerminateSubCaseInstance()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		// when
		terminate(subCaseInstanceId);

		// then
		CmmnExecution subCaseInstance = (CmmnExecution) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Terminated);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK);
		assertNotNull(caseTask);
		assertTrue(caseTask.Active);

		// complete ////////////////////////////////////////////////////////

		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assertion on completion - remove manual start
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testSuspendCaseTask()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		CaseInstance subCaseInstance = queryOneTaskCaseInstance();
		assertTrue(subCaseInstance.Active);

		// when
		suspend(caseTaskId);

		subCaseInstance = queryOneTaskCaseInstance();
		assertTrue(subCaseInstance.Active);

		// complete ////////////////////////////////////////////////////////

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;
		terminate(subCaseInstanceId);
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		terminate(superCaseInstanceId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// default behaviour of manual activation changed - remove manual activation
	  /// change definition of oneTaskCase in order to allow suspension state
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testSuspendSubCaseInstance()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;

		// when
		suspend(subCaseInstanceId);

		// then
		CmmnExecution subCaseInstance = (CmmnExecution) queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Suspended);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK);
		assertNotNull(caseTask);
		assertTrue(caseTask.Active);

		// complete ////////////////////////////////////////////////////////

		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testResumeCaseTask()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK).Id;

		suspend(caseTaskId);

		CaseInstance subCaseInstance = queryOneTaskCaseInstance();
		assertTrue(subCaseInstance.Active);

		// when
		resume(caseTaskId);

		// then
		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK);
		assertTrue(caseTask.Active);

		subCaseInstance = queryOneTaskCaseInstance();
		assertTrue(subCaseInstance.Active);

		// complete ////////////////////////////////////////////////////////

		string subCaseInstanceId = queryOneTaskCaseInstance().Id;
		terminate(subCaseInstanceId);
		close(subCaseInstanceId);
		assertCaseEnded(subCaseInstanceId);

		terminate(caseTaskId);
		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

	  }

	  /// <summary>
	  /// assert on default behaviour - remove manual activation
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/casetask/CaseTaskTest.testNotBlockingCaseTask.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testNotBlockingCaseTask()
	  {
		// given
		string superCaseInstanceId = createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;

		// then
		CaseInstance subCaseInstance = queryOneTaskCaseInstance();
		assertNotNull(subCaseInstance);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK);
		assertNull(caseTask);

		CaseInstance superCaseInstance = caseService.createCaseInstanceQuery().caseDefinitionKey(ONE_CASE_TASK_CASE).singleResult();
		assertNotNull(superCaseInstance);
		assertTrue(superCaseInstance.Completed);

		// complete ////////////////////////////////////////////////////////

		close(superCaseInstanceId);
		assertCaseEnded(superCaseInstanceId);

		terminate(subCaseInstance.Id);
		close(subCaseInstance.Id);
		assertProcessEnded(subCaseInstance.Id);

	  }

	  /// <summary>
	  /// Changed process definition as we prove activity type
	  /// </summary>
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testActivityType()
	  {
		// given
		createCaseInstanceByKey(ONE_CASE_TASK_CASE).Id;

		// when
		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK);

		// then
		assertEquals("caseTask", caseTask.ActivityType);
	  }

	  protected internal override CaseInstance createCaseInstanceByKey(string caseDefinitionKey)
	  {
		return createCaseInstanceByKey(caseDefinitionKey, null, null);
	  }

	  protected internal override CaseInstance createCaseInstanceByKey(string caseDefinitionKey, string businessKey)
	  {
		return caseService.withCaseDefinitionByKey(caseDefinitionKey).businessKey(businessKey).create();
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

	  protected internal virtual Task queryTask()
	  {
		return taskService.createTaskQuery().singleResult();
	  }

	}

}