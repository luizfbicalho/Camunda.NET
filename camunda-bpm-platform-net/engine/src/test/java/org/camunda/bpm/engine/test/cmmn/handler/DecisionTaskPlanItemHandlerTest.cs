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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_ACTIVITY_DESCRIPTION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_ACTIVITY_TYPE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_IS_BLOCKING;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_REQUIRED_RULE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using CmmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.CmmnProperties;
	using CaseControlRule = org.camunda.bpm.engine.impl.cmmn.CaseControlRule;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using DecisionTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.DecisionTaskActivityBehavior;
	using DmnDecisionTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.DmnDecisionTaskActivityBehavior;
	using CasePlanModelHandler = org.camunda.bpm.engine.impl.cmmn.handler.CasePlanModelHandler;
	using DecisionTaskItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.DecisionTaskItemHandler;
	using SentryHandler = org.camunda.bpm.engine.impl.cmmn.handler.SentryHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using BaseCallableElement = org.camunda.bpm.engine.impl.core.model.BaseCallableElement;
	using CallableElementBinding = org.camunda.bpm.engine.impl.core.model.BaseCallableElement.CallableElementBinding;
	using ConstantValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ConstantValueProvider;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using DecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper;
	using ResultListDecisionTableResultMapper = org.camunda.bpm.engine.impl.dmn.result.ResultListDecisionTableResultMapper;
	using SingleResultDecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.SingleResultDecisionResultMapper;
	using ElValueProvider = org.camunda.bpm.engine.impl.el.ElValueProvider;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using Body = org.camunda.bpm.model.cmmn.instance.Body;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using DecisionRefExpression = org.camunda.bpm.model.cmmn.instance.DecisionRefExpression;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;
	using DefaultControl = org.camunda.bpm.model.cmmn.instance.DefaultControl;
	using Documentation = org.camunda.bpm.model.cmmn.instance.Documentation;
	using EntryCriterion = org.camunda.bpm.model.cmmn.instance.EntryCriterion;
	using ExitCriterion = org.camunda.bpm.model.cmmn.instance.ExitCriterion;
	using IfPart = org.camunda.bpm.model.cmmn.instance.IfPart;
	using ItemControl = org.camunda.bpm.model.cmmn.instance.ItemControl;
	using ManualActivationRule = org.camunda.bpm.model.cmmn.instance.ManualActivationRule;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using PlanItemControl = org.camunda.bpm.model.cmmn.instance.PlanItemControl;
	using RepetitionRule = org.camunda.bpm.model.cmmn.instance.RepetitionRule;
	using RequiredRule = org.camunda.bpm.model.cmmn.instance.RequiredRule;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DecisionTaskPlanItemHandlerTest : CmmnElementHandlerTest
	{

	  protected internal DecisionTask decisionTask;
	  protected internal PlanItem planItem;
	  protected internal DecisionTaskItemHandler handler = new DecisionTaskItemHandler();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		decisionTask = createElement(casePlanModel, "aHumanTask", typeof(DecisionTask));

		planItem = createElement(casePlanModel, "PI_aHumanTask", typeof(PlanItem));
		planItem.Definition = decisionTask;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityName()
	  public virtual void testActivityName()
	  {
		// given:
		string name = "A DecisionTask";
		decisionTask.Name = name;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(name, activity.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPlanItemActivityName()
	  public virtual void testPlanItemActivityName()
	  {
		// given:
		string humanTaskName = "A DecisionTask";
		decisionTask.Name = humanTaskName;

		string planItemName = "My LocalName";
		planItem.Name = planItemName;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertNotEquals(humanTaskName, activity.Name);
		assertEquals(planItemName, activity.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityType()
	  public virtual void testActivityType()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		string activityType = (string) activity.getProperty(PROPERTY_ACTIVITY_TYPE);
		assertEquals("decisionTask", activityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDescriptionProperty()
	  public virtual void testDescriptionProperty()
	  {
		// given
		string description = "This is a decisionTask";
		decisionTask.Description = description;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(description, activity.getProperty(PROPERTY_ACTIVITY_DESCRIPTION));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPlanItemDescriptionProperty()
	  public virtual void testPlanItemDescriptionProperty()
	  {
		// given
		string description = "This is a planItem";
		planItem.Description = description;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(description, activity.getProperty(PROPERTY_ACTIVITY_DESCRIPTION));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDocumentation()
	  public virtual void testDocumentation()
	  {
		// given
		string description = "This is a documenation";
		Documentation documentation = createElement(decisionTask, typeof(Documentation));
		documentation.TextContent = description;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(description, activity.getProperty(PROPERTY_ACTIVITY_DESCRIPTION));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPlanItemDocumentation()
	  public virtual void testPlanItemDocumentation()
	  {
		// given
		string description = "This is a planItem";
		Documentation documentationElem = createElement(planItem, typeof(Documentation));
		documentationElem.TextContent = description;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(description, activity.getProperty(PROPERTY_ACTIVITY_DESCRIPTION));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityBehavior()
	  public virtual void testActivityBehavior()
	  {
		// given: a planItem

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		CmmnActivityBehavior behavior = activity.ActivityBehavior;
		assertTrue(behavior is DmnDecisionTaskActivityBehavior);
	  }

	  public virtual void testIsBlockingEqualsTrueProperty()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		bool? isBlocking = (bool?) activity.getProperty(PROPERTY_IS_BLOCKING);
		assertTrue(isBlocking);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsBlockingEqualsFalseProperty()
	  public virtual void testIsBlockingEqualsFalseProperty()
	  {
		// given:
		decisionTask.IsBlocking = false;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		bool? isBlocking = (bool?) activity.getProperty(PROPERTY_IS_BLOCKING);
		assertFalse(isBlocking);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExitCriteria()
	  public virtual void testExitCriteria()
	  {
		// given

		// create sentry containing ifPart
		Sentry sentry = createElement(casePlanModel, "Sentry_1", typeof(Sentry));
		IfPart ifPart = createElement(sentry, "abc", typeof(IfPart));
		ConditionExpression conditionExpression = createElement(ifPart, "def", typeof(ConditionExpression));
		Body body = createElement(conditionExpression, null, typeof(Body));
		body.TextContent = "${test}";

		// set exitCriteria
		ExitCriterion criterion = createElement(planItem, typeof(ExitCriterion));
		criterion.Sentry = sentry;

		// transform casePlanModel as parent
		CmmnActivity parent = (new CasePlanModelHandler()).handleElement(casePlanModel, context);
		context.Parent = parent;

		// transform Sentry
		CmmnSentryDeclaration sentryDeclaration = (new SentryHandler()).handleElement(sentry, context);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		assertTrue(newActivity.EntryCriteria.Count == 0);

		assertFalse(newActivity.ExitCriteria.Count == 0);
		assertEquals(1, newActivity.ExitCriteria.Count);

		assertEquals(sentryDeclaration, newActivity.ExitCriteria[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleExitCriteria()
	  public virtual void testMultipleExitCriteria()
	  {
		// given

		// create first sentry containing ifPart
		Sentry sentry1 = createElement(casePlanModel, "Sentry_1", typeof(Sentry));
		IfPart ifPart1 = createElement(sentry1, "abc", typeof(IfPart));
		ConditionExpression conditionExpression1 = createElement(ifPart1, "def", typeof(ConditionExpression));
		Body body1 = createElement(conditionExpression1, null, typeof(Body));
		body1.TextContent = "${test}";

		// set first exitCriteria
		ExitCriterion criterion1 = createElement(planItem, typeof(ExitCriterion));
		criterion1.Sentry = sentry1;

		// create first sentry containing ifPart
		Sentry sentry2 = createElement(casePlanModel, "Sentry_2", typeof(Sentry));
		IfPart ifPart2 = createElement(sentry2, "ghi", typeof(IfPart));
		ConditionExpression conditionExpression2 = createElement(ifPart2, "jkl", typeof(ConditionExpression));
		Body body2 = createElement(conditionExpression2, null, typeof(Body));
		body2.TextContent = "${test}";

		// set second exitCriteria
		ExitCriterion criterion2 = createElement(planItem, typeof(ExitCriterion));
		criterion2.Sentry = sentry2;

		// transform casePlanModel as parent
		CmmnActivity parent = (new CasePlanModelHandler()).handleElement(casePlanModel, context);
		context.Parent = parent;

		// transform Sentry
		CmmnSentryDeclaration firstSentryDeclaration = (new SentryHandler()).handleElement(sentry1, context);
		CmmnSentryDeclaration secondSentryDeclaration = (new SentryHandler()).handleElement(sentry2, context);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		assertTrue(newActivity.EntryCriteria.Count == 0);

		assertFalse(newActivity.ExitCriteria.Count == 0);
		assertEquals(2, newActivity.ExitCriteria.Count);

		assertTrue(newActivity.ExitCriteria.Contains(firstSentryDeclaration));
		assertTrue(newActivity.ExitCriteria.Contains(secondSentryDeclaration));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEntryCriteria()
	  public virtual void testEntryCriteria()
	  {
		// given

		// create sentry containing ifPart
		Sentry sentry = createElement(casePlanModel, "Sentry_1", typeof(Sentry));
		IfPart ifPart = createElement(sentry, "abc", typeof(IfPart));
		ConditionExpression conditionExpression = createElement(ifPart, "def", typeof(ConditionExpression));
		Body body = createElement(conditionExpression, null, typeof(Body));
		body.TextContent = "${test}";

		// set entryCriteria
		EntryCriterion criterion = createElement(planItem, typeof(EntryCriterion));
		criterion.Sentry = sentry;

		// transform casePlanModel as parent
		CmmnActivity parent = (new CasePlanModelHandler()).handleElement(casePlanModel, context);
		context.Parent = parent;

		// transform Sentry
		CmmnSentryDeclaration sentryDeclaration = (new SentryHandler()).handleElement(sentry, context);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		assertTrue(newActivity.ExitCriteria.Count == 0);

		assertFalse(newActivity.EntryCriteria.Count == 0);
		assertEquals(1, newActivity.EntryCriteria.Count);

		assertEquals(sentryDeclaration, newActivity.EntryCriteria[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleEntryCriteria()
	  public virtual void testMultipleEntryCriteria()
	  {
		// given

		// create first sentry containing ifPart
		Sentry sentry1 = createElement(casePlanModel, "Sentry_1", typeof(Sentry));
		IfPart ifPart1 = createElement(sentry1, "abc", typeof(IfPart));
		ConditionExpression conditionExpression1 = createElement(ifPart1, "def", typeof(ConditionExpression));
		Body body1 = createElement(conditionExpression1, null, typeof(Body));
		body1.TextContent = "${test}";

		// set first entryCriteria
		EntryCriterion criterion1 = createElement(planItem, typeof(EntryCriterion));
		criterion1.Sentry = sentry1;

		// create first sentry containing ifPart
		Sentry sentry2 = createElement(casePlanModel, "Sentry_2", typeof(Sentry));
		IfPart ifPart2 = createElement(sentry2, "ghi", typeof(IfPart));
		ConditionExpression conditionExpression2 = createElement(ifPart2, "jkl", typeof(ConditionExpression));
		Body body2 = createElement(conditionExpression2, null, typeof(Body));
		body2.TextContent = "${test}";

		// set second entryCriteria
		EntryCriterion criterion2 = createElement(planItem, typeof(EntryCriterion));
		criterion2.Sentry = sentry2;

		// transform casePlanModel as parent
		CmmnActivity parent = (new CasePlanModelHandler()).handleElement(casePlanModel, context);
		context.Parent = parent;

		// transform Sentry
		CmmnSentryDeclaration firstSentryDeclaration = (new SentryHandler()).handleElement(sentry1, context);
		CmmnSentryDeclaration secondSentryDeclaration = (new SentryHandler()).handleElement(sentry2, context);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		assertTrue(newActivity.ExitCriteria.Count == 0);

		assertFalse(newActivity.EntryCriteria.Count == 0);
		assertEquals(2, newActivity.EntryCriteria.Count);

		assertTrue(newActivity.EntryCriteria.Contains(firstSentryDeclaration));
		assertTrue(newActivity.EntryCriteria.Contains(secondSentryDeclaration));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEntryCriteriaAndExitCriteria()
	  public virtual void testEntryCriteriaAndExitCriteria()
	  {
		// given

		// create sentry containing ifPart
		Sentry sentry = createElement(casePlanModel, "Sentry_1", typeof(Sentry));
		IfPart ifPart = createElement(sentry, "abc", typeof(IfPart));
		ConditionExpression conditionExpression = createElement(ifPart, "def", typeof(ConditionExpression));
		Body body = createElement(conditionExpression, null, typeof(Body));
		body.TextContent = "${test}";

		// set entry-/exitCriteria
		EntryCriterion criterion1 = createElement(planItem, typeof(EntryCriterion));
		criterion1.Sentry = sentry;
		ExitCriterion criterion2 = createElement(planItem, typeof(ExitCriterion));
		criterion2.Sentry = sentry;

		// transform casePlanModel as parent
		CmmnActivity parent = (new CasePlanModelHandler()).handleElement(casePlanModel, context);
		context.Parent = parent;

		// transform Sentry
		CmmnSentryDeclaration sentryDeclaration = (new SentryHandler()).handleElement(sentry, context);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		assertFalse(newActivity.ExitCriteria.Count == 0);
		assertEquals(1, newActivity.ExitCriteria.Count);
		assertEquals(sentryDeclaration, newActivity.ExitCriteria[0]);

		assertFalse(newActivity.EntryCriteria.Count == 0);
		assertEquals(1, newActivity.EntryCriteria.Count);
		assertEquals(sentryDeclaration, newActivity.EntryCriteria[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualActivationRule()
	  public virtual void testManualActivationRule()
	  {
		// given
		ItemControl itemControl = createElement(planItem, "ItemControl_1", typeof(ItemControl));
		ManualActivationRule manualActivationRule = createElement(itemControl, "ManualActivationRule_1", typeof(ManualActivationRule));
		ConditionExpression expression = createElement(manualActivationRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		object rule = newActivity.getProperty(PROPERTY_MANUAL_ACTIVATION_RULE);
		assertNotNull(rule);
		assertTrue(rule is CaseControlRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualActivationRuleByDefaultPlanItemControl()
	  public virtual void testManualActivationRuleByDefaultPlanItemControl()
	  {
		// given
		PlanItemControl defaultControl = createElement(decisionTask, "ItemControl_1", typeof(DefaultControl));
		ManualActivationRule manualActivationRule = createElement(defaultControl, "ManualActivationRule_1", typeof(ManualActivationRule));
		ConditionExpression expression = createElement(manualActivationRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		object rule = newActivity.getProperty(PROPERTY_MANUAL_ACTIVATION_RULE);
		assertNotNull(rule);
		assertTrue(rule is CaseControlRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRequiredRule()
	  public virtual void testRequiredRule()
	  {
		// given
		ItemControl itemControl = createElement(planItem, "ItemControl_1", typeof(ItemControl));
		RequiredRule requiredRule = createElement(itemControl, "RequiredRule_1", typeof(RequiredRule));
		ConditionExpression expression = createElement(requiredRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		object rule = newActivity.getProperty(PROPERTY_REQUIRED_RULE);
		assertNotNull(rule);
		assertTrue(rule is CaseControlRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRequiredRuleByDefaultPlanItemControl()
	  public virtual void testRequiredRuleByDefaultPlanItemControl()
	  {
		// given
		PlanItemControl defaultControl = createElement(decisionTask, "ItemControl_1", typeof(DefaultControl));
		RequiredRule requiredRule = createElement(defaultControl, "RequiredRule_1", typeof(RequiredRule));
		ConditionExpression expression = createElement(requiredRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		object rule = newActivity.getProperty(PROPERTY_REQUIRED_RULE);
		assertNotNull(rule);
		assertTrue(rule is CaseControlRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRepetitionRuleStandardEvents()
	  public virtual void testRepetitionRuleStandardEvents()
	  {
		// given
		ItemControl itemControl = createElement(planItem, "ItemControl_1", typeof(ItemControl));
		RepetitionRule repetitionRule = createElement(itemControl, "RepititionRule_1", typeof(RepetitionRule));
		ConditionExpression expression = createElement(repetitionRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		IList<string> events = newActivity.Properties.get(CmmnProperties.REPEAT_ON_STANDARD_EVENTS);
		assertNotNull(events);
		assertEquals(2, events.Count);
		assertTrue(events.Contains(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE));
		assertTrue(events.Contains(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRepetitionRuleStandardEventsByDefaultPlanItemControl()
	  public virtual void testRepetitionRuleStandardEventsByDefaultPlanItemControl()
	  {
		// given
		PlanItemControl defaultControl = createElement(decisionTask, "DefaultControl_1", typeof(DefaultControl));
		RepetitionRule repetitionRule = createElement(defaultControl, "RepititionRule_1", typeof(RepetitionRule));
		ConditionExpression expression = createElement(repetitionRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		IList<string> events = newActivity.Properties.get(CmmnProperties.REPEAT_ON_STANDARD_EVENTS);
		assertNotNull(events);
		assertEquals(2, events.Count);
		assertTrue(events.Contains(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE));
		assertTrue(events.Contains(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRepetitionRuleCustomStandardEvents()
	  public virtual void testRepetitionRuleCustomStandardEvents()
	  {
		// given
		ItemControl itemControl = createElement(planItem, "ItemControl_1", typeof(ItemControl));
		RepetitionRule repetitionRule = createElement(itemControl, "RepititionRule_1", typeof(RepetitionRule));
		ConditionExpression expression = createElement(repetitionRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		repetitionRule.CamundaRepeatOnStandardEvent = org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE;

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		IList<string> events = newActivity.Properties.get(CmmnProperties.REPEAT_ON_STANDARD_EVENTS);
		assertNotNull(events);
		assertEquals(1, events.Count);
		assertTrue(events.Contains(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRepetitionRuleCustomStandardEventsByDefaultPlanItemControl()
	  public virtual void testRepetitionRuleCustomStandardEventsByDefaultPlanItemControl()
	  {
		// given
		PlanItemControl defaultControl = createElement(decisionTask, "DefaultControl_1", typeof(DefaultControl));
		RepetitionRule repetitionRule = createElement(defaultControl, "RepititionRule_1", typeof(RepetitionRule));
		ConditionExpression expression = createElement(repetitionRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		repetitionRule.CamundaRepeatOnStandardEvent = org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE;

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		IList<string> events = newActivity.Properties.get(CmmnProperties.REPEAT_ON_STANDARD_EVENTS);
		assertNotNull(events);
		assertEquals(1, events.Count);
		assertTrue(events.Contains(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutParent()
	  public virtual void testWithoutParent()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertNull(activity.Parent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithParent()
	  public virtual void testWithParent()
	  {
		// given
		CmmnCaseDefinition parent = new CmmnCaseDefinition("aParentActivity");
		context.Parent = parent;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(parent, activity.Parent);
		assertTrue(parent.Activities.Contains(activity));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallableElement()
	  public virtual void testCallableElement()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		assertNotNull(behavior.CallableElement);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConstantDecisionRef()
	  public virtual void testConstantDecisionRef()
	  {
		// given:
		string decisionRef = "aDecisionToCall";
		decisionTask.Decision = decisionRef;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		BaseCallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider decisionRefValueProvider = callableElement.DefinitionKeyValueProvider;
		assertNotNull(decisionRefValueProvider);

		assertTrue(decisionRefValueProvider is ConstantValueProvider);
		ConstantValueProvider valueProvider = (ConstantValueProvider) decisionRefValueProvider;
		assertEquals(decisionRef, valueProvider.getValue(null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExpressionDecisionRef()
	  public virtual void testExpressionDecisionRef()
	  {
		// given:
		string decisionRef = "${aDecisionToCall}";
		decisionTask.Decision = decisionRef;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		BaseCallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider caseRefValueProvider = callableElement.DefinitionKeyValueProvider;
		assertNotNull(caseRefValueProvider);

		assertTrue(caseRefValueProvider is ElValueProvider);
		ElValueProvider valueProvider = (ElValueProvider) caseRefValueProvider;
		assertEquals(decisionRef, valueProvider.Expression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConstantDecisionRefExpression()
	  public virtual void testConstantDecisionRefExpression()
	  {
		// given:
		string decision = "aDecisionToCall";
		DecisionRefExpression decisionRefExpression = createElement(decisionTask, typeof(DecisionRefExpression));
		decisionRefExpression.Text = decision;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		BaseCallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider decisionRefValueProvider = callableElement.DefinitionKeyValueProvider;
		assertNotNull(decisionRefValueProvider);

		assertTrue(decisionRefValueProvider is ConstantValueProvider);
		ConstantValueProvider valueProvider = (ConstantValueProvider) decisionRefValueProvider;
		assertEquals(decision, valueProvider.getValue(null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExpressionDecisionRefExpression()
	  public virtual void testExpressionDecisionRefExpression()
	  {
		// given:
		string decision = "${aDecisionToCall}";
		DecisionRefExpression decisionRefExpression = createElement(decisionTask, typeof(DecisionRefExpression));
		decisionRefExpression.Text = decision;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		BaseCallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider caseRefValueProvider = callableElement.DefinitionKeyValueProvider;
		assertNotNull(caseRefValueProvider);

		assertTrue(caseRefValueProvider is ElValueProvider);
		ElValueProvider valueProvider = (ElValueProvider) caseRefValueProvider;
		assertEquals(decision, valueProvider.Expression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinding()
	  public virtual void testBinding()
	  {
		// given:
		BaseCallableElement.CallableElementBinding caseBinding = BaseCallableElement.CallableElementBinding.LATEST;
		decisionTask.CamundaDecisionBinding = caseBinding.Value;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		BaseCallableElement callableElement = behavior.CallableElement;

		BaseCallableElement.CallableElementBinding binding = callableElement.Binding;
		assertNotNull(binding);
		assertEquals(caseBinding, binding);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVersionConstant()
	  public virtual void testVersionConstant()
	  {
		// given:
		string caseVersion = "2";
		decisionTask.CamundaDecisionVersion = caseVersion;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		BaseCallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider caseVersionValueProvider = callableElement.VersionValueProvider;
		assertNotNull(caseVersionValueProvider);

		assertTrue(caseVersionValueProvider is ConstantValueProvider);
		assertEquals(caseVersion, caseVersionValueProvider.getValue(null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVersionExpression()
	  public virtual void testVersionExpression()
	  {
		// given:
		string caseVersion = "${aVersion}";
		decisionTask.CamundaDecisionVersion = caseVersion;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		BaseCallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider caseVersionValueProvider = callableElement.VersionValueProvider;
		assertNotNull(caseVersionValueProvider);

		assertTrue(caseVersionValueProvider is ElValueProvider);
		ElValueProvider valueProvider = (ElValueProvider) caseVersionValueProvider;
		assertEquals(caseVersion, valueProvider.Expression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResultVariable()
	  public virtual void testResultVariable()
	  {
		// given:
		decisionTask.CamundaResultVariable = "aResultVariable";

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DecisionTaskActivityBehavior behavior = (DecisionTaskActivityBehavior) activity.ActivityBehavior;
		assertEquals("aResultVariable", behavior.ResultVariable);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefaultMapDecisionResult()
	  public virtual void testDefaultMapDecisionResult()
	  {
		// given:

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DmnDecisionTaskActivityBehavior behavior = (DmnDecisionTaskActivityBehavior) activity.ActivityBehavior;
		DecisionResultMapper mapper = behavior.DecisionTableResultMapper;
		assertTrue(mapper is ResultListDecisionTableResultMapper);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapDecisionResult()
	  public virtual void testMapDecisionResult()
	  {
		// given:
		decisionTask.CamundaMapDecisionResult = "singleResult";

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		DmnDecisionTaskActivityBehavior behavior = (DmnDecisionTaskActivityBehavior) activity.ActivityBehavior;
		DecisionResultMapper mapper = behavior.DecisionTableResultMapper;
		assertTrue(mapper is SingleResultDecisionResultMapper);
	  }

	}

}