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
namespace org.camunda.bpm.engine.cdi.impl.@event
{
	using org.camunda.bpm.engine.cdi.annotation.@event;
	using BeanManagerLookup = org.camunda.bpm.engine.cdi.impl.util.BeanManagerLookup;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExecutionContext = org.camunda.bpm.engine.impl.context.ExecutionContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;


	/// <summary>
	/// Generic <seealso cref="ExecutionListener"/> publishing events using the cdi event
	/// infrastructure.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class CdiEventListener : TaskListener, ExecutionListener
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(CdiEventListener).FullName);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		// test whether cdi is setup correctly. (if not, just do not deliver the event)
		if (!testCdiSetup())
		{
		  return;
		}

		BusinessProcessEvent @event = createEvent(execution);
		Annotation[] qualifiers = getQualifiers(@event);
		BeanManager.fireEvent(@event, qualifiers);
	  }

	  public virtual void notify(DelegateTask task)
	  {
		// test whether cdi is setup correctly. (if not, just do not deliver the event)
		if (!testCdiSetup())
		{
		  return;
		}

		BusinessProcessEvent @event = createEvent(task);
		Annotation[] qualifiers = getQualifiers(@event);
		BeanManager.fireEvent(@event, qualifiers);
	  }

	  private bool testCdiSetup()
	  {
		try
		{
		  ProgrammaticBeanLookup.lookup(typeof(ProcessEngine));
		}
		catch (Exception)
		{
		  LOGGER.fine("CDI was not setup correctly");
		  return false;
		}
		return true;
	  }

	  protected internal virtual BusinessProcessEvent createEvent(DelegateExecution execution)
	  {
		ProcessDefinition processDefinition = Context.ExecutionContext.ProcessDefinition;

		// map type
		string eventName = execution.EventName;
		BusinessProcessEventType type = null;
		if (org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START.Equals(eventName))
		{
		  type = org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.START_ACTIVITY;
		}
		else if (org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END.Equals(eventName))
		{
		  type = org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.END_ACTIVITY;
		}
		else if (org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE.Equals(eventName))
		{
		  type = org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.TAKE;
		}

		return new CdiBusinessProcessEvent(execution.CurrentActivityId, execution.CurrentTransitionId, processDefinition, execution, type, ClockUtil.CurrentTime);
	  }

	  protected internal virtual BusinessProcessEvent createEvent(DelegateTask task)
	  {
		ExecutionContext executionContext = Context.ExecutionContext;
		ProcessDefinitionEntity processDefinition = null;
		if (executionContext != null)
		{
		  processDefinition = executionContext.ProcessDefinition;
		}

		// map type
		string eventName = task.EventName;
		BusinessProcessEventType type = null;
		if (org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE.Equals(eventName))
		{
		  type = org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.CREATE_TASK;
		}
		else if (org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT.Equals(eventName))
		{
		  type = org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.ASSIGN_TASK;
		}
		else if (org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE.Equals(eventName))
		{
		  type = org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.COMPLETE_TASK;
		}
		else if (org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE.Equals(eventName))
		{
		  type = org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.DELETE_TASK;
		}

		return new CdiBusinessProcessEvent(task, processDefinition, type, ClockUtil.CurrentTime);
	  }


	  protected internal virtual BeanManager BeanManager
	  {
		  get
		  {
			BeanManager bm = BeanManagerLookup.BeanManager;
			if (bm == null)
			{
			  throw new ProcessEngineException("No cdi bean manager available, cannot publish event.");
			}
			return bm;
		  }
	  }

	  protected internal virtual Annotation[] getQualifiers(BusinessProcessEvent @event)
	  {
		ProcessDefinition processDefinition = @event.ProcessDefinition;
		IList<Annotation> annotations = new List<Annotation>();
		if (processDefinition != null)
		{
		  annotations.Add(new BusinessProcessDefinitionLiteral(processDefinition.Key));
		}

		if (@event.Type == org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.TAKE)
		{
		  annotations.Add(new TakeTransitionLiteral(@event.TransitionName));
		}
		else if (@event.Type == org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.START_ACTIVITY)
		{
		  annotations.Add(new StartActivityLiteral(@event.ActivityId));
		}
		else if (@event.Type == org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.END_ACTIVITY)
		{
		  annotations.Add(new EndActivityLiteral(@event.ActivityId));
		}
		else if (@event.Type == org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.CREATE_TASK)
		{
		  annotations.Add(new CreateTaskLiteral(@event.TaskDefinitionKey));
		}
		else if (@event.Type == org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.ASSIGN_TASK)
		{
		  annotations.Add(new AssignTaskLiteral(@event.TaskDefinitionKey));
		}
		else if (@event.Type == org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.COMPLETE_TASK)
		{
		  annotations.Add(new CompleteTaskLiteral(@event.TaskDefinitionKey));
		}
		else if (@event.Type == org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.DELETE_TASK)
		{
		  annotations.Add(new DeleteTaskLiteral(@event.TaskDefinitionKey));
		}
		return annotations.ToArray();
	  }
	}

}