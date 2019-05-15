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
namespace org.camunda.bpm.engine.test.cmmn.handler
{

	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CmmnHandlerContext = org.camunda.bpm.engine.impl.cmmn.handler.CmmnHandlerContext;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CmmnModelElementInstance = org.camunda.bpm.model.cmmn.instance.CmmnModelElementInstance;
	using Definitions = org.camunda.bpm.model.cmmn.instance.Definitions;
	using ExtensionElements = org.camunda.bpm.model.cmmn.instance.ExtensionElements;
	using Before = org.junit.Before;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class CmmnElementHandlerTest
	{

	  protected internal CmmnModelInstance modelInstance;
	  protected internal Definitions definitions;
	  protected internal Case caseDefinition;
	  protected internal CasePlanModel casePlanModel;
	  protected internal CmmnHandlerContext context;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		modelInstance = Cmmn.createEmptyModel();
		definitions = modelInstance.newInstance(typeof(Definitions));
		definitions.TargetNamespace = "http://camunda.org/examples";
		modelInstance.Definitions = definitions;

		caseDefinition = createElement(definitions, "aCaseDefinition", typeof(Case));
		casePlanModel = createElement(caseDefinition, "aCasePlanModel", typeof(CasePlanModel));

		context = new CmmnHandlerContext();

		CaseDefinitionEntity caseDefinition = new CaseDefinitionEntity();
		caseDefinition.TaskDefinitions = new Dictionary<string, TaskDefinition>();
		context.CaseDefinition = caseDefinition;

		ExpressionManager expressionManager = new ExpressionManager();
		context.ExpressionManager = expressionManager;

		DeploymentEntity deployment = new DeploymentEntity();
		deployment.Id = "foo";
		context.Deployment = deployment;
	  }

	  protected internal virtual T createElement<T>(CmmnModelElementInstance parentElement, Type<T> elementClass) where T : org.camunda.bpm.model.cmmn.instance.CmmnModelElementInstance
	  {
		T element = modelInstance.newInstance(elementClass);
		parentElement.addChildElement(element);
		return element;
	  }

	  protected internal virtual T createElement<T>(CmmnModelElementInstance parentElement, string id, Type<T> elementClass) where T : org.camunda.bpm.model.cmmn.instance.CmmnModelElementInstance
	  {
		T element = createElement(parentElement, elementClass);
		element.setAttributeValue("id", id, true);
		return element;
	  }

	  protected internal virtual ExtensionElements addExtensionElements(CmmnModelElementInstance parentElement)
	  {
		return createElement(parentElement, null, typeof(ExtensionElements));
	  }

	}

}