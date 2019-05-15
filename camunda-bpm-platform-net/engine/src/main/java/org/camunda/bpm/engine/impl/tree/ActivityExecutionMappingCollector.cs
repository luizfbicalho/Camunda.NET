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
namespace org.camunda.bpm.engine.impl.tree
{

	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using LegacyBehavior = org.camunda.bpm.engine.impl.pvm.runtime.LegacyBehavior;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// Collect the mappings of scopes and executions. It can be used to collect the mappings over process instances.
	/// </summary>
	/// <seealso cref= ActivityExecution#createActivityExecutionMapping()
	/// 
	/// @author Philipp Ossler
	///  </seealso>
	public class ActivityExecutionMappingCollector : TreeVisitor<ActivityExecution>
	{

	  private readonly IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping = new Dictionary<ScopeImpl, PvmExecutionImpl>();

	  private readonly ActivityExecution initialExecution;
	  private bool initialized = false;

	  public ActivityExecutionMappingCollector(ActivityExecution execution)
	  {
		this.initialExecution = execution;
	  }

	  public virtual void visit(ActivityExecution execution)
	  {
		if (!initialized)
		{
		  // lazy initialization to avoid exceptions on creation
		  appendActivityExecutionMapping(initialExecution);
		  initialized = true;
		}

		appendActivityExecutionMapping(execution);
	  }

	  private void appendActivityExecutionMapping(ActivityExecution execution)
	  {
		if (execution.Activity != null && !LegacyBehavior.hasInvalidIntermediaryActivityId((PvmExecutionImpl) execution))
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  activityExecutionMapping.putAll(execution.createActivityExecutionMapping());
		}
	  }

	  /// <returns> the mapped execution for scope or <code>null</code>, if no mapping exists </returns>
	  public virtual PvmExecutionImpl getExecutionForScope(PvmScope scope)
	  {
		return activityExecutionMapping[scope];
	  }
	}
}