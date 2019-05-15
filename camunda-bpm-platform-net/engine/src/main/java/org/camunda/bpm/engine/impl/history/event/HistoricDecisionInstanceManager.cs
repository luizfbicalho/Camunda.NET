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
namespace org.camunda.bpm.engine.impl.history.@event
{
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using AbstractHistoricManager = org.camunda.bpm.engine.impl.persistence.AbstractHistoricManager;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using AbstractTypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.AbstractTypedValueSerializer;


	/// <summary>
	/// Data base operations for <seealso cref="HistoricDecisionInstanceEntity"/>.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public class HistoricDecisionInstanceManager : AbstractHistoricManager
	{

	  public virtual void deleteHistoricDecisionInstancesByDecisionDefinitionId(string decisionDefinitionId)
	  {
		if (HistoryEnabled)
		{
		  IList<HistoricDecisionInstanceEntity> decisionInstances = findHistoricDecisionInstancesByDecisionDefinitionId(decisionDefinitionId);

		  IList<string> decisionInstanceIds = new List<string>();
		  foreach (HistoricDecisionInstanceEntity decisionInstance in decisionInstances)
		  {
			decisionInstanceIds.Add(decisionInstance.Id);
			// delete decision instance
			decisionInstance.delete();
		  }

		  if (decisionInstanceIds.Count > 0)
		  {
			deleteHistoricDecisionInstanceByIds(decisionInstanceIds);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.util.List<HistoricDecisionInstanceEntity> findHistoricDecisionInstancesByDecisionDefinitionId(String decisionDefinitionId)
	  protected internal virtual IList<HistoricDecisionInstanceEntity> findHistoricDecisionInstancesByDecisionDefinitionId(string decisionDefinitionId)
	  {
		return DbEntityManager.selectList("selectHistoricDecisionInstancesByDecisionDefinitionId", configureParameterizedQuery(decisionDefinitionId));
	  }

	  public virtual void deleteHistoricDecisionInstanceByIds(IList<string> decisionInstanceIds)
	  {
		DbEntityManager.deletePreserveOrder(typeof(ByteArrayEntity), "deleteHistoricDecisionInputInstanceByteArraysByDecisionInstanceIds", decisionInstanceIds);
		DbEntityManager.deletePreserveOrder(typeof(ByteArrayEntity), "deleteHistoricDecisionOutputInstanceByteArraysByDecisionInstanceIds", decisionInstanceIds);
		DbEntityManager.deletePreserveOrder(typeof(HistoricDecisionInputInstanceEntity), "deleteHistoricDecisionInputInstanceByDecisionInstanceIds", decisionInstanceIds);
		DbEntityManager.deletePreserveOrder(typeof(HistoricDecisionOutputInstanceEntity), "deleteHistoricDecisionOutputInstanceByDecisionInstanceIds", decisionInstanceIds);
		DbEntityManager.deletePreserveOrder(typeof(HistoricDecisionInstanceEntity), "deleteHistoricDecisionInstanceByIds", decisionInstanceIds);
	  }

	  public virtual void insertHistoricDecisionInstances(HistoricDecisionEvaluationEvent @event)
	  {
		if (HistoryEnabled)
		{

		  HistoricDecisionInstanceEntity rootHistoricDecisionInstance = @event.RootHistoricDecisionInstance;
		  insertHistoricDecisionInstance(rootHistoricDecisionInstance);

		  foreach (HistoricDecisionInstanceEntity requiredHistoricDecisionInstances in @event.RequiredHistoricDecisionInstances)
		  {
			requiredHistoricDecisionInstances.RootDecisionInstanceId = rootHistoricDecisionInstance.Id;
			insertHistoricDecisionInstance(requiredHistoricDecisionInstances);
		  }
		}
	  }

	  protected internal virtual void insertHistoricDecisionInstance(HistoricDecisionInstanceEntity historicDecisionInstance)
	  {
		DbEntityManager.insert(historicDecisionInstance);

		insertHistoricDecisionInputInstances(historicDecisionInstance.Inputs, historicDecisionInstance.Id);
		insertHistoricDecisionOutputInstances(historicDecisionInstance.Outputs, historicDecisionInstance.Id);
	  }

	  protected internal virtual void insertHistoricDecisionInputInstances(IList<HistoricDecisionInputInstance> inputs, string decisionInstanceId)
	  {
		foreach (HistoricDecisionInputInstance input in inputs)
		{
		  HistoricDecisionInputInstanceEntity inputEntity = (HistoricDecisionInputInstanceEntity) input;
		  inputEntity.DecisionInstanceId = decisionInstanceId;

		  DbEntityManager.insert(inputEntity);
		}
	  }

	  protected internal virtual void insertHistoricDecisionOutputInstances(IList<HistoricDecisionOutputInstance> outputs, string decisionInstanceId)
	  {
		foreach (HistoricDecisionOutputInstance output in outputs)
		{
		  HistoricDecisionOutputInstanceEntity outputEntity = (HistoricDecisionOutputInstanceEntity) output;
		  outputEntity.DecisionInstanceId = decisionInstanceId;

		  DbEntityManager.insert(outputEntity);
		}
	  }

	 public virtual IList<HistoricDecisionInstance> findHistoricDecisionInstancesByQueryCriteria(HistoricDecisionInstanceQueryImpl query, Page page)
	 {
		if (HistoryEnabled)
		{
		  configureQuery(query);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<org.camunda.bpm.engine.history.HistoricDecisionInstance> decisionInstances = getDbEntityManager().selectList("selectHistoricDecisionInstancesByQueryCriteria", query, page);
		  IList<HistoricDecisionInstance> decisionInstances = DbEntityManager.selectList("selectHistoricDecisionInstancesByQueryCriteria", query, page);

		  enrichHistoricDecisionsWithInputsAndOutputs(query, decisionInstances);

		  return decisionInstances;
		}
		else
		{
		  return Collections.emptyList();
		}
	 }

	  public virtual void enrichHistoricDecisionsWithInputsAndOutputs(HistoricDecisionInstanceQueryImpl query, IList<HistoricDecisionInstance> decisionInstances)
	  {
		IDictionary<string, HistoricDecisionInstanceEntity> decisionInstancesById = new Dictionary<string, HistoricDecisionInstanceEntity>();
		foreach (HistoricDecisionInstance decisionInstance in decisionInstances)
		{
		  decisionInstancesById[decisionInstance.Id] = (HistoricDecisionInstanceEntity) decisionInstance;
		}

		if (decisionInstances.Count > 0 && query.IncludeInput)
		{
		  appendHistoricDecisionInputInstances(decisionInstancesById, query);
		}

		if (decisionInstances.Count > 0 && query.IncludeOutputs)
		{
		  appendHistoricDecisionOutputInstances(decisionInstancesById, query);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findHistoricDecisionInstanceIdsForCleanup(System.Nullable<int> batchSize, int minuteFrom, int minuteTo)
	  public virtual IList<string> findHistoricDecisionInstanceIdsForCleanup(int? batchSize, int minuteFrom, int minuteTo)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["currentTimestamp"] = ClockUtil.CurrentTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		ListQueryParameterObject parameterObject = new ListQueryParameterObject(parameters, 0, batchSize.Value);
		return (IList<string>) DbEntityManager.selectList("selectHistoricDecisionInstanceIdsForCleanup", parameterObject);
	  }

	  protected internal virtual void appendHistoricDecisionInputInstances(IDictionary<string, HistoricDecisionInstanceEntity> decisionInstancesById, HistoricDecisionInstanceQueryImpl query)
	  {
		IList<HistoricDecisionInputInstanceEntity> decisionInputInstances = findHistoricDecisionInputInstancesByDecisionInstanceIds(decisionInstancesById.Keys);
		initializeInputInstances(decisionInstancesById.Values);

		foreach (HistoricDecisionInputInstanceEntity decisionInputInstance in decisionInputInstances)
		{

		  HistoricDecisionInstanceEntity historicDecisionInstance = decisionInstancesById[decisionInputInstance.DecisionInstanceId];
		  historicDecisionInstance.addInput(decisionInputInstance);

		  // do not fetch values for byte arrays eagerly (unless requested by the user)
		  if (!isBinaryValue(decisionInputInstance) || query.ByteArrayFetchingEnabled)
		  {
			fetchVariableValue(decisionInputInstance, query.CustomObjectDeserializationEnabled);
		  }
		}
	  }

	  protected internal virtual void initializeInputInstances(ICollection<HistoricDecisionInstanceEntity> decisionInstances)
	  {
		foreach (HistoricDecisionInstanceEntity decisionInstance in decisionInstances)
		{
		  decisionInstance.Inputs = new List<HistoricDecisionInputInstance>();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.util.List<HistoricDecisionInputInstanceEntity> findHistoricDecisionInputInstancesByDecisionInstanceIds(java.util.Set<String> historicDecisionInstanceKeys)
	  protected internal virtual IList<HistoricDecisionInputInstanceEntity> findHistoricDecisionInputInstancesByDecisionInstanceIds(ISet<string> historicDecisionInstanceKeys)
	  {
		return DbEntityManager.selectList("selectHistoricDecisionInputInstancesByDecisionInstanceIds", historicDecisionInstanceKeys);
	  }

	  protected internal virtual bool isBinaryValue(HistoricDecisionInputInstance decisionInputInstance)
	  {
		return AbstractTypedValueSerializer.BINARY_VALUE_TYPES.Contains(decisionInputInstance.TypeName);
	  }

	  protected internal virtual void fetchVariableValue(HistoricDecisionInputInstanceEntity decisionInputInstance, bool isCustomObjectDeserializationEnabled)
	  {
		try
		{
		  decisionInputInstance.getTypedValue(isCustomObjectDeserializationEnabled);
		}
		catch (Exception t)
		{
		  // do not fail if one of the variables fails to load
		  LOG.failedTofetchVariableValue(t);
		}
	  }


	  protected internal virtual void appendHistoricDecisionOutputInstances(IDictionary<string, HistoricDecisionInstanceEntity> decisionInstancesById, HistoricDecisionInstanceQueryImpl query)
	  {
		IList<HistoricDecisionOutputInstanceEntity> decisionOutputInstances = findHistoricDecisionOutputInstancesByDecisionInstanceIds(decisionInstancesById.Keys);
		initializeOutputInstances(decisionInstancesById.Values);

		foreach (HistoricDecisionOutputInstanceEntity decisionOutputInstance in decisionOutputInstances)
		{

		  HistoricDecisionInstanceEntity historicDecisionInstance = decisionInstancesById[decisionOutputInstance.DecisionInstanceId];
		  historicDecisionInstance.addOutput(decisionOutputInstance);

		  // do not fetch values for byte arrays eagerly (unless requested by the user)
		  if (!isBinaryValue(decisionOutputInstance) || query.ByteArrayFetchingEnabled)
		  {
			fetchVariableValue(decisionOutputInstance, query.CustomObjectDeserializationEnabled);
		  }
		}
	  }

	  protected internal virtual void initializeOutputInstances(ICollection<HistoricDecisionInstanceEntity> decisionInstances)
	  {
		foreach (HistoricDecisionInstanceEntity decisionInstance in decisionInstances)
		{
		  decisionInstance.Outputs = new List<HistoricDecisionOutputInstance>();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.util.List<HistoricDecisionOutputInstanceEntity> findHistoricDecisionOutputInstancesByDecisionInstanceIds(java.util.Set<String> decisionInstanceKeys)
	  protected internal virtual IList<HistoricDecisionOutputInstanceEntity> findHistoricDecisionOutputInstancesByDecisionInstanceIds(ISet<string> decisionInstanceKeys)
	  {
		return DbEntityManager.selectList("selectHistoricDecisionOutputInstancesByDecisionInstanceIds", decisionInstanceKeys);
	  }

	  protected internal virtual bool isBinaryValue(HistoricDecisionOutputInstance decisionOutputInstance)
	  {
		return AbstractTypedValueSerializer.BINARY_VALUE_TYPES.Contains(decisionOutputInstance.TypeName);
	  }

	  protected internal virtual void fetchVariableValue(HistoricDecisionOutputInstanceEntity decisionOutputInstance, bool isCustomObjectDeserializationEnabled)
	  {
		try
		{
		  decisionOutputInstance.getTypedValue(isCustomObjectDeserializationEnabled);
		}
		catch (Exception t)
		{
		  // do not fail if one of the variables fails to load
		  LOG.failedTofetchVariableValue(t);
		}
	  }

	  public virtual HistoricDecisionInstanceEntity findHistoricDecisionInstance(string historicDecisionInstanceId)
	  {
		if (HistoryEnabled)
		{
		  return (HistoricDecisionInstanceEntity) DbEntityManager.selectOne("selectHistoricDecisionInstanceByDecisionInstanceId", configureParameterizedQuery(historicDecisionInstanceId));
		}
		return null;
	  }

	  public virtual long findHistoricDecisionInstanceCountByQueryCriteria(HistoricDecisionInstanceQueryImpl query)
	  {
		if (HistoryEnabled)
		{
		  configureQuery(query);
		  return (long?) DbEntityManager.selectOne("selectHistoricDecisionInstanceCountByQueryCriteria", query).Value;
		}
		else
		{
		  return 0;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricDecisionInstance> findHistoricDecisionInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricDecisionInstance> findHistoricDecisionInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectHistoricDecisionInstancesByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricDecisionInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectHistoricDecisionInstanceCountByNativeQuery", parameterMap).Value;
	  }

	  protected internal virtual void configureQuery(HistoricDecisionInstanceQueryImpl query)
	  {
		AuthorizationManager.configureHistoricDecisionInstanceQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult> findCleanableHistoricDecisionInstancesReportByCriteria(org.camunda.bpm.engine.impl.CleanableHistoricDecisionInstanceReportImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<CleanableHistoricDecisionInstanceReportResult> findCleanableHistoricDecisionInstancesReportByCriteria(CleanableHistoricDecisionInstanceReportImpl query, Page page)
	  {
		query.CurrentTimestamp = ClockUtil.CurrentTime;
		AuthorizationManager.configureQueryHistoricFinishedInstanceReport(query, Resources.DECISION_DEFINITION);
		TenantManager.configureQuery(query);
		return DbEntityManager.selectList("selectFinishedDecisionInstancesReportEntities", query, page);
	  }

	  public virtual long findCleanableHistoricDecisionInstancesReportCountByCriteria(CleanableHistoricDecisionInstanceReportImpl query)
	  {
		query.CurrentTimestamp = ClockUtil.CurrentTime;
		AuthorizationManager.configureQueryHistoricFinishedInstanceReport(query, Resources.DECISION_DEFINITION);
		TenantManager.configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectFinishedDecisionInstancesReportEntitiesCount", query).Value;
	  }

	  public virtual void addRemovalTimeToDecisionsByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionInstanceEntity), "updateHistoricDecisionInstancesByRootProcessInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionInputInstanceEntity), "updateHistoricDecisionInputInstancesByRootProcessInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionOutputInstanceEntity), "updateHistoricDecisionOutputInstancesByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToDecisionsByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionInstanceEntity), "updateHistoricDecisionInstancesByProcessInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionInputInstanceEntity), "updateHistoricDecisionInputInstancesByProcessInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionOutputInstanceEntity), "updateHistoricDecisionOutputInstancesByProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToDecisionsByRootDecisionInstanceId(string rootInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootDecisionInstanceId"] = rootInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionInstanceEntity), "updateHistoricDecisionInstancesByRootDecisionInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionInputInstanceEntity), "updateHistoricDecisionInputInstancesByRootDecisionInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionOutputInstanceEntity), "updateHistoricDecisionOutputInstancesByRootDecisionInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(ByteArrayEntity), "updateByteArraysByRootDecisionInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToDecisionsByDecisionInstanceId(string instanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["decisionInstanceId"] = instanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionInstanceEntity), "updateHistoricDecisionInstancesByDecisionInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionInputInstanceEntity), "updateHistoricDecisionInputInstancesByDecisionInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(HistoricDecisionOutputInstanceEntity), "updateHistoricDecisionOutputInstancesByDecisionInstanceId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(ByteArrayEntity), "updateByteArraysByDecisionInstanceId", parameters);
	  }

	  public virtual IDictionary<Type, DbOperation> deleteHistoricDecisionsByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		IDictionary<Type, DbOperation> deleteOperations = new Dictionary<Type, DbOperation>();

		DbOperation deleteDecisionInputInstances = DbEntityManager.deletePreserveOrder(typeof(HistoricDecisionInputInstanceEntity), "deleteHistoricDecisionInputInstancesByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));

		deleteOperations[typeof(HistoricDecisionInputInstanceEntity)] = deleteDecisionInputInstances;

		DbOperation deleteDecisionOutputInstances = DbEntityManager.deletePreserveOrder(typeof(HistoricDecisionOutputInstanceEntity), "deleteHistoricDecisionOutputInstancesByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));

		deleteOperations[typeof(HistoricDecisionOutputInstanceEntity)] = deleteDecisionOutputInstances;

		DbOperation deleteDecisionInstances = DbEntityManager.deletePreserveOrder(typeof(HistoricDecisionInstanceEntity), "deleteHistoricDecisionInstancesByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));

		deleteOperations[typeof(HistoricDecisionInstanceEntity)] = deleteDecisionInstances;

		return deleteOperations;
	  }

	}

}