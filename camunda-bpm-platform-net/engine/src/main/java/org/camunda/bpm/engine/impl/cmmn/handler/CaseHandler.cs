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
namespace org.camunda.bpm.engine.impl.cmmn.handler
{
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnTransformerLogger = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformerLogger;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using ParseUtil = org.camunda.bpm.engine.impl.util.ParseUtil;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using Definitions = org.camunda.bpm.model.cmmn.instance.Definitions;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseHandler : CmmnElementHandler<Case, CmmnCaseDefinition>
	{

	  protected internal static readonly CmmnTransformerLogger LOG = ProcessEngineLogger.CMMN_TRANSFORMER_LOGGER;

	  public virtual CmmnCaseDefinition handleElement(Case element, CmmnHandlerContext context)
	  {
		CaseDefinitionEntity definition = createActivity(element, context);

		initializeActivity(element, definition, context);

		return definition;
	  }

	  protected internal virtual void initializeActivity(Case element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		CaseDefinitionEntity definition = (CaseDefinitionEntity) activity;

		Deployment deployment = context.Deployment;

		definition.Key = element.Id;
		definition.Name = element.Name;
		definition.DeploymentId = deployment.Id;
		definition.TaskDefinitions = new Dictionary<string, TaskDefinition>();
		definition.HistoryTimeToLive = ParseUtil.parseHistoryTimeToLive(element.CamundaHistoryTimeToLiveString);
		CmmnModelInstance model = context.Model;

		Definitions definitions = model.Definitions;
		string category = definitions.TargetNamespace;
		definition.Category = category;
	  }

	  protected internal virtual CaseDefinitionEntity createActivity(CmmnElement element, CmmnHandlerContext context)
	  {
		CaseDefinitionEntity definition = new CaseDefinitionEntity();

		definition.CmmnElement = element;

		return definition;
	  }

	}

}