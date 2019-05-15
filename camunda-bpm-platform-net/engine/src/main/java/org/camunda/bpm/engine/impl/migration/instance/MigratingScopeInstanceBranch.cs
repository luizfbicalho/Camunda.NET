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

	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// Keeps track of scope instances (activity instances; event scope instances) created in a branch
	/// of the activity/event scope tree from the process instance downwards
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class MigratingScopeInstanceBranch
	{
	  protected internal IDictionary<ScopeImpl, MigratingScopeInstance> scopeInstances;

	  public MigratingScopeInstanceBranch() : this(new Dictionary<ScopeImpl, MigratingScopeInstance>())
	  {
	  }

	  protected internal MigratingScopeInstanceBranch(IDictionary<ScopeImpl, MigratingScopeInstance> scopeInstances)
	  {
		this.scopeInstances = scopeInstances;
	  }

	  public virtual MigratingScopeInstanceBranch copy()
	  {
		return new MigratingScopeInstanceBranch(new Dictionary<ScopeImpl, MigratingScopeInstance>(scopeInstances));
	  }

	  public virtual MigratingScopeInstance getInstance(ScopeImpl scope)
	  {
		return scopeInstances[scope];
	  }

	  public virtual bool hasInstance(ScopeImpl scope)
	  {
		return scopeInstances.ContainsKey(scope);
	  }

	  public virtual void visited(MigratingScopeInstance scopeInstance)
	  {
		ScopeImpl targetScope = scopeInstance.TargetScope;
		if (targetScope.Scope)
		{
		  scopeInstances[targetScope] = scopeInstance;
		}
	  }
	}

}