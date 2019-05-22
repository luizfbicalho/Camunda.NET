using System;
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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using org.camunda.bpm.engine.repository;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class DeploymentEntity : DeploymentWithDefinitions, DbEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string name;
	  protected internal IDictionary<string, ResourceEntity> resources;
	  protected internal DateTime deploymentTime;
	  protected internal bool validatingSchema = true;
	  protected internal bool isNew;
	  protected internal string source;
	  protected internal string tenantId;

	  /// <summary>
	  /// Will only be used during actual deployment to pass deployed artifacts (eg process definitions).
	  /// Will be null otherwise.
	  /// </summary>
	  protected internal IDictionary<Type, System.Collections.IList> deployedArtifacts;

	  public virtual ResourceEntity getResource(string resourceName)
	  {
		return Resources[resourceName];
	  }

	  public virtual void addResource(ResourceEntity resource)
	  {
		if (resources == null)
		{
		  resources = new Dictionary<string, ResourceEntity>();
		}
		resources[resource.Name] = resource;
	  }

	  public virtual void clearResources()
	  {
		if (resources != null)
		{
		  resources.Clear();
		}
	  }

	  // lazy loading /////////////////////////////////////////////////////////////
	  public virtual IDictionary<string, ResourceEntity> Resources
	  {
		  get
		  {
			if (resources == null && !string.ReferenceEquals(id, null))
			{
			  IList<ResourceEntity> resourcesList = Context.CommandContext.ResourceManager.findResourcesByDeploymentId(id);
			  resources = new Dictionary<string, ResourceEntity>();
			  foreach (ResourceEntity resource in resourcesList)
			  {
				resources[resource.Name] = resource;
			  }
			}
			return resources;
		  }
		  set
		  {
			this.resources = value;
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			// properties of this entity are immutable
			// so always the same value is returned
			// so never will an update be issued for a DeploymentEntity
			return typeof(DeploymentEntity);
		  }
	  }

	  // Deployed artifacts manipulation //////////////////////////////////////////

	  public virtual void addDeployedArtifact(ResourceDefinitionEntity deployedArtifact)
	  {
		if (deployedArtifacts == null)
		{
		  deployedArtifacts = new Dictionary<Type, System.Collections.IList>();
		}

		Type clazz = deployedArtifact.GetType();
		System.Collections.IList artifacts = deployedArtifacts[clazz];
		if (artifacts == null)
		{
		  artifacts = new List<object>();
		  deployedArtifacts[clazz] = artifacts;
		}

		artifacts.Add(deployedArtifact);
	  }

	  public virtual IDictionary<Type, System.Collections.IList> DeployedArtifacts
	  {
		  get
		  {
			return deployedArtifacts;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> java.util.List<T> getDeployedArtifacts(Class<T> clazz)
	  public virtual IList<T> getDeployedArtifacts<T>(Type clazz)
	  {
			  clazz = typeof(T);
		if (deployedArtifacts == null)
		{
		  return null;
		}
		else
		{
		  return (IList<T>) deployedArtifacts[clazz];
		}
	  }

	  public virtual void removeArtifact(ResourceDefinitionEntity notDeployedArtifact)
	  {
		if (deployedArtifacts != null)
		{
		  System.Collections.IList artifacts = deployedArtifacts[notDeployedArtifact.GetType()];
		  if (artifacts != null)
		  {
			artifacts.Remove(notDeployedArtifact);
			if (artifacts.Count == 0)
			{
			  deployedArtifacts.Remove(notDeployedArtifact.GetType());
			}
		  }
		}
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }



	  public virtual DateTime DeploymentTime
	  {
		  get
		  {
			return deploymentTime;
		  }
		  set
		  {
			this.deploymentTime = value;
		  }
	  }


	  public virtual bool ValidatingSchema
	  {
		  get
		  {
			return validatingSchema;
		  }
		  set
		  {
			this.validatingSchema = value;
		  }
	  }


	  public virtual bool New
	  {
		  get
		  {
			return isNew;
		  }
		  set
		  {
			this.isNew = value;
		  }
	  }


	  public virtual string Source
	  {
		  get
		  {
			return source;
		  }
		  set
		  {
			this.source = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual IList<ProcessDefinition> DeployedProcessDefinitions
	  {
		  get
		  {
			return deployedArtifacts == null ? null : deployedArtifacts[typeof(ProcessDefinitionEntity)];
		  }
	  }

	  public virtual IList<CaseDefinition> DeployedCaseDefinitions
	  {
		  get
		  {
			return deployedArtifacts == null ? null : deployedArtifacts[typeof(CaseDefinitionEntity)];
		  }
	  }

	  public virtual IList<DecisionDefinition> DeployedDecisionDefinitions
	  {
		  get
		  {
			return deployedArtifacts == null ? null : deployedArtifacts[typeof(DecisionDefinitionEntity)];
		  }
	  }

	  public virtual IList<DecisionRequirementsDefinition> DeployedDecisionRequirementsDefinitions
	  {
		  get
		  {
			return deployedArtifacts == null ? null : deployedArtifacts[typeof(DecisionRequirementsDefinitionEntity)];
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", name=" + name + ", resources=" + resources + ", deploymentTime=" + deploymentTime + ", validatingSchema=" + validatingSchema + ", isNew=" + isNew + ", source=" + source + ", tenantId=" + tenantId + "]";
	  }

	}

}