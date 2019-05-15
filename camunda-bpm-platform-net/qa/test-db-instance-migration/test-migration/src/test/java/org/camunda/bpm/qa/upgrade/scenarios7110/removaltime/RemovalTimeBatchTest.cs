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
namespace org.camunda.bpm.qa.upgrade.scenarios7110.removaltime
{
	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class RemovalTimeBatchTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
	  public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void assignService()
	  public virtual void assignService()
	  {
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForRootProcessInstanceOnly()
	  public virtual void shouldSetRemovalTimeForRootProcessInstanceOnly()
	  {
		// given
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKey("rootProcessInstance");

		HistoricProcessInstance historicRootProcessInstance = query.singleResult();

		HistoricProcessInstance historicChildProcessInstance = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(historicRootProcessInstance.Id).singleResult();

		// assume
		assertThat(historicRootProcessInstance.RemovalTime, nullValue());
		assertThat(historicChildProcessInstance.RemovalTime, nullValue());

		// when
		syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(DateTime.Now).byQuery(query).hierarchical().executeAsync());

		historicRootProcessInstance = query.singleResult();

		historicChildProcessInstance = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(historicRootProcessInstance.Id).singleResult();

		// then
		assertThat(historicRootProcessInstance.RemovalTime, notNullValue());
		assertThat(historicChildProcessInstance.RemovalTime, nullValue());
	  }

	  // helper ////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual void syncExec(Batch batch)
	  {
		string seedJobDefinitionId = batch.SeedJobDefinitionId;

		string jobId = engineRule.ManagementService.createJobQuery().jobDefinitionId(seedJobDefinitionId).singleResult().Id;

		engineRule.ManagementService.executeJob(jobId);

		string batchJobDefinitionId = batch.BatchJobDefinitionId;

		IList<Job> jobs = engineRule.ManagementService.createJobQuery().jobDefinitionId(batchJobDefinitionId).list();

		foreach (Job job in jobs)
		{
		  engineRule.ManagementService.executeJob(job.Id);
		}

		string monitorJobDefinitionId = batch.MonitorJobDefinitionId;

		jobId = engineRule.ManagementService.createJobQuery().jobDefinitionId(monitorJobDefinitionId).singleResult().Id;

		engineRule.ManagementService.executeJob(jobId);
	  }

	}

}