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
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Event = org.camunda.bpm.model.bpmn.instance.Event;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class BpmnModelInstanceCmdTest : PluggableProcessEngineTestCase
	{

	  private const string PROCESS_KEY = "one";

	  [Deployment(resources : "org/camunda/bpm/engine/test/repository/one.bpmn20.xml")]
	  public virtual void testRepositoryService()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_KEY).singleResult().Id;

		BpmnModelInstance modelInstance = repositoryService.getBpmnModelInstance(processDefinitionId);
		assertNotNull(modelInstance);

		ICollection<ModelElementInstance> events = modelInstance.getModelElementsByType(modelInstance.Model.getType(typeof(Event)));
		assertEquals(2, events.Count);

		ICollection<ModelElementInstance> sequenceFlows = modelInstance.getModelElementsByType(modelInstance.Model.getType(typeof(SequenceFlow)));
		assertEquals(1, sequenceFlows.Count);

		StartEvent startEvent = modelInstance.getModelElementById("start");
		assertNotNull(startEvent);
	  }

	}

}