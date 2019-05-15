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

	using Incident = org.camunda.bpm.engine.runtime.Incident;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class IncidentManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IncidentEntity> findIncidentsByExecution(String id)
	  public virtual IList<IncidentEntity> findIncidentsByExecution(string id)
	  {
		return DbEntityManager.selectList("selectIncidentsByExecutionId", id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IncidentEntity> findIncidentsByProcessInstance(String id)
	  public virtual IList<IncidentEntity> findIncidentsByProcessInstance(string id)
	  {
		return DbEntityManager.selectList("selectIncidentsByProcessInstanceId", id);
	  }

	  public virtual long findIncidentCountByQueryCriteria(IncidentQueryImpl incidentQuery)
	  {
		configureQuery(incidentQuery);
		return (long?) DbEntityManager.selectOne("selectIncidentCountByQueryCriteria", incidentQuery).Value;
	  }

	  public virtual Incident findIncidentById(string id)
	  {
		return (Incident) DbEntityManager.selectById(typeof(IncidentEntity), id);
	  }

	  public virtual IList<Incident> findIncidentByConfiguration(string configuration)
	  {
		return findIncidentByConfigurationAndIncidentType(configuration, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.Incident> findIncidentByConfigurationAndIncidentType(String configuration, String incidentType)
	  public virtual IList<Incident> findIncidentByConfigurationAndIncidentType(string configuration, string incidentType)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["configuration"] = configuration;
		@params["incidentType"] = incidentType;
		return DbEntityManager.selectList("selectIncidentsByConfiguration", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.Incident> findIncidentByQueryCriteria(org.camunda.bpm.engine.impl.IncidentQueryImpl incidentQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<Incident> findIncidentByQueryCriteria(IncidentQueryImpl incidentQuery, Page page)
	  {
		configureQuery(incidentQuery);
		return DbEntityManager.selectList("selectIncidentByQueryCriteria", incidentQuery, page);
	  }

	  protected internal virtual void configureQuery(IncidentQueryImpl query)
	  {
		AuthorizationManager.configureIncidentQuery(query);
		TenantManager.configureQuery(query);
	  }

	}

}