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
namespace org.camunda.bpm.engine.test.api.runtime.migration.models
{
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class ConditionalModels
	{


	  public const string CONDITIONAL_PROCESS_KEY = "processKey";
	  public const string SUB_PROCESS_ID = "subProcess";
	  public const string BOUNDARY_ID = "boundaryId";
	  public const string PROC_DEF_KEY = "Process";
	  public const string VARIABLE_NAME = "variable";
	  public const string CONDITION_ID = "conditionCatch";
	  public const string VAR_CONDITION = "${variable == 1}";
	  public const string USER_TASK_ID = "userTask";
	}

}