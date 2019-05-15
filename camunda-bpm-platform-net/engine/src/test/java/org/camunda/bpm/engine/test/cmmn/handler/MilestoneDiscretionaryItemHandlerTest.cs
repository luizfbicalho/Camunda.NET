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
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_DISCRETIONARY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_REQUIRED_RULE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using CaseControlRule = org.camunda.bpm.engine.impl.cmmn.CaseControlRule;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using MilestoneActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.MilestoneActivityBehavior;
	using MilestoneItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.MilestoneItemHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using DefaultControl = org.camunda.bpm.model.cmmn.instance.DefaultControl;
	using DiscretionaryItem = org.camunda.bpm.model.cmmn.instance.DiscretionaryItem;
	using ItemControl = org.camunda.bpm.model.cmmn.instance.ItemControl;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
	using PlanItemControl = org.camunda.bpm.model.cmmn.instance.PlanItemControl;
	using PlanningTable = org.camunda.bpm.model.cmmn.instance.PlanningTable;
	using RequiredRule = org.camunda.bpm.model.cmmn.instance.RequiredRule;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class MilestoneDiscretionaryItemHandlerTest : CmmnElementHandlerTest
	{

	  protected internal Milestone milestone;
	  protected internal PlanningTable planningTable;
	  protected internal DiscretionaryItem discretionaryItem;
	  protected internal MilestoneItemHandler handler = new MilestoneItemHandler();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		milestone = createElement(casePlanModel, "aMilestone", typeof(Milestone));

		planningTable = createElement(casePlanModel, "aPlanningTable", typeof(PlanningTable));

		discretionaryItem = createElement(planningTable, "DI_aMilestone", typeof(DiscretionaryItem));
		discretionaryItem.Definition = milestone;

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
		CmmnActivity activity = handler.handleElement(discretionaryItem, context);

		// then
		assertEquals(name, activity.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMilestoneActivityType()
	  public virtual void testMilestoneActivityType()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(discretionaryItem, context);

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
		CmmnActivity activity = handler.handleElement(discretionaryItem, context);

		// then
		assertEquals(description, activity.getProperty(PROPERTY_ACTIVITY_DESCRIPTION));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDiscretionaryItemDescription()
	  public virtual void testDiscretionaryItemDescription()
	  {
		// given
		string description = "This is a discretionaryItem";
		discretionaryItem.Description = description;

		// when
		CmmnActivity activity = handler.handleElement(discretionaryItem, context);

		// then
		assertEquals(description, activity.getProperty(PROPERTY_ACTIVITY_DESCRIPTION));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityBehavior()
	  public virtual void testActivityBehavior()
	  {
		// given: a planItem

		// when
		CmmnActivity activity = handler.handleElement(discretionaryItem, context);

		// then
		CmmnActivityBehavior behavior = activity.ActivityBehavior;
		assertTrue(behavior is MilestoneActivityBehavior);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsDiscretionaryProperty()
	  public virtual void testIsDiscretionaryProperty()
	  {
		// given:
		// a discretionary item to handle

		// when
		CmmnActivity activity = handler.handleElement(discretionaryItem, context);

		// then
		bool? discretionary = (bool?) activity.getProperty(PROPERTY_DISCRETIONARY);
		assertTrue(discretionary);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutParent()
	  public virtual void testWithoutParent()
	  {
		// given: a planItem

		// when
		CmmnActivity activity = handler.handleElement(discretionaryItem, context);

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
		CmmnActivity activity = handler.handleElement(discretionaryItem, context);

		// then
		assertEquals(parent, activity.Parent);
		assertTrue(parent.Activities.Contains(activity));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRequiredRule()
	  public virtual void testRequiredRule()
	  {
		// given
		ItemControl itemControl = createElement(discretionaryItem, "ItemControl_1", typeof(ItemControl));
		RequiredRule requiredRule = createElement(itemControl, "RequiredRule_1", typeof(RequiredRule));
		ConditionExpression expression = createElement(requiredRule, "Expression_1", typeof(ConditionExpression));
		expression.Text = "${true}";

		Cmmn.validateModel(modelInstance);

		// when
		CmmnActivity newActivity = handler.handleElement(discretionaryItem, context);

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
		CmmnActivity newActivity = handler.handleElement(discretionaryItem, context);

		// then
		object rule = newActivity.getProperty(PROPERTY_REQUIRED_RULE);
		assertNotNull(rule);
		assertTrue(rule is CaseControlRule);
	  }

	}

}