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
namespace org.camunda.bpm.engine.impl.dmn.transformer
{
	using DmnDecisionImpl = org.camunda.bpm.dmn.engine.impl.DmnDecisionImpl;
	using DmnElementTransformContext = org.camunda.bpm.dmn.engine.impl.spi.transform.DmnElementTransformContext;
	using DmnDecisionTransformHandler = org.camunda.bpm.dmn.engine.impl.transform.DmnDecisionTransformHandler;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using ParseUtil = org.camunda.bpm.engine.impl.util.ParseUtil;
	using Decision = org.camunda.bpm.model.dmn.instance.Decision;

	public class DecisionDefinitionHandler : DmnDecisionTransformHandler
	{

	  protected internal override DmnDecisionImpl createDmnElement()
	  {
		return new DecisionDefinitionEntity();
	  }

	  protected internal override DmnDecisionImpl createFromDecision(DmnElementTransformContext context, Decision decision)
	  {
		DecisionDefinitionEntity decisionDefinition = (DecisionDefinitionEntity) base.createFromDecision(context, decision);

		string category = context.ModelInstance.Definitions.Namespace;
		decisionDefinition.Category = category;
		decisionDefinition.HistoryTimeToLive = ParseUtil.parseHistoryTimeToLive(decision.CamundaHistoryTimeToLiveString);
		decisionDefinition.VersionTag = decision.VersionTag;

		return decisionDefinition;
	  }

	}

}