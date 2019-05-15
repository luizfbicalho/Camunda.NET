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

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public interface VariableInstanceQueryProperty
	{

	}

	public static class VariableInstanceQueryProperty_Fields
	{
	  public static readonly QueryProperty VARIABLE_NAME = new QueryPropertyImpl("NAME_");
	  public static readonly QueryProperty VARIABLE_TYPE = new QueryPropertyImpl("TYPE_");
	  public static readonly QueryProperty ACTIVITY_INSTANCE_ID = new QueryPropertyImpl("ACT_INST_ID_");
	  public static readonly QueryProperty EXECUTION_ID = new QueryPropertyImpl("EXECUTION_ID_");
	  public static readonly QueryProperty TASK_ID = new QueryPropertyImpl("TASK_ID_");
	  public static readonly QueryProperty CASE_EXECUTION_ID = new QueryPropertyImpl("CASE_EXECUTION_ID_");
	  public static readonly QueryProperty CASE_INSTANCE_ID = new QueryPropertyImpl("CASE_INST_ID_");
	  public static readonly QueryProperty TENANT_ID = new QueryPropertyImpl("TENANT_ID_");
	  public static readonly QueryProperty TEXT = new QueryPropertyImpl("TEXT_");
	  public static readonly QueryProperty TEXT_AS_LOWER = new QueryPropertyImpl("TEXT_", "LOWER");
	  public static readonly QueryProperty DOUBLE = new QueryPropertyImpl("DOUBLE_");
	  public static readonly QueryProperty LONG = new QueryPropertyImpl("LONG_");
	}

}