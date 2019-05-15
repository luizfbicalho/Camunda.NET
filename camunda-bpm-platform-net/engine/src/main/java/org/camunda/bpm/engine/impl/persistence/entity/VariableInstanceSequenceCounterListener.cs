﻿/*
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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using VariableInstanceLifecycleListener = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class VariableInstanceSequenceCounterListener : VariableInstanceLifecycleListener<VariableInstanceEntity>
	{

	  public static readonly VariableInstanceSequenceCounterListener INSTANCE = new VariableInstanceSequenceCounterListener();

	  public virtual void onCreate(VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
	  {
	  }

	  public virtual void onDelete(VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
	  {
		variableInstance.incrementSequenceCounter();
	  }

	  public virtual void onUpdate(VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
	  {
		variableInstance.incrementSequenceCounter();
	  }

	}

}