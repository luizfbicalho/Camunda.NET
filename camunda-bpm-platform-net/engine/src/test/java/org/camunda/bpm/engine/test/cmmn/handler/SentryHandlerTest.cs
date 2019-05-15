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
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using CasePlanModelHandler = org.camunda.bpm.engine.impl.cmmn.handler.CasePlanModelHandler;
	using SentryHandler = org.camunda.bpm.engine.impl.cmmn.handler.SentryHandler;
	using TaskItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.TaskItemHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnIfPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnIfPartDeclaration;
	using CmmnOnPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnOnPartDeclaration;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using CmmnVariableOnPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnVariableOnPartDeclaration;
	using CmmnTransformException = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformException;
	using PlanItemTransition = org.camunda.bpm.model.cmmn.PlanItemTransition;
	using VariableTransition = org.camunda.bpm.model.cmmn.VariableTransition;
	using Body = org.camunda.bpm.model.cmmn.instance.Body;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using ExtensionElements = org.camunda.bpm.model.cmmn.instance.ExtensionElements;
	using IfPart = org.camunda.bpm.model.cmmn.instance.IfPart;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using PlanItemOnPart = org.camunda.bpm.model.cmmn.instance.PlanItemOnPart;
	using PlanItemTransitionStandardEvent = org.camunda.bpm.model.cmmn.instance.PlanItemTransitionStandardEvent;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;
	using CamundaVariableOnPart = org.camunda.bpm.model.cmmn.instance.camunda.CamundaVariableOnPart;
	using CamundaVariableTransitionEvent = org.camunda.bpm.model.cmmn.instance.camunda.CamundaVariableTransitionEvent;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class SentryHandlerTest : CmmnElementHandlerTest
	{

	  protected internal Sentry sentry;
	  protected internal PlanItemOnPart onPart;
	  protected internal CamundaVariableOnPart variableOnPart;
	  protected internal Task task;
	  protected internal PlanItem planItem;
	  protected internal ExtensionElements extensionElements;
	  protected internal TaskItemHandler taskItemHandler = new TaskItemHandler();
	  protected internal SentryHandler sentryHandler = new SentryHandler();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		task = createElement(casePlanModel, "aTask", typeof(Task));

		planItem = createElement(casePlanModel, "PI_aTask", typeof(PlanItem));
		planItem.Definition = task;

		sentry = createElement(casePlanModel, "aSentry", typeof(Sentry));

		onPart = createElement(sentry, "anOnPart", typeof(PlanItemOnPart));
		onPart.Source = planItem;
		createElement(onPart, null, typeof(PlanItemTransitionStandardEvent));
		onPart.StandardEvent = PlanItemTransition.complete;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSentry()
	  public virtual void testSentry()
	  {
		// given

		// when
		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);

		// then
		assertNotNull(sentryDeclaration);

		assertEquals(sentry.Id, sentryDeclaration.Id);

		assertNull(sentryDeclaration.IfPart);
		assertTrue(sentryDeclaration.OnParts.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSentryWithIfPart()
	  public virtual void testSentryWithIfPart()
	  {
		// given
		IfPart ifPart = createElement(sentry, "abc", typeof(IfPart));
		ConditionExpression conditionExpression = createElement(ifPart, "def", typeof(ConditionExpression));
		Body body = createElement(conditionExpression, null, typeof(Body));
		string expression = "${test}";
		body.TextContent = expression;

		// when
		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);

		// then
		assertNotNull(sentryDeclaration);

		CmmnIfPartDeclaration ifPartDeclaration = sentryDeclaration.IfPart;
		assertNotNull(ifPartDeclaration);

		Expression condition = ifPartDeclaration.Condition;
		assertNotNull(condition);
		assertEquals(expression, condition.ExpressionText);

		assertTrue(sentryDeclaration.OnParts.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSentryWithIfPartWithMultipleCondition()
	  public virtual void testSentryWithIfPartWithMultipleCondition()
	  {
		// given
		IfPart ifPart = createElement(sentry, "abc", typeof(IfPart));

		ConditionExpression firstConditionExpression = createElement(ifPart, "con_1", typeof(ConditionExpression));
		Body firstBody = createElement(firstConditionExpression, null, typeof(Body));
		string firstExpression = "${firstExpression}";
		firstBody.TextContent = firstExpression;

		ConditionExpression secondConditionExpression = createElement(ifPart, "con_2", typeof(ConditionExpression));
		Body secondBody = createElement(secondConditionExpression, null, typeof(Body));
		string secondExpression = "${secondExpression}";
		secondBody.TextContent = secondExpression;

		// when
		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);

		// then
		assertNotNull(sentryDeclaration);

		CmmnIfPartDeclaration ifPartDeclaration = sentryDeclaration.IfPart;
		assertNotNull(ifPartDeclaration);

		Expression condition = ifPartDeclaration.Condition;
		assertNotNull(condition);
		assertEquals(firstExpression, condition.ExpressionText);

		// the second condition will be ignored!

		assertTrue(sentryDeclaration.OnParts.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSentryWithOnPart()
	  public virtual void testSentryWithOnPart()
	  {
		// given
		CmmnActivity casePlanModelActivity = (new CasePlanModelHandler()).handleElement(casePlanModel, context);
		context.Parent = casePlanModelActivity;

		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);
		CmmnActivity source = taskItemHandler.handleElement(planItem, context);

		// when
		sentryHandler.initializeOnParts(sentry, context);

		// then
		assertNotNull(sentryDeclaration);

		IList<CmmnOnPartDeclaration> onParts = sentryDeclaration.OnParts;
		assertNotNull(onParts);
		assertFalse(onParts.Count == 0);
		assertEquals(1, onParts.Count);

		IList<CmmnOnPartDeclaration> onPartsAssociatedWithSource = sentryDeclaration.getOnParts(source.Id);
		assertNotNull(onPartsAssociatedWithSource);
		assertFalse(onPartsAssociatedWithSource.Count == 0);
		assertEquals(1, onParts.Count);

		CmmnOnPartDeclaration onPartDeclaration = onPartsAssociatedWithSource[0];
		assertNotNull(onPartDeclaration);
		// source
		assertEquals(source, onPartDeclaration.Source);
		assertEquals(onPart.Source.Id, onPartDeclaration.Source.Id);
		// standardEvent
		assertEquals(onPart.StandardEvent.name(), onPartDeclaration.StandardEvent);
		// sentry
		assertNull(onPartDeclaration.Sentry);

		assertNull(sentryDeclaration.IfPart);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSentryWithOnPartReferencesSentry()
	  public virtual void testSentryWithOnPartReferencesSentry()
	  {
		// given
		Sentry exitSentry = createElement(casePlanModel, "anotherSentry", typeof(Sentry));
		IfPart ifPart = createElement(exitSentry, "IfPart_1", typeof(IfPart));
		ConditionExpression conditionExpression = createElement(ifPart, "con_1", typeof(ConditionExpression));
		Body body = createElement(conditionExpression, null, typeof(Body));
		body.TextContent = "${test}";

		onPart.Sentry = exitSentry;

		CmmnActivity casePlanModelActivity = (new CasePlanModelHandler()).handleElement(casePlanModel, context);
		context.Parent = casePlanModelActivity;

		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);
		CmmnSentryDeclaration exitSentryDeclaration = sentryHandler.handleElement(exitSentry, context);
		CmmnActivity source = taskItemHandler.handleElement(planItem, context);

		// when
		sentryHandler.initializeOnParts(sentry, context);

		// then
		assertNotNull(sentryDeclaration);

		IList<CmmnOnPartDeclaration> onParts = sentryDeclaration.OnParts;
		assertNotNull(onParts);
		assertFalse(onParts.Count == 0);
		assertEquals(1, onParts.Count);

		IList<CmmnOnPartDeclaration> onPartsAssociatedWithSource = sentryDeclaration.getOnParts(source.Id);
		assertNotNull(onPartsAssociatedWithSource);
		assertFalse(onPartsAssociatedWithSource.Count == 0);
		assertEquals(1, onParts.Count);

		CmmnOnPartDeclaration onPartDeclaration = onPartsAssociatedWithSource[0];
		assertNotNull(onPartDeclaration);
		// source
		assertEquals(source, onPartDeclaration.Source);
		assertEquals(onPart.Source.Id, onPartDeclaration.Source.Id);
		// standardEvent
		assertEquals(onPart.StandardEvent.name(), onPartDeclaration.StandardEvent);
		// sentry
		assertNotNull(onPartDeclaration.Sentry);
		assertEquals(exitSentryDeclaration, onPartDeclaration.Sentry);

		assertNull(sentryDeclaration.IfPart);

	  }

	  // variableOnParts
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sentryTransformWithVariableOnPart()
	  public virtual void sentryTransformWithVariableOnPart()
	  {
		// given
		ExtensionElements extensionElements = createElement(sentry, "extensionElements", typeof(ExtensionElements));
		CamundaVariableOnPart variableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		createElement(variableOnPart, null, typeof(CamundaVariableTransitionEvent));
		variableOnPart.VariableEvent = VariableTransition.create;
		variableOnPart.VariableName = "aVariable";

		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);

		// then
		assertNotNull(sentryDeclaration);
		IList<CmmnVariableOnPartDeclaration> variableOnParts = sentryDeclaration.VariableOnParts;
		assertNotNull(variableOnParts);
		assertFalse(variableOnParts.Count == 0);
		assertEquals(1, variableOnParts.Count);

		CmmnVariableOnPartDeclaration transformedVariableOnPart = variableOnParts[0];
		assertEquals("aVariable", transformedVariableOnPart.VariableName);
		assertEquals(VariableTransition.create.name(), transformedVariableOnPart.VariableEvent);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sentryTransformWithMultipleVariableOnPart()
	  public virtual void sentryTransformWithMultipleVariableOnPart()
	  {
		// given
		ExtensionElements extensionElements = createElement(sentry, "extensionElements", typeof(ExtensionElements));
		CamundaVariableOnPart variableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		createElement(variableOnPart, null, typeof(CamundaVariableTransitionEvent));
		variableOnPart.VariableEvent = VariableTransition.create;
		variableOnPart.VariableName = "aVariable";

		CamundaVariableOnPart additionalVariableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		createElement(additionalVariableOnPart, null, typeof(CamundaVariableTransitionEvent));
		additionalVariableOnPart.VariableEvent = VariableTransition.update;
		additionalVariableOnPart.VariableName = "bVariable";

		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);

		// then
		assertNotNull(sentryDeclaration);
		IList<CmmnVariableOnPartDeclaration> variableOnParts = sentryDeclaration.VariableOnParts;
		assertNotNull(variableOnParts);
		assertFalse(variableOnParts.Count == 0);
		assertEquals(2, variableOnParts.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sentryTransformWithSameVariableOnPartTwice()
	  public virtual void sentryTransformWithSameVariableOnPartTwice()
	  {
		// given
		ExtensionElements extensionElements = createElement(sentry, "extensionElements", typeof(ExtensionElements));
		CamundaVariableOnPart variableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		createElement(variableOnPart, null, typeof(CamundaVariableTransitionEvent));
		variableOnPart.VariableEvent = VariableTransition.create;
		variableOnPart.VariableName = "aVariable";

		CamundaVariableOnPart additionalVariableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		createElement(additionalVariableOnPart, null, typeof(CamundaVariableTransitionEvent));
		additionalVariableOnPart.VariableEvent = VariableTransition.create;
		additionalVariableOnPart.VariableName = "aVariable";

		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);

		// then
		assertNotNull(sentryDeclaration);
		IList<CmmnVariableOnPartDeclaration> variableOnParts = sentryDeclaration.VariableOnParts;
		assertNotNull(variableOnParts);
		assertFalse(variableOnParts.Count == 0);
		assertEquals(1, variableOnParts.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sentryTransformShouldFailWithMissingVariableEvent()
	  public virtual void sentryTransformShouldFailWithMissingVariableEvent()
	  {
		// given
		ExtensionElements extensionElements = createElement(sentry, "extensionElements", typeof(ExtensionElements));
		CamundaVariableOnPart variableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		variableOnPart.VariableName = "aVariable";

		thrown.expect(typeof(CmmnTransformException));
		thrown.expectMessage("The variableOnPart of the sentry with id 'aSentry' must have one valid variable event.");
		sentryHandler.handleElement(sentry, context);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sentryTransformShouldFailWithInvalidVariableEvent()
	  public virtual void sentryTransformShouldFailWithInvalidVariableEvent()
	  {
		// given
		ExtensionElements extensionElements = createElement(sentry, "extensionElements", typeof(ExtensionElements));
		CamundaVariableOnPart variableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		CamundaVariableTransitionEvent transitionEvent = createElement(variableOnPart, null, typeof(CamundaVariableTransitionEvent));
		transitionEvent.TextContent = "invalid";
		variableOnPart.VariableName = "aVariable";

		thrown.expect(typeof(CmmnTransformException));
		thrown.expectMessage("The variableOnPart of the sentry with id 'aSentry' must have one valid variable event.");
		sentryHandler.handleElement(sentry, context);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sentryTransformWithMultipleVariableEvent()
	  public virtual void sentryTransformWithMultipleVariableEvent()
	  {
		// given
		ExtensionElements extensionElements = createElement(sentry, "extensionElements", typeof(ExtensionElements));
		CamundaVariableOnPart variableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		CamundaVariableTransitionEvent transitionEvent = createElement(variableOnPart, null, typeof(CamundaVariableTransitionEvent));
		transitionEvent.TextContent = "create";
		CamundaVariableTransitionEvent additionalTransitionEvent = createElement(variableOnPart, null, typeof(CamundaVariableTransitionEvent));
		additionalTransitionEvent.TextContent = "delete";
		variableOnPart.VariableName = "aVariable";

		CmmnSentryDeclaration sentryDeclaration = sentryHandler.handleElement(sentry, context);

		// then
		assertNotNull(sentryDeclaration);
		IList<CmmnVariableOnPartDeclaration> variableOnParts = sentryDeclaration.VariableOnParts;
		assertNotNull(variableOnParts);
		assertFalse(variableOnParts.Count == 0);
		assertEquals(1, variableOnParts.Count);

		CmmnVariableOnPartDeclaration transformedVariableOnPart = variableOnParts[0];
		assertEquals("aVariable", transformedVariableOnPart.VariableName);
		// when there are multiple variable events then, only first variable event is considered.
		assertEquals(VariableTransition.create.name(), transformedVariableOnPart.VariableEvent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sentryTransformShouldFailWithMissingVariableName()
	  public virtual void sentryTransformShouldFailWithMissingVariableName()
	  {
		// given
		ExtensionElements extensionElements = createElement(sentry, "extensionElements", typeof(ExtensionElements));
		CamundaVariableOnPart variableOnPart = createElement(extensionElements, null, typeof(CamundaVariableOnPart));
		createElement(variableOnPart, null, typeof(CamundaVariableTransitionEvent));
		variableOnPart.VariableEvent = VariableTransition.create;

		thrown.expect(typeof(CmmnTransformException));
		thrown.expectMessage("The variableOnPart of the sentry with id 'aSentry' must have variable name.");
		sentryHandler.handleElement(sentry, context);
	  }
	}

}