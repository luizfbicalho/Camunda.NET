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

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class TransitionImpl : CoreModelElement, PvmTransition
	{

	  private const long serialVersionUID = 1L;

	  protected internal ActivityImpl source;
	  protected internal ActivityImpl destination;

	  protected internal ProcessDefinitionImpl processDefinition;

	  /// <summary>
	  /// Graphical information: a list of waypoints: x1, y1, x2, y2, x3, y3, .. </summary>
	  protected internal IList<int> waypoints = new List<int>();


	  public TransitionImpl(string id, ProcessDefinitionImpl processDefinition) : base(id)
	  {
		this.processDefinition = processDefinition;
	  }

	  public virtual ActivityImpl Source
	  {
		  get
		  {
			return source;
		  }
		  set
		  {
			this.source = value;
		  }
	  }

	  public virtual void setDestination(ActivityImpl destination)
	  {
		this.destination = destination;
		destination.IncomingTransitions.Add(this);
	  }

	  [Obsolete]
	  public virtual void addExecutionListener(ExecutionListener executionListener)
	  {
		base.addListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE, executionListener);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) @Deprecated public java.util.List<org.camunda.bpm.engine.delegate.ExecutionListener> getExecutionListeners()
	  [Obsolete]
	  public virtual IList<ExecutionListener> ExecutionListeners
	  {
		  get
		  {
			return (System.Collections.IList) base.getListeners(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE);
		  }
		  set
		  {
			foreach (ExecutionListener executionListener in value)
			{
			  addExecutionListener(executionListener);
			}
		  }
	  }


	  public override string ToString()
	  {
		return "(" + source.Id + ")--" + (!string.ReferenceEquals(id, null)?id + "-->(":">(") + destination.Id + ")";
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual PvmProcessDefinition ProcessDefinition
	  {
		  get
		  {
			return processDefinition;
		  }
	  }


	  public virtual PvmActivity getDestination()
	  {
		return destination;
	  }

	  public virtual IList<int> Waypoints
	  {
		  get
		  {
			return waypoints;
		  }
		  set
		  {
			this.waypoints = value;
		  }
	  }


	}

}