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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.startsWith;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class DecisionRequirementsDefinitionQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public DecisionRequirementsDefinitionQueryTest()
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


	  protected internal const string DRD_SCORE_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdScore.dmn11.xml";
	  protected internal const string DRD_DISH_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";
	  protected internal const string DRD_XYZ_RESOURCE = "org/camunda/bpm/engine/test/api/repository/drdXyz_.dmn11.xml";

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

		firstDeploymentId = testRule.deploy(DRD_DISH_RESOURCE, DRD_SCORE_RESOURCE).Id;
		secondDeploymentId = testRule.deploy(DRD_DISH_RESOURCE).Id;
		thirdDeploymentId = testRule.deploy(DRD_XYZ_RESOURCE).Id;

		decisionRequirementsDefinitionId = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey("score").singleResult().Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionId()
	  public virtual void queryByDecisionRequirementsDefinitionId()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionId("notExisting").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionId(decisionRequirementsDefinitionId).count(), @is(1L));
		assertThat(query.singleResult().Key, @is("score"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionIds()
	  public virtual void queryByDecisionRequirementsDefinitionIds()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionIdIn("not", "existing").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionIdIn(decisionRequirementsDefinitionId, "notExisting").count(), @is(1L));
		assertThat(query.singleResult().Key, @is("score"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionKey()
	  public virtual void queryByDecisionRequirementsDefinitionKey()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionKey("notExisting").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionKey("score").count(), @is(1L));
		assertThat(query.singleResult().Key, @is("score"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionKeyLike()
	  public virtual void queryByDecisionRequirementsDefinitionKeyLike()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionKeyLike("%notExisting%").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionKeyLike("%sco%").count(), @is(1L));
		assertThat(query.decisionRequirementsDefinitionKeyLike("%dis%").count(), @is(2L));
		assertThat(query.decisionRequirementsDefinitionKeyLike("%s%").count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionName()
	  public virtual void queryByDecisionRequirementsDefinitionName()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionName("notExisting").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionName("Score").count(), @is(1L));
		assertThat(query.singleResult().Key, @is("score"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionNameLike()
	  public virtual void queryByDecisionRequirementsDefinitionNameLike()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionNameLike("%notExisting%").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionNameLike("%Sco%").count(), @is(1L));
		assertThat(query.decisionRequirementsDefinitionNameLike("%ish%").count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionCategory()
	  public virtual void queryByDecisionRequirementsDefinitionCategory()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionCategory("notExisting").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionCategory("test-drd-1").count(), @is(1L));
		assertThat(query.singleResult().Key, @is("score"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDecisionRequirementsDefinitionCategoryLike()
	  public virtual void queryByDecisionRequirementsDefinitionCategoryLike()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionCategoryLike("%notExisting%").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionCategoryLike("%test%").count(), @is(3L));

		assertThat(query.decisionRequirementsDefinitionCategoryLike("%z\\_").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByResourceName()
	  public virtual void queryByResourceName()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionResourceName("notExisting").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionResourceName(DRD_SCORE_RESOURCE).count(), @is(1L));
		assertThat(query.singleResult().Key, @is("score"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByResourceNameLike()
	  public virtual void queryByResourceNameLike()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionResourceNameLike("%notExisting%").count(), @is(0L));

		assertThat(query.decisionRequirementsDefinitionResourceNameLike("%.dmn11.xml%").count(), @is(4L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByResourceNameLikeEscape()
	  public virtual void queryByResourceNameLikeEscape()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionResourceNameLike("%z\\_.%").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByVersion()
	  public virtual void queryByVersion()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.decisionRequirementsDefinitionVersion(1).count(), @is(3L));
		assertThat(query.decisionRequirementsDefinitionVersion(2).count(), @is(1L));
		assertThat(query.decisionRequirementsDefinitionVersion(3).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByLatest()
	  public virtual void queryByLatest()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.latestVersion().count(), @is(3L));
		assertThat(query.decisionRequirementsDefinitionKey("score").latestVersion().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDeploymentId()
	  public virtual void queryByDeploymentId()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.deploymentId("notExisting").count(), @is(0L));

		assertThat(query.deploymentId(firstDeploymentId).count(), @is(2L));
		assertThat(query.deploymentId(secondDeploymentId).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void orderByDecisionRequirementsDefinitionId()
	  public virtual void orderByDecisionRequirementsDefinitionId()
	  {
		IList<DecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionId().asc().list();

		assertThat(decisionRequirementsDefinitions.Count, @is(4));
		assertThat(decisionRequirementsDefinitions[0].Id, startsWith("dish:1"));
		assertThat(decisionRequirementsDefinitions[1].Id, startsWith("dish:2"));
		assertThat(decisionRequirementsDefinitions[2].Id, startsWith("score:1"));
		assertThat(decisionRequirementsDefinitions[3].Id, startsWith("xyz:1"));

		decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionId().desc().list();

		assertThat(decisionRequirementsDefinitions[0].Id, startsWith("xyz:1"));
		assertThat(decisionRequirementsDefinitions[1].Id, startsWith("score:1"));
		assertThat(decisionRequirementsDefinitions[2].Id, startsWith("dish:2"));
		assertThat(decisionRequirementsDefinitions[3].Id, startsWith("dish:1"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void orderByDecisionRequirementsDefinitionKey()
	  public virtual void orderByDecisionRequirementsDefinitionKey()
	  {
		IList<DecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionKey().asc().list();

		assertThat(decisionRequirementsDefinitions.Count, @is(4));
		assertThat(decisionRequirementsDefinitions[0].Key, @is("dish"));
		assertThat(decisionRequirementsDefinitions[1].Key, @is("dish"));
		assertThat(decisionRequirementsDefinitions[2].Key, @is("score"));
		assertThat(decisionRequirementsDefinitions[3].Key, @is("xyz"));

		decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionKey().desc().list();

		assertThat(decisionRequirementsDefinitions[0].Key, @is("xyz"));
		assertThat(decisionRequirementsDefinitions[1].Key, @is("score"));
		assertThat(decisionRequirementsDefinitions[2].Key, @is("dish"));
		assertThat(decisionRequirementsDefinitions[3].Key, @is("dish"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void orderByDecisionRequirementsDefinitionName()
	  public virtual void orderByDecisionRequirementsDefinitionName()
	  {
		IList<DecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionName().asc().list();

		assertThat(decisionRequirementsDefinitions.Count, @is(4));
		assertThat(decisionRequirementsDefinitions[0].Name, @is("Dish"));
		assertThat(decisionRequirementsDefinitions[1].Name, @is("Dish"));
		assertThat(decisionRequirementsDefinitions[2].Name, @is("Score"));
		assertThat(decisionRequirementsDefinitions[3].Name, @is("Xyz"));

		decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionName().desc().list();

		assertThat(decisionRequirementsDefinitions[0].Name, @is("Xyz"));
		assertThat(decisionRequirementsDefinitions[1].Name, @is("Score"));
		assertThat(decisionRequirementsDefinitions[2].Name, @is("Dish"));
		assertThat(decisionRequirementsDefinitions[3].Name, @is("Dish"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void orderByDecisionRequirementsDefinitionCategory()
	  public virtual void orderByDecisionRequirementsDefinitionCategory()
	  {
		IList<DecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionCategory().asc().list();

		assertThat(decisionRequirementsDefinitions.Count, @is(4));
		assertThat(decisionRequirementsDefinitions[0].Category, @is("test-drd-1"));
		assertThat(decisionRequirementsDefinitions[1].Category, @is("test-drd-2"));
		assertThat(decisionRequirementsDefinitions[2].Category, @is("test-drd-2"));
		assertThat(decisionRequirementsDefinitions[3].Category, @is("xyz_"));

		decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionCategory().desc().list();

		assertThat(decisionRequirementsDefinitions[0].Category, @is("xyz_"));
		assertThat(decisionRequirementsDefinitions[1].Category, @is("test-drd-2"));
		assertThat(decisionRequirementsDefinitions[2].Category, @is("test-drd-2"));
		assertThat(decisionRequirementsDefinitions[3].Category, @is("test-drd-1"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void orderByDecisionRequirementsDefinitionVersion()
	  public virtual void orderByDecisionRequirementsDefinitionVersion()
	  {
		IList<DecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionVersion().asc().list();

		assertThat(decisionRequirementsDefinitions.Count, @is(4));
		assertThat(decisionRequirementsDefinitions[0].Version, @is(1));
		assertThat(decisionRequirementsDefinitions[1].Version, @is(1));
		assertThat(decisionRequirementsDefinitions[2].Version, @is(1));
		assertThat(decisionRequirementsDefinitions[3].Version, @is(2));

		decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionVersion().desc().list();

		assertThat(decisionRequirementsDefinitions[0].Version, @is(2));
		assertThat(decisionRequirementsDefinitions[1].Version, @is(1));
		assertThat(decisionRequirementsDefinitions[2].Version, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void orderByDeploymentId()
	  public virtual void orderByDeploymentId()
	  {
		IList<DecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDeploymentId().asc().list();

		assertThat(decisionRequirementsDefinitions.Count, @is(4));
		assertThat(decisionRequirementsDefinitions[0].DeploymentId, @is(firstDeploymentId));
		assertThat(decisionRequirementsDefinitions[1].DeploymentId, @is(firstDeploymentId));
		assertThat(decisionRequirementsDefinitions[2].DeploymentId, @is(secondDeploymentId));
		assertThat(decisionRequirementsDefinitions[3].DeploymentId, @is(thirdDeploymentId));

		decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDeploymentId().desc().list();

		assertThat(decisionRequirementsDefinitions[0].DeploymentId, @is(thirdDeploymentId));
		assertThat(decisionRequirementsDefinitions[1].DeploymentId, @is(secondDeploymentId));
		assertThat(decisionRequirementsDefinitions[2].DeploymentId, @is(firstDeploymentId));
		assertThat(decisionRequirementsDefinitions[3].DeploymentId, @is(firstDeploymentId));
	  }

	}

}