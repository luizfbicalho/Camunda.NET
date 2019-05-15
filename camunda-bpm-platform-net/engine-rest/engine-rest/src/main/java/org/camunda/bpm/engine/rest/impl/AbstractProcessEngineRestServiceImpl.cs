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
namespace org.camunda.bpm.engine.rest.impl
{


	using HistoryRestService = org.camunda.bpm.engine.rest.history.HistoryRestService;
	using HistoryRestServiceImpl = org.camunda.bpm.engine.rest.impl.history.HistoryRestServiceImpl;
	using OptimizeRestService = org.camunda.bpm.engine.rest.impl.optimize.OptimizeRestService;
	using ProvidersUtil = org.camunda.bpm.engine.rest.util.ProvidersUtil;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// <para>Abstract process engine resource that provides instantiations of all REST resources.</para>
	/// 
	/// <para>Subclasses can add JAX-RS to methods as required annotations. For example, if only
	/// the process definition resource should be exposed, it is sufficient to add JAX-RS annotations to that
	/// resource. The <code>engineName</code> parameter of all the provided methods may be <code>null</code>
	/// to instantiate a resource for the default engine.</para>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public abstract class AbstractProcessEngineRestServiceImpl
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Context protected javax.ws.rs.ext.Providers providers;
	  protected internal Providers providers;

	  public virtual ProcessDefinitionRestService getProcessDefinitionService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		ProcessDefinitionRestServiceImpl subResource = new ProcessDefinitionRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual ProcessInstanceRestService getProcessInstanceService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		ProcessInstanceRestServiceImpl subResource = new ProcessInstanceRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual ExecutionRestService getExecutionService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		ExecutionRestServiceImpl subResource = new ExecutionRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual TaskRestService getTaskRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		TaskRestServiceImpl subResource = new TaskRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;

		return subResource;
	  }

	  public virtual IdentityRestService getIdentityRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		IdentityRestServiceImpl subResource = new IdentityRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual MessageRestService getMessageRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		MessageRestServiceImpl subResource = new MessageRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual VariableInstanceRestService getVariableInstanceService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		VariableInstanceRestServiceImpl subResource = new VariableInstanceRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual JobDefinitionRestService getJobDefinitionRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		JobDefinitionRestServiceImpl subResource = new JobDefinitionRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual JobRestService getJobRestService(string engineName)
	  {
		  string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		  JobRestServiceImpl subResource = new JobRestServiceImpl(engineName, ObjectMapper);
		  subResource.RelativeRootResourceUri = rootResourcePath;
		  return subResource;
	  }

	  public virtual GroupRestService getGroupRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		GroupRestServiceImpl subResource = new GroupRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual UserRestService getUserRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		UserRestServiceImpl subResource = new UserRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual AuthorizationRestService getAuthorizationRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		AuthorizationRestServiceImpl subResource = new AuthorizationRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual IncidentRestService getIncidentService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		IncidentRestServiceImpl subResource = new IncidentRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual HistoryRestService getHistoryRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		HistoryRestServiceImpl subResource = new HistoryRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual DeploymentRestService getDeploymentRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		DeploymentRestServiceImpl subResource = new DeploymentRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual CaseDefinitionRestService getCaseDefinitionRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		CaseDefinitionRestServiceImpl subResource = new CaseDefinitionRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual CaseInstanceRestService getCaseInstanceRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		CaseInstanceRestServiceImpl subResource = new CaseInstanceRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual CaseExecutionRestService getCaseExecutionRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		CaseExecutionRestServiceImpl subResource = new CaseExecutionRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual FilterRestService getFilterRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		FilterRestServiceImpl subResource = new FilterRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual MetricsRestService getMetricsRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		MetricsRestServiceImpl subResource = new MetricsRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual DecisionDefinitionRestService getDecisionDefinitionRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		DecisionDefinitionRestServiceImpl subResource = new DecisionDefinitionRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual DecisionRequirementsDefinitionRestService getDecisionRequirementsDefinitionRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		DecisionRequirementsDefinitionRestServiceImpl subResource = new DecisionRequirementsDefinitionRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual ExternalTaskRestService getExternalTaskRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		ExternalTaskRestServiceImpl subResource = new ExternalTaskRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual MigrationRestService getMigrationRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		MigrationRestServiceImpl subResource = new MigrationRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual ModificationRestService getModificationRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		ModificationRestServiceImpl subResource = new ModificationRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual BatchRestService getBatchRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		BatchRestServiceImpl subResource = new BatchRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual TenantRestService getTenantRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		TenantRestServiceImpl subResource = new TenantRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual SignalRestService getSignalRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		SignalRestServiceImpl subResource = new SignalRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual ConditionRestService getConditionRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		ConditionRestServiceImpl subResource = new ConditionRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual OptimizeRestService getOptimizeRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		OptimizeRestService subResource = new OptimizeRestService(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual VersionRestService getVersionRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		VersionRestService subResource = new VersionRestService(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  public virtual SchemaLogRestService getSchemaLogRestService(string engineName)
	  {
		string rootResourcePath = getRelativeEngineUri(engineName).toASCIIString();
		SchemaLogRestServiceImpl subResource = new SchemaLogRestServiceImpl(engineName, ObjectMapper);
		subResource.RelativeRootResourceUri = rootResourcePath;
		return subResource;
	  }

	  protected internal abstract URI getRelativeEngineUri(string engineName);

	  protected internal virtual ObjectMapper ObjectMapper
	  {
		  get
		  {
			return ProvidersUtil.resolveFromContext(providers, typeof(ObjectMapper), MediaType.APPLICATION_JSON_TYPE, this.GetType());
		  }
	  }

	}

}