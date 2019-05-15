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
namespace org.camunda.bpm.engine.impl.optimize
{
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using AbstractManager = org.camunda.bpm.engine.impl.persistence.AbstractManager;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;

	public class OptimizeManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity> getHistoricVariableUpdateByteArrays(java.util.List<String> byteArrayIds)
	  public virtual IList<ByteArrayEntity> getHistoricVariableUpdateByteArrays(IList<string> byteArrayIds)
	  {
		return (IList<ByteArrayEntity>) DbEntityManager.selectList("selectByteArrays", byteArrayIds);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricActivityInstance> getCompletedHistoricActivityInstances(java.util.Date finishedAfter, java.util.Date finishedAt, int maxResults)
	  public virtual IList<HistoricActivityInstance> getCompletedHistoricActivityInstances(DateTime finishedAfter, DateTime finishedAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfProcessDefinitions();

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["finishedAfter"] = finishedAfter;
		@params["finishedAt"] = finishedAt;
		@params["maxResults"] = maxResults;

		return DbEntityManager.selectList("selectCompletedHistoricActivityPage", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricActivityInstance> getRunningHistoricActivityInstances(java.util.Date startedAfter, java.util.Date startedAt, int maxResults)
	  public virtual IList<HistoricActivityInstance> getRunningHistoricActivityInstances(DateTime startedAfter, DateTime startedAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfProcessDefinitions();

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["startedAfter"] = startedAfter;
		@params["startedAt"] = startedAt;
		@params["maxResults"] = maxResults;

		return DbEntityManager.selectList("selectRunningHistoricActivityPage", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricTaskInstance> getCompletedHistoricTaskInstances(java.util.Date finishedAfter, java.util.Date finishedAt, int maxResults)
	  public virtual IList<HistoricTaskInstance> getCompletedHistoricTaskInstances(DateTime finishedAfter, DateTime finishedAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfProcessDefinitions();

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["finishedAfter"] = finishedAfter;
		@params["finishedAt"] = finishedAt;
		@params["maxResults"] = maxResults;

		return DbEntityManager.selectList("selectCompletedHistoricTaskInstancePage", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricTaskInstance> getRunningHistoricTaskInstances(java.util.Date startedAfter, java.util.Date startedAt, int maxResults)
	  public virtual IList<HistoricTaskInstance> getRunningHistoricTaskInstances(DateTime startedAfter, DateTime startedAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfProcessDefinitions();

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["startedAfter"] = startedAfter;
		@params["startedAt"] = startedAt;
		@params["maxResults"] = maxResults;

		return DbEntityManager.selectList("selectRunningHistoricTaskInstancePage", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.UserOperationLogEntry> getHistoricUserOperationLogs(java.util.Date occurredAfter, java.util.Date occurredAt, int maxResults)
	  public virtual IList<UserOperationLogEntry> getHistoricUserOperationLogs(DateTime occurredAfter, DateTime occurredAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfProcessDefinitions();

		string[] operationTypes = new string[]{org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ASSIGN, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CLAIM, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE};
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["occurredAfter"] = occurredAfter;
		@params["occurredAt"] = occurredAt;
		@params["operationTypes"] = operationTypes;
		@params["maxResults"] = maxResults;

		return DbEntityManager.selectList("selectHistoricUserOperationLogPage", @params);
	  }

	  private void checkIsAuthorizedToReadHistoryOfProcessDefinitions()
	  {
		AuthorizationManager.checkAuthorization(READ_HISTORY, PROCESS_DEFINITION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricProcessInstance> getCompletedHistoricProcessInstances(java.util.Date finishedAfter, java.util.Date finishedAt, int maxResults)
	  public virtual IList<HistoricProcessInstance> getCompletedHistoricProcessInstances(DateTime finishedAfter, DateTime finishedAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfProcessDefinitions();

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["finishedAfter"] = finishedAfter;
		@params["finishedAt"] = finishedAt;
		@params["maxResults"] = maxResults;

		return DbEntityManager.selectList("selectCompletedHistoricProcessInstancePage", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricProcessInstance> getRunningHistoricProcessInstances(java.util.Date startedAfter, java.util.Date startedAt, int maxResults)
	  public virtual IList<HistoricProcessInstance> getRunningHistoricProcessInstances(DateTime startedAfter, DateTime startedAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfProcessDefinitions();

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["startedAfter"] = startedAfter;
		@params["startedAt"] = startedAt;
		@params["maxResults"] = maxResults;

		return DbEntityManager.selectList("selectRunningHistoricProcessInstancePage", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricVariableUpdate> getHistoricVariableUpdates(java.util.Date occurredAfter, java.util.Date occurredAt, int maxResults)
	  public virtual IList<HistoricVariableUpdate> getHistoricVariableUpdates(DateTime occurredAfter, DateTime occurredAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfProcessDefinitions();

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["occurredAfter"] = occurredAfter;
		@params["occurredAt"] = occurredAt;
		@params["maxResults"] = maxResults;

		return DbEntityManager.selectList("selectHistoricVariableUpdatePage", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricDecisionInstance> getHistoricDecisionInstances(java.util.Date evaluatedAfter, java.util.Date evaluatedAt, int maxResults)
	  public virtual IList<HistoricDecisionInstance> getHistoricDecisionInstances(DateTime evaluatedAfter, DateTime evaluatedAt, int maxResults)
	  {
		checkIsAuthorizedToReadHistoryOfDecisionDefinitions();

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["evaluatedAfter"] = evaluatedAfter;
		@params["evaluatedAt"] = evaluatedAt;
		@params["maxResults"] = maxResults;

		IList<HistoricDecisionInstance> decisionInstances = DbEntityManager.selectList("selectHistoricDecisionInstancePage", @params);

		HistoricDecisionInstanceQueryImpl query = (HistoricDecisionInstanceQueryImpl) (new HistoricDecisionInstanceQueryImpl()).disableBinaryFetching().disableCustomObjectDeserialization().includeInputs().includeOutputs();

		HistoricDecisionInstanceManager.enrichHistoricDecisionsWithInputsAndOutputs(query, decisionInstances);

		return decisionInstances;
	  }

	  private void checkIsAuthorizedToReadHistoryOfDecisionDefinitions()
	  {
		AuthorizationManager.checkAuthorization(READ_HISTORY, DECISION_DEFINITION);
	  }

	}

}