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
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseDefinitionDto
	{

	  protected internal string id;
	  protected internal string key;
	  protected internal string category;
	  protected internal string name;
	  protected internal int version;
	  protected internal string resource;
	  protected internal string deploymentId;
	  protected internal string tenantId;
	  protected internal int? historyTimeToLive;

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

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual int? HistoryTimeToLive
	  {
		  get
		  {
			return historyTimeToLive;
		  }
	  }

	  public static CaseDefinitionDto fromCaseDefinition(CaseDefinition definition)
	  {
		CaseDefinitionDto dto = new CaseDefinitionDto();

		dto.id = definition.Id;
		dto.key = definition.Key;
		dto.category = definition.Category;
		dto.name = definition.Name;
		dto.version = definition.Version;
		dto.resource = definition.ResourceName;
		dto.deploymentId = definition.DeploymentId;
		dto.tenantId = definition.TenantId;
		dto.historyTimeToLive = definition.HistoryTimeToLive;

		return dto;
	  }

	}

}