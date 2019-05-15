using System;
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
namespace org.camunda.bpm.engine.impl.pvm.process
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using CoreActivityBehavior = org.camunda.bpm.engine.impl.core.@delegate.CoreActivityBehavior;
	using ExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.ExecutionImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class ProcessDefinitionImpl : ScopeImpl, PvmProcessDefinition
	{

	  private const long serialVersionUID = 1L;

	  protected internal new string name;
	  protected internal string description;
	  protected internal ActivityImpl initial;
	  protected internal IDictionary<ActivityImpl, IList<ActivityImpl>> initialActivityStacks = new Dictionary<ActivityImpl, IList<ActivityImpl>>();
	  protected internal IList<LaneSet> laneSets;
	  protected internal ParticipantProcess participantProcess;

	  public ProcessDefinitionImpl(string id) : base(id, null)
	  {
		processDefinition = this;
		// the process definition is always "a sub process scope"
		isSubProcessScope = true;
	  }

	  protected internal virtual void ensureDefaultInitialExists()
	  {
		ensureNotNull("Process '" + name + "' has no default start activity (e.g. none start event), hence you cannot use 'startProcessInstanceBy...' but have to start it using one of the modeled start events (e.g. message start events)", "initial", initial);
	  }

	  public virtual PvmProcessInstance createProcessInstance()
	  {
		ensureDefaultInitialExists();
		return createProcessInstance(null, null, initial);
	  }

	  public virtual PvmProcessInstance createProcessInstance(string businessKey)
	  {
		ensureDefaultInitialExists();
		return createProcessInstance(businessKey, null, this.initial);
	  }

	  public virtual PvmProcessInstance createProcessInstance(string businessKey, string caseInstanceId)
	  {
		ensureDefaultInitialExists();
		return createProcessInstance(businessKey, caseInstanceId, this.initial);
	  }

	  public virtual PvmProcessInstance createProcessInstance(string businessKey, ActivityImpl initial)
	  {
		return createProcessInstance(businessKey, null, initial);
	  }

	  public virtual PvmProcessInstance createProcessInstance(string businessKey, string caseInstanceId, ActivityImpl initial)
	  {
		PvmExecutionImpl processInstance = (PvmExecutionImpl) createProcessInstanceForInitial(initial);

		processInstance.BusinessKey = businessKey;
		processInstance.CaseInstanceId = caseInstanceId;

		return processInstance;
	  }

	  /// <summary>
	  /// creates a process instance using the provided activity as initial </summary>
	  public virtual PvmProcessInstance createProcessInstanceForInitial(ActivityImpl initial)
	  {
		ensureNotNull("Cannot start process instance, initial activity where the process instance should start is null", "initial", initial);

		PvmExecutionImpl processInstance = newProcessInstance();

		processInstance.ProcessDefinition = this;

		processInstance.ProcessInstance = processInstance;

		// always set the process instance to the initial activity, no matter how deeply it is nested;
		// this is required for firing history events (cf start activity) and persisting the initial activity
		// on async start
		processInstance.setActivity(initial);

		return processInstance;
	  }

	  protected internal virtual PvmExecutionImpl newProcessInstance()
	  {
		return new ExecutionImpl();
	  }

	  public virtual IList<ActivityImpl> InitialActivityStack
	  {
		  get
		  {
			return getInitialActivityStack(initial);
		  }
	  }

	  public virtual IList<ActivityImpl> getInitialActivityStack(ActivityImpl startActivity)
	  {
		  lock (this)
		  {
			IList<ActivityImpl> initialActivityStack = initialActivityStacks[startActivity];
			if (initialActivityStack == null)
			{
			  initialActivityStack = new List<ActivityImpl>();
			  ActivityImpl activity = startActivity;
			  while (activity != null)
			  {
				initialActivityStack.Insert(0, activity);
				activity = activity.ParentFlowScopeActivity;
			  }
			  initialActivityStacks[startActivity] = initialActivityStack;
			}
			return initialActivityStack;
		  }
	  }

	  public virtual string DiagramResourceName
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual void addLaneSet(LaneSet newLaneSet)
	  {
		LaneSets.Add(newLaneSet);
	  }

	  public virtual Lane getLaneForId(string id)
	  {
		if (laneSets != null && laneSets.Count > 0)
		{
		  Lane lane;
		  foreach (LaneSet set in laneSets)
		  {
			lane = set.getLaneForId(id);
			if (lane != null)
			{
			  return lane;
			}
		  }
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.impl.core.delegate.CoreActivityBehavior<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution> getActivityBehavior()
	  public override CoreActivityBehavior<BaseDelegateExecution> ActivityBehavior
	  {
		  get
		  {
			// unsupported in PVM
			return null;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

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


	  public override string ToString()
	  {
		return "ProcessDefinition(" + id + ")";
	  }

	   public virtual string Description
	   {
		   get
		   {
			return (string) getProperty("documentation");
		   }
	   }

	  /// <returns> all lane-sets defined on this process-instance. Returns an empty list if none are defined. </returns>
	  public virtual IList<LaneSet> LaneSets
	  {
		  get
		  {
			if (laneSets == null)
			{
			  laneSets = new List<LaneSet>();
			}
			return laneSets;
		  }
	  }


	  public virtual ParticipantProcess ParticipantProcess
	  {
		  set
		  {
			this.participantProcess = value;
		  }
		  get
		  {
			return participantProcess;
		  }
	  }


	  public override bool Scope
	  {
		  get
		  {
			return true;
		  }
	  }

	  public override PvmScope EventScope
	  {
		  get
		  {
			return null;
		  }
	  }

	  public override ScopeImpl FlowScope
	  {
		  get
		  {
			return null;
		  }
	  }

	  public override PvmScope LevelOfSubprocessScope
	  {
		  get
		  {
			return null;
		  }
	  }

	  public override bool SubProcessScope
	  {
		  get
		  {
			return true;
		  }
	  }

	}

}