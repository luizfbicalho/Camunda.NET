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
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CmmnModelElementInstanceCmdTest : PluggableProcessEngineTestCase
	{

	  private const string CASE_KEY = "oneTaskCase";

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testRepositoryService()
	  {
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_KEY).singleResult().Id;

		CmmnModelInstance modelInstance = repositoryService.getCmmnModelInstance(caseDefinitionId);
		assertNotNull(modelInstance);

		ICollection<ModelElementInstance> humanTasks = modelInstance.getModelElementsByType(modelInstance.Model.getType(typeof(HumanTask)));
		assertEquals(1, humanTasks.Count);

		ICollection<ModelElementInstance> planItems = modelInstance.getModelElementsByType(modelInstance.Model.getType(typeof(PlanItem)));
		assertEquals(1, planItems.Count);

		ICollection<ModelElementInstance> cases = modelInstance.getModelElementsByType(modelInstance.Model.getType(typeof(Case)));
		assertEquals(1, cases.Count);

	  }

	}

}