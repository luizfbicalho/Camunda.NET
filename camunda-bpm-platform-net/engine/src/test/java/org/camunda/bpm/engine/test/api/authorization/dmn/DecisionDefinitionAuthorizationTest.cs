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
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_DEFINITION;

	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	public class DecisionDefinitionAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "testProcess";
	  protected internal const string DECISION_DEFINITION_KEY = "sampleDecision";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/authorization/singleDecision.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/anotherDecision.dmn11.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given user is not authorized to read any decision definition

		// when
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyDecisionDefinition()
	  {
		// given user gets read permission on any decision definition
		createGrantAuthorization(DECISION_DEFINITION, ANY, userId, READ);

		// when
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryWithReadPermissionOnOneDecisionDefinition()
	  {
		// given user gets read permission on the decision definition
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ);

		// when
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		// then
		verifyQueryResults(query, 1);

		DecisionDefinition definition = query.singleResult();
		assertNotNull(definition);
		assertEquals(DECISION_DEFINITION_KEY, definition.Key);
	  }

	  public virtual void testQueryWithMultiple()
	  {
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ);
		createGrantAuthorization(DECISION_DEFINITION, ANY, userId, READ);

		// when
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testGetDecisionDefinitionWithoutAuthorizations()
	  {
		// given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

		try
		{
		  // when
		  repositoryService.getDecisionDefinition(decisionDefinitionId);
		  fail("Exception expected");

		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(DECISION_DEFINITION_KEY, message);
		  assertTextPresent(DECISION_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetDecisionDefinition()
	  {
		// given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ);

		// when
		DecisionDefinition decisionDefinition = repositoryService.getDecisionDefinition(decisionDefinitionId);

		// then
		assertNotNull(decisionDefinition);
	  }

	  public virtual void testGetDecisionDiagramWithoutAuthorizations()
	  {
		// given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

		try
		{
		  // when
		  repositoryService.getDecisionDiagram(decisionDefinitionId);
		  fail("Exception expected");

		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(DECISION_DEFINITION_KEY, message);
		  assertTextPresent(DECISION_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetDecisionDiagram()
	  {
		// given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ);

		// when
		Stream stream = repositoryService.getDecisionDiagram(decisionDefinitionId);

		// then
		// no decision diagram deployed
		assertNull(stream);
	  }

	  public virtual void testGetDecisionModelWithoutAuthorizations()
	  {
		// given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

		try
		{
		  // when
		  repositoryService.getDecisionModel(decisionDefinitionId);
		  fail("Exception expected");

		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(DECISION_DEFINITION_KEY, message);
		  assertTextPresent(DECISION_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetDecisionModel()
	  {
		// given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ);

		// when
		Stream stream = repositoryService.getDecisionModel(decisionDefinitionId);

		// then
		assertNotNull(stream);
	  }

	  public virtual void testGetDmnModelInstanceWithoutAuthorizations()
	  {
		// given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;

		try
		{
		  // when
		  repositoryService.getDmnModelInstance(decisionDefinitionId);
		  fail("Exception expected");

		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(DECISION_DEFINITION_KEY, message);
		  assertTextPresent(DECISION_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetDmnModelInstance()
	  {
		// given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, READ);

		// when
		DmnModelInstance modelInstance = repositoryService.getDmnModelInstance(decisionDefinitionId);

		// then
		assertNotNull(modelInstance);
	  }

	  public virtual void testDecisionDefinitionUpdateTimeToLive()
	  {
		//given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
		createGrantAuthorization(DECISION_DEFINITION, DECISION_DEFINITION_KEY, userId, UPDATE);

		//when
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);

		//then
		assertEquals(6, selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).HistoryTimeToLive.Value);

	  }

	  public virtual void testDecisionDefinitionUpdateTimeToLiveWithoutAuthorizations()
	  {
		//given
		string decisionDefinitionId = selectDecisionDefinitionByKey(DECISION_DEFINITION_KEY).Id;
		try
		{
		  //when
		  repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);
		  fail("Exception expected");

		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(DECISION_DEFINITION_KEY, message);
		  assertTextPresent(DECISION_DEFINITION.resourceName(), message);
		}

	  }

	}

}