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
namespace org.camunda.bpm.engine.impl.migration.instance
{

	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingActivityInstanceVisitor : MigratingProcessElementInstanceVisitor
	{

	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMappings;

	  public MigratingActivityInstanceVisitor(bool skipCustomListeners, bool skipIoMappings)
	  {
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMappings = skipIoMappings;
	  }

	  protected internal override bool canMigrate(MigratingProcessElementInstance instance)
	  {
		return instance is MigratingActivityInstance || instance is MigratingTransitionInstance;
	  }

	  protected internal override void instantiateScopes(MigratingScopeInstance ancestorScopeInstance, MigratingScopeInstanceBranch executionBranch, IList<ScopeImpl> scopesToInstantiate)
	  {

		if (scopesToInstantiate.Count == 0)
		{
		  return;
		}

		// must always be an activity instance
		MigratingActivityInstance ancestorActivityInstance = (MigratingActivityInstance) ancestorScopeInstance;

		ExecutionEntity newParentExecution = ancestorActivityInstance.createAttachableExecution();

		IDictionary<PvmActivity, PvmExecutionImpl> createdExecutions = newParentExecution.instantiateScopes((System.Collections.IList) scopesToInstantiate, skipCustomListeners, skipIoMappings);

		foreach (ScopeImpl scope in scopesToInstantiate)
		{
		  ExecutionEntity createdExecution = (ExecutionEntity) createdExecutions[scope];
		  createdExecution.setActivity(null);
		  createdExecution.Active = false;
		  executionBranch.visited(new MigratingActivityInstance(scope, createdExecution));
		}
	  }

	}

}