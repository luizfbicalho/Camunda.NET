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
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;

	/// <summary>
	/// <para>An activity instance is the instance of an activity</para>
	/// 
	/// @author roman.smirnov
	/// 
	/// </summary>
	public class ActivityInstanceDto
	{

	  protected internal string id;
	  protected internal string parentActivityInstanceId;
	  protected internal string activityId;
	  protected internal string activityType;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal ActivityInstanceDto[] childActivityInstances;
	  protected internal TransitionInstanceDto[] childTransitionInstances;
	  protected internal string[] executionIds;
	  protected internal string activityName;

	  /// <summary>
	  /// The id of the activity instance </summary>
	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  /// <summary>
	  /// The id of the parent activity instance. If the activity is the process definition,
	  /// <seealso cref="#getId()"/> and <seealso cref="#getParentActivityInstanceId()"/> return the same value 
	  /// </summary>
	  public virtual string ParentActivityInstanceId
	  {
		  get
		  {
			return parentActivityInstanceId;
		  }
	  }

	  /// <summary>
	  /// the id of the activity </summary>
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  /// <summary>
	  /// type of the activity, corresponds to BPMN element name in XML (e.g. 'userTask') </summary>
	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
	  }

	  /// <summary>
	  /// the process instance id </summary>
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  /// <summary>
	  /// the process definition id </summary>
	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  /// <summary>
	  /// Returns the child activity instances.
	  /// Returns an empty list if there are no child instances. 
	  /// </summary>
	  public virtual ActivityInstanceDto[] ChildActivityInstances
	  {
		  get
		  {
			return childActivityInstances;
		  }
	  }

	  public virtual TransitionInstanceDto[] ChildTransitionInstances
	  {
		  get
		  {
			return childTransitionInstances;
		  }
	  }

	  /// <summary>
	  /// the list of executions that are currently waiting in this activity instance </summary>
	  public virtual string[] ExecutionIds
	  {
		  get
		  {
			return executionIds;
		  }
	  }

	  /// <summary>
	  /// the activity name </summary>
	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
	  }

	  /// <summary>
	  /// deprecated; the JSON field with this name was never documented, but existed
	  /// from 7.0 to 7.2
	  /// </summary>
	  public virtual string Name
	  {
		  get
		  {
			return activityName;
		  }
	  }

	  public static ActivityInstanceDto fromActivityInstance(ActivityInstance instance)
	  {
		ActivityInstanceDto result = new ActivityInstanceDto();
		result.id = instance.Id;
		result.parentActivityInstanceId = instance.ParentActivityInstanceId;
		result.activityId = instance.ActivityId;
		result.activityType = instance.ActivityType;
		result.processInstanceId = instance.ProcessInstanceId;
		result.processDefinitionId = instance.ProcessDefinitionId;
		result.childActivityInstances = fromListOfActivityInstance(instance.ChildActivityInstances);
		result.childTransitionInstances = TransitionInstanceDto.fromListOfTransitionInstance(instance.ChildTransitionInstances);
		result.executionIds = instance.ExecutionIds;
		result.activityName = instance.ActivityName;
		return result;
	  }

	  public static ActivityInstanceDto[] fromListOfActivityInstance(ActivityInstance[] instances)
	  {
		ActivityInstanceDto[] result = new ActivityInstanceDto[instances.Length];
		for (int i = 0; i < result.Length; i++)
		{
		  result[i] = fromActivityInstance(instances[i]);
		}
		return result;
	  }

	}

}