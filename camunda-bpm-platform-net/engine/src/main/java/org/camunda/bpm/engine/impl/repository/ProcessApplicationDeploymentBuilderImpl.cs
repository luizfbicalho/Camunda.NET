using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.impl.repository
{

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using ProcessApplicationDeploymentBuilder = org.camunda.bpm.engine.repository.ProcessApplicationDeploymentBuilder;
	using ResumePreviousBy = org.camunda.bpm.engine.repository.ResumePreviousBy;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class ProcessApplicationDeploymentBuilderImpl : DeploymentBuilderImpl, ProcessApplicationDeploymentBuilder
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly ProcessApplicationReference processApplicationReference;
	  protected internal bool isResumePreviousVersions = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string resumePreviousVersionsBy_Conflict = ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY;

	  public ProcessApplicationDeploymentBuilderImpl(RepositoryServiceImpl repositoryService, ProcessApplicationReference reference) : base(repositoryService)
	  {
		this.processApplicationReference = reference;
		source(org.camunda.bpm.engine.repository.ProcessApplicationDeployment_Fields.PROCESS_APPLICATION_DEPLOYMENT_SOURCE);
	  }

	  public virtual ProcessApplicationDeploymentBuilder resumePreviousVersions()
	  {
		this.isResumePreviousVersions = true;
		return this;
	  }

	  public virtual ProcessApplicationDeploymentBuilder resumePreviousVersionsBy(string resumePreviousVersionsBy)
	  {
		this.resumePreviousVersionsBy_Conflict = resumePreviousVersionsBy;
		return this;
	  }
	  // overrides from parent ////////////////////////////////////////////////

	  public override ProcessApplicationDeployment deploy()
	  {
		return (ProcessApplicationDeployment) base.deploy();
	  }

	  public override ProcessApplicationDeploymentBuilderImpl activateProcessDefinitionsOn(DateTime date)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.activateProcessDefinitionsOn(date);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addInputStream(string resourceName, Stream inputStream)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addInputStream(resourceName, inputStream);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addClasspathResource(string resource)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addClasspathResource(resource);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addString(string resourceName, string text)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addString(resourceName, text);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addModelInstance(string resourceName, BpmnModelInstance modelInstance)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addModelInstance(resourceName, modelInstance);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addZipInputStream(ZipInputStream zipInputStream)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addZipInputStream(zipInputStream);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl name(string name)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.name(name);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl tenantId(string tenantId)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.tenantId(tenantId);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl nameFromDeployment(string deploymentId)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.nameFromDeployment(deploymentId);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl source(string source)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.source(source);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl enableDuplicateFiltering()
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.enableDuplicateFiltering();
	  }

	  public override ProcessApplicationDeploymentBuilderImpl enableDuplicateFiltering(bool deployChangedOnly)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.enableDuplicateFiltering(deployChangedOnly);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addDeploymentResources(string deploymentId)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addDeploymentResources(deploymentId);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addDeploymentResourceById(string deploymentId, string resourceId)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addDeploymentResourceById(deploymentId, resourceId);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addDeploymentResourcesById(string deploymentId, IList<string> resourceIds)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addDeploymentResourcesById(deploymentId, resourceIds);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addDeploymentResourceByName(string deploymentId, string resourceName)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addDeploymentResourceByName(deploymentId, resourceName);
	  }

	  public override ProcessApplicationDeploymentBuilderImpl addDeploymentResourcesByName(string deploymentId, IList<string> resourceNames)
	  {
		return (ProcessApplicationDeploymentBuilderImpl) base.addDeploymentResourcesByName(deploymentId, resourceNames);
	  }

	  // getters / setters ///////////////////////////////////////////////

	  public virtual bool ResumePreviousVersions
	  {
		  get
		  {
			return isResumePreviousVersions;
		  }
	  }

	  public virtual ProcessApplicationReference ProcessApplicationReference
	  {
		  get
		  {
			return processApplicationReference;
		  }
	  }

	  public virtual string ResumePreviousVersionsBy
	  {
		  get
		  {
			return resumePreviousVersionsBy_Conflict;
		  }
	  }

	}

}