﻿/*
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

	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;

	public class MockDecisionDefinitionBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string id_Conflict = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string key_Conflict = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string category_Conflict = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string name_Conflict = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private int version_Conflict = 0;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string resource_Conflict = null;
	  private string diagramResource = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string deploymentId_Conflict = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string tenantId_Conflict = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string decisionRequirementsDefinitionId_Conflict = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string decisionRequirementsDefinitionKey_Conflict = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string versionTag_Conflict = null;

	  public virtual MockDecisionDefinitionBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder key(string key)
	  {
		this.key_Conflict = key;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder category(string category)
	  {
		this.category_Conflict = category;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder name(string name)
	  {
		this.name_Conflict = name;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder version(int version)
	  {
		this.version_Conflict = version;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder resource(string resource)
	  {
		this.resource_Conflict = resource;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder diagram(string diagramResource)
	  {
		this.diagramResource = diagramResource;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder deploymentId(string deploymentId)
	  {
		this.deploymentId_Conflict = deploymentId;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder decisionRequirementsDefinitionId(string decisionRequirementsDefinitionId)
	  {
		this.decisionRequirementsDefinitionId_Conflict = decisionRequirementsDefinitionId;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder decisionRequirementsDefinitionKey(string decisionRequirementsDefinitionKey)
	  {
		this.decisionRequirementsDefinitionKey_Conflict = decisionRequirementsDefinitionKey;
		return this;
	  }

	  public virtual MockDecisionDefinitionBuilder versionTag(string versionTag)
	  {
		this.versionTag_Conflict = versionTag;
		return this;
	  }

	  public virtual DecisionDefinition build()
	  {
		DecisionDefinition mockDefinition = mock(typeof(DecisionDefinition));

		when(mockDefinition.Id).thenReturn(id_Conflict);
		when(mockDefinition.Category).thenReturn(category_Conflict);
		when(mockDefinition.Name).thenReturn(name_Conflict);
		when(mockDefinition.Key).thenReturn(key_Conflict);
		when(mockDefinition.Version).thenReturn(version_Conflict);
		when(mockDefinition.ResourceName).thenReturn(resource_Conflict);
		when(mockDefinition.DiagramResourceName).thenReturn(diagramResource);
		when(mockDefinition.DeploymentId).thenReturn(deploymentId_Conflict);
		when(mockDefinition.TenantId).thenReturn(tenantId_Conflict);
		when(mockDefinition.DecisionRequirementsDefinitionId).thenReturn(decisionRequirementsDefinitionId_Conflict);
		when(mockDefinition.DecisionRequirementsDefinitionKey).thenReturn(decisionRequirementsDefinitionKey_Conflict);
		when(mockDefinition.VersionTag).thenReturn(versionTag_Conflict);

		return mockDefinition;
	  }

	}

}