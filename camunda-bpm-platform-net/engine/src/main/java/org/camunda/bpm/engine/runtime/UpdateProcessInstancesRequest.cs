﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.runtime
{
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;

	public interface UpdateProcessInstancesRequest
	{

	  /// <summary>
	  /// Selects a list of process instances with the given list of ids.
	  /// </summary>
	  /// <param name="processInstanceIds">
	  ///          list of ids of the process instances </param>
	  /// <returns> the builder </returns>
	  UpdateProcessInstancesSuspensionStateBuilder byProcessInstanceIds(IList<string> processInstanceIds);

	  /// <summary>
	  /// Selects a list of process instances with the given list of ids.
	  /// </summary>
	  /// <param name="processInstanceIds">
	  ///          list of ids of the process instances </param>
	  /// <returns> the builder </returns>
	  UpdateProcessInstancesSuspensionStateBuilder byProcessInstanceIds(params string[] processInstanceIds);

	  /// <summary>
	  /// Selects a list of process instances with the given a process instance query.
	  /// </summary>
	  /// <param name="processInstanceQuery">
	  ///          process instance query that discribes a list of the process instances </param>
	  /// <returns> the builder </returns>
	  UpdateProcessInstancesSuspensionStateBuilder byProcessInstanceQuery(ProcessInstanceQuery processInstanceQuery);

	  /// <summary>
	  /// Selects a list of process instances with the given a historical process instance query.
	  /// </summary>
	  /// <param name="historicProcessInstanceQuery">
	  ///          historical process instance query that discribes a list of the process instances </param>
	  /// <returns> the builder </returns>
	  UpdateProcessInstancesSuspensionStateBuilder byHistoricProcessInstanceQuery(HistoricProcessInstanceQuery historicProcessInstanceQuery);

	}

}