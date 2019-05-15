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
namespace org.camunda.bpm.engine.test.bpmn.common
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AbstractProcessEngineServicesAccessTest : PluggableProcessEngineTestCase
	{

	  private const string TASK_DEF_KEY = "someTask";

	  private const string PROCESS_DEF_KEY = "testProcess";

	  private const string CALLED_PROCESS_DEF_ID = "calledProcess";

	  protected internal new IList<string> deploymentIds = new List<string>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		foreach (string deploymentId in deploymentIds)
		{
		  repositoryService.deleteDeployment(deploymentId, true);
		}
		base.tearDown();
	  }

	  public virtual void testServicesAccessible()
	  {
		// this test makes sure that the process engine services can be accessed and are non-null.
		createAndDeployModelForClass(TestServiceAccessibleClass);

		// this would fail if api access was not assured.
		runtimeService.startProcessInstanceByKey(PROCESS_DEF_KEY);
	  }

	  public virtual void testQueryAccessible()
	  {
		// this test makes sure we can perform a query
		createAndDeployModelForClass(QueryClass);

		// this would fail if api access was not assured.
		runtimeService.startProcessInstanceByKey(PROCESS_DEF_KEY);
	  }

	  public virtual void testStartProcessInstance()
	  {

		// given
		createAndDeployModelForClass(StartProcessInstanceClass);

		assertStartProcessInstance();
	  }

	  public virtual void testStartProcessInstanceFails()
	  {

		// given
		createAndDeployModelForClass(StartProcessInstanceClass);

		assertStartProcessInstanceFails();
	  }

	  public virtual void testProcessEngineStartProcessInstance()
	  {

		// given
		createAndDeployModelForClass(ProcessEngineStartProcessClass);

		assertStartProcessInstance();
	  }

	  protected internal virtual void assertStartProcessInstanceFails()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CALLED_PROCESS_DEF_ID).startEvent().scriptTask("scriptTask").scriptFormat("groovy").scriptText("throw new RuntimeException(\"BOOOM!\")").endEvent().done();

		deployModel(modelInstance);

		// if
		try
		{
		  runtimeService.startProcessInstanceByKey(PROCESS_DEF_KEY);
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  assertTextPresent("BOOOM", e.Message);
		}

		// then
		// starting the process fails and everything is rolled back:
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

	  protected internal abstract Type TestServiceAccessibleClass {get;}

	  protected internal abstract Type QueryClass {get;}

	  protected internal abstract Type StartProcessInstanceClass {get;}

	  protected internal abstract Type ProcessEngineStartProcessClass {get;}

	  protected internal abstract Task createModelAccessTask(BpmnModelInstance modelInstance, Type delegateClass);

	  // Helper methods //////////////////////////////////////////////

	  private void createAndDeployModelForClass(Type delegateClass)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_DEF_KEY).startEvent().manualTask("templateTask").endEvent().done();

		// replace the template task with the actual task provided by the subtask
		modelInstance.getModelElementById("templateTask").replaceWithElement(createModelAccessTask(modelInstance, delegateClass));

		deployModel(modelInstance);
	  }


	  private void deployModel(BpmnModelInstance model)
	  {
		Deployment deployment = repositoryService.createDeployment().addModelInstance("testProcess.bpmn", model).deploy();
		deploymentIds.Add(deployment.Id);
	  }


	  protected internal virtual void assertStartProcessInstance()
	  {
		deployModel(Bpmn.createExecutableProcess(CALLED_PROCESS_DEF_ID).startEvent().userTask(TASK_DEF_KEY).endEvent().done());

		// if
		runtimeService.startProcessInstanceByKey(PROCESS_DEF_KEY);

		// then
		// the started process instance is still active and waiting at the user task
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey(TASK_DEF_KEY).count());
	  }

	  public virtual void testProcessEngineStartProcessInstanceFails()
	  {

		// given
		createAndDeployModelForClass(ProcessEngineStartProcessClass);

		assertStartProcessInstanceFails();
	  }

	  public static void assertCanAccessServices(ProcessEngineServices services)
	  {
		Assert.assertNotNull(services.AuthorizationService);
		Assert.assertNotNull(services.FormService);
		Assert.assertNotNull(services.HistoryService);
		Assert.assertNotNull(services.IdentityService);
		Assert.assertNotNull(services.ManagementService);
		Assert.assertNotNull(services.RepositoryService);
		Assert.assertNotNull(services.RuntimeService);
		Assert.assertNotNull(services.TaskService);
	  }

	  public static void assertCanPerformQuery(ProcessEngineServices services)
	  {
		services.RepositoryService.createProcessDefinitionQuery().count();
	  }

	  public static void assertCanStartProcessInstance(ProcessEngineServices services)
	  {
		services.RuntimeService.startProcessInstanceByKey(CALLED_PROCESS_DEF_ID);
	  }

	  public static void assertCanStartProcessInstance(ProcessEngine processEngine)
	  {
		processEngine.RuntimeService.startProcessInstanceByKey(CALLED_PROCESS_DEF_ID);
	  }
	}

}