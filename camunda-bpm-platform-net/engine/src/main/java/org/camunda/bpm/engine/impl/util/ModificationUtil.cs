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
namespace org.camunda.bpm.engine.impl.util
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ModificationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ModificationObserverBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
	public abstract class ModificationUtil
	{

	  public static void handleChildRemovalInScope(ExecutionEntity removedExecution)
	  {
		ActivityImpl activity = removedExecution.getActivity();
		if (activity == null)
		{
		  if (removedExecution.getSuperExecution() != null)
		  {
			removedExecution = removedExecution.getSuperExecution();
			activity = removedExecution.getActivity();
			if (activity == null)
			{
			  return;
			}
		  }
		  else
		  {
			return;
		  }
		}
		ScopeImpl flowScope = activity.FlowScope;

		PvmExecutionImpl scopeExecution = removedExecution.getParentScopeExecution(false);
		PvmExecutionImpl executionInParentScope = removedExecution.Concurrent ? removedExecution : removedExecution.Parent;

		if (flowScope.ActivityBehavior != null && flowScope.ActivityBehavior is ModificationObserverBehavior)
		{
		  // let child removal be handled by the scope itself
		  ModificationObserverBehavior behavior = (ModificationObserverBehavior) flowScope.ActivityBehavior;
		  behavior.destroyInnerInstance(executionInParentScope);
		}
		else
		{
		  if (executionInParentScope.Concurrent)
		  {
			executionInParentScope.remove();
			scopeExecution.tryPruneLastConcurrentChild();
			scopeExecution.forceUpdate();
		  }
		}
	  }
	}

}