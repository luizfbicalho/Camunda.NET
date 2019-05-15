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
namespace org.camunda.bpm.engine.impl.cmmn.handler
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnIfPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnIfPartDeclaration;
	using CmmnOnPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnOnPartDeclaration;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using CmmnVariableOnPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnVariableOnPartDeclaration;
	using CmmnTransformerLogger = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformerLogger;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using PlanItemTransition = org.camunda.bpm.model.cmmn.PlanItemTransition;
	using Query = org.camunda.bpm.model.cmmn.Query;
	using VariableTransition = org.camunda.bpm.model.cmmn.VariableTransition;
	using CaseFileItemOnPart = org.camunda.bpm.model.cmmn.instance.CaseFileItemOnPart;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using ExtensionElements = org.camunda.bpm.model.cmmn.instance.ExtensionElements;
	using IfPart = org.camunda.bpm.model.cmmn.instance.IfPart;
	using OnPart = org.camunda.bpm.model.cmmn.instance.OnPart;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using PlanItemOnPart = org.camunda.bpm.model.cmmn.instance.PlanItemOnPart;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using CamundaVariableOnPart = org.camunda.bpm.model.cmmn.instance.camunda.CamundaVariableOnPart;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class SentryHandler : CmmnElementHandler<Sentry, CmmnSentryDeclaration>
	{

	  protected internal static readonly CmmnTransformerLogger LOG = ProcessEngineLogger.CMMN_TRANSFORMER_LOGGER;

	  public virtual CmmnSentryDeclaration handleElement(Sentry element, CmmnHandlerContext context)
	  {

		string id = element.Id;
		ICollection<OnPart> onParts = element.OnParts;
		IfPart ifPart = element.IfPart;
		IList<CamundaVariableOnPart> variableOnParts = queryExtensionElementsByClass(element, typeof(CamundaVariableOnPart));

		if ((ifPart == null || ifPart.Conditions.Empty) && variableOnParts.Count == 0)
		{

		  if (onParts == null || onParts.Count == 0)
		  {
			LOG.ignoredSentryWithMissingCondition(id);
			return null;
		  }
		  else
		  {
			bool atLeastOneOnPartsValid = false;

			foreach (OnPart onPart in onParts)
			{
			  if (onPart is PlanItemOnPart)
			  {
				PlanItemOnPart planItemOnPart = (PlanItemOnPart) onPart;
				if (planItemOnPart.Source != null && planItemOnPart.StandardEvent != null)
				{
				  atLeastOneOnPartsValid = true;
				  break;
				}
			  }
			}

			if (!atLeastOneOnPartsValid)
			{
			  LOG.ignoredSentryWithInvalidParts(id);
			  return null;
			}
		  }
		}

		CmmnSentryDeclaration sentryDeclaration = new CmmnSentryDeclaration(id);

		// the ifPart will be initialized immediately
		initializeIfPart(ifPart, sentryDeclaration, context);

		// the variableOnParts will be initialized immediately as it does not have any dependency
		initializeVariableOnParts(element, sentryDeclaration, context, variableOnParts);

		// ...whereas the onParts will be initialized later because the
		// the reference to the plan items (sourceRef) and the reference
		// to the sentry (sentryRef) cannot be set in this step. To set
		// the corresponding reference (sourceRef or sentryRef) on the
		// transformed sentry all planned items and all sentries inside
		// the current stage should be already transformed.

		CmmnActivity parent = context.Parent;
		if (parent != null)
		{
		  parent.addSentry(sentryDeclaration);
		}

		return sentryDeclaration;
	  }

	  public virtual void initializeOnParts(Sentry sentry, CmmnHandlerContext context)
	  {
		ICollection<OnPart> onParts = sentry.OnParts;
		foreach (OnPart onPart in onParts)
		{
		  if (onPart is PlanItemOnPart)
		  {
			initializeOnPart((PlanItemOnPart) onPart, sentry, context);
		  }
		  else
		  {
			initializeOnPart((CaseFileItemOnPart) onPart, sentry, context);
		  }
		}
	  }

	  protected internal virtual void initializeOnPart(PlanItemOnPart onPart, Sentry sentry, CmmnHandlerContext context)
	  {
		CmmnActivity parent = context.Parent;
		string sentryId = sentry.Id;
		CmmnSentryDeclaration sentryDeclaration = parent.getSentry(sentryId);

		PlanItem source = onPart.Source;
		PlanItemTransition standardEvent = onPart.StandardEvent;

		if (source != null && standardEvent != null)
		{
		  CmmnOnPartDeclaration onPartDeclaration = new CmmnOnPartDeclaration();

		  // initialize standardEvent
		  string standardEventName = standardEvent.name();
		  onPartDeclaration.StandardEvent = standardEventName;

		  // initialize sourceRef
		  string sourceId = source.Id;
		  CmmnActivity sourceActivity = parent.findActivity(sourceId);

		  if (sourceActivity != null)
		  {
			onPartDeclaration.Source = sourceActivity;
		  }

		  // initialize sentryRef
		  Sentry sentryRef = onPart.Sentry;
		  if (sentryRef != null)
		  {
			string sentryRefId = sentryRef.Id;

			CmmnSentryDeclaration sentryRefDeclaration = parent.getSentry(sentryRefId);
			onPartDeclaration.Sentry = sentryRefDeclaration;
		  }

		  // add onPartDeclaration to sentryDeclaration
		  sentryDeclaration.addOnPart(onPartDeclaration);
		}

	  }

	  protected internal virtual void initializeOnPart(CaseFileItemOnPart onPart, Sentry sentry, CmmnHandlerContext context)
	  {
		// not yet implemented
		string id = sentry.Id;
		LOG.ignoredUnsupportedAttribute("onPart", "CaseFileItem", id);
	  }

	  protected internal virtual void initializeIfPart(IfPart ifPart, CmmnSentryDeclaration sentryDeclaration, CmmnHandlerContext context)
	  {
		if (ifPart == null)
		{
		  return;
		}

		ICollection<ConditionExpression> conditions = ifPart.Conditions;

		if (conditions.Count > 1)
		{
		  string id = sentryDeclaration.Id;
		  LOG.multipleIgnoredConditions(id);
		}

		ExpressionManager expressionManager = context.ExpressionManager;
		ConditionExpression condition = conditions.GetEnumerator().next();
		Expression conditionExpression = expressionManager.createExpression(condition.Text);

		CmmnIfPartDeclaration ifPartDeclaration = new CmmnIfPartDeclaration();
		ifPartDeclaration.Condition = conditionExpression;
		sentryDeclaration.IfPart = ifPartDeclaration;
	  }

	  protected internal virtual void initializeVariableOnParts(CmmnElement element, CmmnSentryDeclaration sentryDeclaration, CmmnHandlerContext context, IList<CamundaVariableOnPart> variableOnParts)
	  {
		foreach (CamundaVariableOnPart variableOnPart in variableOnParts)
		{
		  initializeVariableOnPart(variableOnPart, sentryDeclaration, context);
		}
	  }

	  protected internal virtual void initializeVariableOnPart(CamundaVariableOnPart variableOnPart, CmmnSentryDeclaration sentryDeclaration, CmmnHandlerContext context)
	  {
		VariableTransition variableTransition;

		try
		{
		  variableTransition = variableOnPart.VariableEvent;
		}
		catch (System.ArgumentException)
		{
		  throw LOG.nonMatchingVariableEvents(sentryDeclaration.Id);
		}
		catch (System.NullReferenceException)
		{
		  throw LOG.nonMatchingVariableEvents(sentryDeclaration.Id);
		}

		string variableName = variableOnPart.VariableName;
		string variableEventName = variableTransition.name();

		if (!string.ReferenceEquals(variableName, null))
		{
		  if (!sentryDeclaration.hasVariableOnPart(variableEventName, variableName))
		  {
			CmmnVariableOnPartDeclaration variableOnPartDeclaration = new CmmnVariableOnPartDeclaration();
			variableOnPartDeclaration.VariableEvent = variableEventName;
			variableOnPartDeclaration.VariableName = variableName;
			sentryDeclaration.addVariableOnParts(variableOnPartDeclaration);
		  }
		}
		else
		{
		  throw LOG.emptyVariableName(sentryDeclaration.Id);
		}
	  }

	  protected internal virtual IList<V> queryExtensionElementsByClass<V>(CmmnElement element, Type<V> cls) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ExtensionElements extensionElements = element.ExtensionElements;

		if (extensionElements != null)
		{
		  Query<ModelElementInstance> query = extensionElements.ElementsQuery;
		  return query.filterByType(cls).list();

		}
		else
		{
		  return new List<V>();
		}
	  }
	}

}