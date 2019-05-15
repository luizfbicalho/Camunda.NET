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

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using CmmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.CmmnProperties;
	using CaseControlRule = org.camunda.bpm.engine.impl.cmmn.CaseControlRule;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using ProcessTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.ProcessTaskActivityBehavior;
	using CasePlanModelHandler = org.camunda.bpm.engine.impl.cmmn.handler.CasePlanModelHandler;
	using ProcessTaskItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.ProcessTaskItemHandler;
	using SentryHandler = org.camunda.bpm.engine.impl.cmmn.handler.SentryHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using CallableElementBinding = org.camunda.bpm.engine.impl.core.model.BaseCallableElement.CallableElementBinding;
	using CallableElement = org.camunda.bpm.engine.impl.core.model.CallableElement;
	using CallableElementParameter = org.camunda.bpm.engine.impl.core.model.CallableElementParameter;
	using ConstantValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ConstantValueProvider;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ElValueProvider = org.camunda.bpm.engine.impl.el.ElValueProvider;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using Body = org.camunda.bpm.model.cmmn.instance.Body;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using DefaultControl = org.camunda.bpm.model.cmmn.instance.DefaultControl;
	using EntryCriterion = org.camunda.bpm.model.cmmn.instance.EntryCriterion;
	using ExitCriterion = org.camunda.bpm.model.cmmn.instance.ExitCriterion;
	using ExtensionElements = org.camunda.bpm.model.cmmn.instance.ExtensionElements;
	using IfPart = org.camunda.bpm.model.cmmn.instance.IfPart;
	using ItemControl = org.camunda.bpm.model.cmmn.instance.ItemControl;
	using ManualActivationRule = org.camunda.bpm.model.cmmn.instance.ManualActivationRule;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using PlanItemControl = org.camunda.bpm.model.cmmn.instance.PlanItemControl;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using RepetitionRule = org.camunda.bpm.model.cmmn.instance.RepetitionRule;
	using RequiredRule = org.camunda.bpm.model.cmmn.instance.RequiredRule;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using CamundaIn = org.camunda.bpm.model.cmmn.instance.camunda.CamundaIn;
	using CamundaOut = org.camunda.bpm.model.cmmn.instance.camunda.CamundaOut;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessTaskPlanItemHandlerTest : CmmnElementHandlerTest
	{

	  protected internal ProcessTask processTask;
	  protected internal PlanItem planItem;
	  protected internal ProcessTaskItemHandler handler = new ProcessTaskItemHandler();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processTask = createElement(casePlanModel, "aProcessTask", typeof(ProcessTask));

		planItem = createElement(casePlanModel, "PI_aProcessTask", typeof(PlanItem));
		planItem.Definition = processTask;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessTaskActivityName()
	  public virtual void testProcessTaskActivityName()
	  {
		// given:
		// the processTask has a name "A ProcessTask"
		string name = "A ProcessTask";
		processTask.Name = name;

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
		// the processTask has a name "A CaseTask"
		string processTaskName = "A ProcessTask";
		processTask.Name = processTaskName;

		// the planItem has an own name "My LocalName"
		string planItemName = "My LocalName";
		planItem.Name = planItemName;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertNotEquals(processTaskName, activity.Name);
		assertEquals(planItemName, activity.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessTaskActivityType()
	  public virtual void testProcessTaskActivityType()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		string activityType = (string) activity.getProperty(PROPERTY_ACTIVITY_TYPE);
		assertEquals("processTask", activityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessTaskDescription()
	  public virtual void testProcessTaskDescription()
	  {
		// given
		string description = "This is a processTask";
		processTask.Description = description;

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
		assertTrue(behavior is ProcessTaskActivityBehavior);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsBlockingEqualsTrueProperty()
	  public virtual void testIsBlockingEqualsTrueProperty()
	  {
		// given: a processTask with isBlocking = true (defaultValue)

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
		// a processTask with isBlocking = false
		processTask.IsBlocking = false;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		bool? isBlocking = (bool?) activity.getProperty(PROPERTY_IS_BLOCKING);
		assertFalse(isBlocking);
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
//ORIGINAL LINE: @Test public void testCallableElement()
	  public virtual void testCallableElement()
	  {
		// given: a plan item

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		// there exists a callableElement
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;

		assertNotNull(behavior.CallableElement);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessRefConstant()
	  public virtual void testProcessRefConstant()
	  {
		// given:
		string processRef = "aProcessToCall";
		processTask.Process = processRef;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider processRefValueProvider = callableElement.DefinitionKeyValueProvider;
		assertNotNull(processRefValueProvider);

		assertTrue(processRefValueProvider is ConstantValueProvider);
		ConstantValueProvider valueProvider = (ConstantValueProvider) processRefValueProvider;
		assertEquals(processRef, valueProvider.getValue(null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessRefExpression()
	  public virtual void testProcessRefExpression()
	  {
		// given:
		string processRef = "${aProcessToCall}";
		processTask.Process = processRef;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider processRefValueProvider = callableElement.DefinitionKeyValueProvider;
		assertNotNull(processRefValueProvider);

		assertTrue(processRefValueProvider is ElValueProvider);
		ElValueProvider valueProvider = (ElValueProvider) processRefValueProvider;
		assertEquals(processRef, valueProvider.Expression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinding()
	  public virtual void testBinding()
	  {
		// given:
		CallableElementBinding processBinding = CallableElementBinding.LATEST;
		processTask.CamundaProcessBinding = processBinding.Value;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		CallableElementBinding binding = callableElement.Binding;
		assertNotNull(binding);
		assertEquals(processBinding, binding);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVersionConstant()
	  public virtual void testVersionConstant()
	  {
		// given:
		string processVersion = "2";
		processTask.CamundaProcessVersion = processVersion;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider processVersionValueProvider = callableElement.VersionValueProvider;
		assertNotNull(processVersionValueProvider);

		assertTrue(processVersionValueProvider is ConstantValueProvider);
		assertEquals(processVersion, processVersionValueProvider.getValue(null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVersionExpression()
	  public virtual void testVersionExpression()
	  {
		// given:
		string processVersion = "${aVersion}";
		processTask.CamundaProcessVersion = processVersion;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider processVersionValueProvider = callableElement.VersionValueProvider;
		assertNotNull(processVersionValueProvider);

		assertTrue(processVersionValueProvider is ElValueProvider);
		ElValueProvider valueProvider = (ElValueProvider) processVersionValueProvider;
		assertEquals(processVersion, valueProvider.Expression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBusinessKeyConstant()
	  public virtual void testBusinessKeyConstant()
	  {
		// given:
		string businessKey = "myBusinessKey";
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaIn businessKeyElement = createElement(extensionElements, null, typeof(CamundaIn));
		businessKeyElement.CamundaBusinessKey = businessKey;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider businessKeyValueProvider = callableElement.BusinessKeyValueProvider;
		assertNotNull(businessKeyValueProvider);

		assertTrue(businessKeyValueProvider is ConstantValueProvider);
		assertEquals(businessKey, businessKeyValueProvider.getValue(null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBusinessKeyExpression()
	  public virtual void testBusinessKeyExpression()
	  {
		// given:
		string businessKey = "${myBusinessKey}";
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaIn businessKeyElement = createElement(extensionElements, null, typeof(CamundaIn));
		businessKeyElement.CamundaBusinessKey = businessKey;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		ParameterValueProvider businessKeyValueProvider = callableElement.BusinessKeyValueProvider;
		assertNotNull(businessKeyValueProvider);

		assertTrue(businessKeyValueProvider is ElValueProvider);
		ElValueProvider valueProvider = (ElValueProvider) businessKeyValueProvider;
		assertEquals(businessKey, valueProvider.Expression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInputs()
	  public virtual void testInputs()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaIn variablesElement = createElement(extensionElements, null, typeof(CamundaIn));
		variablesElement.CamundaVariables = "all";
		CamundaIn sourceElement = createElement(extensionElements, null, typeof(CamundaIn));
		sourceElement.CamundaSource = "a";
		CamundaIn sourceExpressionElement = createElement(extensionElements, null, typeof(CamundaIn));
		sourceExpressionElement.CamundaSourceExpression = "${b}";

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then

		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		IList<CallableElementParameter> inputs = callableElement.Inputs;
		assertNotNull(inputs);
		assertFalse(inputs.Count == 0);
		assertEquals(3, inputs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInputVariables()
	  public virtual void testInputVariables()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaIn variablesElement = createElement(extensionElements, null, typeof(CamundaIn));
		variablesElement.CamundaVariables = "all";

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then

		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;
		CallableElementParameter parameter = callableElement.Inputs[0];

		assertNotNull(parameter);
		assertTrue(parameter.AllVariables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInputSource()
	  public virtual void testInputSource()
	  {
		// given:
		string source = "a";
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaIn sourceElement = createElement(extensionElements, null, typeof(CamundaIn));
		sourceElement.CamundaSource = source;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;
		CallableElementParameter parameter = callableElement.Inputs[0];

		assertNotNull(parameter);
		assertFalse(parameter.AllVariables);

		ParameterValueProvider sourceValueProvider = parameter.SourceValueProvider;
		assertNotNull(sourceValueProvider);

		assertTrue(sourceValueProvider is ConstantValueProvider);
		assertEquals(source, sourceValueProvider.getValue(null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInputSourceExpression()
	  public virtual void testInputSourceExpression()
	  {
		// given:
		string source = "${a}";
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaIn sourceElement = createElement(extensionElements, null, typeof(CamundaIn));
		sourceElement.CamundaSourceExpression = source;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;
		CallableElementParameter parameter = callableElement.Inputs[0];

		assertNotNull(parameter);
		assertFalse(parameter.AllVariables);

		ParameterValueProvider sourceExpressionValueProvider = parameter.SourceValueProvider;
		assertNotNull(sourceExpressionValueProvider);

		assertTrue(sourceExpressionValueProvider is ElValueProvider);
		ElValueProvider valueProvider = (ElValueProvider) sourceExpressionValueProvider;
		assertEquals(source, valueProvider.Expression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInputTarget()
	  public virtual void testInputTarget()
	  {
		// given:
		string target = "b";
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaIn sourceElement = createElement(extensionElements, null, typeof(CamundaIn));
		sourceElement.CamundaTarget = target;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;
		CallableElementParameter parameter = callableElement.Inputs[0];

		assertNotNull(parameter);
		assertFalse(parameter.AllVariables);

		assertEquals(target, parameter.Target);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOutputs()
	  public virtual void testOutputs()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaOut variablesElement = createElement(extensionElements, null, typeof(CamundaOut));
		variablesElement.CamundaVariables = "all";
		CamundaOut sourceElement = createElement(extensionElements, null, typeof(CamundaOut));
		sourceElement.CamundaSource = "a";
		CamundaOut sourceExpressionElement = createElement(extensionElements, null, typeof(CamundaOut));
		sourceExpressionElement.CamundaSourceExpression = "${b}";

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then

		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		IList<CallableElementParameter> outputs = callableElement.Outputs;
		assertNotNull(outputs);
		assertFalse(outputs.Count == 0);
		assertEquals(3, outputs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOutputVariables()
	  public virtual void testOutputVariables()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaOut variablesElement = createElement(extensionElements, null, typeof(CamundaOut));
		variablesElement.CamundaVariables = "all";

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then

		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;
		CallableElementParameter parameter = callableElement.Outputs[0];

		assertNotNull(parameter);
		assertTrue(parameter.AllVariables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOutputSource()
	  public virtual void testOutputSource()
	  {
		// given:
		string source = "a";
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaOut sourceElement = createElement(extensionElements, null, typeof(CamundaOut));
		sourceElement.CamundaSource = source;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;
		CallableElementParameter parameter = callableElement.Outputs[0];

		assertNotNull(parameter);
		assertFalse(parameter.AllVariables);

		ParameterValueProvider sourceValueProvider = parameter.SourceValueProvider;
		assertNotNull(sourceValueProvider);

		assertTrue(sourceValueProvider is ConstantValueProvider);
		assertEquals(source, sourceValueProvider.getValue(null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOutputSourceExpression()
	  public virtual void testOutputSourceExpression()
	  {
		// given:
		string source = "${a}";
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaOut sourceElement = createElement(extensionElements, null, typeof(CamundaOut));
		sourceElement.CamundaSourceExpression = source;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;
		CallableElementParameter parameter = callableElement.Outputs[0];

		assertNotNull(parameter);
		assertFalse(parameter.AllVariables);

		ParameterValueProvider sourceExpressionValueProvider = parameter.SourceValueProvider;
		assertNotNull(sourceExpressionValueProvider);

		assertTrue(sourceExpressionValueProvider is ElValueProvider);
		ElValueProvider valueProvider = (ElValueProvider) sourceExpressionValueProvider;
		assertEquals(source, valueProvider.Expression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOutputTarget()
	  public virtual void testOutputTarget()
	  {
		// given:
		string target = "b";
		ExtensionElements extensionElements = addExtensionElements(processTask);
		CamundaOut sourceElement = createElement(extensionElements, null, typeof(CamundaOut));
		sourceElement.CamundaTarget = target;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		ProcessTaskActivityBehavior behavior = (ProcessTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;
		CallableElementParameter parameter = callableElement.Outputs[0];

		assertNotNull(parameter);
		assertFalse(parameter.AllVariables);

		assertEquals(target, parameter.Target);
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
		conditionExpression1.Text = "${test}";

		// set first exitCriteria
		ExitCriterion criterion1 = createElement(planItem, typeof(ExitCriterion));
		criterion1.Sentry = sentry1;

		// create first sentry containing ifPart
		Sentry sentry2 = createElement(casePlanModel, "Sentry_2", typeof(Sentry));
		IfPart ifPart2 = createElement(sentry2, "ghi", typeof(IfPart));
		ConditionExpression conditionExpression2 = createElement(ifPart2, "jkl", typeof(ConditionExpression));
		conditionExpression2.Text = "${test}";

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
		PlanItemControl defaultControl = createElement(processTask, "ItemControl_1", typeof(DefaultControl));
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
		PlanItemControl defaultControl = createElement(processTask, "ItemControl_1", typeof(DefaultControl));
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
		PlanItemControl defaultControl = createElement(processTask, "DefaultControl_1", typeof(DefaultControl));
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
		PlanItemControl defaultControl = createElement(processTask, "DefaultControl_1", typeof(DefaultControl));
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
		PlanItemControl defaultControl = createElement(processTask, "DefaultControl_1", typeof(DefaultControl));
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

	}

}