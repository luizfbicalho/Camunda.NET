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

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseDefinitionQueryTest : AbstractDefinitionQueryTest
	{

	  private string deploymentThreeId;

	  protected internal override string ResourceOnePath
	  {
		  get
		  {
			return "org/camunda/bpm/engine/test/repository/one.cmmn";
		  }
	  }

	  protected internal override string ResourceTwoPath
	  {
		  get
		  {
			return "org/camunda/bpm/engine/test/repository/two.cmmn";
		  }
	  }

	  protected internal virtual string ResourceThreePath
	  {
		  get
		  {
			return "org/camunda/bpm/engine/test/api/repository/three_.cmmn";
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		deploymentThreeId = repositoryService.createDeployment().name("thirdDeployment").addClasspathResource(ResourceThreePath).deploy().Id;
		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();
		repositoryService.deleteDeployment(deploymentThreeId, true);
	  }

	  public virtual void testCaseDefinitionProperties()
	  {
		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().orderByCaseDefinitionName().asc().orderByCaseDefinitionVersion().asc().orderByCaseDefinitionCategory().asc().list();

		CaseDefinition caseDefinition = caseDefinitions[0];
		assertEquals("one", caseDefinition.Key);
		assertEquals("One", caseDefinition.Name);
		assertTrue(caseDefinition.Id.StartsWith("one:1", StringComparison.Ordinal));
		assertEquals("Examples", caseDefinition.Category);
		assertEquals(1, caseDefinition.Version);
		assertEquals("org/camunda/bpm/engine/test/repository/one.cmmn", caseDefinition.ResourceName);
		assertEquals(deploymentOneId, caseDefinition.DeploymentId);

		caseDefinition = caseDefinitions[1];
		assertEquals("one", caseDefinition.Key);
		assertEquals("One", caseDefinition.Name);
		assertTrue(caseDefinition.Id.StartsWith("one:2", StringComparison.Ordinal));
		assertEquals("Examples", caseDefinition.Category);
		assertEquals(2, caseDefinition.Version);
		assertEquals("org/camunda/bpm/engine/test/repository/one.cmmn", caseDefinition.ResourceName);
		assertEquals(deploymentTwoId, caseDefinition.DeploymentId);

		caseDefinition = caseDefinitions[2];
		assertEquals("two", caseDefinition.Key);
		assertEquals("Two", caseDefinition.Name);
		assertTrue(caseDefinition.Id.StartsWith("two:1", StringComparison.Ordinal));
		assertEquals("Examples2", caseDefinition.Category);
		assertEquals(1, caseDefinition.Version);
		assertEquals("org/camunda/bpm/engine/test/repository/two.cmmn", caseDefinition.ResourceName);
		assertEquals(deploymentOneId, caseDefinition.DeploymentId);
	  }

	  public virtual void testQueryByCaseDefinitionIds()
	  {
		// empty list
		assertTrue(repositoryService.createCaseDefinitionQuery().caseDefinitionIdIn("a", "b").list().Empty);

		// collect all ids
		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().list();
		// no point of the test if the caseDefinitions is empty
		assertFalse(caseDefinitions.Count == 0);
		IList<string> ids = new List<string>();
		foreach (CaseDefinition caseDefinition in caseDefinitions)
		{
		  ids.Add(caseDefinition.Id);
		}

		caseDefinitions = repositoryService.createCaseDefinitionQuery().caseDefinitionIdIn(ids.ToArray()).list();

		assertEquals(ids.Count, caseDefinitions.Count);
		foreach (CaseDefinition caseDefinition in caseDefinitions)
		{
		  if (!ids.Contains(caseDefinition.Id))
		  {
			fail("Expected to find case definition " + caseDefinition);
		  }
		}

		assertEquals(0, repositoryService.createCaseDefinitionQuery().caseDefinitionIdIn(ids.ToArray()).caseDefinitionId("nonExistent").count());
	  }

	  public virtual void testQueryByDeploymentId()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.deploymentId(deploymentOneId);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidDeploymentId()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

	   query.deploymentId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.deploymentId(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByName()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionName("Two");

		verifyQueryResults(query, 1);

		query.caseDefinitionName("One");

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidName()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionName("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionName(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByNameLike()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionNameLike("%w%");

		verifyQueryResults(query, 1);

		query.caseDefinitionNameLike("%z\\_");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidNameLike()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionNameLike("%invalid%");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionNameLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByResourceNameLike()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionResourceNameLike("%ree%");

		verifyQueryResults(query, 1);

		query.caseDefinitionResourceNameLike("%e\\_%");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidResourceNameLike()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionResourceNameLike("%invalid%");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionNameLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByKey()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		// case one
		query.caseDefinitionKey("one");

		verifyQueryResults(query, 2);

		// case two
		query.caseDefinitionKey("two");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidKey()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionKey("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionKey(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByKeyLike()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionKeyLike("%o%");

		verifyQueryResults(query, 3);

		query.caseDefinitionKeyLike("%z\\_");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidKeyLike()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionKeyLike("%invalid%");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionKeyLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByCategory()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionCategory("Examples");

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidCategory()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionCategory("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionCategory(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByCategoryLike()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionCategoryLike("%Example%");

		verifyQueryResults(query, 3);

		query.caseDefinitionCategoryLike("%amples2");

		verifyQueryResults(query, 1);

		query.caseDefinitionCategoryLike("%z\\_");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByInvalidCategoryLike()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionCategoryLike("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionCategoryLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByVersion()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionVersion(2);

		verifyQueryResults(query, 1);

		query.caseDefinitionVersion(1);

		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryByInvalidVersion()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.caseDefinitionVersion(3);

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionVersion(-1);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.caseDefinitionVersion(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQueryByLatest()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		query.latestVersion();

		verifyQueryResults(query, 3);

		query.caseDefinitionKey("one").latestVersion();

		verifyQueryResults(query, 1);

		query.caseDefinitionKey("two").latestVersion();
		verifyQueryResults(query, 1);
	  }

	  public virtual void testInvalidUsageOfLatest()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		try
		{
		  query.caseDefinitionId("test").latestVersion().list();
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.caseDefinitionName("test").latestVersion().list();
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.caseDefinitionNameLike("test").latestVersion().list();
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.caseDefinitionVersion(1).latestVersion().list();
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.deploymentId("test").latestVersion().list();
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

	  public virtual void testQuerySorting()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		// asc
		query.orderByCaseDefinitionId().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createCaseDefinitionQuery();

		query.orderByDeploymentId().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createCaseDefinitionQuery();

		query.orderByCaseDefinitionKey().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createCaseDefinitionQuery();

		query.orderByCaseDefinitionVersion().asc();
		verifyQueryResults(query, 4);

		// desc

		query = repositoryService.createCaseDefinitionQuery();

		query.orderByCaseDefinitionId().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createCaseDefinitionQuery();

		query.orderByDeploymentId().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createCaseDefinitionQuery();

		query.orderByCaseDefinitionKey().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createCaseDefinitionQuery();

		query.orderByCaseDefinitionVersion().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createCaseDefinitionQuery();

		// Typical use case
		query.orderByCaseDefinitionKey().asc().orderByCaseDefinitionVersion().desc();

		IList<CaseDefinition> caseDefinitions = query.list();
		assertEquals(4, caseDefinitions.Count);

		assertEquals("one", caseDefinitions[0].Key);
		assertEquals(2, caseDefinitions[0].Version);
		assertEquals("one", caseDefinitions[1].Key);
		assertEquals(1, caseDefinitions[1].Version);
		assertEquals("two", caseDefinitions[2].Key);
		assertEquals(1, caseDefinitions[2].Version);
	  }

	}

}