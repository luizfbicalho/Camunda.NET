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
namespace org.camunda.bpm.engine.rest.dto.history
{
	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;

	public class HistoricIdentityLinkLogDto
	{
	  protected internal string id;
	  protected internal DateTime time;
	  protected internal string type;
	  protected internal string userId;
	  protected internal string groupId;
	  protected internal string taskId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string operationType;
	  protected internal string assignerId;
	  protected internal string tenantId;
	  protected internal DateTime removalTime;
	  protected internal string rootProcessInstanceId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual DateTime Time
	  {
		  get
		  {
			return time;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
	  }

	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual string OperationType
	  {
		  get
		  {
			return operationType;
		  }
	  }

	  public virtual string AssignerId
	  {
		  get
		  {
			return assignerId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
	  }

	  public static HistoricIdentityLinkLogDto fromHistoricIdentityLink(HistoricIdentityLinkLog historicIdentityLink)
	  {
		HistoricIdentityLinkLogDto dto = new HistoricIdentityLinkLogDto();
		fromHistoricIdentityLink(dto, historicIdentityLink);
		return dto;
	  }

	  public static void fromHistoricIdentityLink(HistoricIdentityLinkLogDto dto, HistoricIdentityLinkLog historicIdentityLink)
	  {
		dto.id = historicIdentityLink.Id;
		dto.assignerId = historicIdentityLink.AssignerId;
		dto.groupId = historicIdentityLink.GroupId;
		dto.operationType = historicIdentityLink.OperationType;
		dto.taskId = historicIdentityLink.TaskId;
		dto.time = historicIdentityLink.Time;
		dto.type = historicIdentityLink.Type;
		dto.processDefinitionId = historicIdentityLink.ProcessDefinitionId;
		dto.processDefinitionKey = historicIdentityLink.ProcessDefinitionKey;
		dto.userId = historicIdentityLink.UserId;
		dto.tenantId = historicIdentityLink.TenantId;
		dto.removalTime = historicIdentityLink.RemovalTime;
		dto.rootProcessInstanceId = historicIdentityLink.RootProcessInstanceId;
	  }
	}

}