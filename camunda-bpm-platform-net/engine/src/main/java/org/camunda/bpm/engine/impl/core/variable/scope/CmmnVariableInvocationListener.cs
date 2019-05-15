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
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class CmmnVariableInvocationListener : VariableInstanceLifecycleListener<VariableInstanceEntity>
	{

	  public static readonly CmmnVariableInvocationListener INSTANCE = new CmmnVariableInvocationListener();

	  public virtual void onCreate(VariableInstanceEntity variable, AbstractVariableScope sourceScope)
	  {
		sourceScope.dispatchEvent(new VariableEvent(variable, org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE, sourceScope));
	  }

	  public virtual void onUpdate(VariableInstanceEntity variable, AbstractVariableScope sourceScope)
	  {
		sourceScope.dispatchEvent(new VariableEvent(variable, org.camunda.bpm.engine.@delegate.VariableListener_Fields.UPDATE, sourceScope));
	  }

	  public virtual void onDelete(VariableInstanceEntity variable, AbstractVariableScope sourceScope)
	  {
		sourceScope.dispatchEvent(new VariableEvent(variable, org.camunda.bpm.engine.@delegate.VariableListener_Fields.DELETE, sourceScope));
	  }
	}

}