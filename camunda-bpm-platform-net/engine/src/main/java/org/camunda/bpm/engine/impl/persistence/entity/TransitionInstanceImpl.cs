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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TransitionInstanceImpl : ProcessElementInstanceImpl, TransitionInstance
	{

	  protected internal string executionId;
	  protected internal string activityId;
	  protected internal string activityName;
	  protected internal string activityType;

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual string TargetActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }


	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
		  set
		  {
			this.activityType = value;
		  }
	  }


	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
		  set
		  {
			this.activityName = value;
		  }
	  }


	  public override string ToString()
	  {
		return this.GetType().Name + "[executionId=" + executionId + ", targetActivityId=" + activityId + ", activityName=" + activityName + ", activityType=" + activityType + ", id=" + id + ", parentActivityInstanceId=" + parentActivityInstanceId + ", processInstanceId=" + processInstanceId + ", processDefinitionId=" + processDefinitionId + "]";
	  }

	}

}