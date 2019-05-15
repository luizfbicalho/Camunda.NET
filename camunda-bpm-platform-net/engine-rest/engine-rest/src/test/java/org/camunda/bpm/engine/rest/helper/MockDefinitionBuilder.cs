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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	public class MockDefinitionBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string id_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string key_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string category_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string description_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string name_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private int version_Renamed = 0;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string resource_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string deploymentId_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string diagram_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private bool suspended_Renamed = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private bool startFormKey_Renamed = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string tenantId_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string versionTag_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private bool isStartableInTasklist_Renamed = true;

	  public virtual MockDefinitionBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockDefinitionBuilder key(string key)
	  {
		this.key_Renamed = key;
		return this;
	  }

	  public virtual MockDefinitionBuilder category(string category)
	  {
		this.category_Renamed = category;
		return this;
	  }

	  public virtual MockDefinitionBuilder description(string description)
	  {
		this.description_Renamed = description;
		return this;
	  }

	  public virtual MockDefinitionBuilder name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MockDefinitionBuilder version(int version)
	  {
		this.version_Renamed = version;
		return this;
	  }

	  public virtual MockDefinitionBuilder resource(string resource)
	  {
		this.resource_Renamed = resource;
		return this;
	  }

	  public virtual MockDefinitionBuilder deploymentId(string deploymentId)
	  {
		this.deploymentId_Renamed = deploymentId;
		return this;
	  }

	  public virtual MockDefinitionBuilder diagram(string diagram)
	  {
		this.diagram_Renamed = diagram;
		return this;
	  }

	  public virtual MockDefinitionBuilder suspended(bool suspended)
	  {
		this.suspended_Renamed = suspended;
		return this;
	  }

	  public virtual MockDefinitionBuilder startFormKey(bool startFormKey)
	  {
		this.startFormKey_Renamed = startFormKey;
		return this;
	  }

	  public virtual MockDefinitionBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MockDefinitionBuilder versionTag(string versionTag)
	  {
		this.versionTag_Renamed = versionTag;
		return this;
	  }

	  public virtual MockDefinitionBuilder isStartableInTasklist(bool isStartableInTasklist)
	  {
		this.isStartableInTasklist_Renamed = isStartableInTasklist;
		return this;
	  }

	  public virtual ProcessDefinition build()
	  {
		ProcessDefinition mockDefinition = mock(typeof(ProcessDefinition));
		when(mockDefinition.Id).thenReturn(id_Renamed);
		when(mockDefinition.Category).thenReturn(category_Renamed);
		when(mockDefinition.Name).thenReturn(name_Renamed);
		when(mockDefinition.Key).thenReturn(key_Renamed);
		when(mockDefinition.Description).thenReturn(description_Renamed);
		when(mockDefinition.Version).thenReturn(version_Renamed);
		when(mockDefinition.ResourceName).thenReturn(resource_Renamed);
		when(mockDefinition.DeploymentId).thenReturn(deploymentId_Renamed);
		when(mockDefinition.DiagramResourceName).thenReturn(diagram_Renamed);
		when(mockDefinition.Suspended).thenReturn(suspended_Renamed);
		when(mockDefinition.hasStartFormKey()).thenReturn(startFormKey_Renamed);
		when(mockDefinition.TenantId).thenReturn(tenantId_Renamed);
		when(mockDefinition.VersionTag).thenReturn(versionTag_Renamed);
		when(mockDefinition.StartableInTasklist).thenReturn(isStartableInTasklist_Renamed);
		return mockDefinition;
	  }
	}

}