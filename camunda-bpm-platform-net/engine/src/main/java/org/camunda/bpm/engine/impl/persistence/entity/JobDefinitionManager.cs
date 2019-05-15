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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;

	/// <summary>
	/// <para>Manager implementation for <seealso cref="JobDefinitionEntity"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JobDefinitionManager : AbstractManager
	{

	  public virtual JobDefinitionEntity findById(string jobDefinitionId)
	  {
		return DbEntityManager.selectById(typeof(JobDefinitionEntity), jobDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<JobDefinitionEntity> findByProcessDefinitionId(String processDefinitionId)
	  public virtual IList<JobDefinitionEntity> findByProcessDefinitionId(string processDefinitionId)
	  {
		return DbEntityManager.selectList("selectJobDefinitionsByProcessDefinitionId", processDefinitionId);
	  }

	  public virtual void deleteJobDefinitionsByProcessDefinitionId(string id)
	  {
		DbEntityManager.delete(typeof(JobDefinitionEntity), "deleteJobDefinitionsByProcessDefinitionId", id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.management.JobDefinition> findJobDefnitionByQueryCriteria(org.camunda.bpm.engine.impl.JobDefinitionQueryImpl jobDefinitionQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<JobDefinition> findJobDefnitionByQueryCriteria(JobDefinitionQueryImpl jobDefinitionQuery, Page page)
	  {
		configureQuery(jobDefinitionQuery);
		return DbEntityManager.selectList("selectJobDefinitionByQueryCriteria", jobDefinitionQuery, page);
	  }

	  public virtual long findJobDefinitionCountByQueryCriteria(JobDefinitionQueryImpl jobDefinitionQuery)
	  {
		configureQuery(jobDefinitionQuery);
		return (long?) DbEntityManager.selectOne("selectJobDefinitionCountByQueryCriteria", jobDefinitionQuery).Value;
	  }

	  public virtual void updateJobDefinitionSuspensionStateById(string jobDefinitionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["jobDefinitionId"] = jobDefinitionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobDefinitionEntity), "updateJobDefinitionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateJobDefinitionSuspensionStateByProcessDefinitionId(string processDefinitionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobDefinitionEntity), "updateJobDefinitionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateJobDefinitionSuspensionStateByProcessDefinitionKey(string processDefinitionKey, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = false;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobDefinitionEntity), "updateJobDefinitionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateJobDefinitionSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = true;
		parameters["processDefinitionTenantId"] = processDefinitionTenantId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobDefinitionEntity), "updateJobDefinitionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  protected internal virtual void configureQuery(JobDefinitionQueryImpl query)
	  {
		AuthorizationManager.configureJobDefinitionQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	}

}