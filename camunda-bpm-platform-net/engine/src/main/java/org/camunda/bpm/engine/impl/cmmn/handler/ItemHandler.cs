using System;
using System.Collections.Generic;
using System.Text;

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
//	import static org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE;


	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using CaseVariableListener = org.camunda.bpm.engine.@delegate.CaseVariableListener;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableListener = org.camunda.bpm.engine.@delegate.VariableListener;
	using CmmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.CmmnProperties;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using CaseControlRuleImpl = org.camunda.bpm.engine.impl.cmmn.behavior.CaseControlRuleImpl;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using ClassDelegateCaseExecutionListener = org.camunda.bpm.engine.impl.cmmn.listener.ClassDelegateCaseExecutionListener;
	using DelegateExpressionCaseExecutionListener = org.camunda.bpm.engine.impl.cmmn.listener.DelegateExpressionCaseExecutionListener;
	using ExpressionCaseExecutionListener = org.camunda.bpm.engine.impl.cmmn.listener.ExpressionCaseExecutionListener;
	using ScriptCaseExecutionListener = org.camunda.bpm.engine.impl.cmmn.listener.ScriptCaseExecutionListener;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using FixedValue = org.camunda.bpm.engine.impl.el.FixedValue;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ScriptingEngines = org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines;
	using ScriptUtil = org.camunda.bpm.engine.impl.util.ScriptUtil;
	using ClassDelegateCaseVariableListener = org.camunda.bpm.engine.impl.variable.listener.ClassDelegateCaseVariableListener;
	using DelegateExpressionCaseVariableListener = org.camunda.bpm.engine.impl.variable.listener.DelegateExpressionCaseVariableListener;
	using ExpressionCaseVariableListener = org.camunda.bpm.engine.impl.variable.listener.ExpressionCaseVariableListener;
	using ScriptCaseVariableListener = org.camunda.bpm.engine.impl.variable.listener.ScriptCaseVariableListener;
	using Query = org.camunda.bpm.model.cmmn.Query;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using DiscretionaryItem = org.camunda.bpm.model.cmmn.instance.DiscretionaryItem;
	using Documentation = org.camunda.bpm.model.cmmn.instance.Documentation;
	using ExtensionElements = org.camunda.bpm.model.cmmn.instance.ExtensionElements;
	using ManualActivationRule = org.camunda.bpm.model.cmmn.instance.ManualActivationRule;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using PlanItemControl = org.camunda.bpm.model.cmmn.instance.PlanItemControl;
	using PlanItemDefinition = org.camunda.bpm.model.cmmn.instance.PlanItemDefinition;
	using RepetitionRule = org.camunda.bpm.model.cmmn.instance.RepetitionRule;
	using RequiredRule = org.camunda.bpm.model.cmmn.instance.RequiredRule;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using CamundaCaseExecutionListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaCaseExecutionListener;
	using CamundaExpression = org.camunda.bpm.model.cmmn.instance.camunda.CamundaExpression;
	using CamundaField = org.camunda.bpm.model.cmmn.instance.camunda.CamundaField;
	using CamundaScript = org.camunda.bpm.model.cmmn.instance.camunda.CamundaScript;
	using CamundaString = org.camunda.bpm.model.cmmn.instance.camunda.CamundaString;
	using CamundaVariableListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaVariableListener;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class ItemHandler : CmmnElementHandler<CmmnElement, CmmnActivity>
	{

	  public const string PROPERTY_AUTO_COMPLETE = "autoComplete";
	  public const string PROPERTY_REQUIRED_RULE = "requiredRule";
	  public const string PROPERTY_MANUAL_ACTIVATION_RULE = "manualActivationRule";
	  public const string PROPERTY_REPETITION_RULE = "repetitionRule";
	  public const string PROPERTY_IS_BLOCKING = "isBlocking";
	  public const string PROPERTY_DISCRETIONARY = "discretionary";
	  public const string PROPERTY_ACTIVITY_TYPE = "activityType";
	  public const string PROPERTY_ACTIVITY_DESCRIPTION = "description";

	  protected internal const string PARENT_COMPLETE = "parentComplete";

	  public static IList<string> TASK_OR_STAGE_CREATE_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE);

	  public static IList<string> TASK_OR_STAGE_UPDATE_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.ENABLE, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ENABLE, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.START, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.MANUAL_START, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_SUSPEND, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_RESUME);

	  public static IList<string> TASK_OR_STAGE_END_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.EXIT, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE, PARENT_COMPLETE);

	  public static IList<string> TASK_OR_STAGE_EVENTS = new List<string>();

	  public static IList<string> EVENT_LISTENER_OR_MILESTONE_CREATE_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE);

	  public static IList<string> EVENT_LISTENER_OR_MILESTONE_UPDATE_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME);

	  public static IList<string> EVENT_LISTENER_OR_MILESTONE_END_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_TERMINATE, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.OCCUR, PARENT_COMPLETE);

	  public static IList<string> EVENT_LISTENER_OR_MILESTONE_EVENTS = new List<string>();

	  public static IList<string> CASE_PLAN_MODEL_CREATE_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE);

	  public static IList<string> CASE_PLAN_MODEL_UPDATE_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE, org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ACTIVATE);

	  public static IList<string> CASE_PLAN_MODEL_CLOSE_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CLOSE);

	  public static IList<string> CASE_PLAN_MODEL_EVENTS = new List<string>();

	  public static IList<string> DEFAULT_VARIABLE_EVENTS = Arrays.asList(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE, org.camunda.bpm.engine.@delegate.VariableListener_Fields.DELETE, org.camunda.bpm.engine.@delegate.VariableListener_Fields.UPDATE);

	  static ItemHandler()
	  {
		((IList<string>)TASK_OR_STAGE_EVENTS).AddRange(TASK_OR_STAGE_CREATE_EVENTS);
		((IList<string>)TASK_OR_STAGE_EVENTS).AddRange(TASK_OR_STAGE_UPDATE_EVENTS);
		((IList<string>)TASK_OR_STAGE_EVENTS).AddRange(TASK_OR_STAGE_END_EVENTS);

		((IList<string>)EVENT_LISTENER_OR_MILESTONE_EVENTS).AddRange(EVENT_LISTENER_OR_MILESTONE_CREATE_EVENTS);
		((IList<string>)EVENT_LISTENER_OR_MILESTONE_EVENTS).AddRange(EVENT_LISTENER_OR_MILESTONE_UPDATE_EVENTS);
		((IList<string>)EVENT_LISTENER_OR_MILESTONE_EVENTS).AddRange(EVENT_LISTENER_OR_MILESTONE_END_EVENTS);

		((IList<string>)CASE_PLAN_MODEL_EVENTS).AddRange(CASE_PLAN_MODEL_CREATE_EVENTS);
		((IList<string>)CASE_PLAN_MODEL_EVENTS).AddRange(CASE_PLAN_MODEL_UPDATE_EVENTS);
		((IList<string>)CASE_PLAN_MODEL_EVENTS).AddRange(CASE_PLAN_MODEL_CLOSE_EVENTS);
	  }

	  protected internal virtual CmmnActivity createActivity(CmmnElement element, CmmnHandlerContext context)
	  {
		string id = element.Id;
		CmmnActivity parent = context.Parent;

		CmmnActivity newActivity = null;

		if (parent != null)
		{
		  newActivity = parent.createActivity(id);

		}
		else
		{
		  CmmnCaseDefinition caseDefinition = context.CaseDefinition;
		  newActivity = new CmmnActivity(id, caseDefinition);
		}

		newActivity.CmmnElement = element;

		CmmnActivityBehavior behavior = ActivityBehavior;
		newActivity.ActivityBehavior = behavior;

		return newActivity;
	  }

	  protected internal virtual CmmnActivityBehavior ActivityBehavior
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual CmmnActivity handleElement(CmmnElement element, CmmnHandlerContext context)
	  {
		// create a new activity
		CmmnActivity newActivity = createActivity(element, context);

		// initialize activity
		initializeActivity(element, newActivity, context);

		return newActivity;
	  }

	  protected internal virtual void initializeActivity(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		if (isDiscretionaryItem(element))
		{
		  activity.setProperty(PROPERTY_DISCRETIONARY, true);
		}

		string name = getName(element);

		if (string.ReferenceEquals(name, null))
		{
		  PlanItemDefinition definition = getDefinition(element);
		  if (definition != null)
		  {
			name = definition.Name;
		  }
		}

		activity.Name = name;

		// activityType
		initializeActivityType(element, activity, context);

		// description
		initializeDescription(element, activity, context);

		// autoComplete
		initializeAutoComplete(element, activity, context);

		// requiredRule
		initializeRequiredRule(element, activity, context);

		// manualActivation
		initializeManualActivationRule(element, activity, context);

		// repetitionRule
		initializeRepetitionRule(element, activity, context);

		// case execution listeners
		initializeCaseExecutionListeners(element, activity, context);

		// variable listeners
		initializeVariableListeners(element, activity, context);

		// initialize entry criteria
		initializeEntryCriterias(element, activity, context);

		// initialize exit criteria
		initializeExitCriterias(element, activity, context);

	  }

	  protected internal virtual void initializeActivityType(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		PlanItemDefinition definition = getDefinition(element);

		string activityType = null;
		if (definition != null)
		{
		  ModelElementType elementType = definition.ElementType;
		  if (elementType != null)
		  {
			activityType = elementType.TypeName;
		  }
		}

		activity.setProperty(PROPERTY_ACTIVITY_TYPE, activityType);
	  }

	  protected internal virtual void initializeDescription(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		string description = getDesciption(element);
		if (string.ReferenceEquals(description, null))
		{
		  description = getDocumentation(element);
		}
		activity.setProperty(PROPERTY_ACTIVITY_DESCRIPTION, description);
	  }

	  protected internal virtual void initializeAutoComplete(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		// noop
	  }

	  protected internal virtual void initializeRequiredRule(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		PlanItemControl itemControl = getItemControl(element);
		PlanItemControl defaultControl = getDefaultControl(element);

		RequiredRule requiredRule = null;
		if (itemControl != null)
		{
		  requiredRule = itemControl.RequiredRule;
		}
		if (requiredRule == null && defaultControl != null)
		{
		  requiredRule = defaultControl.RequiredRule;
		}

		if (requiredRule != null)
		{
		  CaseControlRule caseRule = initializeCaseControlRule(requiredRule.Condition, context);
		  activity.setProperty(PROPERTY_REQUIRED_RULE, caseRule);
		}

	  }

	  protected internal virtual void initializeManualActivationRule(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		PlanItemControl itemControl = getItemControl(element);
		PlanItemControl defaultControl = getDefaultControl(element);

		ManualActivationRule manualActivationRule = null;
		if (itemControl != null)
		{
		  manualActivationRule = itemControl.ManualActivationRule;
		}
		if (manualActivationRule == null && defaultControl != null)
		{
		  manualActivationRule = defaultControl.ManualActivationRule;
		}

		if (manualActivationRule != null)
		{
		  CaseControlRule caseRule = initializeCaseControlRule(manualActivationRule.Condition, context);
		  activity.setProperty(PROPERTY_MANUAL_ACTIVATION_RULE, caseRule);
		}

	  }

	  protected internal virtual void initializeRepetitionRule(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		PlanItemControl itemControl = getItemControl(element);
		PlanItemControl defaultControl = getDefaultControl(element);

		RepetitionRule repetitionRule = null;
		if (itemControl != null)
		{
		  repetitionRule = itemControl.RepetitionRule;
		}
		if (repetitionRule == null && defaultControl != null)
		{
		  repetitionRule = defaultControl.RepetitionRule;
		}

		if (repetitionRule != null)
		{
		  ConditionExpression condition = repetitionRule.Condition;
		  CaseControlRule caseRule = initializeCaseControlRule(condition, context);
		  activity.setProperty(PROPERTY_REPETITION_RULE, caseRule);

		  IList<string> events = Arrays.asList(TERMINATE, COMPLETE);
		  string repeatOnStandardEvent = repetitionRule.CamundaRepeatOnStandardEvent;
		  if (!string.ReferenceEquals(repeatOnStandardEvent, null) && repeatOnStandardEvent.Length > 0)
		  {
			events = Arrays.asList(repeatOnStandardEvent);
		  }
		  activity.Properties.set(CmmnProperties.REPEAT_ON_STANDARD_EVENTS, events);
		}
	  }

	  protected internal virtual CaseControlRule initializeCaseControlRule(ConditionExpression condition, CmmnHandlerContext context)
	  {
		Expression expression = null;

		if (condition != null)
		{
		  string rule = condition.Text;
		  if (!string.ReferenceEquals(rule, null) && rule.Length > 0)
		  {
			ExpressionManager expressionManager = context.ExpressionManager;
			expression = expressionManager.createExpression(rule);
		  }
		}

		return new CaseControlRuleImpl(expression);
	  }

	  protected internal virtual void initializeCaseExecutionListeners(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		PlanItemDefinition definition = getDefinition(element);

		IList<CamundaCaseExecutionListener> listeners = queryExtensionElementsByClass(definition, typeof(CamundaCaseExecutionListener));

		foreach (CamundaCaseExecutionListener listener in listeners)
		{
		  CaseExecutionListener caseExecutionListener = initializeCaseExecutionListener(element, activity, context, listener);

		  string eventName = listener.CamundaEvent;
		  if (!string.ReferenceEquals(eventName, null))
		  {
			activity.addListener(eventName, caseExecutionListener);

		  }
		  else
		  {
			foreach (string @event in getStandardEvents(element))
			{
			  activity.addListener(@event, caseExecutionListener);
			}
		  }
		}
	  }

	  protected internal virtual CaseExecutionListener initializeCaseExecutionListener(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, CamundaCaseExecutionListener listener)
	  {
		ICollection<CamundaField> fields = listener.CamundaFields;
		IList<FieldDeclaration> fieldDeclarations = initializeFieldDeclarations(element, activity, context, fields);

		ExpressionManager expressionManager = context.ExpressionManager;

		CaseExecutionListener caseExecutionListener = null;

		string className = listener.CamundaClass;
		string expression = listener.CamundaExpression;
		string delegateExpression = listener.CamundaDelegateExpression;
		CamundaScript scriptElement = listener.CamundaScript;

		if (!string.ReferenceEquals(className, null))
		{
		  caseExecutionListener = new ClassDelegateCaseExecutionListener(className, fieldDeclarations);

		}
		else if (!string.ReferenceEquals(expression, null))
		{
		  Expression expressionExp = expressionManager.createExpression(expression);
		  caseExecutionListener = new ExpressionCaseExecutionListener(expressionExp);

		}
		else if (!string.ReferenceEquals(delegateExpression, null))
		{
		  Expression delegateExp = expressionManager.createExpression(delegateExpression);
		  caseExecutionListener = new DelegateExpressionCaseExecutionListener(delegateExp, fieldDeclarations);

		}
		else if (scriptElement != null)
		{
		  ExecutableScript executableScript = initializeScript(element, activity, context, scriptElement);
		  if (executableScript != null)
		  {
			caseExecutionListener = new ScriptCaseExecutionListener(executableScript);
		  }
		}

		return caseExecutionListener;
	  }

	  protected internal virtual void initializeVariableListeners(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		PlanItemDefinition definition = getDefinition(element);

		IList<CamundaVariableListener> listeners = queryExtensionElementsByClass(definition, typeof(CamundaVariableListener));

		foreach (CamundaVariableListener listener in listeners)
		{
		  CaseVariableListener variableListener = initializeVariableListener(element, activity, context, listener);

		  string eventName = listener.CamundaEvent;
		  if (!string.ReferenceEquals(eventName, null))
		  {
			activity.addVariableListener(eventName, variableListener);

		  }
		  else
		  {
			foreach (string @event in DEFAULT_VARIABLE_EVENTS)
			{
			  activity.addVariableListener(@event, variableListener);
			}
		  }
		}
	  }

	  protected internal virtual CaseVariableListener initializeVariableListener(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, CamundaVariableListener listener)
	  {
		ICollection<CamundaField> fields = listener.CamundaFields;
		IList<FieldDeclaration> fieldDeclarations = initializeFieldDeclarations(element, activity, context, fields);

		ExpressionManager expressionManager = context.ExpressionManager;

		string className = listener.CamundaClass;
		string expression = listener.CamundaExpression;
		string delegateExpression = listener.CamundaDelegateExpression;
		CamundaScript scriptElement = listener.CamundaScript;

		CaseVariableListener variableListener = null;
		if (!string.ReferenceEquals(className, null))
		{
		  variableListener = new ClassDelegateCaseVariableListener(className, fieldDeclarations);

		}
		else if (!string.ReferenceEquals(expression, null))
		{
		  Expression expressionExp = expressionManager.createExpression(expression);
		  variableListener = new ExpressionCaseVariableListener(expressionExp);

		}
		else if (!string.ReferenceEquals(delegateExpression, null))
		{
		  Expression delegateExp = expressionManager.createExpression(delegateExpression);
		  variableListener = new DelegateExpressionCaseVariableListener(delegateExp, fieldDeclarations);

		}
		else if (scriptElement != null)
		{
		  ExecutableScript executableScript = initializeScript(element, activity, context, scriptElement);
		  if (executableScript != null)
		  {
			variableListener = new ScriptCaseVariableListener(executableScript);
		  }
		}

		return variableListener;
	  }

	  protected internal virtual ExecutableScript initializeScript(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, CamundaScript script)
	  {
		string language = script.CamundaScriptFormat;
		string resource = script.CamundaResource;
		string source = script.TextContent;

		if (string.ReferenceEquals(language, null))
		{
		  language = ScriptingEngines.DEFAULT_SCRIPTING_LANGUAGE;
		}

		try
		{
		  return ScriptUtil.getScript(language, source, resource, context.ExpressionManager);
		}
		catch (ProcessEngineException)
		{
		  // ignore
		  return null;
		}
	  }

	  protected internal virtual IList<FieldDeclaration> initializeFieldDeclarations(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, ICollection<CamundaField> fields)
	  {
		IList<FieldDeclaration> fieldDeclarations = new List<FieldDeclaration>();

		foreach (CamundaField field in fields)
		{
		  FieldDeclaration fieldDeclaration = initializeFieldDeclaration(element, activity, context, field);
		  fieldDeclarations.Add(fieldDeclaration);
		}

		return fieldDeclarations;
	  }

	  protected internal virtual FieldDeclaration initializeFieldDeclaration(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, CamundaField field)
	  {
		string name = field.CamundaName;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		string type = typeof(Expression).FullName;

		object value = getFixedValue(field);

		if (value == null)
		{
		  ExpressionManager expressionManager = context.ExpressionManager;
		  value = getExpressionValue(field, expressionManager);
		}

		return new FieldDeclaration(name, type, value);
	  }

	  protected internal virtual FixedValue getFixedValue(CamundaField field)
	  {
		CamundaString strg = field.CamundaString;

		string value = null;
		if (strg != null)
		{
		  value = strg.TextContent;
		}

		if (string.ReferenceEquals(value, null))
		{
		  value = field.CamundaStringValue;
		}

		if (!string.ReferenceEquals(value, null))
		{
		  return new FixedValue(value);
		}

		return null;
	  }

	  protected internal virtual Expression getExpressionValue(CamundaField field, ExpressionManager expressionManager)
	  {
		CamundaExpression expression = field.CamundaExpressionChild;

		string value = null;
		if (expression != null)
		{
		  value = expression.TextContent;

		}

		if (string.ReferenceEquals(value, null))
		{
		  value = field.CamundaExpression;
		}

		if (!string.ReferenceEquals(value, null))
		{
		  return expressionManager.createExpression(value);
		}

		return null;
	  }

	  protected internal virtual void initializeEntryCriterias(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		ICollection<Sentry> entryCriterias = getEntryCriterias(element);

		if (entryCriterias.Count > 0)
		{
		  CmmnActivity parent = activity.Parent;
		  if (parent != null)
		  {
			foreach (Sentry sentry in entryCriterias)
			{
			  string sentryId = sentry.Id;
			  CmmnSentryDeclaration sentryDeclaration = parent.getSentry(sentryId);
			  if (sentryDeclaration != null)
			  {
				activity.addEntryCriteria(sentryDeclaration);
			  }
			}
		  }
		}
	  }

	  protected internal virtual void initializeExitCriterias(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		ICollection<Sentry> exitCriterias = getExitCriterias(element);

		if (exitCriterias.Count > 0)
		{
		  CmmnActivity parent = activity.Parent;
		  if (parent != null)
		  {
			foreach (Sentry sentry in exitCriterias)
			{
			  string sentryId = sentry.Id;
			  CmmnSentryDeclaration sentryDeclaration = parent.getSentry(sentryId);
			  if (sentryDeclaration != null)
			  {
				activity.addExitCriteria(sentryDeclaration);
			  }
			}
		  }
		}
	  }

	  protected internal virtual PlanItemControl getDefaultControl(CmmnElement element)
	  {
		PlanItemDefinition definition = getDefinition(element);

		return definition.DefaultControl;
	  }

	  protected internal virtual IList<V> queryExtensionElementsByClass<V>(CmmnElement element, Type cls) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  cls = typeof(V);
		ExtensionElements extensionElements = getExtensionElements(element);

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

	  protected internal virtual ExtensionElements getExtensionElements(CmmnElement element)
	  {
		return element.ExtensionElements;
	  }

	  protected internal virtual PlanItemControl getItemControl(CmmnElement element)
	  {
		if (isPlanItem(element))
		{
		  PlanItem planItem = (PlanItem) element;
		  return planItem.ItemControl;
		}
		else
		{
		if (isDiscretionaryItem(element))
		{
		  DiscretionaryItem discretionaryItem = (DiscretionaryItem) element;
		  return discretionaryItem.ItemControl;
		}
		}

		return null;
	  }

	  protected internal virtual string getName(CmmnElement element)
	  {
		string name = null;
		if (isPlanItem(element))
		{
		  PlanItem planItem = (PlanItem) element;
		  name = planItem.Name;
		}

		if (string.ReferenceEquals(name, null) || name.Length == 0)
		{
		  PlanItemDefinition definition = getDefinition(element);
		  if (definition != null)
		  {
			name = definition.Name;
		  }
		}

		return name;
	  }

	  protected internal virtual PlanItemDefinition getDefinition(CmmnElement element)
	  {
		if (isPlanItem(element))
		{
		  PlanItem planItem = (PlanItem) element;
		  return planItem.Definition;
		}
		else
		{
		if (isDiscretionaryItem(element))
		{
		  DiscretionaryItem discretionaryItem = (DiscretionaryItem) element;
		  return discretionaryItem.Definition;
		}
		}

		return null;
	  }

	  protected internal virtual ICollection<Sentry> getEntryCriterias(CmmnElement element)
	  {
		if (isPlanItem(element))
		{
		  PlanItem planItem = (PlanItem) element;
		  return planItem.EntryCriteria;
		}

		return new List<Sentry>();
	  }

	  protected internal virtual ICollection<Sentry> getExitCriterias(CmmnElement element)
	  {
		if (isPlanItem(element))
		{
		  PlanItem planItem = (PlanItem) element;
		  return planItem.ExitCriteria;
		}

		return new List<Sentry>();
	  }

	  protected internal virtual string getDesciption(CmmnElement element)
	  {
		string description = element.Description;

		if (string.ReferenceEquals(description, null))
		{
		  PlanItemDefinition definition = getDefinition(element);
		  description = definition.Description;
		}

		return description;
	  }

	  protected internal virtual string getDocumentation(CmmnElement element)
	  {
		ICollection<Documentation> documentations = element.Documentations;

		if (documentations.Count == 0)
		{
		  PlanItemDefinition definition = getDefinition(element);
		  documentations = definition.Documentations;
		}

		if (documentations.Count == 0)
		{
		  return null;
		}

		StringBuilder builder = new StringBuilder();
		foreach (Documentation doc in documentations)
		{

		  string content = doc.TextContent;
		  if (string.ReferenceEquals(content, null) || content.Length == 0)
		  {
			continue;
		  }

		  if (builder.Length != 0)
		  {
			builder.Append("\n\n");
		  }

		  builder.Append(content.Trim());
		}

		return builder.ToString();


	  }

	  protected internal virtual bool isPlanItem(CmmnElement element)
	  {
		return element is PlanItem;
	  }

	  protected internal virtual bool isDiscretionaryItem(CmmnElement element)
	  {
		return element is DiscretionaryItem;
	  }

	  protected internal abstract IList<string> getStandardEvents(CmmnElement element);

	}

}