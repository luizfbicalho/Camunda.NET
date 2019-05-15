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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class DecisionDefinitionQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public DecisionDefinitionQueryTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string DMN_ONE_RESOURCE = "org/camunda/bpm/engine/test/repository/one.dmn";
	  protected internal const string DMN_TWO_RESOURCE = "org/camunda/bpm/engine/test/repository/two.dmn";
	  protected internal const string DMN_THREE_RESOURCE = "org/camunda/bpm/engine/test/api/repository/three_.dmn";

	  protected internal const string DRD_SCORE_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdScore.dmn11.xml";
	  protected internal const string DRD_DISH_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RepositoryService repositoryService;

	  protected internal string decisionRequirementsDefinitionId;
	  protected internal string firstDeploymentId;
	  protected internal string secondDeploymentId;
	  protected internal string thirdDeploymentId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		repositoryService = engineRule.RepositoryService;

		firstDeploymentId = testRule.deploy(DMN_ONE_RESOURCE, DMN_TWO_RESOURCE).Id;
		secondDeploymentId = testRule.deploy(DMN_ONE_RESOURCE).Id;
		thirdDeploymentId = testRule.deploy(DMN_THREE_RESOURCE).Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionDefinitionProperties()
	  public virtual void decisionDefinitionProperties()
	  {
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().orderByDecisionDefinitionName().asc().orderByDecisionDefinitionVersion().asc().orderByDecisionDefinitionCategory().asc().list();

		DecisionDefinition decisionDefinition = decisionDefinitions[0];
		assertEquals("one", decisionDefinition.Key);
		assertEquals("One", decisionDefinition.Name);
		assertTrue(decisionDefinition.Id.StartsWith("one:1", StringComparison.Ordinal));
		assertEquals("Examples", decisionDefinition.Category);
		assertEquals(1, decisionDefinition.Version);
		assertEquals("org/camunda/bpm/engine/test/repository/one.dmn", decisionDefinition.ResourceName);
		assertEquals(firstDeploymentId, decisionDefinition.DeploymentId);

		decisionDefinition = decisionDefinitions[1];
		assertEquals("one", decisionDefinition.Key);
		assertEquals("One", decisionDefinition.Name);
		assertTrue(decisionDefinition.Id.StartsWith("one:2", StringComparison.Ordinal));
		assertEquals("Examples", decisionDefinition.Category);
		assertEquals(2, decisionDefinition.Version);
		assertEquals("org/camunda/bpm/engine/test/repository/one.dmn", decisionDefinition.ResourceName);
		assertEquals(secondDeploymentId, decisionDefinition.DeploymentId);

		decisionDefinition = decisionDefinitions[2];
		assertEquals("two", decisionDefinition.Key);
		assertEquals("Two", decisionDefinition.Name);
		assertTrue(decisionDefinition.Id.StartsWith("two:1", StringComparison.Ordinal));
		assertEquals("Examples2", decisionDefinition.Category);
		assertEquals(1, decisionDefinition.Version);
		assertEquals("org/camunda/bpm/engine/test/repository/two.dmn", decisionDefinition.ResourceName);
		assertEquals(firstDeploymentId, decisionDefinition.DeploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionDefinitionIds()
	  public virtual void queryByDecisionDefinitionIds()
	  {
		// empty list
		assertTrue(repositoryService.createDecisionDefinitionQuery().decisionDefinitionIdIn("a", "b").list().Empty);

		// collect all ids
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().list();
		IList<string> ids = new List<string>();
		foreach (DecisionDefinition decisionDefinition in decisionDefinitions)
		{
		  ids.Add(decisionDefinition.Id);
		}

		decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionIdIn(ids.ToArray()).list();

		assertEquals(ids.Count, decisionDefinitions.Count);
		foreach (DecisionDefinition decisionDefinition in decisionDefinitions)
		{
		  if (!ids.Contains(decisionDefinition.Id))
		  {
			fail("Expected to find decision definition " + decisionDefinition);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDeploymentId()
	  public virtual void queryByDeploymentId()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.deploymentId(firstDeploymentId);

		verifyQueryResults(query, 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidDeploymentId()
	  public virtual void queryByInvalidDeploymentId()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByName()
	  public virtual void queryByName()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionName("Two");

		verifyQueryResults(query, 1);

		query.decisionDefinitionName("One");

		verifyQueryResults(query, 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidName()
	  public virtual void queryByInvalidName()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionName("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.decisionDefinitionName(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByNameLike()
	  public virtual void queryByNameLike()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionNameLike("%w%");

		verifyQueryResults(query, 1);

		query.decisionDefinitionNameLike("%z\\_");

		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidNameLike()
	  public virtual void queryByInvalidNameLike()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionNameLike("%invalid%");

		verifyQueryResults(query, 0);

		try
		{
		  query.decisionDefinitionNameLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByResourceNameLike()
	  public virtual void queryByResourceNameLike()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionResourceNameLike("%ree%");

		verifyQueryResults(query, 1);

		query.decisionDefinitionResourceNameLike("%ee\\_%");

		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidNResourceNameLike()
	  public virtual void queryByInvalidNResourceNameLike()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionResourceNameLike("%invalid%");

		verifyQueryResults(query, 0);

		try
		{
		  query.decisionDefinitionNameLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByKey()
	  public virtual void queryByKey()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		// decision one
		query.decisionDefinitionKey("one");

		verifyQueryResults(query, 2);

		// decision two
		query.decisionDefinitionKey("two");

		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidKey()
	  public virtual void queryByInvalidKey()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionKey("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.decisionDefinitionKey(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByKeyLike()
	  public virtual void queryByKeyLike()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionKeyLike("%o%");

		verifyQueryResults(query, 3);

		query.decisionDefinitionKeyLike("%z\\_");

		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidKeyLike()
	  public virtual void queryByInvalidKeyLike()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionKeyLike("%invalid%");

		verifyQueryResults(query, 0);

		try
		{
		  query.decisionDefinitionKeyLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByCategory()
	  public virtual void queryByCategory()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionCategory("Examples");

		verifyQueryResults(query, 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidCategory()
	  public virtual void queryByInvalidCategory()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionCategory("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.decisionDefinitionCategory(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByCategoryLike()
	  public virtual void queryByCategoryLike()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionCategoryLike("%Example%");

		verifyQueryResults(query, 3);

		query.decisionDefinitionCategoryLike("%amples2");

		verifyQueryResults(query, 1);

		query.decisionDefinitionCategoryLike("%z\\_");

		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidCategoryLike()
	  public virtual void queryByInvalidCategoryLike()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionCategoryLike("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.decisionDefinitionCategoryLike(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByVersion()
	  public virtual void queryByVersion()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionVersion(2);

		verifyQueryResults(query, 1);

		query.decisionDefinitionVersion(1);

		verifyQueryResults(query, 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByInvalidVersion()
	  public virtual void queryByInvalidVersion()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.decisionDefinitionVersion(3);

		verifyQueryResults(query, 0);

		try
		{
		  query.decisionDefinitionVersion(-1);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.decisionDefinitionVersion(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByLatest()
	  public virtual void queryByLatest()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		query.latestVersion();

		verifyQueryResults(query, 3);

		query.decisionDefinitionKey("one").latestVersion();

		verifyQueryResults(query, 1);

		query.decisionDefinitionKey("two").latestVersion();
		verifyQueryResults(query, 1);
	  }

	  public virtual void testInvalidUsageOfLatest()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		try
		{
		  query.decisionDefinitionId("test").latestVersion().list();
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.decisionDefinitionName("test").latestVersion().list();
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.decisionDefinitionNameLike("test").latestVersion().list();
		  fail();
		}
		catch (NotValidException)
		{
		  // Expected exception
		}

		try
		{
		  query.decisionDefinitionVersion(1).latestVersion().list();
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionId()
	  public virtual void queryByDecisionRequirementsDefinitionId()
	  {
		testRule.deploy(DRD_DISH_RESOURCE, DRD_SCORE_RESOURCE);

		IList<DecisionRequirementsDefinition> drds = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionName().asc().list();

		string dishDrdId = drds[0].Id;
		string scoreDrdId = drds[1].Id;

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		verifyQueryResults(query.decisionRequirementsDefinitionId("non existing"), 0);
		verifyQueryResults(query.decisionRequirementsDefinitionId(dishDrdId), 3);
		verifyQueryResults(query.decisionRequirementsDefinitionId(scoreDrdId), 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionKey()
	  public virtual void queryByDecisionRequirementsDefinitionKey()
	  {
		testRule.deploy(DRD_DISH_RESOURCE, DRD_SCORE_RESOURCE);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		verifyQueryResults(query.decisionRequirementsDefinitionKey("non existing"), 0);
		verifyQueryResults(query.decisionRequirementsDefinitionKey("dish"), 3);
		verifyQueryResults(query.decisionRequirementsDefinitionKey("score"), 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByWithoutDecisionRequirementsDefinition()
	  public virtual void queryByWithoutDecisionRequirementsDefinition()
	  {
		testRule.deploy(DRD_DISH_RESOURCE, DRD_SCORE_RESOURCE);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		verifyQueryResults(query, 9);
		verifyQueryResults(query.withoutDecisionRequirementsDefinition(), 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void querySorting()
	  public virtual void querySorting()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		// asc
		query.orderByDecisionDefinitionId().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createDecisionDefinitionQuery();

		query.orderByDeploymentId().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createDecisionDefinitionQuery();

		query.orderByDecisionDefinitionKey().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createDecisionDefinitionQuery();

		query.orderByDecisionDefinitionVersion().asc();
		verifyQueryResults(query, 4);

		// desc

		query = repositoryService.createDecisionDefinitionQuery();

		query.orderByDecisionDefinitionId().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createDecisionDefinitionQuery();

		query.orderByDeploymentId().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createDecisionDefinitionQuery();

		query.orderByDecisionDefinitionKey().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createDecisionDefinitionQuery();

		query.orderByDecisionDefinitionVersion().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createDecisionDefinitionQuery();

		// Typical use decision
		query.orderByDecisionDefinitionKey().asc().orderByDecisionDefinitionVersion().desc();

		IList<DecisionDefinition> decisionDefinitions = query.list();
		assertEquals(4, decisionDefinitions.Count);

		assertEquals("one", decisionDefinitions[0].Key);
		assertEquals(2, decisionDefinitions[0].Version);
		assertEquals("one", decisionDefinitions[1].Key);
		assertEquals(1, decisionDefinitions[1].Version);
		assertEquals("two", decisionDefinitions[2].Key);
		assertEquals(1, decisionDefinitions[2].Version);
	  }


	  protected internal virtual void verifyQueryResults(DecisionDefinitionQuery query, int expectedCount)
	  {
		assertEquals(expectedCount, query.count());
		assertEquals(expectedCount, query.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/repository/versionTag.dmn", "org/camunda/bpm/engine/test/api/repository/versionTagHigher.dmn" }) @Test public void testQueryOrderByVersionTag()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/repository/versionTag.dmn", "org/camunda/bpm/engine/test/api/repository/versionTagHigher.dmn" })]
	  public virtual void testQueryOrderByVersionTag()
	  {
		IList<DecisionDefinition> decisionDefinitionList = repositoryService.createDecisionDefinitionQuery().versionTagLike("1%").orderByVersionTag().asc().list();

		assertEquals("1.1.0", decisionDefinitionList[1].VersionTag);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/repository/versionTag.dmn", "org/camunda/bpm/engine/test/api/repository/versionTagHigher.dmn" }) @Test public void testQueryByVersionTag()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/repository/versionTag.dmn", "org/camunda/bpm/engine/test/api/repository/versionTagHigher.dmn" })]
	  public virtual void testQueryByVersionTag()
	  {
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().versionTag("1.0.0").singleResult();

		assertEquals("versionTag", decisionDefinition.Key);
		assertEquals("1.0.0", decisionDefinition.VersionTag);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/repository/versionTag.dmn", "org/camunda/bpm/engine/test/api/repository/versionTagHigher.dmn" }) @Test public void testQueryByVersionTagLike()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/repository/versionTag.dmn", "org/camunda/bpm/engine/test/api/repository/versionTagHigher.dmn" })]
	  public virtual void testQueryByVersionTagLike()
	  {
		IList<DecisionDefinition> decisionDefinitionList = repositoryService.createDecisionDefinitionQuery().versionTagLike("1%").list();

		assertEquals(2, decisionDefinitionList.Count);
	  }
	}

}