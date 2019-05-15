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
namespace org.camunda.bpm.engine.rest.hal.processDefinition
{

	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using DeploymentResourcesResource = org.camunda.bpm.engine.rest.sub.repository.DeploymentResourcesResource;
	using ApplicationContextPathUtil = org.camunda.bpm.engine.rest.util.ApplicationContextPathUtil;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HalProcessDefinition : HalResource<HalProcessDefinition>, HalIdResource
	{

	  public static readonly HalRelation REL_SELF = HalRelation.build("self", typeof(ProcessDefinitionRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.ProcessDefinitionRestService_Fields.PATH).path("{id}"));
	  public static readonly HalRelation REL_DEPLOYMENT = HalRelation.build("deployment", typeof(DeploymentRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.DeploymentRestService_Fields.PATH).path("{id}"));
	  public static readonly HalRelation REL_DEPLOYMENT_RESOURCE = HalRelation.build("resource", typeof(DeploymentResourcesResource), UriBuilder.fromPath(org.camunda.bpm.engine.rest.DeploymentRestService_Fields.PATH).path("{deploymentId}").path("resources").path("{resourceId}"));

	  protected internal string id;
	  protected internal string key;
	  protected internal string category;
	  protected internal string description;
	  protected internal string name;
	  protected internal string versionTag;
	  protected internal int version;
	  protected internal string resource;
	  protected internal string deploymentId;
	  protected internal string diagram;
	  protected internal bool suspended;
	  protected internal string contextPath;

	  public static HalProcessDefinition fromProcessDefinition(ProcessDefinition processDefinition, ProcessEngine processEngine)
	  {
		HalProcessDefinition halProcDef = new HalProcessDefinition();

		halProcDef.id = processDefinition.Id;
		halProcDef.key = processDefinition.Key;
		halProcDef.category = processDefinition.Category;
		halProcDef.description = processDefinition.Description;
		halProcDef.name = processDefinition.Name;
		halProcDef.version = processDefinition.Version;
		halProcDef.versionTag = processDefinition.VersionTag;
		halProcDef.resource = processDefinition.ResourceName;
		halProcDef.deploymentId = processDefinition.DeploymentId;
		halProcDef.diagram = processDefinition.DiagramResourceName;
		halProcDef.suspended = processDefinition.Suspended;
		halProcDef.contextPath = ApplicationContextPathUtil.getApplicationPathForDeployment(processEngine, processDefinition.DeploymentId);

		halProcDef.linker.createLink(REL_SELF, processDefinition.Id);
		halProcDef.linker.createLink(REL_DEPLOYMENT, processDefinition.DeploymentId);
		halProcDef.linker.createLink(REL_DEPLOYMENT_RESOURCE, processDefinition.DeploymentId, processDefinition.ResourceName);

		return halProcDef;
	  }

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

	  public virtual string VersionTag
	  {
		  get
		  {
			return versionTag;
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

	  public virtual string ContextPath
	  {
		  get
		  {
			return contextPath;
		  }
	  }

	}

}