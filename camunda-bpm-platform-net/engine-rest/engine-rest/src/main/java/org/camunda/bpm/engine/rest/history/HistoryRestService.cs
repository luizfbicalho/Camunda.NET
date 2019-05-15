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
namespace org.camunda.bpm.engine.rest.history
{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoryRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoryRestService
	public interface HistoryRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricProcessInstanceRestService_Fields.PATH) HistoricProcessInstanceRestService getProcessInstanceService();
	  HistoricProcessInstanceRestService ProcessInstanceService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricCaseInstanceRestService_Fields.PATH) HistoricCaseInstanceRestService getCaseInstanceService();
	  HistoricCaseInstanceRestService CaseInstanceService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricActivityInstanceRestService_Fields.PATH) HistoricActivityInstanceRestService getActivityInstanceService();
	  HistoricActivityInstanceRestService ActivityInstanceService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricCaseActivityInstanceRestService_Fields.PATH) HistoricCaseActivityInstanceRestService getCaseActivityInstanceService();
	  HistoricCaseActivityInstanceRestService CaseActivityInstanceService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricVariableInstanceRestService_Fields.PATH) HistoricVariableInstanceRestService getVariableInstanceService();
	  HistoricVariableInstanceRestService VariableInstanceService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricProcessDefinitionRestService_Fields.PATH) HistoricProcessDefinitionRestService getProcessDefinitionService();
	  HistoricProcessDefinitionRestService ProcessDefinitionService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricDecisionDefinitionRestService_Fields.PATH) HistoricDecisionDefinitionRestService getDecisionDefinitionService();
	  HistoricDecisionDefinitionRestService DecisionDefinitionService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricCaseDefinitionRestService_Fields.PATH) HistoricCaseDefinitionRestService getCaseDefinitionService();
	  HistoricCaseDefinitionRestService CaseDefinitionService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricDecisionStatisticsRestService_Fields.PATH) HistoricDecisionStatisticsRestService getDecisionStatisticsService();
	  HistoricDecisionStatisticsRestService DecisionStatisticsService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(UserOperationLogRestService_Fields.PATH) UserOperationLogRestService getUserOperationLogRestService();
	  UserOperationLogRestService UserOperationLogRestService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricDetailRestService_Fields.PATH) HistoricDetailRestService getDetailService();
	  HistoricDetailRestService DetailService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricTaskInstanceRestService_Fields.PATH) HistoricTaskInstanceRestService getTaskInstanceService();
	  HistoricTaskInstanceRestService TaskInstanceService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricIncidentRestService_Fields.PATH) HistoricIncidentRestService getIncidentService();
	  HistoricIncidentRestService IncidentService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricJobLogRestService_Fields.PATH) HistoricJobLogRestService getJobLogService();
	  HistoricJobLogRestService JobLogService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricDecisionInstanceRestService_Fields.PATH) HistoricDecisionInstanceRestService getDecisionInstanceService();
	  HistoricDecisionInstanceRestService DecisionInstanceService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricIdentityLinkLogRestService_Fields.PATH) HistoricIdentityLinkLogRestService getIdentityLinkService();
	  HistoricIdentityLinkLogRestService IdentityLinkService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricBatchRestService_Fields.PATH) HistoricBatchRestService getBatchService();
	  HistoricBatchRestService BatchService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricExternalTaskLogRestService_Fields.PATH) HistoricExternalTaskLogRestService getExternalTaskLogService();
	  HistoricExternalTaskLogRestService ExternalTaskLogService {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoryCleanupRestService_Fields.PATH) HistoryCleanupRestService getHistoryCleanupRestService();
	  HistoryCleanupRestService HistoryCleanupRestService {get;}
	}

	public static class HistoryRestService_Fields
	{
	  public const string PATH = "/history";
	}

}