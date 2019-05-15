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
namespace org.camunda.bpm.engine.test.api.repository
{

	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using EmbeddedProcessApplication = org.camunda.bpm.application.impl.EmbeddedProcessApplication;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Query = org.camunda.bpm.engine.query.Query;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using Resource = org.camunda.bpm.engine.repository.Resource;
	using ResumePreviousBy = org.camunda.bpm.engine.repository.ResumePreviousBy;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class RedeploymentTest : PluggableProcessEngineTestCase
	{

	  public const string DEPLOYMENT_NAME = "my-deployment";
	  public const string PROCESS_KEY = "process";
	  public const string PROCESS_1_KEY = "process-1";
	  public const string PROCESS_2_KEY = "process-2";
	  public const string PROCESS_3_KEY = "process-3";
	  public const string RESOURCE_NAME = "path/to/my/process.bpmn";
	  public const string RESOURCE_1_NAME = "path/to/my/process1.bpmn";
	  public const string RESOURCE_2_NAME = "path/to/my/process2.bpmn";
	  public const string RESOURCE_3_NAME = "path/to/my/process3.bpmn";

	  public virtual void testRedeployInvalidDeployment()
	  {

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources("not-existing").deploy();
		  fail("It should not be able to re-deploy an unexisting deployment");
		}
		catch (NotFoundException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById("not-existing", "an-id").deploy();
		  fail("It should not be able to re-deploy an unexisting deployment");
		}
		catch (NotFoundException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesById("not-existing", Arrays.asList("an-id")).deploy();
		  fail("It should not be able to re-deploy an unexisting deployment");
		}
		catch (NotFoundException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceByName("not-existing", "a-name").deploy();
		  fail("It should not be able to re-deploy an unexisting deployment");
		}
		catch (NotFoundException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesByName("not-existing", Arrays.asList("a-name")).deploy();
		  fail("It should not be able to re-deploy an unexisting deployment");
		}
		catch (NotFoundException)
		{
		  // expected
		}
	  }

	  public virtual void testNotValidDeploymentId()
	  {
		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(null);
		  fail("It should not be possible to pass a null deployment id");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById(null, "an-id");
		  fail("It should not be possible to pass a null deployment id");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesById(null, Arrays.asList("an-id"));
		  fail("It should not be possible to pass a null deployment id");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceByName(null, "a-name");
		  fail("It should not be possible to pass a null deployment id");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesByName(null, Arrays.asList("a-name"));
		  fail("It should not be possible to pass a null deployment id");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	  public virtual void testRedeployUnexistingDeploymentResource()
	  {
		// given
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).deploy();

		try
		{
		  // when
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceByName(deployment.Id, "not-existing-resource.bpmn").deploy();
		  fail("It should not be possible to re-deploy a not existing deployment resource");
		}
		catch (NotFoundException)
		{
		  // then
		  // expected
		}

		try
		{
		  // when
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesByName(deployment.Id, Arrays.asList("not-existing-resource.bpmn")).deploy();
		  fail("It should not be possible to re-deploy a not existing deployment resource");
		}
		catch (NotFoundException)
		{
		  // then
		  // expected
		}

		try
		{
		  // when
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById(deployment.Id, "not-existing-resource-id").deploy();
		  fail("It should not be possible to re-deploy a not existing deployment resource");
		}
		catch (NotFoundException)
		{
		  // then
		  // expected
		}

		try
		{
		  // when
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesById(deployment.Id, Arrays.asList("not-existing-resource-id")).deploy();
		  fail("It should not be possible to re-deploy a not existing deployment resource");
		}
		catch (NotFoundException)
		{
		  // then
		  // expected
		}

		deleteDeployments(deployment);
	  }

	  public virtual void testNotValidResource()
	  {
		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById("an-id", null);
		  fail("It should not be possible to pass a null resource id");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesById("an-id", null);
		  fail("It should not be possible to pass a null resource id");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesById("an-id", Arrays.asList((string)null));
		  fail("It should not be possible to pass a null resource id");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesById("an-id", new List<string>());
		  fail("It should not be possible to pass a null resource id");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceByName("an-id", null);
		  fail("It should not be possible to pass a null resource name");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesByName("an-id", null);
		  fail("It should not be possible to pass a null resource name");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesByName("an-id", Arrays.asList((string)null));
		  fail("It should not be possible to pass a null resource name");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesByName("an-id", new List<string>());
		  fail("It should not be possible to pass a null resource name");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	  public virtual void testRedeployNewDeployment()
	  {
		// given
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).deploy();

		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentName(DEPLOYMENT_NAME);

		assertNotNull(deployment1.Id);
		verifyQueryResults(query, 1);

		// when
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		// then
		assertNotNull(deployment2);
		assertNotNull(deployment2.Id);
		assertFalse(deployment1.Id.Equals(deployment2.Id));

		verifyQueryResults(query, 2);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testFailingDeploymentName()
	  {
		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME).nameFromDeployment("a-deployment-id");
		  fail("Cannot set name() and nameFromDeployment().");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().nameFromDeployment("a-deployment-id").name(DEPLOYMENT_NAME);
		  fail("Cannot set name() and nameFromDeployment().");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	  public virtual void testRedeployDeploymentName()
	  {
		// given
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).deploy();

		assertEquals(DEPLOYMENT_NAME, deployment1.Name);

		// when
		Deployment deployment2 = repositoryService.createDeployment().nameFromDeployment(deployment1.Id).addDeploymentResources(deployment1.Id).deploy();

		// then
		assertNotNull(deployment2);
		assertEquals(deployment1.Name, deployment2.Name);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testRedeployDeploymentDifferentName()
	  {
		// given
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).deploy();

		assertEquals(DEPLOYMENT_NAME, deployment1.Name);

		// when
		Deployment deployment2 = repositoryService.createDeployment().name("my-another-deployment").addDeploymentResources(deployment1.Id).deploy();

		// then
		assertNotNull(deployment2);
		assertFalse(deployment1.Name.Equals(deployment2.Name));

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testRedeployDeploymentSourcePropertyNotSet()
	  {
		// given
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).source("my-deployment-source").addModelInstance(RESOURCE_NAME, model).deploy();

		assertEquals("my-deployment-source", deployment1.Source);

		// when
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		// then
		assertNotNull(deployment2);
		assertNull(deployment2.Source);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testRedeploySetDeploymentSourceProperty()
	  {
		// given
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).source("my-deployment-source").addModelInstance(RESOURCE_NAME, model).deploy();

		assertEquals("my-deployment-source", deployment1.Source);

		// when
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).source("my-another-deployment-source").deploy();

		// then
		assertNotNull(deployment2);
		assertEquals("my-another-deployment-source", deployment2.Source);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testRedeployDeploymentResource()
	  {
		// given

		// first deployment
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).deploy();

		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_NAME);

		// second deployment
		model = createProcessWithUserTask(PROCESS_KEY);
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).deploy();

		Resource resource2 = getResourceByName(deployment2.Id, RESOURCE_NAME);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		// then
		Resource resource3 = getResourceByName(deployment3.Id, RESOURCE_NAME);
		assertNotNull(resource3);

		// id
		assertNotNull(resource3.Id);
		assertFalse(resource1.Id.Equals(resource3.Id));

		// deployment id
		assertEquals(deployment3.Id, resource3.DeploymentId);

		// name
		assertEquals(resource1.Name, resource3.Name);

		// bytes
		sbyte[] bytes1 = ((ResourceEntity) resource1).Bytes;
		sbyte[] bytes2 = ((ResourceEntity) resource2).Bytes;
		sbyte[] bytes3 = ((ResourceEntity) resource3).Bytes;
		assertTrue(Arrays.Equals(bytes1, bytes3));
		assertFalse(Arrays.Equals(bytes2, bytes3));

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployAllDeploymentResources()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		// second deployment
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model2).addModelInstance(RESOURCE_2_NAME, model1).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 3);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployOneDeploymentResourcesByName()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployMultipleDeploymentResourcesByName()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);
		BpmnModelInstance model3 = createProcessWithScriptTask(PROCESS_3_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 1);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);
		model3 = createProcessWithUserTask(PROCESS_3_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 2);

		// when (1)
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).addDeploymentResourceByName(deployment1.Id, RESOURCE_3_NAME).deploy();

		// then (1)
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 3);

		// when (2)
		Deployment deployment4 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesByName(deployment2.Id, Arrays.asList(RESOURCE_1_NAME, RESOURCE_3_NAME)).deploy();

		// then (2)
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 4);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 4);

		deleteDeployments(deployment1, deployment2, deployment3, deployment4);
	  }

	  public virtual void testRedeployOneAndMultipleDeploymentResourcesByName()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);
		BpmnModelInstance model3 = createProcessWithScriptTask(PROCESS_3_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 1);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);
		model3 = createProcessWithUserTask(PROCESS_3_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).addDeploymentResourcesByName(deployment1.Id, Arrays.asList(RESOURCE_2_NAME, RESOURCE_3_NAME)).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 3);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testSameDeploymentResourceByName()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).addDeploymentResourcesByName(deployment1.Id, Arrays.asList(RESOURCE_1_NAME)).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployOneDeploymentResourcesById()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		Resource resource = getResourceByName(deployment1.Id, RESOURCE_1_NAME);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById(deployment1.Id, resource.Id).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployMultipleDeploymentResourcesById()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);
		BpmnModelInstance model3 = createProcessWithScriptTask(PROCESS_3_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 1);

		Resource resource11 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);
		Resource resource13 = getResourceByName(deployment1.Id, RESOURCE_3_NAME);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);
		model3 = createProcessWithUserTask(PROCESS_3_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 2);

		Resource resource21 = getResourceByName(deployment2.Id, RESOURCE_1_NAME);
		Resource resource23 = getResourceByName(deployment2.Id, RESOURCE_3_NAME);

		// when (1)
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById(deployment1.Id, resource11.Id).addDeploymentResourceById(deployment1.Id, resource13.Id).deploy();

		// then (1)
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 3);

		// when (2)
		Deployment deployment4 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourcesById(deployment2.Id, Arrays.asList(resource21.Id, resource23.Id)).deploy();

		// then (2)
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 4);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 4);

		deleteDeployments(deployment1, deployment2, deployment3, deployment4);
	  }

	  public virtual void testRedeployOneAndMultipleDeploymentResourcesById()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);
		BpmnModelInstance model3 = createProcessWithScriptTask(PROCESS_3_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 1);

		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);
		Resource resource2 = getResourceByName(deployment1.Id, RESOURCE_2_NAME);
		Resource resource3 = getResourceByName(deployment1.Id, RESOURCE_3_NAME);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);
		model3 = createProcessWithUserTask(PROCESS_3_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById(deployment1.Id, resource1.Id).addDeploymentResourcesById(deployment1.Id, Arrays.asList(resource2.Id, resource3.Id)).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 3);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeploySameDeploymentResourceById()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById(deployment1.Id, resource1.Id).addDeploymentResourcesById(deployment1.Id, Arrays.asList(resource1.Id)).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployDeploymentResourceByIdAndName()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);
		Resource resource2 = getResourceByName(deployment1.Id, RESOURCE_2_NAME);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResourceById(deployment1.Id, resource1.Id).addDeploymentResourceByName(deployment1.Id, resource2.Name).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 3);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployDeploymentResourceByIdAndNameMultiple()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		BpmnModelInstance model2 = createProcessWithUserTask(PROCESS_2_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);
		Resource resource2 = getResourceByName(deployment1.Id, RESOURCE_2_NAME);

		// second deployment
		model1 = createProcessWithScriptTask(PROCESS_1_KEY);
		model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).addModelInstance(RESOURCE_2_NAME, model2).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		// when
		Deployment deployment3 = repositoryService.createDeployment().addDeploymentResourcesById(deployment1.Id, Arrays.asList(resource1.Id)).addDeploymentResourcesByName(deployment1.Id, Arrays.asList(resource2.Name)).deploy();

		// then
		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 3);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 3);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployFormDifferentDeployments()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-1").addModelInstance(RESOURCE_1_NAME, model1).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment1.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);

		// second deployment
		BpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-2").addModelInstance(RESOURCE_2_NAME, model2).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment2.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-3").addDeploymentResources(deployment1.Id).addDeploymentResources(deployment2.Id).deploy();

		assertEquals(2, repositoryService.getDeploymentResources(deployment3.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployFormDifferentDeploymentsById()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-1").addModelInstance(RESOURCE_1_NAME, model1).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment1.Id).Count);
		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);

		// second deployment
		BpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-2").addModelInstance(RESOURCE_2_NAME, model2).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment2.Id).Count);
		Resource resource2 = getResourceByName(deployment2.Id, RESOURCE_2_NAME);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-3").addDeploymentResourceById(deployment1.Id, resource1.Id).addDeploymentResourceById(deployment2.Id, resource2.Id).deploy();

		assertEquals(2, repositoryService.getDeploymentResources(deployment3.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployFormDifferentDeploymentsByName()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-1").addModelInstance(RESOURCE_1_NAME, model1).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment1.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);

		// second deployment
		BpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-2").addModelInstance(RESOURCE_2_NAME, model2).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment2.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-3").addDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).addDeploymentResourceByName(deployment2.Id, RESOURCE_2_NAME).deploy();

		assertEquals(2, repositoryService.getDeploymentResources(deployment3.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployFormDifferentDeploymentsByNameAndId()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-1").addModelInstance(RESOURCE_1_NAME, model1).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment1.Id).Count);
		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_1_NAME);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);

		// second deployment
		BpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-2").addModelInstance(RESOURCE_2_NAME, model2).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment2.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		// when
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-3").addDeploymentResourceById(deployment1.Id, resource1.Id).addDeploymentResourceByName(deployment2.Id, RESOURCE_2_NAME).deploy();

		assertEquals(2, repositoryService.getDeploymentResources(deployment3.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployFormDifferentDeploymentsAddsNewSource()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-1").addModelInstance(RESOURCE_1_NAME, model1).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment1.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);

		// second deployment
		BpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-2").addModelInstance(RESOURCE_2_NAME, model2).deploy();

		assertEquals(1, repositoryService.getDeploymentResources(deployment2.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 1);

		// when
		BpmnModelInstance model3 = createProcessWithUserTask(PROCESS_3_KEY);
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-3").addDeploymentResources(deployment1.Id).addDeploymentResources(deployment2.Id).addModelInstance(RESOURCE_3_NAME, model3).deploy();

		assertEquals(3, repositoryService.getDeploymentResources(deployment3.Id).Count);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_2_KEY), 2);
		verifyQueryResults(query.processDefinitionKey(PROCESS_3_KEY), 1);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testRedeployFormDifferentDeploymentsSameResourceName()
	  {
		// given
		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-1").addModelInstance(RESOURCE_1_NAME, model1).deploy();

		// second deployment
		BpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-2").addModelInstance(RESOURCE_1_NAME, model2).deploy();

		// when
		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-3").addDeploymentResources(deployment1.Id).addDeploymentResources(deployment2.Id).deploy();
		  fail("It should not be possible to deploy different resources with same name.");
		}
		catch (NotValidException)
		{
		  // expected
		}

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testRedeployAndAddNewResourceWithSameName()
	  {
		// given
		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);

		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-1").addModelInstance(RESOURCE_1_NAME, model1).deploy();

		// when
		BpmnModelInstance model2 = createProcessWithReceiveTask(PROCESS_2_KEY);

		try
		{
		  repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-2").addModelInstance(RESOURCE_1_NAME, model2).addDeploymentResourceByName(deployment1.Id, RESOURCE_1_NAME).deploy();
		  fail("It should not be possible to deploy different resources with same name.");
		}
		catch (NotValidException)
		{
		  // expected
		}

		deleteDeployments(deployment1);
	  }

	  public virtual void testRedeployEnableDuplcateChecking()
	  {
		// given
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		BpmnModelInstance model1 = createProcessWithServiceTask(PROCESS_1_KEY);
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_1_NAME, model1).deploy();

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);

		// when
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).enableDuplicateFiltering(true).deploy();

		assertEquals(deployment1.Id, deployment2.Id);

		verifyQueryResults(query.processDefinitionKey(PROCESS_1_KEY), 1);

		deleteDeployments(deployment1);
	  }

	  public virtual void testSimpleProcessApplicationDeployment()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).enableDuplicateFiltering(true).deploy();

		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_NAME);

		// when
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(DEPLOYMENT_NAME).addDeploymentResourceById(deployment1.Id, resource1.Id).deploy();

		// then
		// registration was performed:
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(1, deploymentIds.Count);
		assertTrue(deploymentIds.Contains(deployment2.Id));

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testRedeployProcessApplicationDeploymentResumePreviousVersions()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		// first deployment
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).enableDuplicateFiltering(true).deploy();

		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_NAME);

		// second deployment
		model = createProcessWithUserTask(PROCESS_KEY);
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).enableDuplicateFiltering(true).deploy();

		// when
		ProcessApplicationDeployment deployment3 = repositoryService.createDeployment(processApplication.Reference).name(DEPLOYMENT_NAME).resumePreviousVersions().addDeploymentResourceById(deployment1.Id, resource1.Id).deploy();

		// then
		// old deployments was resumed
		ProcessApplicationRegistration registration = deployment3.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(3, deploymentIds.Count);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testProcessApplicationDeploymentResumePreviousVersionsByDeploymentName()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		// first deployment
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).enableDuplicateFiltering(true).deploy();

		Resource resource1 = getResourceByName(deployment1.Id, RESOURCE_NAME);

		// second deployment
		model = createProcessWithUserTask(PROCESS_KEY);
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).enableDuplicateFiltering(true).deploy();

		// when
		ProcessApplicationDeployment deployment3 = repositoryService.createDeployment(processApplication.Reference).name(DEPLOYMENT_NAME).resumePreviousVersions().resumePreviousVersionsBy(ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME).addDeploymentResourceById(deployment1.Id, resource1.Id).deploy();

		// then
		// old deployment was resumed
		ProcessApplicationRegistration registration = deployment3.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(3, deploymentIds.Count);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  // helper ///////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults<T1>(Query<T1> query, int countExpected)
	  {
		assertEquals(countExpected, query.count());
	  }

	  protected internal virtual Resource getResourceByName(string deploymentId, string resourceName)
	  {
		IList<Resource> resources = repositoryService.getDeploymentResources(deploymentId);

		foreach (Resource resource in resources)
		{
		  if (resource.Name.Equals(resourceName))
		  {
			return resource;
		  }
		}

		return null;
	  }

	  protected internal virtual void deleteDeployments(params Deployment[] deployments)
	  {
		foreach (Deployment deployment in deployments)
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  protected internal virtual BpmnModelInstance createProcessWithServiceTask(string key)
	  {
		return Bpmn.createExecutableProcess(key).startEvent().serviceTask().camundaExpression("${true}").endEvent().done();
	  }

	  protected internal virtual BpmnModelInstance createProcessWithUserTask(string key)
	  {
		return Bpmn.createExecutableProcess(key).startEvent().userTask().endEvent().done();
	  }

	  protected internal virtual BpmnModelInstance createProcessWithReceiveTask(string key)
	  {
		return Bpmn.createExecutableProcess(key).startEvent().receiveTask().endEvent().done();
	  }

	  protected internal virtual BpmnModelInstance createProcessWithScriptTask(string key)
	  {
		return Bpmn.createExecutableProcess(key).startEvent().scriptTask().scriptFormat("javascript").scriptText("return true").userTask().endEvent().done();
	  }

	}

}