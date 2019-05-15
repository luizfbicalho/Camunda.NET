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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using CmmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.CmmnProperties;
	using CaseControlRule = org.camunda.bpm.engine.impl.cmmn.CaseControlRule;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using HumanTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.HumanTaskActivityBehavior;
	using CasePlanModelHandler = org.camunda.bpm.engine.impl.cmmn.handler.CasePlanModelHandler;
	using HumanTaskItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.HumanTaskItemHandler;
	using SentryHandler = org.camunda.bpm.engine.impl.cmmn.handler.SentryHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using ClassDelegateTaskListener = org.camunda.bpm.engine.impl.task.listener.ClassDelegateTaskListener;
	using DelegateExpressionTaskListener = org.camunda.bpm.engine.impl.task.listener.DelegateExpressionTaskListener;
	using ExpressionTaskListener = org.camunda.bpm.engine.impl.task.listener.ExpressionTaskListener;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using Body = org.camunda.bpm.model.cmmn.instance.Body;
	using CaseRole = org.camunda.bpm.model.cmmn.instance.CaseRole;
	using ConditionExpression = org.camunda.bpm.model.cmmn.instance.ConditionExpression;
	using DefaultControl = org.camunda.bpm.model.cmmn.instance.DefaultControl;
	using EntryCriterion = org.camunda.bpm.model.cmmn.instance.EntryCriterion;
	using ExitCriterion = org.camunda.bpm.model.cmmn.instance.ExitCriterion;
	using ExtensionElements = org.camunda.bpm.model.cmmn.instance.ExtensionElements;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using IfPart = org.camunda.bpm.model.cmmn.instance.IfPart;
	using ItemControl = org.camunda.bpm.model.cmmn.instance.ItemControl;
	using ManualActivationRule = org.camunda.bpm.model.cmmn.instance.ManualActivationRule;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using PlanItemControl = org.camunda.bpm.model.cmmn.instance.PlanItemControl;
	using RepetitionRule = org.camunda.bpm.model.cmmn.instance.RepetitionRule;
	using RequiredRule = org.camunda.bpm.model.cmmn.instance.RequiredRule;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using CamundaTaskListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaTaskListener;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HumanTaskPlanItemHandlerTest : CmmnElementHandlerTest
	{

	  protected internal HumanTask humanTask;
	  protected internal PlanItem planItem;
	  protected internal HumanTaskItemHandler handler = new HumanTaskItemHandler();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		humanTask = createElement(casePlanModel, "aHumanTask", typeof(HumanTask));

		planItem = createElement(casePlanModel, "PI_aHumanTask", typeof(PlanItem));
		planItem.Definition = humanTask;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHumanTaskActivityName()
	  public virtual void testHumanTaskActivityName()
	  {
		// given:
		// the humanTask has a name "A HumanTask"
		string name = "A HumanTask";
		humanTask.Name = name;

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
		// the humanTask has a name "A HumanTask"
		string humanTaskName = "A HumanTask";
		humanTask.Name = humanTaskName;

		// the planItem has an own name "My LocalName"
		string planItemName = "My LocalName";
		planItem.Name = planItemName;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertNotEquals(humanTaskName, activity.Name);
		assertEquals(planItemName, activity.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHumanTaskActivityType()
	  public virtual void testHumanTaskActivityType()
	  {
		// given

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		string activityType = (string) activity.getProperty(PROPERTY_ACTIVITY_TYPE);
		assertEquals("humanTask", activityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHumanTaskDescriptionProperty()
	  public virtual void testHumanTaskDescriptionProperty()
	  {
		// given
		string description = "This is a humanTask";
		humanTask.Description = description;

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
//ORIGINAL LINE: @Test public void testActivityBehavior()
	  public virtual void testActivityBehavior()
	  {
		// given: a planItem

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		CmmnActivityBehavior behavior = activity.ActivityBehavior;
		assertTrue(behavior is HumanTaskActivityBehavior);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsBlockingEqualsTrueProperty()
	  public virtual void testIsBlockingEqualsTrueProperty()
	  {
		// given: a humanTask with isBlocking = true (defaultValue)

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
		// a humanTask with isBlocking = false
		humanTask.IsBlocking = false;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		// According to the specification:
		// When a HumanTask is not 'blocking'
		// (isBlocking is 'false'), it can be
		// considered a 'manual' Task, i.e.,
		// the Case management system is not
		// tracking the lifecycle of the HumanTask (instance).
		assertNull(activity);
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
//ORIGINAL LINE: @Test public void testTaskDecorator()
	  public virtual void testTaskDecorator()
	  {
		// given: a plan item

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		// there exists a taskDecorator
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;

		assertNotNull(behavior.TaskDecorator);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinition()
	  public virtual void testTaskDefinition()
	  {
		// given: a plan item

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		// there exists a taskDefinition
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;

		assertNotNull(behavior.TaskDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExpressionManager()
	  public virtual void testExpressionManager()
	  {
		// given: a plan item

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;

		ExpressionManager expressionManager = behavior.ExpressionManager;
		assertNotNull(expressionManager);
		assertEquals(context.ExpressionManager, expressionManager);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionHumanTaskNameExpression()
	  public virtual void testTaskDefinitionHumanTaskNameExpression()
	  {
		// given
		string name = "A HumanTask";
		humanTask.Name = name;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;

		Expression nameExpression = behavior.TaskDefinition.NameExpression;
		assertNotNull(nameExpression);
		assertEquals("A HumanTask", nameExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionPlanItemNameExpression()
	  public virtual void testTaskDefinitionPlanItemNameExpression()
	  {
		// given
		string name = "A HumanTask";
		humanTask.Name = name;

		string planItemName = "My LocalName";
		planItem.Name = planItemName;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression nameExpression = taskDefinition.NameExpression;
		assertNotNull(nameExpression);
		assertEquals("My LocalName", nameExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionDueDateExpression()
	  public virtual void testTaskDefinitionDueDateExpression()
	  {
		// given
		string aDueDate = "aDueDate";
		humanTask.CamundaDueDate = aDueDate;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression dueDateExpression = taskDefinition.DueDateExpression;
		assertNotNull(dueDateExpression);
		assertEquals(aDueDate, dueDateExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionFollowUpDateExpression()
	  public virtual void testTaskDefinitionFollowUpDateExpression()
	  {
		// given
		string aFollowUpDate = "aFollowDate";
		humanTask.CamundaFollowUpDate = aFollowUpDate;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression followUpDateExpression = taskDefinition.FollowUpDateExpression;
		assertNotNull(followUpDateExpression);
		assertEquals(aFollowUpDate, followUpDateExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionPriorityExpression()
	  public virtual void testTaskDefinitionPriorityExpression()
	  {
		// given
		string aPriority = "aPriority";
		humanTask.CamundaPriority = aPriority;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression priorityExpression = taskDefinition.PriorityExpression;
		assertNotNull(priorityExpression);
		assertEquals(aPriority, priorityExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionPeformerExpression()
	  public virtual void testTaskDefinitionPeformerExpression()
	  {
		// given
		CaseRole role = createElement(caseDefinition, "aRole", typeof(CaseRole));
		role.Name = "aPerformerRole";

		humanTask.Performer = role;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression assigneeExpression = taskDefinition.AssigneeExpression;
		assertNotNull(assigneeExpression);
		assertEquals("aPerformerRole", assigneeExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionAssigneeExpression()
	  public virtual void testTaskDefinitionAssigneeExpression()
	  {
		// given
		string aPriority = "aPriority";
		humanTask.CamundaPriority = aPriority;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression priorityExpression = taskDefinition.PriorityExpression;
		assertNotNull(priorityExpression);
		assertEquals(aPriority, priorityExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionCandidateUsers()
	  public virtual void testTaskDefinitionCandidateUsers()
	  {
		// given
		string aCandidateUsers = "mary,john,peter";
		humanTask.CamundaCandidateUsers = aCandidateUsers;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		ISet<Expression> candidateUserExpressions = taskDefinition.CandidateUserIdExpressions;
		assertEquals(3, candidateUserExpressions.Count);

		foreach (Expression candidateUserExpression in candidateUserExpressions)
		{
		  string candidateUser = candidateUserExpression.ExpressionText;
		  if ("mary".Equals(candidateUser))
		  {
			assertEquals("mary", candidateUser);
		  }
		  else if ("john".Equals(candidateUser))
		  {
			assertEquals("john", candidateUser);
		  }
		  else if ("peter".Equals(candidateUser))
		  {
			assertEquals("peter", candidateUser);
		  }
		  else
		  {
			fail("Unexpected candidate user: " + candidateUser);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionCandidateGroups()
	  public virtual void testTaskDefinitionCandidateGroups()
	  {
		// given
		string aCandidateGroups = "accounting,management,backoffice";
		humanTask.CamundaCandidateGroups = aCandidateGroups;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		ISet<Expression> candidateGroupExpressions = taskDefinition.CandidateGroupIdExpressions;
		assertEquals(3, candidateGroupExpressions.Count);

		foreach (Expression candidateGroupExpression in candidateGroupExpressions)
		{
		  string candidateGroup = candidateGroupExpression.ExpressionText;
		  if ("accounting".Equals(candidateGroup))
		  {
			assertEquals("accounting", candidateGroup);
		  }
		  else if ("management".Equals(candidateGroup))
		  {
			assertEquals("management", candidateGroup);
		  }
		  else if ("backoffice".Equals(candidateGroup))
		  {
			assertEquals("backoffice", candidateGroup);
		  }
		  else
		  {
			fail("Unexpected candidate group: " + candidateGroup);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDefinitionFormKey()
	  public virtual void testTaskDefinitionFormKey()
	  {
		// given
		string aFormKey = "aFormKey";
		humanTask.CamundaFormKey = aFormKey;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression formKeyExpression = taskDefinition.FormKey;
		assertNotNull(formKeyExpression);
		assertEquals(aFormKey, formKeyExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHumanTaskDescription()
	  public virtual void testHumanTaskDescription()
	  {
		// given
		string description = "A description";
		humanTask.Description = description;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression descriptionExpression = taskDefinition.DescriptionExpression;
		assertNotNull(descriptionExpression);
		assertEquals(description, descriptionExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPlanItemDescription()
	  public virtual void testPlanItemDescription()
	  {
		// given
		string description = "A description";
		humanTask.Description = description;

		// the planItem has an own description
		string localDescription = "My Local Description";
		planItem.Description = localDescription;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		Expression descriptionExpression = taskDefinition.DescriptionExpression;
		assertNotNull(descriptionExpression);
		assertEquals(localDescription, descriptionExpression.ExpressionText);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTaskListenerByClass()
	  public virtual void testCreateTaskListenerByClass()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string className = "org.camunda.bpm.test.tasklistener.ABC";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaClass = className;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is ClassDelegateTaskListener);

		ClassDelegateTaskListener classDelegateListener = (ClassDelegateTaskListener) listener;
		assertEquals(className, classDelegateListener.ClassName);
		assertTrue(classDelegateListener.FieldDeclarations.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTaskListenerByDelegateExpression()
	  public virtual void testCreateTaskListenerByDelegateExpression()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string delegateExpression = "${myDelegateExpression}";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaDelegateExpression = delegateExpression;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is DelegateExpressionTaskListener);

		DelegateExpressionTaskListener delegateExpressionListener = (DelegateExpressionTaskListener) listener;
		assertEquals(delegateExpression, delegateExpressionListener.ExpressionText);
		assertTrue(delegateExpressionListener.FieldDeclarations.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTaskListenerByExpression()
	  public virtual void testCreateTaskListenerByExpression()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string expression = "${myExpression}";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaExpression = expression;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is ExpressionTaskListener);

		ExpressionTaskListener expressionListener = (ExpressionTaskListener) listener;
		assertEquals(expression, expressionListener.ExpressionText);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskListenerByClass()
	  public virtual void testCompleteTaskListenerByClass()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string className = "org.camunda.bpm.test.tasklistener.ABC";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaClass = className;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is ClassDelegateTaskListener);

		ClassDelegateTaskListener classDelegateListener = (ClassDelegateTaskListener) listener;
		assertEquals(className, classDelegateListener.ClassName);
		assertTrue(classDelegateListener.FieldDeclarations.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskListenerByDelegateExpression()
	  public virtual void testCompleteTaskListenerByDelegateExpression()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string delegateExpression = "${myDelegateExpression}";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaDelegateExpression = delegateExpression;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is DelegateExpressionTaskListener);

		DelegateExpressionTaskListener delegateExpressionListener = (DelegateExpressionTaskListener) listener;
		assertEquals(delegateExpression, delegateExpressionListener.ExpressionText);
		assertTrue(delegateExpressionListener.FieldDeclarations.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskListenerByExpression()
	  public virtual void testCompleteTaskListenerByExpression()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string expression = "${myExpression}";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaExpression = expression;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is ExpressionTaskListener);

		ExpressionTaskListener expressionListener = (ExpressionTaskListener) listener;
		assertEquals(expression, expressionListener.ExpressionText);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAssignmentTaskListenerByClass()
	  public virtual void testAssignmentTaskListenerByClass()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string className = "org.camunda.bpm.test.tasklistener.ABC";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaClass = className;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is ClassDelegateTaskListener);

		ClassDelegateTaskListener classDelegateListener = (ClassDelegateTaskListener) listener;
		assertEquals(className, classDelegateListener.ClassName);
		assertTrue(classDelegateListener.FieldDeclarations.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAssignmentTaskListenerByDelegateExpression()
	  public virtual void testAssignmentTaskListenerByDelegateExpression()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string delegateExpression = "${myDelegateExpression}";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaDelegateExpression = delegateExpression;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is DelegateExpressionTaskListener);

		DelegateExpressionTaskListener delegateExpressionListener = (DelegateExpressionTaskListener) listener;
		assertEquals(delegateExpression, delegateExpressionListener.ExpressionText);
		assertTrue(delegateExpressionListener.FieldDeclarations.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAssignmentTaskListenerByExpression()
	  public virtual void testAssignmentTaskListenerByExpression()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string expression = "${myExpression}";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaExpression = expression;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is ExpressionTaskListener);

		ExpressionTaskListener expressionListener = (ExpressionTaskListener) listener;
		assertEquals(expression, expressionListener.ExpressionText);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTaskListenerByClass()
	  public virtual void testDeleteTaskListenerByClass()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string className = "org.camunda.bpm.test.tasklistener.ABC";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaClass = className;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is ClassDelegateTaskListener);

		ClassDelegateTaskListener classDelegateListener = (ClassDelegateTaskListener) listener;
		assertEquals(className, classDelegateListener.ClassName);
		assertTrue(classDelegateListener.FieldDeclarations.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTaskListenerByDelegateExpression()
	  public virtual void testDeleteTaskListenerByDelegateExpression()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string delegateExpression = "${myDelegateExpression}";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaDelegateExpression = delegateExpression;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is DelegateExpressionTaskListener);

		DelegateExpressionTaskListener delegateExpressionListener = (DelegateExpressionTaskListener) listener;
		assertEquals(delegateExpression, delegateExpressionListener.ExpressionText);
		assertTrue(delegateExpressionListener.FieldDeclarations.Count == 0);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTaskListenerByExpression()
	  public virtual void testDeleteTaskListenerByExpression()
	  {
		// given:
		ExtensionElements extensionElements = addExtensionElements(humanTask);
		CamundaTaskListener taskListener = createElement(extensionElements, null, typeof(CamundaTaskListener));

		string expression = "${myExpression}";
		string @event = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE;
		taskListener.CamundaEvent = @event;
		taskListener.CamundaExpression = expression;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		assertEquals(0, activity.Listeners.Count);

		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		TaskDefinition taskDefinition = behavior.TaskDefinition;

		assertNotNull(taskDefinition);

		assertEquals(1, taskDefinition.TaskListeners.Count);

		IList<TaskListener> createListeners = taskDefinition.getTaskListeners(@event);
		assertEquals(1, createListeners.Count);
		TaskListener listener = createListeners[0];

		assertTrue(listener is ExpressionTaskListener);

		ExpressionTaskListener expressionListener = (ExpressionTaskListener) listener;
		assertEquals(expression, expressionListener.ExpressionText);

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

		// set entryCriterion
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

		// set entryCriteria
		EntryCriterion criterion1 = createElement(planItem, typeof(EntryCriterion));
		criterion1.Sentry = sentry;

		// set exitCriterion
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
		PlanItemControl defaultControl = createElement(humanTask, "ItemControl_1", typeof(DefaultControl));
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
		PlanItemControl defaultControl = createElement(humanTask, "ItemControl_1", typeof(DefaultControl));
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
		PlanItemControl defaultControl = createElement(humanTask, "DefaultControl_1", typeof(DefaultControl));
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
		PlanItemControl defaultControl = createElement(humanTask, "DefaultControl_1", typeof(DefaultControl));
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
		PlanItemControl defaultControl = createElement(humanTask, "DefaultControl_1", typeof(DefaultControl));
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