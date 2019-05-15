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

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;
	using Decision = org.camunda.bpm.model.dmn.instance.Decision;
	using DecisionTable = org.camunda.bpm.model.dmn.instance.DecisionTable;
	using Input = org.camunda.bpm.model.dmn.instance.Input;
	using Output = org.camunda.bpm.model.dmn.instance.Output;
	using Rule = org.camunda.bpm.model.dmn.instance.Rule;

	public class DmnModelElementInstanceCmdTest : PluggableProcessEngineTestCase
	{

	  private const string DECISION_KEY = "one";

	  [Deployment(resources : "org/camunda/bpm/engine/test/repository/one.dmn")]
	  public virtual void testRepositoryService()
	  {
		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_KEY).singleResult().Id;

		DmnModelInstance modelInstance = repositoryService.getDmnModelInstance(decisionDefinitionId);
		assertNotNull(modelInstance);

		ICollection<Decision> decisions = modelInstance.getModelElementsByType(typeof(Decision));
		assertEquals(1, decisions.Count);

		ICollection<DecisionTable> decisionTables = modelInstance.getModelElementsByType(typeof(DecisionTable));
		assertEquals(1, decisionTables.Count);

		ICollection<Input> inputs = modelInstance.getModelElementsByType(typeof(Input));
		assertEquals(1, inputs.Count);

		ICollection<Output> outputs = modelInstance.getModelElementsByType(typeof(Output));
		assertEquals(1, outputs.Count);

		ICollection<Rule> rules = modelInstance.getModelElementsByType(typeof(Rule));
		assertEquals(2, rules.Count);
	  }

	}

}