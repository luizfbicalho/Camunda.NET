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

	/// <summary>
	/// A process element instance that can have other process element instances as children
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public abstract class MigratingScopeInstance : MigratingProcessElementInstance
	{

	  public abstract void removeChild(MigratingScopeInstance migratingActivityInstance);

	  public abstract void addChild(MigratingScopeInstance migratingActivityInstance);

	  public abstract void removeChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription);

	  public abstract void addChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription);

	  public abstract bool migrates();

	  public abstract void detachChildren();

	  /// <summary>
	  /// removes this scope; parameters are hints and may be ignored by the implementation
	  /// </summary>
	  public abstract void remove(bool skipCustomListeners, bool skipIoMappings);

	  /// <summary>
	  /// gets all children
	  /// </summary>
	  public abstract ICollection<MigratingProcessElementInstance> Children {get;}

	  /// <summary>
	  /// gets those children that are itself scope instances
	  /// </summary>
	  public abstract ICollection<MigratingScopeInstance> ChildScopeInstances {get;}

	  public abstract void removeUnmappedDependentInstances();

	}

}