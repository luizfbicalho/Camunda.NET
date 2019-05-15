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
namespace org.camunda.bpm.engine.externaltask
{

	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	public interface UpdateExternalTaskRetriesSelectBuilder
	{

	  /// <summary>
	  /// Selects a list of external tasks with the given list of ids.
	  /// </summary>
	  UpdateExternalTaskRetriesBuilder externalTaskIds(IList<string> externalTaskIds);

	  /// <summary>
	  /// Selects a list of external tasks with the given list of ids.
	  /// </summary>
	  UpdateExternalTaskRetriesBuilder externalTaskIds(params string[] externalTaskIds);

	  /// <summary>
	  /// Selects a list of external tasks with the given list of process instances ids.
	  /// </summary>
	  UpdateExternalTaskRetriesBuilder processInstanceIds(IList<string> processInstanceIds);

	  /// <summary>
	  /// Selects a list of external tasks with the given list of process instances ids.
	  /// </summary>
	  UpdateExternalTaskRetriesBuilder processInstanceIds(params string[] processInstanceIds);

	  /// <summary>
	  /// Selects a list of external tasks with the given external task query.
	  /// </summary>
	  UpdateExternalTaskRetriesBuilder externalTaskQuery(ExternalTaskQuery externalTaskQuery);

	  /// <summary>
	  /// Selects a list of external tasks with the given process instance query.
	  /// </summary>
	  UpdateExternalTaskRetriesBuilder processInstanceQuery(ProcessInstanceQuery processInstanceQuery);

	  /// <summary>
	  /// Selects a list of external tasks with the given historic process instance query.
	  /// </summary>
	  UpdateExternalTaskRetriesBuilder historicProcessInstanceQuery(HistoricProcessInstanceQuery historicProcessInstanceQuery);

	}

}