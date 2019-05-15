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
namespace org.camunda.bpm.engine.test.api.authorization.optimize
{

	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using OptimizeService = org.camunda.bpm.engine.impl.OptimizeService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class OptimizeDecisionDefinitionServiceAuthorizationTest : AuthorizationTest
	{

	  protected internal new string deploymentId;
	  private OptimizeService optimizeService;

	  public const string DECISION_PROCESS = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask.bpmn20.xml";
	  public const string DECISION_SINGLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;
		optimizeService = config.OptimizeService;

		base.setUp();
	  }

	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void testGetDecisionInstancesWithoutAuthorization()
	  {
		// given
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		startProcessInstanceByKey("testProcess", variables);

		try
		{
		  // when
		  optimizeService.getHistoricDecisionInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the decision instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(DECISION_DEFINITION.resourceName(), exceptionMessage);
		}

	  }

	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void testGetDecisionInstancesWithAuthorization()
	  {
		// given
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		startProcessInstanceByKey("testProcess", variables);
		createGrantAuthorization(DECISION_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(new DateTime(0L), null, 10);

		// then
		assertThat(decisionInstances.Count, @is(1));
	  }

	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void testAuthorizationsOnSingleDecisionDefinitionIsNotEnough()
	  {
		// given
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		startProcessInstanceByKey("testProcess", variables);
		createGrantAuthorization(DECISION_DEFINITION, "testProcess", userId, READ_HISTORY);

		try
		{
		  // when
		  optimizeService.getHistoricDecisionInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the decision instances");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(DECISION_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void testGrantAuthorizationWithAllPermissions()
	  {
		// given
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		startProcessInstanceByKey("testProcess", variables);
		createGrantAuthorization(DECISION_DEFINITION, "*", userId, ALL);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(new DateTime(0L), null, 10);

		// then
		assertThat(decisionInstances.Count, @is(1));
	  }

	}

}