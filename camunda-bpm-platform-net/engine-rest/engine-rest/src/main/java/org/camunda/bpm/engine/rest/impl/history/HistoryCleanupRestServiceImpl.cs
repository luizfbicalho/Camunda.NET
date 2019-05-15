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
namespace org.camunda.bpm.engine.rest.impl.history
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using BatchWindow = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.BatchWindow;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using HistoryCleanupConfigurationDto = org.camunda.bpm.engine.rest.dto.history.HistoryCleanupConfigurationDto;
	using JobDto = org.camunda.bpm.engine.rest.dto.runtime.JobDto;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using HistoryCleanupRestService = org.camunda.bpm.engine.rest.history.HistoryCleanupRestService;
	using Job = org.camunda.bpm.engine.runtime.Job;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoryCleanupRestServiceImpl : HistoryCleanupRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

		public HistoryCleanupRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
		{
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
		}

	  public virtual JobDto cleanupAsync(bool immediatelyDue)
	  {
		Job job = processEngine.HistoryService.cleanUpHistoryAsync(immediatelyDue);
		return JobDto.fromJob(job);
	  }

	  public virtual JobDto findCleanupJob()
	  {
		Job job = processEngine.HistoryService.findHistoryCleanupJob();
		if (job == null)
		{
		  throw new RestException(Status.NOT_FOUND, "History cleanup job does not exist");
		}
		return JobDto.fromJob(job);
	  }

	  public virtual IList<JobDto> findCleanupJobs()
	  {
		IList<Job> jobs = processEngine.HistoryService.findHistoryCleanupJobs();
		if (jobs == null || jobs.Count == 0)
		{
		  throw new RestException(Status.NOT_FOUND, "History cleanup jobs are empty");
		}
		IList<JobDto> dtos = new List<JobDto>();
		foreach (Job job in jobs)
		{
		  JobDto dto = JobDto.fromJob(job);
		  dtos.Add(dto);
		}
		return dtos;
	  }

	  public virtual HistoryCleanupConfigurationDto HistoryCleanupConfiguration
	  {
		  get
		  {
			HistoryCleanupConfigurationDto configurationDto = new HistoryCleanupConfigurationDto();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = (org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl) processEngine.getProcessEngineConfiguration();
			ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
			DateTime now = ClockUtil.CurrentTime;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.camunda.bpm.engine.impl.jobexecutor.historycleanup.BatchWindow batchWindow = processEngineConfiguration.getBatchWindowManager().getCurrentOrNextBatchWindow(now, processEngineConfiguration);
			BatchWindow batchWindow = processEngineConfiguration.BatchWindowManager.getCurrentOrNextBatchWindow(now, processEngineConfiguration);
			if (batchWindow == null)
			{
			  return configurationDto;
			}
			configurationDto.BatchWindowStartTime = batchWindow.Start;
			configurationDto.BatchWindowEndTime = batchWindow.End;
			return configurationDto;
		  }
	  }
	}

}