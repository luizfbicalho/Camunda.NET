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
namespace org.camunda.bpm.engine.impl
{
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using OptimizeCompletedHistoricActivityInstanceQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeCompletedHistoricActivityInstanceQueryCmd;
	using OptimizeCompletedHistoricProcessInstanceQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeCompletedHistoricProcessInstanceQueryCmd;
	using OptimizeCompletedHistoricTaskInstanceQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeCompletedHistoricTaskInstanceQueryCmd;
	using OptimizeHistoricDecisionInstanceQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeHistoricDecisionInstanceQueryCmd;
	using OptimizeHistoricIdentityLinkLogQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeHistoricIdentityLinkLogQueryCmd;
	using OptimizeHistoricUserOperationsLogQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeHistoricUserOperationsLogQueryCmd;
	using OptimizeHistoricVariableUpdateQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeHistoricVariableUpdateQueryCmd;
	using OptimizeRunningHistoricActivityInstanceQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeRunningHistoricActivityInstanceQueryCmd;
	using OptimizeRunningHistoricProcessInstanceQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeRunningHistoricProcessInstanceQueryCmd;
	using OptimizeRunningHistoricTaskInstanceQueryCmd = org.camunda.bpm.engine.impl.cmd.optimize.OptimizeRunningHistoricTaskInstanceQueryCmd;
	using OptimizeHistoricIdentityLinkLogEntity = org.camunda.bpm.engine.impl.persistence.entity.optimize.OptimizeHistoricIdentityLinkLogEntity;


	public class OptimizeService : ServiceImpl
	{

	  public virtual IList<HistoricActivityInstance> getCompletedHistoricActivityInstances(DateTime finishedAfter, DateTime finishedAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeCompletedHistoricActivityInstanceQueryCmd(finishedAfter, finishedAt, maxResults)
	   );
	  }

	  public virtual IList<HistoricActivityInstance> getRunningHistoricActivityInstances(DateTime startedAfter, DateTime startedAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeRunningHistoricActivityInstanceQueryCmd(startedAfter, startedAt, maxResults)
	   );
	  }

	  public virtual IList<HistoricTaskInstance> getCompletedHistoricTaskInstances(DateTime finishedAfter, DateTime finishedAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeCompletedHistoricTaskInstanceQueryCmd(finishedAfter, finishedAt, maxResults)
	   );
	  }

	  public virtual IList<HistoricTaskInstance> getRunningHistoricTaskInstances(DateTime startedAfter, DateTime startedAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeRunningHistoricTaskInstanceQueryCmd(startedAfter, startedAt, maxResults)
	   );
	  }

	  public virtual IList<UserOperationLogEntry> getHistoricUserOperationLogs(DateTime occurredAfter, DateTime occurredAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeHistoricUserOperationsLogQueryCmd(occurredAfter, occurredAt, maxResults)
	   );
	  }

	  public virtual IList<OptimizeHistoricIdentityLinkLogEntity> getHistoricIdentityLinkLogs(DateTime occurredAfter, DateTime occurredAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeHistoricIdentityLinkLogQueryCmd(occurredAfter, occurredAt, maxResults)
	   );
	  }

	  public virtual IList<HistoricProcessInstance> getCompletedHistoricProcessInstances(DateTime finishedAfter, DateTime finishedAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeCompletedHistoricProcessInstanceQueryCmd(finishedAfter, finishedAt, maxResults)
	   );
	  }

	  public virtual IList<HistoricProcessInstance> getRunningHistoricProcessInstances(DateTime startedAfter, DateTime startedAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeRunningHistoricProcessInstanceQueryCmd(startedAfter, startedAt, maxResults)
	   );
	  }

	  public virtual IList<HistoricVariableUpdate> getHistoricVariableUpdates(DateTime occurredAfter, DateTime occurredAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeHistoricVariableUpdateQueryCmd(occurredAfter, occurredAt, maxResults)
	   );
	  }

	  public virtual IList<HistoricDecisionInstance> getHistoricDecisionInstances(DateTime evaluatedAfter, DateTime evaluatedAt, int maxResults)
	  {
		return commandExecutor.execute(new OptimizeHistoricDecisionInstanceQueryCmd(evaluatedAfter, evaluatedAt, maxResults)
	   );
	  }


	}

}