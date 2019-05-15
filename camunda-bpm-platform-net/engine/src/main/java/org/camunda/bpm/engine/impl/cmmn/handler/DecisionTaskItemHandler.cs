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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.DecisionEvaluationUtil.getDecisionResultMapperForName;

	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using DmnDecisionTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.DmnDecisionTaskActivityBehavior;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using BaseCallableElement = org.camunda.bpm.engine.impl.core.model.BaseCallableElement;
	using DecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using DecisionRefExpression = org.camunda.bpm.model.cmmn.instance.DecisionRefExpression;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DecisionTaskItemHandler : CallingTaskItemHandler
	{

	  protected internal override void initializeActivity(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		base.initializeActivity(element, activity, context);

		initializeResultVariable(element, activity, context);

		initializeDecisionTableResultMapper(element, activity, context);
	  }

	  protected internal virtual void initializeResultVariable(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		DecisionTask decisionTask = getDefinition(element);
		DmnDecisionTaskActivityBehavior behavior = getActivityBehavior(activity);
		string resultVariable = decisionTask.CamundaResultVariable;
		behavior.ResultVariable = resultVariable;
	  }

	  protected internal virtual void initializeDecisionTableResultMapper(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		DecisionTask decisionTask = getDefinition(element);
		DmnDecisionTaskActivityBehavior behavior = getActivityBehavior(activity);
		string mapper = decisionTask.CamundaMapDecisionResult;
		DecisionResultMapper decisionResultMapper = getDecisionResultMapperForName(mapper);
		behavior.DecisionTableResultMapper = decisionResultMapper;
	  }

	  protected internal override BaseCallableElement createCallableElement()
	  {
		return new BaseCallableElement();
	  }

	  protected internal override CmmnActivityBehavior ActivityBehavior
	  {
		  get
		  {
			return new DmnDecisionTaskActivityBehavior();
		  }
	  }

	  protected internal virtual DmnDecisionTaskActivityBehavior getActivityBehavior(CmmnActivity activity)
	  {
		return (DmnDecisionTaskActivityBehavior) activity.ActivityBehavior;
	  }

	  protected internal override string getDefinitionKey(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		DecisionTask definition = getDefinition(element);
		string decision = definition.Decision;

		if (string.ReferenceEquals(decision, null))
		{
		  DecisionRefExpression decisionExpression = definition.DecisionExpression;
		  if (decisionExpression != null)
		  {
			decision = decisionExpression.Text;
		  }
		}

		return decision;
	  }

	  protected internal override string getBinding(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		DecisionTask definition = getDefinition(element);
		return definition.CamundaDecisionBinding;
	  }

	  protected internal override string getVersion(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		DecisionTask definition = getDefinition(element);
		return definition.CamundaDecisionVersion;
	  }

	  protected internal override string getTenantId(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		DecisionTask definition = getDefinition(element);
		return definition.CamundaDecisionTenantId;
	  }


	  protected internal override DecisionTask getDefinition(CmmnElement element)
	  {
		return (DecisionTask) base.getDefinition(element);
	  }

	}

}