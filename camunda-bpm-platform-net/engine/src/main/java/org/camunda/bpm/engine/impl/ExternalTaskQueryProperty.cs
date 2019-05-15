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
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface ExternalTaskQueryProperty
	{

	}

	public static class ExternalTaskQueryProperty_Fields
	{
	  public static readonly QueryProperty ID = new QueryPropertyImpl("ID_");
	  public static readonly QueryProperty LOCK_EXPIRATION_TIME = new QueryPropertyImpl("LOCK_EXP_TIME_");
	  public static readonly QueryProperty PROCESS_INSTANCE_ID = new QueryPropertyImpl("PROC_INST_ID_");
	  public static readonly QueryProperty PROCESS_DEFINITION_ID = new QueryPropertyImpl("PROC_DEF_ID_");
	  public static readonly QueryProperty PROCESS_DEFINITION_KEY = new QueryPropertyImpl("PROC_DEF_KEY_");
	  public static readonly QueryProperty TENANT_ID = new QueryPropertyImpl("TENANT_ID_");
	  public static readonly QueryProperty PRIORITY = new QueryPropertyImpl("PRIORITY_");
	}

}