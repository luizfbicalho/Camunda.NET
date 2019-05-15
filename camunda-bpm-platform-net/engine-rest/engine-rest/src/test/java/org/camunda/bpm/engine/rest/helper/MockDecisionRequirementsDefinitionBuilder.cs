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

	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;

	public class MockDecisionRequirementsDefinitionBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string id_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string key_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string category_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string name_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private int version_Renamed = 0;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string resource_Renamed = null;
	  private string diagramResource = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string deploymentId_Renamed = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string tenantId_Renamed = null;

	  public virtual MockDecisionRequirementsDefinitionBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockDecisionRequirementsDefinitionBuilder key(string key)
	  {
		this.key_Renamed = key;
		return this;
	  }

	  public virtual MockDecisionRequirementsDefinitionBuilder category(string category)
	  {
		this.category_Renamed = category;
		return this;
	  }

	  public virtual MockDecisionRequirementsDefinitionBuilder name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MockDecisionRequirementsDefinitionBuilder version(int version)
	  {
		this.version_Renamed = version;
		return this;
	  }

	  public virtual MockDecisionRequirementsDefinitionBuilder resource(string resource)
	  {
		this.resource_Renamed = resource;
		return this;
	  }

	  public virtual MockDecisionRequirementsDefinitionBuilder diagram(string diagramResource)
	  {
		this.diagramResource = diagramResource;
		return this;
	  }

	  public virtual MockDecisionRequirementsDefinitionBuilder deploymentId(string deploymentId)
	  {
		this.deploymentId_Renamed = deploymentId;
		return this;
	  }

	  public virtual MockDecisionRequirementsDefinitionBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual DecisionRequirementsDefinition build()
	  {
		DecisionRequirementsDefinition mockDefinition = mock(typeof(DecisionRequirementsDefinition));

		when(mockDefinition.Id).thenReturn(id_Renamed);
		when(mockDefinition.Category).thenReturn(category_Renamed);
		when(mockDefinition.Name).thenReturn(name_Renamed);
		when(mockDefinition.Key).thenReturn(key_Renamed);
		when(mockDefinition.Version).thenReturn(version_Renamed);
		when(mockDefinition.ResourceName).thenReturn(resource_Renamed);
		when(mockDefinition.DiagramResourceName).thenReturn(diagramResource);
		when(mockDefinition.DeploymentId).thenReturn(deploymentId_Renamed);
		when(mockDefinition.TenantId).thenReturn(tenantId_Renamed);

		return mockDefinition;
	  }

	}

}