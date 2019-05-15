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

	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class VariableInstanceManager : AbstractManager
	{

	  public virtual IList<VariableInstanceEntity> findVariableInstancesByTaskId(string taskId)
	  {
		return findVariableInstancesByTaskIdAndVariableNames(taskId, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByTaskIdAndVariableNames(String taskId, java.util.Collection<String> variableNames)
	  public virtual IList<VariableInstanceEntity> findVariableInstancesByTaskIdAndVariableNames(string taskId, ICollection<string> variableNames)
	  {
		IDictionary<string, object> parameter = new Dictionary<string, object>();
		parameter["taskId"] = taskId;
		parameter["variableNames"] = variableNames;
		return DbEntityManager.selectList("selectVariablesByTaskId", parameter);
	  }

	  public virtual IList<VariableInstanceEntity> findVariableInstancesByExecutionId(string executionId)
	  {
		return findVariableInstancesByExecutionIdAndVariableNames(executionId, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByExecutionIdAndVariableNames(String executionId, java.util.Collection<String> variableNames)
	  public virtual IList<VariableInstanceEntity> findVariableInstancesByExecutionIdAndVariableNames(string executionId, ICollection<string> variableNames)
	  {
		IDictionary<string, object> parameter = new Dictionary<string, object>();
		parameter["executionId"] = executionId;
		parameter["variableNames"] = variableNames;
		return DbEntityManager.selectList("selectVariablesByExecutionId", parameter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByProcessInstanceId(String processInstanceId)
	  public virtual IList<VariableInstanceEntity> findVariableInstancesByProcessInstanceId(string processInstanceId)
	  {
		return DbEntityManager.selectList("selectVariablesByProcessInstanceId", processInstanceId);
	  }

	  public virtual IList<VariableInstanceEntity> findVariableInstancesByCaseExecutionId(string caseExecutionId)
	  {
		return findVariableInstancesByCaseExecutionIdAndVariableNames(caseExecutionId, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByCaseExecutionIdAndVariableNames(String caseExecutionId, java.util.Collection<String> variableNames)
	  public virtual IList<VariableInstanceEntity> findVariableInstancesByCaseExecutionIdAndVariableNames(string caseExecutionId, ICollection<string> variableNames)
	  {
		IDictionary<string, object> parameter = new Dictionary<string, object>();
		parameter["caseExecutionId"] = caseExecutionId;
		parameter["variableNames"] = variableNames;
		return DbEntityManager.selectList("selectVariablesByCaseExecutionId", parameter);
	  }

	  public virtual void deleteVariableInstanceByTask(TaskEntity task)
	  {
		IList<VariableInstanceEntity> variableInstances = task.variableStore.Variables;
		foreach (VariableInstanceEntity variableInstance in variableInstances)
		{
		  variableInstance.delete();
		}
	  }

	  public virtual long findVariableInstanceCountByQueryCriteria(VariableInstanceQueryImpl variableInstanceQuery)
	  {
		configureQuery(variableInstanceQuery);
		return (long?) DbEntityManager.selectOne("selectVariableInstanceCountByQueryCriteria", variableInstanceQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.VariableInstance> findVariableInstanceByQueryCriteria(org.camunda.bpm.engine.impl.VariableInstanceQueryImpl variableInstanceQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<VariableInstance> findVariableInstanceByQueryCriteria(VariableInstanceQueryImpl variableInstanceQuery, Page page)
	  {
		configureQuery(variableInstanceQuery);
		return DbEntityManager.selectList("selectVariableInstanceByQueryCriteria", variableInstanceQuery, page);
	  }

	  protected internal virtual void configureQuery(VariableInstanceQueryImpl query)
	  {
		AuthorizationManager.configureVariableInstanceQuery(query);
		TenantManager.configureQuery(query);
	  }

	}

}