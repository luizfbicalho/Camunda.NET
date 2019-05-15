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
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_REPETITION_RULE;
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

	using CaseControlRule = org.camunda.bpm.engine.impl.cmmn.CaseControlRule;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using MilestoneActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.MilestoneActivityBehavior;
	using CasePlanModelHandler = org.camunda.bpm.engine.impl.cmmn.handler.CasePlanModelHandler;
	using MilestoneItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.MilestoneItemHandler;
	using SentryHandler = org.camunda.bpm.engine.impl.cmmn.handler.SentryHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using DefaultControl = org.camunda.bpm.model.cmmn.instance.DefaultControl;
	using EntryCriterion = org.camunda.bpm.model.cmmn.instance.EntryCriterion;
	using ExitCriterion = org.camunda.bpm.model.cmmn.instance.ExitCriterion;
	using IfPart = org.camunda.bpm.model.cmmn.instance.IfPart;
	using ItemControl = org.camunda.bpm.model.cmmn.instance.ItemControl;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
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
	public class MilestonePlanItemHandlerTest : CmmnElementHandlerTest
	{

	  protected internal Milestone milestone;
	  protected internal PlanItem planItem;
	  protected internal MilestoneItemHandler handler = new MilestoneItemHandler();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		milestone = createElement(casePlanModel, "aMilestone", typeof(Milestone));

		planItem = createElement(casePlanModel, "PI_aMilestone", typeof(PlanItem));
		planItem.Definition = milestone;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMilestoneActivityName()
	  public virtual void testMilestoneActivityName()
	  {
		// given:
		// the Milestone has a name "A Milestone"
		string name = "A Milestone";
		milestone.Name = name;

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
		// the Milestone has a name "A Milestone"
		string milestoneName = "A Milestone";
		milestone.Name = milestoneName;

		// the planItem has an own name "My LocalName"
		string planItemName = "My LocalName";
		planItem.Name = planItemName;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertNotEquals(milestoneName, activity.Name);
		assertEquals(planItemName, activity.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMilestoneActivityType()
	  public virtual void testMilestoneActivityType()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		string activityType = (string) activity.getProperty(PROPERTY_ACTIVITY_TYPE);
		assertEquals("milestone", activityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMilestoneDescription()
	  public virtual void testMilestoneDescription()
	  {
		// given
		string description = "This is a milestone";
		milestone.Description = description;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(description, activity.getProperty(PROPERTY_ACTIVITY_DESCRIPTION));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPlanItemDescription()
	  public virtual void testPlanItemDescription()
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
//ORIGINAL LINE: @Test public void testActivityBehavior()
	  public virtual void testActivityBehavior()
	  {
		// given: a planItem

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		CmmnActivityBehavior behavior = activity.ActivityBehavior;
		assertTrue(behavior is MilestoneActivityBehavior);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutParent()
	  public virtual void testWithoutParent()
	  {
		// given: a planItem

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertNull(activity.Parent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithParent()
	  public virtual void testWithParent()
	  {
		// given:
		// a new activity as parent
		CmmnCaseDefinition parent = new CmmnCaseDefinition("aParentActivity");
		context.Parent = parent;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(parent, activity.Parent);
		assertTrue(parent.Activities.Contains(activity));
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
		conditionExpression.Text = "${test}";

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
		conditionExpression1.Text = "${test}";

		// set first entryCriteria
		EntryCriterion criterion1 = createElement(planItem, typeof(EntryCriterion));
		criterion1.Sentry = sentry1;

		// create first sentry containing ifPart
		Sentry sentry2 = createElement(casePlanModel, "Sentry_2", typeof(Sentry));
		IfPart ifPart2 = createElement(sentry2, "ghi", typeof(IfPart));
		ConditionExpression conditionExpression2 = createElement(ifPart2, "jkl", typeof(ConditionExpression));
		conditionExpression2.Text = "${test}";

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
		conditionExpression.Text = "${test}";

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
		assertTrue(newActivity.ExitCriteria.Count == 0);

		assertFalse(newActivity.EntryCriteria.Count == 0);
		assertEquals(1, newActivity.EntryCriteria.Count);
		assertEquals(sentryDeclaration, newActivity.EntryCriteria[0]);

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
		PlanItemControl defaultControl = createElement(milestone, "ItemControl_1", typeof(DefaultControl));
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
//ORIGINAL LINE: @Test public void testRepetitionRule()
	  public virtual void testRepetitionRule()
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
		object rule = newActivity.getProperty(PROPERTY_REPETITION_RULE);
		assertNotNull(rule);
		assertTrue(rule is CaseControlRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRepetitionRuleByDefaultPlanItemControl()
	  public virtual void testRepetitionRuleByDefaultPlanItemControl()
	  {
		// given
		PlanItemControl defaultControl = createElement(milestone, "DefaultControl_1", typeof(DefaultControl));
		RepetitionRule repetitionRule = createElement(defaultControl, "RepititionRule_1", typeof(RepetitionRule));
		ConditionExpression expression = createElement(repetitionRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(planItem, context);

		// then
		object rule = newActivity.getProperty(PROPERTY_REPETITION_RULE);
		assertNotNull(rule);
		assertTrue(rule is CaseControlRule);
	  }

	}

}