using System;

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
namespace org.camunda.bpm.model.bpmn.builder
{
	using Process = org.camunda.bpm.model.bpmn.instance.Process;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractProcessBuilder<B> : AbstractCallableElementBuilder<B, Process> where B : AbstractProcessBuilder<B>
	{

	  protected internal AbstractProcessBuilder(BpmnModelInstance modelInstance, Process element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the process type for this.
	  /// </summary>
	  /// <param name="processType">  the process type to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B processType(ProcessType processType)
	  {
		element.ProcessType = processType;
		return myself;
	  }

	  /// <summary>
	  /// Sets this closed.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B closed()
	  {
		element.Closed = true;
		return myself;
	  }

	  /// <summary>
	  /// Sets this executable.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B executable()
	  {
		element.Executable = true;
		return myself;
	  }

	  public virtual B camundaJobPriority(string jobPriority)
	  {
		element.CamundaJobPriority = jobPriority;
		return myself;
	  }

	  /// <summary>
	  /// Set the camunda task priority attribute.
	  /// The priority is only used for service tasks which have as type value
	  /// <code>external</code>
	  /// </summary>
	  /// <param name="taskPriority"> the task priority which should used for the external tasks </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaTaskPriority(string taskPriority)
	  {
		element.CamundaTaskPriority = taskPriority;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda history time to live.
	  /// </summary>
	  /// <param name="historyTimeToLive"> value for history time to live, must be either null or non-negative integer. </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaHistoryTimeToLive(int? historyTimeToLive)
	  {
		element.CamundaHistoryTimeToLive = historyTimeToLive;
		return myself;
	  }

	  /// <summary>
	  /// Set whenever the process is startable in Tasklist
	  /// </summary>
	  /// <param name="isStartableInTasklist"> default value is true </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaStartableInTasklist(bool? isStartableInTasklist)
	  {
		element.CamundaIsStartableInTasklist = isStartableInTasklist;
		return myself;
	  }

	  /// <summary>
	  /// Set to specify a version tag for the process definition.
	  /// </summary>
	  /// <param name="versionTag"> the version of the process definition </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaVersionTag(string versionTag)
	  {
		element.CamundaVersionTag = versionTag;
		return myself;
	  }
	}

}