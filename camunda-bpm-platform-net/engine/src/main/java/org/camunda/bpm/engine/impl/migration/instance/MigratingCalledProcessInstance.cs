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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingCalledProcessInstance : MigratingInstance
	{

	  public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal ExecutionEntity processInstance;

	  public MigratingCalledProcessInstance(ExecutionEntity processInstance)
	  {
		this.processInstance = processInstance;
	  }

	  public virtual bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(processInstance.SuperExecutionId, null);
		  }
	  }

	  public virtual void detachState()
	  {
		processInstance.setSuperExecution(null);
	  }

	  public virtual void attachState(MigratingScopeInstance targetActivityInstance)
	  {
		processInstance.setSuperExecution(targetActivityInstance.resolveRepresentativeExecution());
	  }

	  public virtual void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
	  }

	  public virtual void migrateState()
	  {
		// nothing to do
	  }

	  public virtual void migrateDependentEntities()
	  {
		// nothing to do
	  }

	}

}