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
namespace org.camunda.bpm.engine.rest.dto.externaltask
{
	using ExternalTaskQueryBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryBuilder;
	using ExternalTaskQueryTopicBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryTopicBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static true;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class FetchExternalTasksDto
	{

	  protected internal int maxTasks;
	  protected internal string workerId;
	  protected internal bool usePriority = false;
	  protected internal IList<FetchExternalTaskTopicDto> topics;

	  public virtual int MaxTasks
	  {
		  get
		  {
			return maxTasks;
		  }
		  set
		  {
			this.maxTasks = value;
		  }
	  }
	  public virtual string WorkerId
	  {
		  get
		  {
			return workerId;
		  }
		  set
		  {
			this.workerId = value;
		  }
	  }
	  public virtual IList<FetchExternalTaskTopicDto> Topics
	  {
		  get
		  {
			return topics;
		  }
		  set
		  {
			this.topics = value;
		  }
	  }

	  public virtual bool UsePriority
	  {
		  get
		  {
			return usePriority;
		  }
		  set
		  {
			this.usePriority = value;
		  }
	  }


	  public class FetchExternalTaskTopicDto
	  {
		protected internal string topicName;
		protected internal string businessKey;
		protected internal string processDefinitionId;
		protected internal string[] processDefinitionIdIn;
		protected internal string processDefinitionKey;
		protected internal string[] processDefinitionKeyIn;
		protected internal long lockDuration;
		protected internal IList<string> variables;
		protected internal Dictionary<string, object> processVariables;
		protected internal bool deserializeValues = false;
		protected internal bool localVariables = false;

		protected internal bool withoutTenantId;
		protected internal string[] tenantIdIn;

		public virtual string TopicName
		{
			get
			{
			  return topicName;
			}
			set
			{
			  this.topicName = value;
			}
		}
		public virtual string BusinessKey
		{
			get
			{
			  return businessKey;
			}
			set
			{
			  this.businessKey = value;
			}
		}
		public virtual string ProcessDefinitionId
		{
			get
			{
			  return processDefinitionId;
			}
			set
			{
			  this.processDefinitionId = value;
			}
		}
		public virtual string[] ProcessDefinitionIdIn
		{
			get
			{
			  return processDefinitionIdIn;
			}
			set
			{
			  this.processDefinitionIdIn = value;
			}
		}
		public virtual string ProcessDefinitionKey
		{
			get
			{
			  return processDefinitionKey;
			}
			set
			{
			  this.processDefinitionKey = value;
			}
		}
		public virtual string[] ProcessDefinitionKeyIn
		{
			get
			{
			  return processDefinitionKeyIn;
			}
			set
			{
			  this.processDefinitionKeyIn = value;
			}
		}
		public virtual long LockDuration
		{
			get
			{
			  return lockDuration;
			}
			set
			{
			  this.lockDuration = value;
			}
		}
		public virtual IList<string> Variables
		{
			get
			{
			  return variables;
			}
			set
			{
			  this.variables = value;
			}
		}
		public virtual Dictionary<string, object> ProcessVariables
		{
			get
			{
			  return processVariables;
			}
			set
			{
			  this.processVariables = value;
			}
		}
		public virtual bool DeserializeValues
		{
			get
			{
			  return deserializeValues;
			}
			set
			{
			  this.deserializeValues = value;
			}
		}
		public virtual bool LocalVariables
		{
			get
			{
			  return localVariables;
			}
			set
			{
			  this.localVariables = value;
			}
		}
		public virtual bool WithoutTenantId
		{
			get
			{
			  return withoutTenantId;
			}
			set
			{
			  this.withoutTenantId = value;
			}
		}
		public virtual string[] TenantIdIn
		{
			get
			{
			  return tenantIdIn;
			}
			set
			{
			  this.tenantIdIn = value;
			}
		}
	  }

	  public virtual ExternalTaskQueryBuilder buildQuery(ProcessEngine processEngine)
	  {
		ExternalTaskQueryBuilder fetchBuilder = processEngine.ExternalTaskService.fetchAndLock(MaxTasks, WorkerId, UsePriority);

		if (Topics != null)
		{
		  foreach (FetchExternalTaskTopicDto topicDto in Topics)
		  {
			ExternalTaskQueryTopicBuilder topicFetchBuilder = fetchBuilder.topic(topicDto.TopicName, topicDto.LockDuration);

			if (!string.ReferenceEquals(topicDto.BusinessKey, null))
			{
			  topicFetchBuilder = topicFetchBuilder.businessKey(topicDto.BusinessKey);
			}

			if (!string.ReferenceEquals(topicDto.ProcessDefinitionId, null))
			{
			  topicFetchBuilder.processDefinitionId(topicDto.ProcessDefinitionId);
			}

			if (topicDto.ProcessDefinitionIdIn != null)
			{
			  topicFetchBuilder.processDefinitionIdIn(topicDto.ProcessDefinitionIdIn);
			}

			if (!string.ReferenceEquals(topicDto.ProcessDefinitionKey, null))
			{
			  topicFetchBuilder.processDefinitionKey(topicDto.ProcessDefinitionKey);
			}

			if (topicDto.ProcessDefinitionKeyIn != null)
			{
			  topicFetchBuilder.processDefinitionKeyIn(topicDto.ProcessDefinitionKeyIn);
			}

			if (topicDto.Variables != null)
			{
			  topicFetchBuilder = topicFetchBuilder.variables(topicDto.Variables);
			}

			if (topicDto.ProcessVariables != null)
			{
			  topicFetchBuilder = topicFetchBuilder.processInstanceVariableEquals(topicDto.ProcessVariables);
			}

			if (topicDto.DeserializeValues)
			{
			  topicFetchBuilder = topicFetchBuilder.enableCustomObjectDeserialization();
			}

			if (topicDto.LocalVariables)
			{
			  topicFetchBuilder = topicFetchBuilder.localVariables();
			}

			if (TRUE.Equals(topicDto.WithoutTenantId))
			{
			  topicFetchBuilder = topicFetchBuilder.withoutTenantId();
			}

			if (topicDto.TenantIdIn != null)
			{
			  topicFetchBuilder = topicFetchBuilder.tenantIdIn(topicDto.TenantIdIn);
			}

			fetchBuilder = topicFetchBuilder;
		  }
		}

		return fetchBuilder;
	  }

	}

}