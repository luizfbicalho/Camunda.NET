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

	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CustomHistoryTest : ResourceProcessEngineTestCase
	{

	  public CustomHistoryTest() : base("org/camunda/bpm/engine/test/standalone/history/customhistory.camunda.cfg.xml")
	  {
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testReceivesVariableUpdates()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		string value = "a Variable Value";
		runtimeService.setVariable(instance.Id, "aStringVariable", value);
		runtimeService.setVariable(instance.Id, "aBytesVariable", value.GetBytes());

		// then the historic variable instances and their values exist
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().count());

		HistoricVariableInstance historicStringVariable = historyService.createHistoricVariableInstanceQuery().variableName("aStringVariable").singleResult();
		assertNotNull(historicStringVariable);
		assertEquals(value, historicStringVariable.Value);

		HistoricVariableInstance historicBytesVariable = historyService.createHistoricVariableInstanceQuery().variableName("aBytesVariable").singleResult();
		assertNotNull(historicBytesVariable);
		assertTrue(Arrays.Equals(value.GetBytes(), (sbyte[]) historicBytesVariable.Value));

		// then the historic variable updates and their values exist
		assertEquals(2, historyService.createHistoricDetailQuery().variableUpdates().count());

		HistoricVariableUpdate historicStringVariableUpdate = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(historicStringVariable.Id).singleResult();

		assertNotNull(historicStringVariableUpdate);
		assertEquals(value, historicStringVariableUpdate.Value);

		HistoricVariableUpdate historicByteVariableUpdate = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(historicBytesVariable.Id).singleResult();
		assertNotNull(historicByteVariableUpdate);
		assertTrue(Arrays.Equals(value.GetBytes(), (sbyte[]) historicByteVariableUpdate.Value));

	  }
	}

}