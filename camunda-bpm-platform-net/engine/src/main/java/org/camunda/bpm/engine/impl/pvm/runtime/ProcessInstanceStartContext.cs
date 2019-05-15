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
namespace org.camunda.bpm.engine.impl.pvm.runtime
{

	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using FlowScopeWalker = org.camunda.bpm.engine.impl.tree.FlowScopeWalker;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;
	using WalkCondition = org.camunda.bpm.engine.impl.tree.ReferenceWalker.WalkCondition;
	using ScopeCollector = org.camunda.bpm.engine.impl.tree.ScopeCollector;

	/// <summary>
	/// Callback for being notified when a model instance has started.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessInstanceStartContext : ExecutionStartContext
	{

	  protected internal ActivityImpl initial;

	  protected internal new InstantiationStack instantiationStack;

	  /// <param name="initial"> </param>
	  public ProcessInstanceStartContext(ActivityImpl initial)
	  {
		this.initial = initial;
	  }

	  public virtual ActivityImpl Initial
	  {
		  get
		  {
			return initial;
		  }
		  set
		  {
			this.initial = value;
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public InstantiationStack getInstantiationStack()
	  public override InstantiationStack InstantiationStack
	  {
		  get
		  {
    
			if (instantiationStack == null)
			{
			  FlowScopeWalker flowScopeWalker = new FlowScopeWalker(initial.FlowScope);
			  ScopeCollector scopeCollector = new ScopeCollector();
			  flowScopeWalker.addPreVisitor(scopeCollector).walkWhile(new WalkConditionAnonymousInnerClass(this));
    
			  IList<PvmActivity> scopeActivities = (System.Collections.IList) scopeCollector.Scopes;
			  scopeActivities.Reverse();
    
			  instantiationStack = new InstantiationStack(scopeActivities, initial, null);
			}
    
			return instantiationStack;
		  }
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<ScopeImpl>
	  {
		  private readonly ProcessInstanceStartContext outerInstance;

		  public WalkConditionAnonymousInnerClass(ProcessInstanceStartContext outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public bool isFulfilled(ScopeImpl element)
		  {
			return element == null || element == outerInstance.initial.ProcessDefinition;
		  }
	  }

	  public virtual bool Async
	  {
		  get
		  {
			return initial.AsyncBefore;
		  }
	  }

	}

}