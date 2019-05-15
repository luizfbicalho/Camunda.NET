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
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;

	/// <summary>
	/// @author Stefan Hentschel
	/// 
	/// </summary>
	public class TaskReportManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult> createTaskCountByCandidateGroupReport(org.camunda.bpm.engine.impl.TaskReportImpl query)
	  public virtual IList<TaskCountByCandidateGroupResult> createTaskCountByCandidateGroupReport(TaskReportImpl query)
	  {
		configureQuery(query);
		return DbEntityManager.selectListWithRawParameter("selectTaskCountByCandidateGroupReportQuery", query, 0, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult> selectHistoricTaskInstanceCountByTaskNameReport(org.camunda.bpm.engine.impl.HistoricTaskInstanceReportImpl query)
	  public virtual IList<HistoricTaskInstanceReportResult> selectHistoricTaskInstanceCountByTaskNameReport(HistoricTaskInstanceReportImpl query)
	  {
		configureQuery(query);
		return DbEntityManager.selectListWithRawParameter("selectHistoricTaskInstanceCountByTaskNameReport", query, 0, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult> selectHistoricTaskInstanceCountByProcDefKeyReport(org.camunda.bpm.engine.impl.HistoricTaskInstanceReportImpl query)
	  public virtual IList<HistoricTaskInstanceReportResult> selectHistoricTaskInstanceCountByProcDefKeyReport(HistoricTaskInstanceReportImpl query)
	  {
		configureQuery(query);
		return DbEntityManager.selectListWithRawParameter("selectHistoricTaskInstanceCountByProcDefKeyReport", query, 0, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.DurationReportResult> createHistoricTaskDurationReport(org.camunda.bpm.engine.impl.HistoricTaskInstanceReportImpl query)
	  public virtual IList<DurationReportResult> createHistoricTaskDurationReport(HistoricTaskInstanceReportImpl query)
	  {
		configureQuery(query);
		return DbEntityManager.selectListWithRawParameter("selectHistoricTaskInstanceDurationReport", query, 0, int.MaxValue);
	  }

	  protected internal virtual void configureQuery(HistoricTaskInstanceReportImpl parameter)
	  {
		AuthorizationManager.checkAuthorization(Permissions.READ_HISTORY, Resources.TASK, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY);
		TenantManager.configureTenantCheck(parameter.TenantCheck);
	  }

	  protected internal virtual void configureQuery(TaskReportImpl parameter)
	  {
		AuthorizationManager.checkAuthorization(Permissions.READ, Resources.TASK, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY);
		TenantManager.configureTenantCheck(parameter.TenantCheck);
	  }

	}

}