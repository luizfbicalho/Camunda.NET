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
namespace org.camunda.bpm.engine.rest.impl.optimize
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using OptimizeHistoricIdentityLinkLogEntity = org.camunda.bpm.engine.impl.persistence.entity.optimize.OptimizeHistoricIdentityLinkLogEntity;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using HistoricActivityInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricActivityInstanceDto;
	using HistoricDecisionInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceDto;
	using HistoricProcessInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto;
	using HistoricTaskInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceDto;
	using UserOperationLogEntryDto = org.camunda.bpm.engine.rest.dto.history.UserOperationLogEntryDto;
	using OptimizeHistoricIdentityLinkLogDto = org.camunda.bpm.engine.rest.dto.history.optimize.OptimizeHistoricIdentityLinkLogDto;
	using OptimizeHistoricVariableUpdateDto = org.camunda.bpm.engine.rest.dto.history.optimize.OptimizeHistoricVariableUpdateDto;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public class OptimizeRestService extends org.camunda.bpm.engine.rest.impl.AbstractRestProcessEngineAware
	public class OptimizeRestService : AbstractRestProcessEngineAware
	{

	  public const string PATH = "/optimize";

	  private DateConverter dateConverter;

	  public OptimizeRestService(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
		dateConverter = new DateConverter();
		dateConverter.ObjectMapper = objectMapper;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/activity-instance/completed") public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricActivityInstanceDto> getCompletedHistoricActivityInstances(@QueryParam("finishedAfter") String finishedAfterAsString, @QueryParam("finishedAt") String finishedAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<HistoricActivityInstanceDto> getCompletedHistoricActivityInstances(string finishedAfterAsString, string finishedAtAsString, int maxResults)
	  {

		DateTime finishedAfter = dateConverter.convertQueryParameterToType(finishedAfterAsString);
		DateTime finishedAt = dateConverter.convertQueryParameterToType(finishedAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;

		IList<HistoricActivityInstance> historicActivityInstances = config.OptimizeService.getCompletedHistoricActivityInstances(finishedAfter, finishedAt, maxResults);

		IList<HistoricActivityInstanceDto> result = new List<HistoricActivityInstanceDto>();
		foreach (HistoricActivityInstance instance in historicActivityInstances)
		{
		  HistoricActivityInstanceDto dto = HistoricActivityInstanceDto.fromHistoricActivityInstance(instance);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/activity-instance/running") public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricActivityInstanceDto> getRunningHistoricActivityInstances(@QueryParam("startedAfter") String startedAfterAsString, @QueryParam("startedAt") String startedAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<HistoricActivityInstanceDto> getRunningHistoricActivityInstances(string startedAfterAsString, string startedAtAsString, int maxResults)
	  {

		DateTime startedAfter = dateConverter.convertQueryParameterToType(startedAfterAsString);
		DateTime startedAt = dateConverter.convertQueryParameterToType(startedAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;

		IList<HistoricActivityInstance> historicActivityInstances = config.OptimizeService.getRunningHistoricActivityInstances(startedAfter, startedAt, maxResults);

		IList<HistoricActivityInstanceDto> result = new List<HistoricActivityInstanceDto>();
		foreach (HistoricActivityInstance instance in historicActivityInstances)
		{
		  HistoricActivityInstanceDto dto = HistoricActivityInstanceDto.fromHistoricActivityInstance(instance);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/task-instance/completed") public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceDto> getCompletedHistoricTaskInstances(@QueryParam("finishedAfter") String finishedAfterAsString, @QueryParam("finishedAt") String finishedAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<HistoricTaskInstanceDto> getCompletedHistoricTaskInstances(string finishedAfterAsString, string finishedAtAsString, int maxResults)
	  {

		DateTime finishedAfter = dateConverter.convertQueryParameterToType(finishedAfterAsString);
		DateTime finishedAt = dateConverter.convertQueryParameterToType(finishedAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;

		IList<HistoricTaskInstance> historicTaskInstances = config.OptimizeService.getCompletedHistoricTaskInstances(finishedAfter, finishedAt, maxResults);

		IList<HistoricTaskInstanceDto> result = new List<HistoricTaskInstanceDto>();
		foreach (HistoricTaskInstance instance in historicTaskInstances)
		{
		  HistoricTaskInstanceDto dto = HistoricTaskInstanceDto.fromHistoricTaskInstance(instance);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/task-instance/running") public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceDto> getRunningHistoricTaskInstances(@QueryParam("startedAfter") String startedAfterAsString, @QueryParam("startedAt") String startedAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<HistoricTaskInstanceDto> getRunningHistoricTaskInstances(string startedAfterAsString, string startedAtAsString, int maxResults)
	  {

		DateTime startedAfter = dateConverter.convertQueryParameterToType(startedAfterAsString);
		DateTime startedAt = dateConverter.convertQueryParameterToType(startedAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;

		IList<HistoricTaskInstance> historicTaskInstances = config.OptimizeService.getRunningHistoricTaskInstances(startedAfter, startedAt, maxResults);

		IList<HistoricTaskInstanceDto> result = new List<HistoricTaskInstanceDto>();
		foreach (HistoricTaskInstance instance in historicTaskInstances)
		{
		  HistoricTaskInstanceDto dto = HistoricTaskInstanceDto.fromHistoricTaskInstance(instance);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/user-operation") public java.util.List<org.camunda.bpm.engine.rest.dto.history.UserOperationLogEntryDto> getHistoricUserOperationLogs(@QueryParam("occurredAfter") String occurredAfterAsString, @QueryParam("occurredAt") String occurredAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<UserOperationLogEntryDto> getHistoricUserOperationLogs(string occurredAfterAsString, string occurredAtAsString, int maxResults)
	  {

		DateTime occurredAfter = dateConverter.convertQueryParameterToType(occurredAfterAsString);
		DateTime occurredAt = dateConverter.convertQueryParameterToType(occurredAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;

		IList<UserOperationLogEntry> operationLogEntries = config.OptimizeService.getHistoricUserOperationLogs(occurredAfter, occurredAt, maxResults);

		IList<UserOperationLogEntryDto> result = new List<UserOperationLogEntryDto>();
		foreach (UserOperationLogEntry logEntry in operationLogEntries)
		{
		  UserOperationLogEntryDto dto = UserOperationLogEntryDto.map(logEntry);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/identity-link-log") public java.util.List<org.camunda.bpm.engine.rest.dto.history.optimize.OptimizeHistoricIdentityLinkLogDto> getHistoricIdentityLinkLogs(@QueryParam("occurredAfter") String occurredAfterAsString, @QueryParam("occurredAt") String occurredAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<OptimizeHistoricIdentityLinkLogDto> getHistoricIdentityLinkLogs(string occurredAfterAsString, string occurredAtAsString, int maxResults)
	  {

		DateTime occurredAfter = dateConverter.convertQueryParameterToType(occurredAfterAsString);
		DateTime occurredAt = dateConverter.convertQueryParameterToType(occurredAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;

		IList<OptimizeHistoricIdentityLinkLogEntity> operationLogEntries = config.OptimizeService.getHistoricIdentityLinkLogs(occurredAfter, occurredAt, maxResults);

		IList<OptimizeHistoricIdentityLinkLogDto> result = new List<OptimizeHistoricIdentityLinkLogDto>();
		foreach (OptimizeHistoricIdentityLinkLogEntity logEntry in operationLogEntries)
		{
		  OptimizeHistoricIdentityLinkLogDto dto = OptimizeHistoricIdentityLinkLogDto.fromHistoricIdentityLink(logEntry);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/process-instance/completed") public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto> getCompletedHistoricProcessInstances(@QueryParam("finishedAfter") String finishedAfterAsString, @QueryParam("finishedAt") String finishedAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<HistoricProcessInstanceDto> getCompletedHistoricProcessInstances(string finishedAfterAsString, string finishedAtAsString, int maxResults)
	  {
		DateTime finishedAfter = dateConverter.convertQueryParameterToType(finishedAfterAsString);
		DateTime finishedAt = dateConverter.convertQueryParameterToType(finishedAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;
		IList<HistoricProcessInstance> historicProcessInstances = config.OptimizeService.getCompletedHistoricProcessInstances(finishedAfter, finishedAt, maxResults);

		IList<HistoricProcessInstanceDto> result = new List<HistoricProcessInstanceDto>();
		foreach (HistoricProcessInstance instance in historicProcessInstances)
		{
		  HistoricProcessInstanceDto dto = HistoricProcessInstanceDto.fromHistoricProcessInstance(instance);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/process-instance/running") public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto> getRunningHistoricProcessInstances(@QueryParam("startedAfter") String startedAfterAsString, @QueryParam("startedAt") String startedAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<HistoricProcessInstanceDto> getRunningHistoricProcessInstances(string startedAfterAsString, string startedAtAsString, int maxResults)
	  {
		DateTime startedAfter = dateConverter.convertQueryParameterToType(startedAfterAsString);
		DateTime startedAt = dateConverter.convertQueryParameterToType(startedAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;
		IList<HistoricProcessInstance> historicProcessInstances = config.OptimizeService.getRunningHistoricProcessInstances(startedAfter, startedAt, maxResults);

		IList<HistoricProcessInstanceDto> result = new List<HistoricProcessInstanceDto>();
		foreach (HistoricProcessInstance instance in historicProcessInstances)
		{
		  HistoricProcessInstanceDto dto = HistoricProcessInstanceDto.fromHistoricProcessInstance(instance);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/variable-update") public java.util.List<org.camunda.bpm.engine.rest.dto.history.optimize.OptimizeHistoricVariableUpdateDto> getHistoricVariableUpdates(@QueryParam("occurredAfter") String occurredAfterAsString, @QueryParam("occurredAt") String occurredAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<OptimizeHistoricVariableUpdateDto> getHistoricVariableUpdates(string occurredAfterAsString, string occurredAtAsString, int maxResults)
	  {
		DateTime occurredAfter = dateConverter.convertQueryParameterToType(occurredAfterAsString);
		DateTime occurredAt = dateConverter.convertQueryParameterToType(occurredAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;
		IList<HistoricVariableUpdate> historicVariableUpdates = config.OptimizeService.getHistoricVariableUpdates(occurredAfter, occurredAt, maxResults);

		IList<OptimizeHistoricVariableUpdateDto> result = new List<OptimizeHistoricVariableUpdateDto>();
		foreach (HistoricVariableUpdate instance in historicVariableUpdates)
		{
		  OptimizeHistoricVariableUpdateDto dto = OptimizeHistoricVariableUpdateDto.fromHistoricVariableUpdate(instance);
		  result.Add(dto);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/decision-instance") public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceDto> getHistoricDecisionInstances(@QueryParam("evaluatedAfter") String evaluatedAfterAsString, @QueryParam("evaluatedAt") String evaluatedAtAsString, @QueryParam("maxResults") int maxResults)
	  public virtual IList<HistoricDecisionInstanceDto> getHistoricDecisionInstances(string evaluatedAfterAsString, string evaluatedAtAsString, int maxResults)
	  {
		DateTime evaluatedAfter = dateConverter.convertQueryParameterToType(evaluatedAfterAsString);
		DateTime evaluatedAt = dateConverter.convertQueryParameterToType(evaluatedAtAsString);
		maxResults = ensureValidMaxResults(maxResults);

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;
		IList<HistoricDecisionInstance> historicDecisionInstances = config.OptimizeService.getHistoricDecisionInstances(evaluatedAfter, evaluatedAt, maxResults);

		IList<HistoricDecisionInstanceDto> resultList = new List<HistoricDecisionInstanceDto>();
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  HistoricDecisionInstanceDto dto = HistoricDecisionInstanceDto.fromHistoricDecisionInstance(historicDecisionInstance);
		  resultList.Add(dto);
		}

		return resultList;
	  }

	  protected internal virtual int ensureValidMaxResults(int givenMaxResults)
	  {
		return givenMaxResults > 0 ? givenMaxResults : int.MaxValue;
	  }
	}

}