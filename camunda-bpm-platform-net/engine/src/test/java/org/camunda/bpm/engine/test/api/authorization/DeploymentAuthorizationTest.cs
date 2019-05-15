using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DEPLOYMENT;


	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using EmbeddedProcessApplication = org.camunda.bpm.application.impl.EmbeddedProcessApplication;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using AuthorizationQuery = org.camunda.bpm.engine.authorization.AuthorizationQuery;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using Resource = org.camunda.bpm.engine.repository.Resource;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DeploymentAuthorizationTest : AuthorizationTest
	{

	  private const string REQUIRED_ADMIN_AUTH_EXCEPTION = "ENGINE-03029 Required admin authenticated group or user.";
	  protected internal const string FIRST_RESOURCE = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string SECOND_RESOURCE = "org/camunda/bpm/engine/test/api/authorization/messageBoundaryEventProcess.bpmn20.xml";

	  // query ////////////////////////////////////////////////////////////

	  public virtual void testSimpleDeploymentQueryWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		// when
		DeploymentQuery query = repositoryService.createDeploymentQuery();

		// then
		verifyQueryResults(query, 0);

		deleteDeployment(deploymentId);
	  }

	  public virtual void testSimpleDeploymentQueryWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, deploymentId, userId, READ);

		// when
		DeploymentQuery query = repositoryService.createDeploymentQuery();

		// then
		verifyQueryResults(query, 1);

		deleteDeployment(deploymentId);
	  }

	  public virtual void testSimpleDeploymentQueryWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		// when
		DeploymentQuery query = repositoryService.createDeploymentQuery();

		// then
		verifyQueryResults(query, 1);

		deleteDeployment(deploymentId);
	  }

	  public virtual void testSimpleDeploymentQueryWithMultiple()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, deploymentId, userId, READ);
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		// when
		DeploymentQuery query = repositoryService.createDeploymentQuery();

		// then
		verifyQueryResults(query, 1);

		deleteDeployment(deploymentId);
	  }

	  public virtual void testDeploymentQueryWithoutAuthorization()
	  {
		// given
		string deploymentId1 = createDeployment("first");
		string deploymentId2 = createDeployment("second");

		// when
		DeploymentQuery query = repositoryService.createDeploymentQuery();

		// then
		verifyQueryResults(query, 0);

		deleteDeployment(deploymentId1);
		deleteDeployment(deploymentId2);
	  }

	  public virtual void testDeploymentQueryWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId1 = createDeployment("first");
		string deploymentId2 = createDeployment("second");
		createGrantAuthorization(DEPLOYMENT, deploymentId1, userId, READ);

		// when
		DeploymentQuery query = repositoryService.createDeploymentQuery();

		// then
		verifyQueryResults(query, 1);

		deleteDeployment(deploymentId1);
		deleteDeployment(deploymentId2);
	  }

	  public virtual void testDeploymentQueryWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId1 = createDeployment("first");
		string deploymentId2 = createDeployment("second");
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		// when
		DeploymentQuery query = repositoryService.createDeploymentQuery();

		// then
		verifyQueryResults(query, 2);

		deleteDeployment(deploymentId1);
		deleteDeployment(deploymentId2);
	  }

	  // create deployment ///////////////////////////////////////////////

	  public virtual void testCreateDeploymentWithoutAuthoriatzion()
	  {
		// given

		try
		{
		  // when
		  repositoryService.createDeployment().addClasspathResource(FIRST_RESOURCE).deploy();
		  fail("Exception expected: It should not be possible to create a new deployment");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(CREATE.Name, message);
		  assertTextPresent(DEPLOYMENT.resourceName(), message);
		}
	  }

	  public virtual void testCreateDeployment()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, CREATE);

		// when
		Deployment deployment = repositoryService.createDeployment().addClasspathResource(FIRST_RESOURCE).deploy();

		// then
		disableAuthorization();
		DeploymentQuery query = repositoryService.createDeploymentQuery();
		verifyQueryResults(query, 1);
		enableAuthorization();

		deleteDeployment(deployment.Id);
	  }

	  // delete deployment //////////////////////////////////////////////

	  public virtual void testDeleteDeploymentWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		try
		{
		  // when
		  repositoryService.deleteDeployment(deploymentId);
		  fail("Exception expected: it should not be possible to delete a deployment");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(DEPLOYMENT.resourceName(), message);
		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testDeleteDeploymentWithDeletePermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, deploymentId, userId, DELETE);

		// when
		repositoryService.deleteDeployment(deploymentId);

		// then
		disableAuthorization();
		DeploymentQuery query = repositoryService.createDeploymentQuery();
		verifyQueryResults(query, 0);
		enableAuthorization();

		deleteDeployment(deploymentId);
	  }

	  public virtual void testDeleteDeploymentWithDeletePermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, ANY, userId, DELETE);

		// when
		repositoryService.deleteDeployment(deploymentId);

		// then
		disableAuthorization();
		DeploymentQuery query = repositoryService.createDeploymentQuery();
		verifyQueryResults(query, 0);
		enableAuthorization();

		deleteDeployment(deploymentId);
	  }

	  // get deployment resource names //////////////////////////////////

	  public virtual void testGetDeploymentResourceNamesWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		try
		{
		  // when
		  repositoryService.getDeploymentResourceNames(deploymentId);
		  fail("Exception expected: it should not be possible to retrieve deployment resource names");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(DEPLOYMENT.resourceName(), message);
		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetDeploymentResourceNamesWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, deploymentId, userId, READ);

		// when
		IList<string> names = repositoryService.getDeploymentResourceNames(deploymentId);

		// then
		assertFalse(names.Count == 0);
		assertEquals(2, names.Count);
		assertTrue(names.Contains(FIRST_RESOURCE));
		assertTrue(names.Contains(SECOND_RESOURCE));

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetDeploymentResourceNamesWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		// when
		IList<string> names = repositoryService.getDeploymentResourceNames(deploymentId);

		// then
		assertFalse(names.Count == 0);
		assertEquals(2, names.Count);
		assertTrue(names.Contains(FIRST_RESOURCE));
		assertTrue(names.Contains(SECOND_RESOURCE));

		deleteDeployment(deploymentId);
	  }

	  // get deployment resources //////////////////////////////////

	  public virtual void testGetDeploymentResourcesWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		try
		{
		  // when
		  repositoryService.getDeploymentResources(deploymentId);
		  fail("Exception expected: it should not be possible to retrieve deployment resources");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(DEPLOYMENT.resourceName(), message);
		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetDeploymentResourcesWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, deploymentId, userId, READ);

		// when
		IList<Resource> resources = repositoryService.getDeploymentResources(deploymentId);

		// then
		assertFalse(resources.Count == 0);
		assertEquals(2, resources.Count);

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetDeploymentResourcesWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		// when
		IList<Resource> resources = repositoryService.getDeploymentResources(deploymentId);

		// then
		assertFalse(resources.Count == 0);
		assertEquals(2, resources.Count);

		deleteDeployment(deploymentId);
	  }

	  // get resource as stream //////////////////////////////////

	  public virtual void testGetResourceAsStreamWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		try
		{
		  // when
		  repositoryService.getResourceAsStream(deploymentId, FIRST_RESOURCE);
		  fail("Exception expected: it should not be possible to retrieve a resource as stream");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(DEPLOYMENT.resourceName(), message);
		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetResourceAsStreamWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, deploymentId, userId, READ);

		// when
		Stream stream = repositoryService.getResourceAsStream(deploymentId, FIRST_RESOURCE);

		// then
		assertNotNull(stream);

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetResourceAsStreamWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		// when
		Stream stream = repositoryService.getResourceAsStream(deploymentId, FIRST_RESOURCE);

		// then
		assertNotNull(stream);

		deleteDeployment(deploymentId);
	  }

	  // get resource as stream by id//////////////////////////////////

	  public virtual void testGetResourceAsStreamByIdWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		disableAuthorization();
		IList<Resource> resources = repositoryService.getDeploymentResources(deploymentId);
		enableAuthorization();
		string resourceId = resources[0].Id;

		try
		{
		  // when
		  repositoryService.getResourceAsStreamById(deploymentId, resourceId);
		  fail("Exception expected: it should not be possible to retrieve a resource as stream");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(DEPLOYMENT.resourceName(), message);
		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetResourceAsStreamByIdWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, deploymentId, userId, READ);

		disableAuthorization();
		IList<Resource> resources = repositoryService.getDeploymentResources(deploymentId);
		enableAuthorization();
		string resourceId = resources[0].Id;

		// when
		Stream stream = repositoryService.getResourceAsStreamById(deploymentId, resourceId);

		// then
		assertNotNull(stream);

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetResourceAsStreamByIdWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(DEPLOYMENT, ANY, userId, READ);

		disableAuthorization();
		IList<Resource> resources = repositoryService.getDeploymentResources(deploymentId);
		enableAuthorization();
		string resourceId = resources[0].Id;

		// when
		Stream stream = repositoryService.getResourceAsStreamById(deploymentId, resourceId);

		// then
		assertNotNull(stream);

		deleteDeployment(deploymentId);
	  }

	  // should create authorization /////////////////////////////////////

	  public virtual void testCreateAuthorizationOnDeploy()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, CREATE);
		Deployment deployment = repositoryService.createDeployment().addClasspathResource(FIRST_RESOURCE).deploy();

		// when
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn(userId).resourceId(deployment.Id).singleResult();

		// then
		assertNotNull(authorization);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DELETE));
		assertFalse(authorization.isPermissionGranted(UPDATE));

		deleteDeployment(deployment.Id);
	  }

	  // clear authorization /////////////////////////////////////

	  public virtual void testClearAuthorizationOnDeleteDeployment()
	  {
		// given
		createGrantAuthorization(DEPLOYMENT, ANY, userId, CREATE);
		Deployment deployment = repositoryService.createDeployment().addClasspathResource(FIRST_RESOURCE).deploy();

		string deploymentId = deployment.Id;

		AuthorizationQuery query = authorizationService.createAuthorizationQuery().userIdIn(userId).resourceId(deploymentId);

		Authorization authorization = query.singleResult();
		assertNotNull(authorization);

		// when
		repositoryService.deleteDeployment(deploymentId);

		authorization = query.singleResult();
		assertNull(authorization);

		deleteDeployment(deploymentId);
	  }

	  // register process application ///////////////////////////////////

	  public virtual void testRegisterProcessApplicationWithoutAuthorization()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		ProcessApplicationReference reference = processApplication.Reference;
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		try
		{
		  // when
		  managementService.registerProcessApplication(deploymentId, reference);
		  fail("Exception expected: It should not be possible to register a process application");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);

		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testRegisterProcessApplicationAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		ProcessApplicationReference reference = processApplication.Reference;
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		// when
		ProcessApplicationRegistration registration = managementService.registerProcessApplication(deploymentId, reference);

		// then
		assertNotNull(registration);
		assertNotNull(getProcessApplicationForDeployment(deploymentId));

		deleteDeployment(deploymentId);
	  }

	  // unregister process application ///////////////////////////////////

	  public virtual void testUnregisterProcessApplicationWithoutAuthorization()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;
		ProcessApplicationReference reference = processApplication.Reference;
		registerProcessApplication(deploymentId, reference);

		try
		{
		  // when
		  managementService.unregisterProcessApplication(deploymentId, true);
		  fail("Exception expected: It should not be possible to unregister a process application");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);

		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testUnregisterProcessApplicationAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;
		ProcessApplicationReference reference = processApplication.Reference;
		registerProcessApplication(deploymentId, reference);

		// when
		managementService.unregisterProcessApplication(deploymentId, true);

		// then
		assertNull(getProcessApplicationForDeployment(deploymentId));

		deleteDeployment(deploymentId);
	  }

	  // get process application for deployment ///////////////////////////////////

	  public virtual void testGetProcessApplicationForDeploymentWithoutAuthorization()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;
		ProcessApplicationReference reference = processApplication.Reference;
		registerProcessApplication(deploymentId, reference);

		try
		{
		  // when
		  managementService.getProcessApplicationForDeployment(deploymentId);
		  fail("Exception expected: It should not be possible to get the process application");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);

		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetProcessApplicationForDeploymentAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;
		ProcessApplicationReference reference = processApplication.Reference;
		registerProcessApplication(deploymentId, reference);

		// when
		string application = managementService.getProcessApplicationForDeployment(deploymentId);

		// then
		assertNotNull(application);

		deleteDeployment(deploymentId);
	  }

	  // get registered deployments ///////////////////////////////////

	  public virtual void testGetRegisteredDeploymentsWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		try
		{
		  // when
		  managementService.RegisteredDeployments;
		  fail("Exception expected: It should not be possible to get the registered deployments");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);

		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetRegisteredDeploymentsAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		// when
		ISet<string> deployments = managementService.RegisteredDeployments;

		// then
		assertTrue(deployments.Contains(deploymentId));

		deleteDeployment(deploymentId);
	  }

	  // register deployment for job executor ///////////////////////////////////

	  public virtual void testRegisterDeploymentForJobExecutorWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		try
		{
		  // when
		  managementService.registerDeploymentForJobExecutor(deploymentId);
		  fail("Exception expected: It should not be possible to register the deployment");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);

		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testRegisterDeploymentForJobExecutorAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		// when
		managementService.registerDeploymentForJobExecutor(deploymentId);

		// then
		assertTrue(RegisteredDeployments.Contains(deploymentId));

		deleteDeployment(deploymentId);
	  }

	  // unregister deployment for job executor ///////////////////////////////////

	  public virtual void testUnregisterDeploymentForJobExecutorWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		try
		{
		  // when
		  managementService.unregisterDeploymentForJobExecutor(deploymentId);
		  fail("Exception expected: It should not be possible to unregister the deployment");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);

		}

		deleteDeployment(deploymentId);
	  }

	  public virtual void testUnregisterDeploymentForJobExecutorAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		// when
		managementService.unregisterDeploymentForJobExecutor(deploymentId);

		// then
		assertFalse(RegisteredDeployments.Contains(deploymentId));

		deleteDeployment(deploymentId);
	  }

	  // helper /////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(DeploymentQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual string createDeployment(string name)
	  {
		return createDeployment(name, FIRST_RESOURCE, SECOND_RESOURCE).Id;
	  }

	  protected internal virtual void registerProcessApplication(string deploymentId, ProcessApplicationReference reference)
	  {
		disableAuthorization();
		managementService.registerProcessApplication(deploymentId, reference);
		enableAuthorization();
	  }

	  protected internal virtual string getProcessApplicationForDeployment(string deploymentId)
	  {
		disableAuthorization();
		string applications = managementService.getProcessApplicationForDeployment(deploymentId);
		enableAuthorization();
		return applications;
	  }

	  protected internal virtual ISet<string> RegisteredDeployments
	  {
		  get
		  {
			disableAuthorization();
			ISet<string> deployments = managementService.RegisteredDeployments;
			enableAuthorization();
			return deployments;
		  }
	  }

	}

}