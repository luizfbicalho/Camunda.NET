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
namespace org.camunda.bpm.engine.rest.dto.runtime
{
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TransitionInstanceDto
	{

	  protected internal string id;
	  protected internal string parentActivityInstanceId;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string activityId;
	  protected internal string activityName;
	  protected internal string activityType;
	  protected internal string executionId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string ParentActivityInstanceId
	  {
		  get
		  {
			return parentActivityInstanceId;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  [Obsolete]
	  public virtual string TargetActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
	  }

	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public static TransitionInstanceDto fromTransitionInstance(TransitionInstance instance)
	  {
		TransitionInstanceDto result = new TransitionInstanceDto();
		result.id = instance.Id;
		result.parentActivityInstanceId = instance.ParentActivityInstanceId;
		result.activityId = instance.ActivityId;
		result.activityName = instance.ActivityName;
		result.activityType = instance.ActivityType;
		result.processInstanceId = instance.ProcessInstanceId;
		result.processDefinitionId = instance.ProcessDefinitionId;
		result.executionId = instance.ExecutionId;
		return result;
	  }


	  public static TransitionInstanceDto[] fromListOfTransitionInstance(TransitionInstance[] instances)
	  {
		TransitionInstanceDto[] result = new TransitionInstanceDto[instances.Length];
		for (int i = 0; i < result.Length; i++)
		{
		  result[i] = fromTransitionInstance(instances[i]);
		}
		return result;
	  }

	}

}