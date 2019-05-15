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
namespace org.camunda.bpm.engine.impl
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using CleanableHistoricBatchReport = org.camunda.bpm.engine.history.CleanableHistoricBatchReport;
	using CleanableHistoricCaseInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReport;
	using CleanableHistoricDecisionInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReport;
	using CleanableHistoricProcessInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReport;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using HistoricActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricActivityStatisticsQuery;
	using HistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseActivityInstanceQuery;
	using HistoricCaseActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricCaseActivityStatisticsQuery;
	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using HistoricDecisionInstanceStatisticsQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatisticsQuery;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using HistoricIncidentQuery = org.camunda.bpm.engine.history.HistoricIncidentQuery;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricProcessInstanceReport = org.camunda.bpm.engine.history.HistoricProcessInstanceReport;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using HistoricTaskInstanceReport = org.camunda.bpm.engine.history.HistoricTaskInstanceReport;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using NativeHistoricActivityInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricActivityInstanceQuery;
	using NativeHistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricCaseActivityInstanceQuery;
	using NativeHistoricCaseInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricCaseInstanceQuery;
	using NativeHistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricDecisionInstanceQuery;
	using NativeHistoricProcessInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricProcessInstanceQuery;
	using NativeHistoricTaskInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricTaskInstanceQuery;
	using NativeHistoricVariableInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricVariableInstanceQuery;
	using SetRemovalTimeSelectModeForHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricBatchesBuilder;
	using SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder;
	using SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder;
	using SetRemovalTimeToHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricBatchesBuilder;
	using SetRemovalTimeToHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricDecisionInstancesBuilder;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using DeleteHistoricBatchCmd = org.camunda.bpm.engine.impl.batch.history.DeleteHistoricBatchCmd;
	using HistoricBatchQueryImpl = org.camunda.bpm.engine.impl.batch.history.HistoricBatchQueryImpl;
	using DeleteHistoricCaseInstanceCmd = org.camunda.bpm.engine.impl.cmd.DeleteHistoricCaseInstanceCmd;
	using DeleteHistoricCaseInstancesBulkCmd = org.camunda.bpm.engine.impl.cmd.DeleteHistoricCaseInstancesBulkCmd;
	using DeleteHistoricProcessInstancesCmd = org.camunda.bpm.engine.impl.cmd.DeleteHistoricProcessInstancesCmd;
	using DeleteHistoricTaskInstanceCmd = org.camunda.bpm.engine.impl.cmd.DeleteHistoricTaskInstanceCmd;
	using DeleteHistoricVariableInstanceCmd = org.camunda.bpm.engine.impl.cmd.DeleteHistoricVariableInstanceCmd;
	using DeleteHistoricVariableInstancesByProcessInstanceIdCmd = org.camunda.bpm.engine.impl.cmd.DeleteHistoricVariableInstancesByProcessInstanceIdCmd;
	using DeleteUserOperationLogEntryCmd = org.camunda.bpm.engine.impl.cmd.DeleteUserOperationLogEntryCmd;
	using FindHistoryCleanupJobsCmd = org.camunda.bpm.engine.impl.cmd.FindHistoryCleanupJobsCmd;
	using GetHistoricExternalTaskLogErrorDetailsCmd = org.camunda.bpm.engine.impl.cmd.GetHistoricExternalTaskLogErrorDetailsCmd;
	using GetHistoricJobLogExceptionStacktraceCmd = org.camunda.bpm.engine.impl.cmd.GetHistoricJobLogExceptionStacktraceCmd;
	using HistoryCleanupCmd = org.camunda.bpm.engine.impl.cmd.HistoryCleanupCmd;
	using DeleteHistoricProcessInstancesBatchCmd = org.camunda.bpm.engine.impl.cmd.batch.DeleteHistoricProcessInstancesBatchCmd;
	using DeleteHistoricDecisionInstanceByDefinitionIdCmd = org.camunda.bpm.engine.impl.dmn.cmd.DeleteHistoricDecisionInstanceByDefinitionIdCmd;
	using DeleteHistoricDecisionInstanceByInstanceIdCmd = org.camunda.bpm.engine.impl.dmn.cmd.DeleteHistoricDecisionInstanceByInstanceIdCmd;
	using DeleteHistoricDecisionInstancesBatchCmd = org.camunda.bpm.engine.impl.dmn.cmd.DeleteHistoricDecisionInstancesBatchCmd;
	using DeleteHistoricDecisionInstancesBulkCmd = org.camunda.bpm.engine.impl.dmn.cmd.DeleteHistoricDecisionInstancesBulkCmd;
	using SetRemovalTimeToHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricProcessInstancesBuilder;
	using SetRemovalTimeToHistoricBatchesBuilderImpl = org.camunda.bpm.engine.impl.history.SetRemovalTimeToHistoricBatchesBuilderImpl;
	using SetRemovalTimeToHistoricDecisionInstancesBuilderImpl = org.camunda.bpm.engine.impl.history.SetRemovalTimeToHistoricDecisionInstancesBuilderImpl;
	using SetRemovalTimeToHistoricProcessInstancesBuilderImpl = org.camunda.bpm.engine.impl.history.SetRemovalTimeToHistoricProcessInstancesBuilderImpl;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Bernd Ruecker (camunda)
	/// @author Christian Stettler
	/// </summary>
	public class HistoryServiceImpl : ServiceImpl, HistoryService
	{

	  public virtual HistoricProcessInstanceQuery createHistoricProcessInstanceQuery()
	  {
		return new HistoricProcessInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricActivityInstanceQuery createHistoricActivityInstanceQuery()
	  {
		return new HistoricActivityInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricActivityStatisticsQuery createHistoricActivityStatisticsQuery(string processDefinitionId)
	  {
		return new HistoricActivityStatisticsQueryImpl(processDefinitionId, commandExecutor);
	  }

	  public virtual HistoricCaseActivityStatisticsQuery createHistoricCaseActivityStatisticsQuery(string caseDefinitionId)
	  {
		return new HistoricCaseActivityStatisticsQueryImpl(caseDefinitionId, commandExecutor);
	  }

	  public virtual HistoricTaskInstanceQuery createHistoricTaskInstanceQuery()
	  {
		return new HistoricTaskInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricDetailQuery createHistoricDetailQuery()
	  {
		return new HistoricDetailQueryImpl(commandExecutor);
	  }

	  public virtual UserOperationLogQuery createUserOperationLogQuery()
	  {
		return new UserOperationLogQueryImpl(commandExecutor);
	  }

	  public virtual HistoricVariableInstanceQuery createHistoricVariableInstanceQuery()
	  {
		return new HistoricVariableInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricIncidentQuery createHistoricIncidentQuery()
	  {
		return new HistoricIncidentQueryImpl(commandExecutor);
	  }

	  public virtual HistoricIdentityLinkLogQueryImpl createHistoricIdentityLinkLogQuery()
	  {
		return new HistoricIdentityLinkLogQueryImpl(commandExecutor);
	  }

	  public virtual HistoricCaseInstanceQuery createHistoricCaseInstanceQuery()
	  {
		return new HistoricCaseInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricCaseActivityInstanceQuery createHistoricCaseActivityInstanceQuery()
	  {
		return new HistoricCaseActivityInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricDecisionInstanceQuery createHistoricDecisionInstanceQuery()
	  {
		return new HistoricDecisionInstanceQueryImpl(commandExecutor);
	  }

	  public virtual void deleteHistoricTaskInstance(string taskId)
	  {
		commandExecutor.execute(new DeleteHistoricTaskInstanceCmd(taskId));
	  }

	  public virtual void deleteHistoricProcessInstance(string processInstanceId)
	  {
		deleteHistoricProcessInstances(Arrays.asList(processInstanceId));
	  }

	  public virtual void deleteHistoricProcessInstanceIfExists(string processInstanceId)
	  {
		deleteHistoricProcessInstancesIfExists(Arrays.asList(processInstanceId));
	  }

	  public virtual void deleteHistoricProcessInstances(IList<string> processInstanceIds)
	  {
		commandExecutor.execute(new DeleteHistoricProcessInstancesCmd(processInstanceIds, true));
	  }

	  public virtual void deleteHistoricProcessInstancesIfExists(IList<string> processInstanceIds)
	  {
		commandExecutor.execute(new DeleteHistoricProcessInstancesCmd(processInstanceIds, false));
	  }

	  public virtual void deleteHistoricProcessInstancesBulk(IList<string> processInstanceIds)
	  {
		deleteHistoricProcessInstances(processInstanceIds);
	  }

	  public virtual Job cleanUpHistoryAsync()
	  {
		return cleanUpHistoryAsync(false);
	  }

	  public virtual Job cleanUpHistoryAsync(bool immediatelyDue)
	  {
		return commandExecutor.execute(new HistoryCleanupCmd(immediatelyDue));
	  }

	  public virtual Job findHistoryCleanupJob()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> jobs = commandExecutor.execute(new org.camunda.bpm.engine.impl.cmd.FindHistoryCleanupJobsCmd());
		IList<Job> jobs = commandExecutor.execute(new FindHistoryCleanupJobsCmd());
		if (jobs.Count > 0)
		{
		  return jobs[0];
		}
		else
		{
		  return null;
		}
	  }

	  public virtual IList<Job> findHistoryCleanupJobs()
	  {
		return commandExecutor.execute(new FindHistoryCleanupJobsCmd());
	  }

	  public virtual Batch deleteHistoricProcessInstancesAsync(IList<string> processInstanceIds, string deleteReason)
	  {
		return this.deleteHistoricProcessInstancesAsync(processInstanceIds,null,deleteReason);
	  }

	  public virtual Batch deleteHistoricProcessInstancesAsync(HistoricProcessInstanceQuery query, string deleteReason)
	  {
		return this.deleteHistoricProcessInstancesAsync(null,query,deleteReason);
	  }

	  public virtual Batch deleteHistoricProcessInstancesAsync(IList<string> processInstanceIds, HistoricProcessInstanceQuery query, string deleteReason)
	  {
		return commandExecutor.execute(new DeleteHistoricProcessInstancesBatchCmd(processInstanceIds, query, deleteReason));
	  }

	  public virtual void deleteUserOperationLogEntry(string entryId)
	  {
		commandExecutor.execute(new DeleteUserOperationLogEntryCmd(entryId));
	  }

	  public virtual void deleteHistoricCaseInstance(string caseInstanceId)
	  {
		commandExecutor.execute(new DeleteHistoricCaseInstanceCmd(caseInstanceId));
	  }

	  public virtual void deleteHistoricCaseInstancesBulk(IList<string> caseInstanceIds)
	  {
		commandExecutor.execute(new DeleteHistoricCaseInstancesBulkCmd(caseInstanceIds));
	  }

	  public virtual void deleteHistoricDecisionInstance(string decisionDefinitionId)
	  {
		deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);
	  }

	  public virtual void deleteHistoricDecisionInstancesBulk(IList<string> decisionInstanceIds)
	  {
		commandExecutor.execute(new DeleteHistoricDecisionInstancesBulkCmd(decisionInstanceIds));
	  }

	  public virtual void deleteHistoricDecisionInstanceByDefinitionId(string decisionDefinitionId)
	  {
		commandExecutor.execute(new DeleteHistoricDecisionInstanceByDefinitionIdCmd(decisionDefinitionId));
	  }

	  public virtual void deleteHistoricDecisionInstanceByInstanceId(string historicDecisionInstanceId)
	  {
		commandExecutor.execute(new DeleteHistoricDecisionInstanceByInstanceIdCmd(historicDecisionInstanceId));
	  }

	  public virtual Batch deleteHistoricDecisionInstancesAsync(IList<string> decisionInstanceIds, string deleteReason)
	  {
		return deleteHistoricDecisionInstancesAsync(decisionInstanceIds, null, deleteReason);
	  }

	  public virtual Batch deleteHistoricDecisionInstancesAsync(HistoricDecisionInstanceQuery query, string deleteReason)
	  {
		return deleteHistoricDecisionInstancesAsync(null, query, deleteReason);
	  }

	  public virtual Batch deleteHistoricDecisionInstancesAsync(IList<string> decisionInstanceIds, HistoricDecisionInstanceQuery query, string deleteReason)
	  {
		return commandExecutor.execute(new DeleteHistoricDecisionInstancesBatchCmd(decisionInstanceIds, query, deleteReason));
	  }

	  public virtual void deleteHistoricVariableInstance(string variableInstanceId)
	  {
		commandExecutor.execute(new DeleteHistoricVariableInstanceCmd(variableInstanceId));
	  }

	  public virtual void deleteHistoricVariableInstancesByProcessInstanceId(string processInstanceId)
	  {
		commandExecutor.execute(new DeleteHistoricVariableInstancesByProcessInstanceIdCmd(processInstanceId));
	  }

	  public virtual NativeHistoricProcessInstanceQuery createNativeHistoricProcessInstanceQuery()
	  {
		return new NativeHistoricProcessInstanceQueryImpl(commandExecutor);
	  }

	  public virtual NativeHistoricTaskInstanceQuery createNativeHistoricTaskInstanceQuery()
	  {
		return new NativeHistoricTaskInstanceQueryImpl(commandExecutor);
	  }

	  public virtual NativeHistoricActivityInstanceQuery createNativeHistoricActivityInstanceQuery()
	  {
		return new NativeHistoricActivityInstanceQueryImpl(commandExecutor);
	  }

	  public virtual NativeHistoricCaseInstanceQuery createNativeHistoricCaseInstanceQuery()
	  {
		return new NativeHistoricCaseInstanceQueryImpl(commandExecutor);
	  }

	  public virtual NativeHistoricCaseActivityInstanceQuery createNativeHistoricCaseActivityInstanceQuery()
	  {
		return new NativeHistoricCaseActivityInstanceQueryImpl(commandExecutor);
	  }

	  public virtual NativeHistoricDecisionInstanceQuery createNativeHistoricDecisionInstanceQuery()
	  {
		return new NativeHistoryDecisionInstanceQueryImpl(commandExecutor);
	  }

	  public virtual NativeHistoricVariableInstanceQuery createNativeHistoricVariableInstanceQuery()
	  {
		return new NativeHistoricVariableInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricJobLogQuery createHistoricJobLogQuery()
	  {
		return new HistoricJobLogQueryImpl(commandExecutor);
	  }

	  public virtual string getHistoricJobLogExceptionStacktrace(string historicJobLogId)
	  {
		return commandExecutor.execute(new GetHistoricJobLogExceptionStacktraceCmd(historicJobLogId));
	  }

	  public virtual HistoricProcessInstanceReport createHistoricProcessInstanceReport()
	  {
		return new HistoricProcessInstanceReportImpl(commandExecutor);
	  }

	  public virtual HistoricTaskInstanceReport createHistoricTaskInstanceReport()
	  {
		return new HistoricTaskInstanceReportImpl(commandExecutor);
	  }

	  public virtual CleanableHistoricProcessInstanceReport createCleanableHistoricProcessInstanceReport()
	  {
		return new CleanableHistoricProcessInstanceReportImpl(commandExecutor);
	  }

	  public virtual CleanableHistoricDecisionInstanceReport createCleanableHistoricDecisionInstanceReport()
	  {
		return new CleanableHistoricDecisionInstanceReportImpl(commandExecutor);
	  }

	  public virtual CleanableHistoricCaseInstanceReport createCleanableHistoricCaseInstanceReport()
	  {
		return new CleanableHistoricCaseInstanceReportImpl(commandExecutor);
	  }

	  public virtual CleanableHistoricBatchReport createCleanableHistoricBatchReport()
	  {
		return new CleanableHistoricBatchReportImpl(commandExecutor);
	  }

	  public virtual HistoricBatchQuery createHistoricBatchQuery()
	  {
		return new HistoricBatchQueryImpl(commandExecutor);
	  }

	  public virtual void deleteHistoricBatch(string batchId)
	  {
		commandExecutor.execute(new DeleteHistoricBatchCmd(batchId));
	  }

	  public virtual HistoricDecisionInstanceStatisticsQuery createHistoricDecisionInstanceStatisticsQuery(string decisionRequirementsDefinitionId)
	  {
		return new HistoricDecisionInstanceStatisticsQueryImpl(decisionRequirementsDefinitionId, commandExecutor);
	  }

	  public virtual HistoricExternalTaskLogQuery createHistoricExternalTaskLogQuery()
	  {
		return new HistoricExternalTaskLogQueryImpl(commandExecutor);
	  }

	  public virtual string getHistoricExternalTaskLogErrorDetails(string historicExternalTaskLogId)
	  {
		return commandExecutor.execute(new GetHistoricExternalTaskLogErrorDetailsCmd(historicExternalTaskLogId));
	  }

	  public virtual SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder setRemovalTimeToHistoricProcessInstances()
	  {
		return new SetRemovalTimeToHistoricProcessInstancesBuilderImpl(commandExecutor);
	  }

	  public virtual SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder setRemovalTimeToHistoricDecisionInstances()
	  {
		return new SetRemovalTimeToHistoricDecisionInstancesBuilderImpl(commandExecutor);
	  }

	  public virtual SetRemovalTimeSelectModeForHistoricBatchesBuilder setRemovalTimeToHistoricBatches()
	  {
		return new SetRemovalTimeToHistoricBatchesBuilderImpl(commandExecutor);
	  }

	}

}