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
namespace org.camunda.bpm.engine.rest.impl.history
{
	using HistoricActivityInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricActivityInstanceRestService;
	using HistoricProcessDefinitionRestService = org.camunda.bpm.engine.rest.history.HistoricProcessDefinitionRestService;
	using HistoricBatchRestService = org.camunda.bpm.engine.rest.history.HistoricBatchRestService;
	using HistoricCaseActivityInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricCaseActivityInstanceRestService;
	using HistoricCaseDefinitionRestService = org.camunda.bpm.engine.rest.history.HistoricCaseDefinitionRestService;
	using HistoricCaseInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricCaseInstanceRestService;
	using HistoricDecisionDefinitionRestService = org.camunda.bpm.engine.rest.history.HistoricDecisionDefinitionRestService;
	using HistoricDecisionInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricDecisionInstanceRestService;
	using HistoricDecisionStatisticsRestService = org.camunda.bpm.engine.rest.history.HistoricDecisionStatisticsRestService;
	using HistoricDetailRestService = org.camunda.bpm.engine.rest.history.HistoricDetailRestService;
	using HistoricExternalTaskLogRestService = org.camunda.bpm.engine.rest.history.HistoricExternalTaskLogRestService;
	using HistoricIdentityLinkLogRestService = org.camunda.bpm.engine.rest.history.HistoricIdentityLinkLogRestService;
	using HistoricIncidentRestService = org.camunda.bpm.engine.rest.history.HistoricIncidentRestService;
	using HistoricJobLogRestService = org.camunda.bpm.engine.rest.history.HistoricJobLogRestService;
	using HistoricProcessInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricProcessInstanceRestService;
	using HistoricTaskInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricTaskInstanceRestService;
	using HistoricVariableInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricVariableInstanceRestService;
	using HistoryCleanupRestService = org.camunda.bpm.engine.rest.history.HistoryCleanupRestService;
	using HistoryRestService = org.camunda.bpm.engine.rest.history.HistoryRestService;
	using UserOperationLogRestService = org.camunda.bpm.engine.rest.history.UserOperationLogRestService;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoryRestServiceImpl : AbstractRestProcessEngineAware, HistoryRestService
	{

	  public HistoryRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual HistoricProcessInstanceRestService ProcessInstanceService
	  {
		  get
		  {
			return new HistoricProcessInstanceRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricCaseInstanceRestService CaseInstanceService
	  {
		  get
		  {
			return new HistoricCaseInstanceRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricActivityInstanceRestService ActivityInstanceService
	  {
		  get
		  {
			return new HistoricActivityInstanceRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricCaseActivityInstanceRestService CaseActivityInstanceService
	  {
		  get
		  {
			return new HistoricCaseActivityInstanceRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricVariableInstanceRestService VariableInstanceService
	  {
		  get
		  {
			return new HistoricVariableInstanceRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricProcessDefinitionRestService ProcessDefinitionService
	  {
		  get
		  {
			return new HistoricProcessDefinitionRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricDecisionDefinitionRestService DecisionDefinitionService
	  {
		  get
		  {
			return new HistoricDecisionDefinitionRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricDecisionStatisticsRestService DecisionStatisticsService
	  {
		  get
		  {
			return new HistoricDecisionStatisticsRestServiceImpl(ProcessEngine);
		  }
	  }

	  public virtual HistoricCaseDefinitionRestService CaseDefinitionService
	  {
		  get
		  {
			return new HistoricCaseDefinitionRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual UserOperationLogRestService UserOperationLogRestService
	  {
		  get
		  {
			return new UserOperationLogRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricDetailRestService DetailService
	  {
		  get
		  {
			return new HistoricDetailRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricTaskInstanceRestService TaskInstanceService
	  {
		  get
		  {
			return new HistoricTaskInstanceRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricIncidentRestService IncidentService
	  {
		  get
		  {
			return new HistoricIncidentRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricIdentityLinkLogRestService IdentityLinkService
	  {
		  get
		  {
			return new HistoricIdentityLinkLogRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricJobLogRestService JobLogService
	  {
		  get
		  {
			return new HistoricJobLogRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricDecisionInstanceRestService DecisionInstanceService
	  {
		  get
		  {
			return new HistoricDecisionInstanceRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricBatchRestService BatchService
	  {
		  get
		  {
			return new HistoricBatchRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoricExternalTaskLogRestService ExternalTaskLogService
	  {
		  get
		  {
			return new HistoricExternalTaskLogRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }

	  public virtual HistoryCleanupRestService HistoryCleanupRestService
	  {
		  get
		  {
			return new HistoryCleanupRestServiceImpl(ObjectMapper, ProcessEngine);
		  }
	  }
	}

}