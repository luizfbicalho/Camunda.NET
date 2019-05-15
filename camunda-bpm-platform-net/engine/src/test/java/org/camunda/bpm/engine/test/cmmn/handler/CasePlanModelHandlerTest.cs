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
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_AUTO_COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using StageActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.StageActivityBehavior;
	using CasePlanModelHandler = org.camunda.bpm.engine.impl.cmmn.handler.CasePlanModelHandler;
	using SentryHandler = org.camunda.bpm.engine.impl.cmmn.handler.SentryHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using Body = org.camunda.bpm.model.cmmn.instance.Body;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using ExitCriterion = org.camunda.bpm.model.cmmn.instance.ExitCriterion;
	using IfPart = org.camunda.bpm.model.cmmn.instance.IfPart;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CasePlanModelHandlerTest : CmmnElementHandlerTest
	{

	  protected internal CasePlanModelHandler handler = new CasePlanModelHandler();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCasePlanModelActivityName()
	  public virtual void testCasePlanModelActivityName()
	  {
		// given:
		// the case plan model has a name "A CasePlanModel"
		string name = "A CasePlanModel";
		casePlanModel.Name = name;

		// when
		CmmnActivity activity = handler.handleElement(casePlanModel, context);

		// then
		assertEquals(name, activity.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCasePlanModelActivityType()
	  public virtual void testCasePlanModelActivityType()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(casePlanModel, context);

		// then
		string activityType = (string) activity.getProperty(PROPERTY_ACTIVITY_TYPE);
		assertEquals("casePlanModel", activityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCasePlanModelDescription()
	  public virtual void testCasePlanModelDescription()
	  {
		// given
		string description = "This is a casePlanModal";
		casePlanModel.Description = description;

		// when
		CmmnActivity activity = handler.handleElement(casePlanModel, context);

		// then
		assertEquals(description, activity.getProperty(PROPERTY_ACTIVITY_DESCRIPTION));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityBehavior()
	  public virtual void testActivityBehavior()
	  {
		// given: a case plan model

		// when
		CmmnActivity activity = handler.handleElement(casePlanModel, context);

		// then
		CmmnActivityBehavior behavior = activity.ActivityBehavior;
		assertTrue(behavior is StageActivityBehavior);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutParent()
	  public virtual void testWithoutParent()
	  {
		// given: a casePlanModel

		// when
		CmmnActivity activity = handler.handleElement(casePlanModel, context);

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
		CmmnActivity activity = handler.handleElement(casePlanModel, context);

		// then
		assertEquals(parent, activity.Parent);
		assertTrue(parent.Activities.Contains(activity));
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
		ExitCriterion criterion = createElement(casePlanModel, typeof(ExitCriterion));
		criterion.Sentry = sentry;

		// transform casePlanModel
		CmmnActivity newActivity = handler.handleElement(casePlanModel, context);

		// transform Sentry
		context.Parent = newActivity;
		SentryHandler sentryHandler = new SentryHandler();
		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);

		// when
		handler.initializeExitCriterias(casePlanModel, newActivity, context);

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
		conditionExpression1.Text = "${test}";

		// set first exitCriteria
		ExitCriterion criterion1 = createElement(casePlanModel, typeof(ExitCriterion));
		criterion1.Sentry = sentry1;

		// create first sentry containing ifPart
		Sentry sentry2 = createElement(casePlanModel, "Sentry_2", typeof(Sentry));
		IfPart ifPart2 = createElement(sentry2, "ghi", typeof(IfPart));
		ConditionExpression conditionExpression2 = createElement(ifPart2, "jkl", typeof(ConditionExpression));
		conditionExpression2.Text = "${test}";

		// set second exitCriteria
		ExitCriterion criterion2 = createElement(casePlanModel, typeof(ExitCriterion));
		criterion2.Sentry = sentry2;

		// transform casePlanModel
		CmmnActivity newActivity = handler.handleElement(casePlanModel, context);

		context.Parent = newActivity;
		SentryHandler sentryHandler = new SentryHandler();

		// transform first Sentry
		CmmnSentryDeclaration firstSentryDeclaration = sentryHandler.handleElement(sentry1, context);

		// transform second Sentry
		CmmnSentryDeclaration secondSentryDeclaration = sentryHandler.handleElement(sentry2, context);

		// when
		handler.initializeExitCriterias(casePlanModel, newActivity, context);

		// then
		assertTrue(newActivity.EntryCriteria.Count == 0);

		assertFalse(newActivity.ExitCriteria.Count == 0);
		assertEquals(2, newActivity.ExitCriteria.Count);

		assertTrue(newActivity.ExitCriteria.Contains(firstSentryDeclaration));
		assertTrue(newActivity.ExitCriteria.Contains(secondSentryDeclaration));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAutoComplete()
	  public virtual void testAutoComplete()
	  {
		// given
		casePlanModel.AutoComplete = true;

		// when
		CmmnActivity newActivity = handler.handleElement(casePlanModel, context);

		// then
		object autoComplete = newActivity.getProperty(PROPERTY_AUTO_COMPLETE);
		assertNotNull(autoComplete);
		assertTrue((bool?) autoComplete);
	  }

	}

}