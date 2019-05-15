﻿using System.Collections.Generic;
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
namespace org.camunda.bpm.engine.test.api.authorization.dmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_REQUIREMENTS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class DecisionRequirementsDefinitionAuthorizationTest
	public class DecisionRequirementsDefinitionAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public DecisionRequirementsDefinitionAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			chain = RuleChain.outerRule(engineRule).around(authRule);
		}


	  protected internal const string DMN_FILE = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";
	  protected internal const string DRD_FILE = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.png";

	  protected internal const string DEFINITION_KEY = "dish";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;

	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(DECISION_REQUIREMENTS_DEFINITION, DEFINITION_KEY, "userId", Permissions.READ)), scenario().withAuthorizations(grant(DECISION_REQUIREMENTS_DEFINITION, DEFINITION_KEY, "userId", Permissions.READ)).succeeds(), scenario().withAuthorizations(grant(DECISION_REQUIREMENTS_DEFINITION, "*", "userId", Permissions.READ)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { DMN_FILE }) public void getDecisionRequirementsDefinition()
	  [Deployment(resources : { DMN_FILE })]
	  public virtual void getDecisionRequirementsDefinition()
	  {

		string decisionRequirementsDefinitionId = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DEFINITION_KEY).singleResult().Id;

		// when
		authRule.init(scenario).withUser("userId").bindResource("decisionRequirementsDefinitionKey", DEFINITION_KEY).start();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.getDecisionRequirementsDefinition(decisionRequirementsDefinitionId);

		if (authRule.assertScenario(scenario))
		{
		  assertNotNull(decisionRequirementsDefinition);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { DMN_FILE }) public void getDecisionRequirementsModel()
	  [Deployment(resources : { DMN_FILE })]
	  public virtual void getDecisionRequirementsModel()
	  {

		// given
		string decisionRequirementsDefinitionId = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DEFINITION_KEY).singleResult().Id;

		// when
		authRule.init(scenario).withUser("userId").bindResource("decisionRequirementsDefinitionKey", DEFINITION_KEY).start();

		Stream decisionRequirementsModel = repositoryService.getDecisionRequirementsModel(decisionRequirementsDefinitionId);

		if (authRule.assertScenario(scenario))
		{
		  assertNotNull(decisionRequirementsModel);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { DMN_FILE, DRD_FILE }) public void getDecisionRequirementsDiagram()
	  [Deployment(resources : { DMN_FILE, DRD_FILE })]
	  public virtual void getDecisionRequirementsDiagram()
	  {

		// given
		string decisionRequirementsDefinitionId = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DEFINITION_KEY).singleResult().Id;

		// when
		authRule.init(scenario).withUser("userId").bindResource("decisionRequirementsDefinitionKey", DEFINITION_KEY).start();

		Stream decisionRequirementsDiagram = repositoryService.getDecisionRequirementsDiagram(decisionRequirementsDefinitionId);

		if (authRule.assertScenario(scenario))
		{
		  assertNotNull(decisionRequirementsDiagram);
		}
	  }
	}

}