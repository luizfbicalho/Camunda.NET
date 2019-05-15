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
namespace org.camunda.bpm.engine.rest.dto.repository
{
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	public class ProcessDefinitionDto
	{

	  protected internal string id;
	  protected internal string key;
	  protected internal string category;
	  protected internal string description;
	  protected internal string name;
	  protected internal int version;
	  protected internal string resource;
	  protected internal string deploymentId;
	  protected internal string diagram;
	  protected internal bool suspended;
	  protected internal string tenantId;
	  protected internal string versionTag;
	  protected internal int? historyTimeToLive;
	  protected internal bool isStartableInTasklist;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
	  }

	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
	  }

	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual int Version
	  {
		  get
		  {
			return version;
		  }
	  }

	  public virtual string Resource
	  {
		  get
		  {
			return resource;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
	  }

	  public virtual string Diagram
	  {
		  get
		  {
			return diagram;
		  }
	  }

	  public virtual bool Suspended
	  {
		  get
		  {
			return suspended;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual string VersionTag
	  {
		  get
		  {
			return versionTag;
		  }
	  }

	  public virtual int? HistoryTimeToLive
	  {
		  get
		  {
			return historyTimeToLive;
		  }
	  }

	  public virtual bool StartableInTasklist
	  {
		  get
		  {
			return isStartableInTasklist;
		  }
	  }

	  public static ProcessDefinitionDto fromProcessDefinition(ProcessDefinition definition)
	  {
		ProcessDefinitionDto dto = new ProcessDefinitionDto();
		dto.id = definition.Id;
		dto.key = definition.Key;
		dto.category = definition.Category;
		dto.description = definition.Description;
		dto.name = definition.Name;
		dto.version = definition.Version;
		dto.resource = definition.ResourceName;
		dto.deploymentId = definition.DeploymentId;
		dto.diagram = definition.DiagramResourceName;
		dto.suspended = definition.Suspended;
		dto.tenantId = definition.TenantId;
		dto.versionTag = definition.VersionTag;
		dto.historyTimeToLive = definition.HistoryTimeToLive;
		dto.isStartableInTasklist = definition.StartableInTasklist;
		return dto;
	  }

	}

}