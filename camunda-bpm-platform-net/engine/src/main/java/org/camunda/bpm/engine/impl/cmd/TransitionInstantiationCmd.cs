using System.Text;

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
namespace org.camunda.bpm.engine.impl.cmd
{
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TransitionInstantiationCmd : AbstractInstantiationCmd
	{

	  protected internal string transitionId;

	  public TransitionInstantiationCmd(string transitionId) : this(null, transitionId)
	  {
	  }

	  public TransitionInstantiationCmd(string processInstanceId, string transitionId) : this(processInstanceId, transitionId, null)
	  {
	  }

	  public TransitionInstantiationCmd(string processInstanceId, string transitionId, string ancestorActivityInstanceId) : base(processInstanceId, ancestorActivityInstanceId)
	  {
		this.transitionId = transitionId;
	  }

	  protected internal override ScopeImpl getTargetFlowScope(ProcessDefinitionImpl processDefinition)
	  {
		TransitionImpl transition = processDefinition.findTransition(transitionId);
		return transition.Source.FlowScope;
	  }

	  protected internal override CoreModelElement getTargetElement(ProcessDefinitionImpl processDefinition)
	  {
		TransitionImpl transition = processDefinition.findTransition(transitionId);
		return transition;
	  }

	  public override string TargetElementId
	  {
		  get
		  {
			return transitionId;
		  }
	  }

	  protected internal override string describe()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("Start transition '");
		sb.Append(transitionId);
		sb.Append("'");
		if (!string.ReferenceEquals(ancestorActivityInstanceId, null))
		{
		  sb.Append(" with ancestor activity instance '");
		  sb.Append(ancestorActivityInstanceId);
		  sb.Append("'");
		}

		return sb.ToString();
	  }
	}

}