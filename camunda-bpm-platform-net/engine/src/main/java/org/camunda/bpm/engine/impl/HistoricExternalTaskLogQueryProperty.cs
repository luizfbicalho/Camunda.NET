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
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;

	public interface HistoricExternalTaskLogQueryProperty
	{

	}

	public static class HistoricExternalTaskLogQueryProperty_Fields
	{
	  public static readonly QueryProperty EXTERNAL_TASK_ID = new QueryPropertyImpl("EXT_TASK_ID_");
	  public static readonly QueryProperty TIMESTAMP = new QueryPropertyImpl("TIMESTAMP_");
	  public static readonly QueryProperty TOPIC_NAME = new QueryPropertyImpl("TOPIC_NAME_");
	  public static readonly QueryProperty WORKER_ID = new QueryPropertyImpl("WORKER_ID_");
	  public static readonly QueryProperty ACTIVITY_ID = new QueryPropertyImpl("ACT_ID_");
	  public static readonly QueryProperty ACTIVITY_INSTANCE_ID = new QueryPropertyImpl("ACT_INST_ID_");
	  public static readonly QueryProperty EXECUTION_ID = new QueryPropertyImpl("EXECUTION_ID_");
	  public static readonly QueryProperty PROCESS_INSTANCE_ID = new QueryPropertyImpl("PROC_INST_ID_");
	  public static readonly QueryProperty PROCESS_DEFINITION_ID = new QueryPropertyImpl("PROC_DEF_ID_");
	  public static readonly QueryProperty PROCESS_DEFINITION_KEY = new QueryPropertyImpl("PROC_DEF_KEY_");
	  public static readonly QueryProperty RETRIES = new QueryPropertyImpl("RETRIES_");
	  public static readonly QueryProperty PRIORITY = new QueryPropertyImpl("PRIORITY_");
	  public static readonly QueryProperty TENANT_ID = new QueryPropertyImpl("TENANT_ID_");
	}

}