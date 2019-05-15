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
namespace org.camunda.spin.plugin.variables
{

	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;

	public class HistoricDecisionInstanceSerializationTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/spin/plugin/DecisionSingleOutput.dmn11.xml"})]
	  public virtual void testListJsonProperty()
	  {
		JsonListSerializable<string> list = new JsonListSerializable<string>();
		list.addElement("foo");

		ObjectValue objectValue = Variables.objectValue(list).serializationDataFormat(DataFormats.JSON_DATAFORMAT_NAME).create();

		VariableMap variables = Variables.createVariables().putValueTyped("input1", objectValue);

		decisionService.evaluateDecisionTableByKey("testDecision", variables);

		HistoricDecisionInstance testDecision = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("testDecision").includeInputs().includeOutputs().singleResult();
		assertNotNull(testDecision);

		IList<HistoricDecisionInputInstance> inputs = testDecision.Inputs;
		assertEquals(1, inputs.Count);

		HistoricDecisionInputInstance inputInstance = inputs[0];
		assertEquals(list.ListProperty, inputInstance.Value);

		IList<HistoricDecisionOutputInstance> outputs = testDecision.Outputs;
		assertEquals(1, outputs.Count);

		HistoricDecisionOutputInstance outputInstance = outputs[0];
		assertEquals(list.ListProperty, outputInstance.Value);

	  }

	}

}