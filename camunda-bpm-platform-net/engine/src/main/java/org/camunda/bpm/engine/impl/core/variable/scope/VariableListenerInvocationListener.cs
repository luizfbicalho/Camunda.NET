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
namespace org.camunda.bpm.engine.impl.core.variable.scope
{
	using VariableListener = org.camunda.bpm.engine.@delegate.VariableListener;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// @author Ryan Johnston
	/// 
	/// </summary>
	public class VariableListenerInvocationListener : VariableInstanceLifecycleListener<VariableInstanceEntity>
	{

	  protected internal readonly AbstractVariableScope targetScope;

	  public VariableListenerInvocationListener(AbstractVariableScope targetScope)
	  {
		this.targetScope = targetScope;
	  }

	  public virtual void onCreate(VariableInstanceEntity variable, AbstractVariableScope sourceScope)
	  {
		handleEvent(new VariableEvent(variable, org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE, sourceScope));
	  }

	  public virtual void onUpdate(VariableInstanceEntity variable, AbstractVariableScope sourceScope)
	  {
		handleEvent(new VariableEvent(variable, org.camunda.bpm.engine.@delegate.VariableListener_Fields.UPDATE, sourceScope));
	  }

	  public virtual void onDelete(VariableInstanceEntity variable, AbstractVariableScope sourceScope)
	  {
		handleEvent(new VariableEvent(variable, org.camunda.bpm.engine.@delegate.VariableListener_Fields.DELETE, sourceScope));
	  }

	  protected internal virtual void handleEvent(VariableEvent @event)
	  {
		AbstractVariableScope sourceScope = @event.SourceScope;

		if (sourceScope is ExecutionEntity)
		{
		  addEventToScopeExecution((ExecutionEntity) sourceScope, @event);
		}
		else if (sourceScope is TaskEntity)
		{
		  TaskEntity task = (TaskEntity) sourceScope;
		  ExecutionEntity execution = task.getExecution();
		  if (execution != null)
		  {
			addEventToScopeExecution(execution, @event);
		  }
		}
		else if (sourceScope.ParentVariableScope is ExecutionEntity)
		{
		  addEventToScopeExecution((ExecutionEntity)sourceScope.ParentVariableScope, @event);
		}
		else
		{
		  throw new ProcessEngineException("BPMN execution scope expected");
		}
	  }

	  protected internal virtual void addEventToScopeExecution(ExecutionEntity sourceScope, VariableEvent @event)
	  {

		// ignore events of variables that are not set in an execution
		ExecutionEntity sourceExecution = sourceScope;
		ExecutionEntity scopeExecution = sourceExecution.Scope ? sourceExecution : sourceExecution.Parent;
		scopeExecution.delayEvent((ExecutionEntity) targetScope, @event);

	  }
	}

}