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
namespace org.camunda.bpm.engine.rest.impl
{


	using ProcessEngineDto = org.camunda.bpm.engine.rest.dto.ProcessEngineDto;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using HistoryRestService = org.camunda.bpm.engine.rest.history.HistoryRestService;
	using OptimizeRestService = org.camunda.bpm.engine.rest.impl.optimize.OptimizeRestService;
	using ProcessEngineProvider = org.camunda.bpm.engine.rest.spi.ProcessEngineProvider;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(NamedProcessEngineRestServiceImpl.PATH) public class NamedProcessEngineRestServiceImpl extends AbstractProcessEngineRestServiceImpl
	public class NamedProcessEngineRestServiceImpl : AbstractProcessEngineRestServiceImpl
	{

	  public const string PATH = "/engine";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.ProcessDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ProcessDefinitionRestService getProcessDefinitionService(@PathParam("name") String engineName)
	  public override ProcessDefinitionRestService getProcessDefinitionService(string engineName)
	  {
		return base.getProcessDefinitionService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.ProcessInstanceRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ProcessInstanceRestService getProcessInstanceService(@PathParam("name") String engineName)
	  public override ProcessInstanceRestService getProcessInstanceService(string engineName)
	  {
		return base.getProcessInstanceService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.ExecutionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ExecutionRestService getExecutionService(@PathParam("name") String engineName)
	  public override ExecutionRestService getExecutionService(string engineName)
	  {
		return base.getExecutionService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH) public org.camunda.bpm.engine.rest.TaskRestService getTaskRestService(@PathParam("name") String engineName)
	  public override TaskRestService getTaskRestService(string engineName)
	  {
		return base.getTaskRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.IdentityRestService_Fields.PATH) public org.camunda.bpm.engine.rest.IdentityRestService getIdentityRestService(@PathParam("name") String engineName)
	  public override IdentityRestService getIdentityRestService(string engineName)
	  {
		return base.getIdentityRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.MessageRestService_Fields.PATH) public org.camunda.bpm.engine.rest.MessageRestService getMessageRestService(@PathParam("name") String engineName)
	  public override MessageRestService getMessageRestService(string engineName)
	  {
		return base.getMessageRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.VariableInstanceRestService_Fields.PATH) public org.camunda.bpm.engine.rest.VariableInstanceRestService getVariableInstanceService(@PathParam("name") String engineName)
	  public override VariableInstanceRestService getVariableInstanceService(string engineName)
	  {
		return base.getVariableInstanceService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.JobDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.JobDefinitionRestService getJobDefinitionRestService(@PathParam("name") String engineName)
	  public override JobDefinitionRestService getJobDefinitionRestService(string engineName)
	  {
		return base.getJobDefinitionRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.JobRestService_Fields.PATH) public org.camunda.bpm.engine.rest.JobRestService getJobRestService(@PathParam("name") String engineName)
	  public override JobRestService getJobRestService(string engineName)
	  {
		return base.getJobRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.GroupRestService_Fields.PATH) public org.camunda.bpm.engine.rest.GroupRestService getGroupRestService(@PathParam("name") String engineName)
	  public override GroupRestService getGroupRestService(string engineName)
	  {
		return base.getGroupRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.UserRestService_Fields.PATH) public org.camunda.bpm.engine.rest.UserRestService getUserRestService(@PathParam("name") String engineName)
	  public override UserRestService getUserRestService(string engineName)
	  {
		return base.getUserRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.AuthorizationRestService_Fields.PATH) public org.camunda.bpm.engine.rest.AuthorizationRestService getAuthorizationRestService(@PathParam("name") String engineName)
	  public override AuthorizationRestService getAuthorizationRestService(string engineName)
	  {
		return base.getAuthorizationRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.IncidentRestService_Fields.PATH) public org.camunda.bpm.engine.rest.IncidentRestService getIncidentService(@PathParam("name") String engineName)
	  public override IncidentRestService getIncidentService(string engineName)
	  {
		return base.getIncidentService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.history.HistoryRestService_Fields.PATH) public org.camunda.bpm.engine.rest.history.HistoryRestService getHistoryRestService(@PathParam("name") String engineName)
	  public override HistoryRestService getHistoryRestService(string engineName)
	  {
		return base.getHistoryRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.DeploymentRestService_Fields.PATH) public org.camunda.bpm.engine.rest.DeploymentRestService getDeploymentRestService(@PathParam("name") String engineName)
	  public override DeploymentRestService getDeploymentRestService(string engineName)
	  {
		return base.getDeploymentRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.CaseDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.CaseDefinitionRestService getCaseDefinitionRestService(@PathParam("name") String engineName)
	  public override CaseDefinitionRestService getCaseDefinitionRestService(string engineName)
	  {
		return base.getCaseDefinitionRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.CaseInstanceRestService_Fields.PATH) public org.camunda.bpm.engine.rest.CaseInstanceRestService getCaseInstanceRestService(@PathParam("name") String engineName)
	  public override CaseInstanceRestService getCaseInstanceRestService(string engineName)
	  {
		return base.getCaseInstanceRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.CaseExecutionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.CaseExecutionRestService getCaseExecutionRestService(@PathParam("name") String engineName)
	  public override CaseExecutionRestService getCaseExecutionRestService(string engineName)
	  {
		return base.getCaseExecutionRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.FilterRestService_Fields.PATH) public org.camunda.bpm.engine.rest.FilterRestService getFilterRestService(@PathParam("name") String engineName)
	  public override FilterRestService getFilterRestService(string engineName)
	  {
		return base.getFilterRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.MetricsRestService_Fields.PATH) public org.camunda.bpm.engine.rest.MetricsRestService getMetricsRestService(@PathParam("name") String engineName)
	  public override MetricsRestService getMetricsRestService(string engineName)
	  {
		return base.getMetricsRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.DecisionDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.DecisionDefinitionRestService getDecisionDefinitionRestService(@PathParam("name") String engineName)
	  public override DecisionDefinitionRestService getDecisionDefinitionRestService(string engineName)
	  {
		return base.getDecisionDefinitionRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.DecisionRequirementsDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.DecisionRequirementsDefinitionRestService getDecisionRequirementsDefinitionRestService(@PathParam("name") String engineName)
	  public override DecisionRequirementsDefinitionRestService getDecisionRequirementsDefinitionRestService(string engineName)
	  {
		return base.getDecisionRequirementsDefinitionRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.ExternalTaskRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ExternalTaskRestService getExternalTaskRestService(@PathParam("name") String engineName)
	  public override ExternalTaskRestService getExternalTaskRestService(string engineName)
	  {
		return base.getExternalTaskRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.MigrationRestService_Fields.PATH) public org.camunda.bpm.engine.rest.MigrationRestService getMigrationRestService(@PathParam("name") String engineName)
	  public override MigrationRestService getMigrationRestService(string engineName)
	  {
		return base.getMigrationRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.ModificationRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ModificationRestService getModificationRestService(@PathParam("name") String engineName)
	  public override ModificationRestService getModificationRestService(string engineName)
	  {
		return base.getModificationRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.BatchRestService_Fields.PATH) public org.camunda.bpm.engine.rest.BatchRestService getBatchRestService(@PathParam("name") String engineName)
	  public override BatchRestService getBatchRestService(string engineName)
	  {
		return base.getBatchRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.TenantRestService_Fields.PATH) public org.camunda.bpm.engine.rest.TenantRestService getTenantRestService(@PathParam("name") String engineName)
	  public override TenantRestService getTenantRestService(string engineName)
	  {
		return base.getTenantRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.SignalRestService_Fields.PATH) public org.camunda.bpm.engine.rest.SignalRestService getSignalRestService(@PathParam("name") String engineName)
	  public override SignalRestService getSignalRestService(string engineName)
	  {
		return base.getSignalRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Path("/{name}" + org.camunda.bpm.engine.rest.ConditionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ConditionRestService getConditionRestService(@PathParam("name") String engineName)
	  public override ConditionRestService getConditionRestService(string engineName)
	  {
		return base.getConditionRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{name}" + org.camunda.bpm.engine.rest.impl.optimize.OptimizeRestService.PATH) public org.camunda.bpm.engine.rest.impl.optimize.OptimizeRestService getOptimizeRestService(@PathParam("name") String engineName)
	  public virtual OptimizeRestService getOptimizeRestService(string engineName)
	  {
		return base.getOptimizeRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{name}" + VersionRestService.PATH) public VersionRestService getVersionRestService(@PathParam("name") String engineName)
	  public virtual VersionRestService getVersionRestService(string engineName)
	  {
		return base.getVersionRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{name}" + org.camunda.bpm.engine.rest.SchemaLogRestService_Fields.PATH) public org.camunda.bpm.engine.rest.SchemaLogRestService getSchemaLogRestService(@PathParam("name") String engineName)
	  public virtual SchemaLogRestService getSchemaLogRestService(string engineName)
	  {
		return base.getSchemaLogRestService(engineName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public java.util.List<org.camunda.bpm.engine.rest.dto.ProcessEngineDto> getProcessEngineNames()
	  public virtual IList<ProcessEngineDto> ProcessEngineNames
	  {
		  get
		  {
			ProcessEngineProvider provider = ProcessEngineProvider;
			ISet<string> engineNames = provider.ProcessEngineNames;
    
			IList<ProcessEngineDto> results = new List<ProcessEngineDto>();
			foreach (string engineName in engineNames)
			{
			  ProcessEngineDto dto = new ProcessEngineDto();
			  dto.Name = engineName;
			  results.Add(dto);
			}
    
			return results;
		  }
	  }

	  protected internal override URI getRelativeEngineUri(string engineName)
	  {
		return UriBuilder.fromResource(typeof(NamedProcessEngineRestServiceImpl)).path("{name}").build(engineName);
	  }

	  protected internal virtual ProcessEngineProvider ProcessEngineProvider
	  {
		  get
		  {
			ServiceLoader<ProcessEngineProvider> serviceLoader = ServiceLoader.load(typeof(ProcessEngineProvider));
			IEnumerator<ProcessEngineProvider> iterator = serviceLoader.GetEnumerator();
    
	//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (iterator.hasNext())
			{
	//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			  ProcessEngineProvider provider = iterator.next();
			  return provider;
			}
			else
			{
			  throw new RestException(Status.INTERNAL_SERVER_ERROR, "No process engine provider found");
			}
		  }
	  }

	}

}