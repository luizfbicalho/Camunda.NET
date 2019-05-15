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
namespace org.camunda.bpm.engine.test.api.repository
{

	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Ingo Richtsmeier
	/// </summary>
	public class DeploymentQueryTest : PluggableProcessEngineTestCase
	{

	  private string deploymentOneId;
	  private string deploymentTwoId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		deploymentOneId = repositoryService.createDeployment().name("org/camunda/bpm/engine/test/repository/one.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/repository/one.bpmn20.xml").source(org.camunda.bpm.engine.repository.ProcessApplicationDeployment_Fields.PROCESS_APPLICATION_DEPLOYMENT_SOURCE).deploy().Id;

		deploymentTwoId = repositoryService.createDeployment().name("org/camunda/bpm/engine/test/repository/two_.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/repository/two.bpmn20.xml").deploy().Id;

		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();
		repositoryService.deleteDeployment(deploymentOneId, true);
		repositoryService.deleteDeployment(deploymentTwoId, true);
	  }

	  public virtual void testQueryNoCriteria()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery();
		assertEquals(2, query.list().size());
		assertEquals(2, query.count());

		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByDeploymentId()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentId(deploymentOneId);
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidDeploymentId()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentId("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  repositoryService.createDeploymentQuery().deploymentId(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByName()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentName("org/camunda/bpm/engine/test/repository/two_.bpmn20.xml");
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidName()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentName("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  repositoryService.createDeploymentQuery().deploymentName(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNameLike()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentNameLike("%camunda%");
		assertEquals(2, query.list().size());
		assertEquals(2, query.count());

		query = repositoryService.createDeploymentQuery().deploymentNameLike("%two\\_%");
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
		assertEquals("org/camunda/bpm/engine/test/repository/two_.bpmn20.xml", query.singleResult().Name);
	  }

	  public virtual void testQueryByInvalidNameLike()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentNameLike("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  repositoryService.createDeploymentQuery().deploymentNameLike(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryByDeploymentBefore() throws Exception
	  public virtual void testQueryByDeploymentBefore()
	  {
		DateTime later = DateTimeUtil.now().plus(10 * 3600).toDate();
		DateTime earlier = DateTimeUtil.now().minus(10 * 3600).toDate();

		long count = repositoryService.createDeploymentQuery().deploymentBefore(later).count();
		assertEquals(2, count);

		count = repositoryService.createDeploymentQuery().deploymentBefore(earlier).count();
		assertEquals(0, count);

		try
		{
		  repositoryService.createDeploymentQuery().deploymentBefore(null);
		  fail("Exception expected");
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryDeploymentAfter() throws Exception
	  public virtual void testQueryDeploymentAfter()
	  {
		DateTime later = DateTimeUtil.now().plus(10 * 3600).toDate();
		DateTime earlier = DateTimeUtil.now().minus(10 * 3600).toDate();

		long count = repositoryService.createDeploymentQuery().deploymentAfter(later).count();
		assertEquals(0, count);

		count = repositoryService.createDeploymentQuery().deploymentAfter(earlier).count();
		assertEquals(2, count);

		try
		{
		  repositoryService.createDeploymentQuery().deploymentAfter(null);
		  fail("Exception expected");
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

	  public virtual void testQueryBySource()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentSource(org.camunda.bpm.engine.repository.ProcessApplicationDeployment_Fields.PROCESS_APPLICATION_DEPLOYMENT_SOURCE);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByNullSource()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentSource(null);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidSource()
	  {
		DeploymentQuery query = repositoryService.createDeploymentQuery().deploymentSource("invalid");

		assertEquals(0, query.list().size());
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryDeploymentBetween() throws Exception
	  public virtual void testQueryDeploymentBetween()
	  {
		DateTime later = DateTimeUtil.now().plus(10 * 3600).toDate();
		DateTime earlier = DateTimeUtil.now().minus(10 * 3600).toDate();

		long count = repositoryService.createDeploymentQuery().deploymentAfter(earlier).deploymentBefore(later).count();
		assertEquals(2, count);

		count = repositoryService.createDeploymentQuery().deploymentAfter(later).deploymentBefore(later).count();
		assertEquals(0, count);

		count = repositoryService.createDeploymentQuery().deploymentAfter(earlier).deploymentBefore(earlier).count();
		assertEquals(0, count);

		count = repositoryService.createDeploymentQuery().deploymentAfter(later).deploymentBefore(earlier).count();
		assertEquals(0, count);
	  }

	  public virtual void testVerifyDeploymentProperties()
	  {
		IList<Deployment> deployments = repositoryService.createDeploymentQuery().orderByDeploymentName().asc().list();

		Deployment deploymentOne = deployments[0];
		assertEquals("org/camunda/bpm/engine/test/repository/one.bpmn20.xml", deploymentOne.Name);
		assertEquals(deploymentOneId, deploymentOne.Id);
		assertEquals(org.camunda.bpm.engine.repository.ProcessApplicationDeployment_Fields.PROCESS_APPLICATION_DEPLOYMENT_SOURCE, deploymentOne.Source);
		assertNull(deploymentOne.TenantId);

		Deployment deploymentTwo = deployments[1];
		assertEquals("org/camunda/bpm/engine/test/repository/two_.bpmn20.xml", deploymentTwo.Name);
		assertEquals(deploymentTwoId, deploymentTwo.Id);
		assertNull(deploymentTwo.Source);
		assertNull(deploymentTwo.TenantId);
	  }

	  public virtual void testQuerySorting()
	  {
		assertEquals(2, repositoryService.createDeploymentQuery().orderByDeploymentName().asc().list().size());

		assertEquals(2, repositoryService.createDeploymentQuery().orderByDeploymentId().asc().list().size());

		assertEquals(2, repositoryService.createDeploymentQuery().orderByDeploymenTime().asc().list().size());

		assertEquals(2, repositoryService.createDeploymentQuery().orderByDeploymentTime().asc().list().size());
	  }

	}

}