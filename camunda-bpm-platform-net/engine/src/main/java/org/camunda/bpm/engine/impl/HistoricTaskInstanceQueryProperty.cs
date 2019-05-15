﻿/*
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
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public interface HistoricTaskInstanceQueryProperty
	{

	}

	public static class HistoricTaskInstanceQueryProperty_Fields
	{
	  public static readonly QueryProperty HISTORIC_TASK_INSTANCE_ID = new QueryPropertyImpl("ID_");
	  public static readonly QueryProperty PROCESS_DEFINITION_ID = new QueryPropertyImpl("PROC_DEF_ID_");
	  public static readonly QueryProperty PROCESS_INSTANCE_ID = new QueryPropertyImpl("PROC_INST_ID_");
	  public static readonly QueryProperty EXECUTION_ID = new QueryPropertyImpl("EXECUTION_ID_");
	  public static readonly QueryProperty ACTIVITY_INSTANCE_ID = new QueryPropertyImpl("ACT_INST_ID_");
	  public static readonly QueryProperty TASK_NAME = new QueryPropertyImpl("NAME_");
	  public static readonly QueryProperty TASK_DESCRIPTION = new QueryPropertyImpl("DESCRIPTION_");
	  public static readonly QueryProperty TASK_ASSIGNEE = new QueryPropertyImpl("ASSIGNEE_");
	  public static readonly QueryProperty TASK_OWNER = new QueryPropertyImpl("OWNER_");
	  public static readonly QueryProperty TASK_DEFINITION_KEY = new QueryPropertyImpl("TASK_DEF_KEY_");
	  public static readonly QueryProperty DELETE_REASON = new QueryPropertyImpl("DELETE_REASON_");
	  public static readonly QueryProperty START = new QueryPropertyImpl("START_TIME_");
	  public static readonly QueryProperty END = new QueryPropertyImpl("END_TIME_");
	  public static readonly QueryProperty DURATION = new QueryPropertyImpl("DURATION_");
	  public static readonly QueryProperty TASK_PRIORITY = new QueryPropertyImpl("PRIORITY_");
	  public static readonly QueryProperty TASK_DUE_DATE = new QueryPropertyImpl("DUE_DATE_");
	  public static readonly QueryProperty TASK_FOLLOW_UP_DATE = new QueryPropertyImpl("FOLLOW_UP_DATE_");
	  public static readonly QueryProperty CASE_DEFINITION_ID = new QueryPropertyImpl("CASE_DEFINITION_ID_");
	  public static readonly QueryProperty CASE_INSTANCE_ID = new QueryPropertyImpl("CASE_INSTANCE_ID_");
	  public static readonly QueryProperty CASE_EXECUTION_ID = new QueryPropertyImpl("CASE_EXECUTION_ID_");
	  public static readonly QueryProperty TENANT_ID = new QueryPropertyImpl("TENANT_ID_");
	}

}