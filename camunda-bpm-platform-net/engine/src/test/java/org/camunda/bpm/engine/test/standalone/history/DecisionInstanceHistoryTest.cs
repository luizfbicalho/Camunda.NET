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
namespace org.camunda.bpm.engine.test.standalone.history
{

	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	public class DecisionInstanceHistoryTest : ResourceProcessEngineTestCase
	{

	  public const string DECISION_SINGLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";

	  public DecisionInstanceHistoryTest() : base("org/camunda/bpm/engine/test/standalone/history/decisionInstanceHistory.camunda.cfg.xml")
	  {
	  }

	  [Deployment(resources : DECISION_SINGLE_OUTPUT_DMN)]
	  public virtual void testDecisionDefinitionPassedToHistoryLevel()
	  {
		RecordHistoryLevel historyLevel = (RecordHistoryLevel) processEngineConfiguration.HistoryLevel;
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("testDecision").singleResult();

		VariableMap variables = Variables.createVariables().putValue("input1", true);
		decisionService.evaluateDecisionTableByKey("testDecision", variables);

		IList<RecordHistoryLevel.ProducedHistoryEvent> producedHistoryEvents = historyLevel.ProducedHistoryEvents;
		assertEquals(1, producedHistoryEvents.Count);

		RecordHistoryLevel.ProducedHistoryEvent producedHistoryEvent = producedHistoryEvents[0];
		assertEquals(HistoryEventTypes.DMN_DECISION_EVALUATE, producedHistoryEvent.eventType);

		DecisionDefinition entity = (DecisionDefinition) producedHistoryEvent.entity;
		assertNotNull(entity);
		assertEquals(decisionDefinition.Id, entity.Id);
	  }

	}

}