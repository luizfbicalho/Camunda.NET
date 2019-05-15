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
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ActivityBeforeInstantiationCmd : AbstractInstantiationCmd
	{

	  protected internal string activityId;

	  public ActivityBeforeInstantiationCmd(string activityId) : this(null, activityId)
	  {
	  }

	  public ActivityBeforeInstantiationCmd(string processInstanceId, string activityId) : this(processInstanceId, activityId, null)
	  {
	  }

	  public ActivityBeforeInstantiationCmd(string processInstanceId, string activityId, string ancestorActivityInstanceId) : base(processInstanceId, ancestorActivityInstanceId)
	  {
		this.activityId = activityId;
	  }

	  public override Void execute(CommandContext commandContext)
	  {
		ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);
		ProcessDefinitionImpl processDefinition = processInstance.getProcessDefinition();

		PvmActivity activity = processDefinition.findActivity(activityId);

		// forbid instantiation of compensation boundary events
		if (activity != null && "compensationBoundaryCatch".Equals(activity.getProperty("type")))
		{
		  throw new ProcessEngineException("Cannot start before activity " + activityId + "; activity " + "is a compensation boundary event.");
		}

		return base.execute(commandContext);
	  }

	  protected internal override ScopeImpl getTargetFlowScope(ProcessDefinitionImpl processDefinition)
	  {
		PvmActivity activity = processDefinition.findActivity(activityId);
		return activity.FlowScope;
	  }

	  protected internal override CoreModelElement getTargetElement(ProcessDefinitionImpl processDefinition)
	  {
		ActivityImpl activity = processDefinition.findActivity(activityId);
		return activity;
	  }

	  public override string TargetElementId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  protected internal override string describe()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("Start before activity '");
		sb.Append(activityId);
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