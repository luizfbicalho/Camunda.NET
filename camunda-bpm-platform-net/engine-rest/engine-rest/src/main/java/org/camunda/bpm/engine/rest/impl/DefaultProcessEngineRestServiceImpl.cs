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
	using OptimizeRestService = org.camunda.bpm.engine.rest.impl.optimize.OptimizeRestService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(DefaultProcessEngineRestServiceImpl.PATH) public class DefaultProcessEngineRestServiceImpl extends AbstractProcessEngineRestServiceImpl
	public class DefaultProcessEngineRestServiceImpl : AbstractProcessEngineRestServiceImpl
	{

	  public const string PATH = "";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.ProcessDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ProcessDefinitionRestService getProcessDefinitionService()
	  public virtual ProcessDefinitionRestService ProcessDefinitionService
	  {
		  get
		  {
			return base.getProcessDefinitionService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.ProcessInstanceRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ProcessInstanceRestService getProcessInstanceService()
	  public virtual ProcessInstanceRestService ProcessInstanceService
	  {
		  get
		  {
			return base.getProcessInstanceService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.ExecutionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ExecutionRestService getExecutionService()
	  public virtual ExecutionRestService ExecutionService
	  {
		  get
		  {
			return base.getExecutionService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH) public org.camunda.bpm.engine.rest.TaskRestService getTaskRestService()
	  public virtual TaskRestService TaskRestService
	  {
		  get
		  {
			return base.getTaskRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.IdentityRestService_Fields.PATH) public org.camunda.bpm.engine.rest.IdentityRestService getIdentityRestService()
	  public virtual IdentityRestService IdentityRestService
	  {
		  get
		  {
			return base.getIdentityRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.MessageRestService_Fields.PATH) public org.camunda.bpm.engine.rest.MessageRestService getMessageRestService()
	  public virtual MessageRestService MessageRestService
	  {
		  get
		  {
			return base.getMessageRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.VariableInstanceRestService_Fields.PATH) public org.camunda.bpm.engine.rest.VariableInstanceRestService getVariableInstanceService()
	  public virtual VariableInstanceRestService VariableInstanceService
	  {
		  get
		  {
			return base.getVariableInstanceService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.JobDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.JobDefinitionRestService getJobDefinitionRestService()
	  public virtual JobDefinitionRestService JobDefinitionRestService
	  {
		  get
		  {
			return base.getJobDefinitionRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.JobRestService_Fields.PATH) public org.camunda.bpm.engine.rest.JobRestService getJobRestService()
	  public virtual JobRestService JobRestService
	  {
		  get
		  {
			return base.getJobRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.GroupRestService_Fields.PATH) public org.camunda.bpm.engine.rest.GroupRestService getGroupRestService()
	  public virtual GroupRestService GroupRestService
	  {
		  get
		  {
			return base.getGroupRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.UserRestService_Fields.PATH) public org.camunda.bpm.engine.rest.UserRestService getUserRestService()
	  public virtual UserRestService UserRestService
	  {
		  get
		  {
			return base.getUserRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.AuthorizationRestService_Fields.PATH) public org.camunda.bpm.engine.rest.AuthorizationRestService getAuthorizationRestService()
	  public virtual AuthorizationRestService AuthorizationRestService
	  {
		  get
		  {
			return base.getAuthorizationRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.IncidentRestService_Fields.PATH) public org.camunda.bpm.engine.rest.IncidentRestService getIncidentService()
	  public virtual IncidentRestService IncidentService
	  {
		  get
		  {
			return base.getIncidentService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.history.HistoryRestService_Fields.PATH) public org.camunda.bpm.engine.rest.history.HistoryRestService getHistoryRestService()
	  public virtual HistoryRestService HistoryRestService
	  {
		  get
		  {
			return base.getHistoryRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.DeploymentRestService_Fields.PATH) public org.camunda.bpm.engine.rest.DeploymentRestService getDeploymentRestService()
	  public virtual DeploymentRestService DeploymentRestService
	  {
		  get
		  {
			return base.getDeploymentRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.CaseDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.CaseDefinitionRestService getCaseDefinitionRestService()
	  public virtual CaseDefinitionRestService CaseDefinitionRestService
	  {
		  get
		  {
			return base.getCaseDefinitionRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.CaseInstanceRestService_Fields.PATH) public org.camunda.bpm.engine.rest.CaseInstanceRestService getCaseInstanceRestService()
	  public virtual CaseInstanceRestService CaseInstanceRestService
	  {
		  get
		  {
			return base.getCaseInstanceRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.CaseExecutionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.CaseExecutionRestService getCaseExecutionRestService()
	  public virtual CaseExecutionRestService CaseExecutionRestService
	  {
		  get
		  {
			return base.getCaseExecutionRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.FilterRestService_Fields.PATH) public org.camunda.bpm.engine.rest.FilterRestService getFilterRestService()
	  public virtual FilterRestService FilterRestService
	  {
		  get
		  {
			return base.getFilterRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.MetricsRestService_Fields.PATH) public org.camunda.bpm.engine.rest.MetricsRestService getMetricsRestService()
	  public virtual MetricsRestService MetricsRestService
	  {
		  get
		  {
			return base.getMetricsRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.DecisionDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.DecisionDefinitionRestService getDecisionDefinitionRestService()
	  public virtual DecisionDefinitionRestService DecisionDefinitionRestService
	  {
		  get
		  {
			return base.getDecisionDefinitionRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.DecisionRequirementsDefinitionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.DecisionRequirementsDefinitionRestService getDecisionRequirementsDefinitionRestService()
	  public virtual DecisionRequirementsDefinitionRestService DecisionRequirementsDefinitionRestService
	  {
		  get
		  {
			return base.getDecisionRequirementsDefinitionRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.ExternalTaskRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ExternalTaskRestService getExternalTaskRestService()
	  public virtual ExternalTaskRestService ExternalTaskRestService
	  {
		  get
		  {
			return base.getExternalTaskRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.MigrationRestService_Fields.PATH) public org.camunda.bpm.engine.rest.MigrationRestService getMigrationRestService()
	  public virtual MigrationRestService MigrationRestService
	  {
		  get
		  {
			return base.getMigrationRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.ModificationRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ModificationRestService getModificationRestService()
	  public virtual ModificationRestService ModificationRestService
	  {
		  get
		  {
			return base.getModificationRestService(null);
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.BatchRestService_Fields.PATH) public org.camunda.bpm.engine.rest.BatchRestService getBatchRestService()
	  public virtual BatchRestService BatchRestService
	  {
		  get
		  {
			return base.getBatchRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.TenantRestService_Fields.PATH) public org.camunda.bpm.engine.rest.TenantRestService getTenantRestService()
	  public virtual TenantRestService TenantRestService
	  {
		  get
		  {
			return base.getTenantRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.SignalRestService_Fields.PATH) public org.camunda.bpm.engine.rest.SignalRestService getSignalRestService()
	  public virtual SignalRestService SignalRestService
	  {
		  get
		  {
			return base.getSignalRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.ConditionRestService_Fields.PATH) public org.camunda.bpm.engine.rest.ConditionRestService getConditionRestService()
	  public virtual ConditionRestService ConditionRestService
	  {
		  get
		  {
			return base.getConditionRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.impl.optimize.OptimizeRestService.PATH) public org.camunda.bpm.engine.rest.impl.optimize.OptimizeRestService getOptimizeRestService()
	  public virtual OptimizeRestService OptimizeRestService
	  {
		  get
		  {
			return base.getOptimizeRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(VersionRestService.PATH) public VersionRestService getVersionRestService()
	  public virtual VersionRestService VersionRestService
	  {
		  get
		  {
			return base.getVersionRestService(null);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(org.camunda.bpm.engine.rest.SchemaLogRestService_Fields.PATH) public org.camunda.bpm.engine.rest.SchemaLogRestService getSchemaLogRestService()
	  public virtual SchemaLogRestService SchemaLogRestService
	  {
		  get
		  {
			return base.getSchemaLogRestService(null);
		  }
	  }

	  protected internal override URI getRelativeEngineUri(string engineName)
	  {
		// the default engine
		return URI.create("/");
	  }
	}

}