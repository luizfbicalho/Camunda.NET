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
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using HumanTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.HumanTaskActivityBehavior;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using DefaultTaskFormHandler = org.camunda.bpm.engine.impl.form.handler.DefaultTaskFormHandler;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using TaskDecorator = org.camunda.bpm.engine.impl.task.TaskDecorator;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using ClassDelegateTaskListener = org.camunda.bpm.engine.impl.task.listener.ClassDelegateTaskListener;
	using DelegateExpressionTaskListener = org.camunda.bpm.engine.impl.task.listener.DelegateExpressionTaskListener;
	using ExpressionTaskListener = org.camunda.bpm.engine.impl.task.listener.ExpressionTaskListener;
	using ScriptTaskListener = org.camunda.bpm.engine.impl.task.listener.ScriptTaskListener;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using Role = org.camunda.bpm.model.cmmn.instance.Role;
	using CamundaField = org.camunda.bpm.model.cmmn.instance.camunda.CamundaField;
	using CamundaScript = org.camunda.bpm.model.cmmn.instance.camunda.CamundaScript;
	using CamundaTaskListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaTaskListener;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HumanTaskItemHandler : TaskItemHandler
	{

	  public override CmmnActivity handleElement(CmmnElement element, CmmnHandlerContext context)
	  {
		HumanTask definition = getDefinition(element);

		if (!definition.Blocking)
		{
		  // The CMMN 1.0 specification says:
		  // When a HumanTask is not 'blocking' (isBlocking is 'false'),
		  // it can be considered a 'manual' Task, i.e., the Case management
		  // system is not tracking the lifecycle of the HumanTask (instance).
		  return null;
		}

		return base.handleElement(element, context);
	  }

	  protected internal override void initializeActivity(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		// execute standard initialization
		base.initializeActivity(element, activity, context);

		// create a taskDefinition
		TaskDefinition taskDefinition = createTaskDefinition(element, context);

		// get the caseDefinition...
		CaseDefinitionEntity caseDefinition = (CaseDefinitionEntity) context.CaseDefinition;
		// ... and taskDefinition to caseDefinition
		caseDefinition.TaskDefinitions[taskDefinition.Key] = taskDefinition;

		ExpressionManager expressionManager = context.ExpressionManager;
		// create decorator
		TaskDecorator taskDecorator = new TaskDecorator(taskDefinition, expressionManager);

		// set taskDecorator on behavior
		HumanTaskActivityBehavior behavior = (HumanTaskActivityBehavior) activity.ActivityBehavior;
		behavior.TaskDecorator = taskDecorator;

		// task listeners
		initializeTaskListeners(element, activity, context, taskDefinition);

	  }

	  protected internal virtual TaskDefinition createTaskDefinition(CmmnElement element, CmmnHandlerContext context)
	  {
		Deployment deployment = context.Deployment;
		string deploymentId = deployment.Id;

		// at the moment a default task form handler is only supported,
		// custom task form handler are not supported.
		DefaultTaskFormHandler taskFormHandler = new DefaultTaskFormHandler();
		taskFormHandler.DeploymentId = deploymentId;

		// create new taskDefinition
		TaskDefinition taskDefinition = new TaskDefinition(taskFormHandler);

		// the plan item id will be handled as taskDefinitionKey
		string taskDefinitionKey = element.Id;
		taskDefinition.Key = taskDefinitionKey;

		// name
		initializeTaskDefinitionName(element, taskDefinition, context);
		// dueDate
		initializeTaskDefinitionDueDate(element, taskDefinition, context);
		// followUp
		initializeTaskDefinitionFollowUpDate(element, taskDefinition, context);
		// priority
		initializeTaskDefinitionPriority(element, taskDefinition, context);
		// assignee
		initializeTaskDefinitionAssignee(element, taskDefinition, context);
		// candidateUsers
		initializeTaskDefinitionCandidateUsers(element, taskDefinition, context);
		// candidateGroups
		initializeTaskDefinitionCandidateGroups(element, taskDefinition, context);
		// formKey
		initializeTaskDefinitionFormKey(element, taskDefinition, context);
		// description
		initializeTaskDescription(element, taskDefinition, context);

		return taskDefinition;
	  }

	  protected internal virtual void initializeTaskDefinitionName(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		string name = getName(element);
		if (string.ReferenceEquals(name, null))
		{
		  HumanTask definition = getDefinition(element);
		  name = definition.Name;
		}

		if (!string.ReferenceEquals(name, null))
		{
		  ExpressionManager expressionManager = context.ExpressionManager;
		  Expression nameExpression = expressionManager.createExpression(name);
		  taskDefinition.NameExpression = nameExpression;
		}

	  }

	  protected internal virtual void initializeTaskDefinitionFormKey(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		HumanTask definition = getDefinition(element);

		string formKey = definition.CamundaFormKey;
		if (!string.ReferenceEquals(formKey, null))
		{
		  ExpressionManager expressionManager = context.ExpressionManager;
		  Expression formKeyExpression = expressionManager.createExpression(formKey);
		  taskDefinition.FormKey = formKeyExpression;
		}
	  }

	  protected internal virtual void initializeTaskDefinitionAssignee(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		HumanTask definition = getDefinition(element);
		Role performer = definition.Performer;

		string assignee = null;
		if (performer != null)
		{
		  assignee = performer.Name;
		}
		else
		{
		  assignee = definition.CamundaAssignee;
		}

		if (!string.ReferenceEquals(assignee, null))
		{
		  ExpressionManager expressionManager = context.ExpressionManager;
		  Expression assigneeExpression = expressionManager.createExpression(assignee);
		  taskDefinition.AssigneeExpression = assigneeExpression;
		}
	  }

	  protected internal virtual void initializeTaskDefinitionCandidateUsers(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		HumanTask definition = getDefinition(element);
		ExpressionManager expressionManager = context.ExpressionManager;

		IList<string> candidateUsers = definition.CamundaCandidateUsersList;
		foreach (string candidateUser in candidateUsers)
		{
		  Expression candidateUserExpression = expressionManager.createExpression(candidateUser);
		  taskDefinition.addCandidateUserIdExpression(candidateUserExpression);
		}
	  }

	  protected internal virtual void initializeTaskDefinitionCandidateGroups(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		HumanTask definition = getDefinition(element);
		ExpressionManager expressionManager = context.ExpressionManager;

		IList<string> candidateGroups = definition.CamundaCandidateGroupsList;
		foreach (string candidateGroup in candidateGroups)
		{
		  Expression candidateGroupExpression = expressionManager.createExpression(candidateGroup);
		  taskDefinition.addCandidateGroupIdExpression(candidateGroupExpression);
		}
	  }

	  protected internal virtual void initializeTaskDefinitionDueDate(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		HumanTask definition = getDefinition(element);

		string dueDate = definition.CamundaDueDate;
		if (!string.ReferenceEquals(dueDate, null))
		{
		  ExpressionManager expressionManager = context.ExpressionManager;
		  Expression dueDateExpression = expressionManager.createExpression(dueDate);
		  taskDefinition.DueDateExpression = dueDateExpression;
		}
	  }

	  protected internal virtual void initializeTaskDefinitionFollowUpDate(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		HumanTask definition = getDefinition(element);

		string followUpDate = definition.CamundaFollowUpDate;
		if (!string.ReferenceEquals(followUpDate, null))
		{
		  ExpressionManager expressionManager = context.ExpressionManager;
		  Expression followUpDateExpression = expressionManager.createExpression(followUpDate);
		  taskDefinition.FollowUpDateExpression = followUpDateExpression;
		}
	  }

	  protected internal virtual void initializeTaskDefinitionPriority(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		HumanTask definition = getDefinition(element);

		string priority = definition.CamundaPriority;
		if (!string.ReferenceEquals(priority, null))
		{
		  ExpressionManager expressionManager = context.ExpressionManager;
		  Expression priorityExpression = expressionManager.createExpression(priority);
		  taskDefinition.PriorityExpression = priorityExpression;
		}
	  }

	  protected internal virtual void initializeTaskDescription(CmmnElement element, TaskDefinition taskDefinition, CmmnHandlerContext context)
	  {
		string description = getDesciption(element);
		if (!string.ReferenceEquals(description, null) && description.Length > 0)
		{
		  ExpressionManager expressionManager = context.ExpressionManager;
		  Expression descriptionExpression = expressionManager.createExpression(description);
		  taskDefinition.DescriptionExpression = descriptionExpression;
		}
		else
		{
		  string documentation = getDocumentation(element);
		  if (!string.ReferenceEquals(documentation, null) && documentation.Length > 0)
		  {
			ExpressionManager expressionManager = context.ExpressionManager;
			Expression documentationExpression = expressionManager.createExpression(documentation);
			taskDefinition.DescriptionExpression = documentationExpression;
		  }
		}
	  }

	  protected internal virtual void initializeTaskListeners(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, TaskDefinition taskDefinition)
	  {
		HumanTask humanTask = getDefinition(element);

		IList<CamundaTaskListener> listeners = queryExtensionElementsByClass(humanTask, typeof(CamundaTaskListener));

		foreach (CamundaTaskListener listener in listeners)
		{
		  TaskListener taskListener = initializeTaskListener(element, activity, context, listener);

		  string eventName = listener.CamundaEvent;
		  if (!string.ReferenceEquals(eventName, null))
		  {
			taskDefinition.addTaskListener(eventName, taskListener);

		  }
		  else
		  {
			taskDefinition.addTaskListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, taskListener);
			taskDefinition.addTaskListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT, taskListener);
			taskDefinition.addTaskListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE, taskListener);
			taskDefinition.addTaskListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE, taskListener);
		  }
		}
	  }

	  protected internal virtual TaskListener initializeTaskListener(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, CamundaTaskListener listener)
	  {
		ICollection<CamundaField> fields = listener.CamundaFields;
		IList<FieldDeclaration> fieldDeclarations = initializeFieldDeclarations(element, activity, context, fields);

		ExpressionManager expressionManager = context.ExpressionManager;

		TaskListener taskListener = null;

		string className = listener.CamundaClass;
		string expression = listener.CamundaExpression;
		string delegateExpression = listener.CamundaDelegateExpression;
		CamundaScript scriptElement = listener.CamundaScript;

		if (!string.ReferenceEquals(className, null))
		{
		  taskListener = new ClassDelegateTaskListener(className, fieldDeclarations);

		}
		else if (!string.ReferenceEquals(expression, null))
		{
		  Expression expressionExp = expressionManager.createExpression(expression);
		  taskListener = new ExpressionTaskListener(expressionExp);

		}
		else if (!string.ReferenceEquals(delegateExpression, null))
		{
		  Expression delegateExp = expressionManager.createExpression(delegateExpression);
		  taskListener = new DelegateExpressionTaskListener(delegateExp, fieldDeclarations);

		}
		else if (scriptElement != null)
		{
		  ExecutableScript executableScript = initializeScript(element, activity, context, scriptElement);
		  if (executableScript != null)
		  {
			taskListener = new ScriptTaskListener(executableScript);
		  }
		}

		return taskListener;
	  }

	  protected internal override HumanTask getDefinition(CmmnElement element)
	  {
		return (HumanTask) base.getDefinition(element);
	  }

	  protected internal override CmmnActivityBehavior ActivityBehavior
	  {
		  get
		  {
			return new HumanTaskActivityBehavior();
		  }
	  }

	}

}