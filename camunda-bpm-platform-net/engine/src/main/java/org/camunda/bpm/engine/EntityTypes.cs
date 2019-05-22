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
namespace org.camunda.bpm.engine
{
	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class EntityTypes
	{

	  public const string APPLICATION = "Application";
	  public const string ATTACHMENT = "Attachment";
	  public const string AUTHORIZATION = "Authorization";
	  public const string FILTER = "Filter";
	  public const string GROUP = "Group";
	  public const string GROUP_MEMBERSHIP = "Group membership";
	  public const string IDENTITY_LINK = "IdentityLink";
	  public const string TASK = "Task";
	  public const string USER = "User";
	  public const string PROCESS_INSTANCE = "ProcessInstance";
	  public const string PROCESS_DEFINITION = "ProcessDefinition";
	  public const string JOB = "Job";
	  public const string JOB_DEFINITION = "JobDefinition";
	  public const string VARIABLE = "Variable";
	  public const string DEPLOYMENT = "Deployment";
	  public const string DECISION_DEFINITION = "DecisionDefinition";
	  public const string CASE_DEFINITION = "CaseDefinition";
	  public const string EXTERNAL_TASK = "ExternalTask";
	  public const string TENANT = "Tenant";
	  public const string TENANT_MEMBERSHIP = "TenantMembership";
	  public const string BATCH = "Batch";
	  public const string DECISION_REQUIREMENTS_DEFINITION = "DecisionRequirementsDefinition";
	  public const string DECISION_INSTANCE = "DecisionInstance";
	  public const string REPORT = "Report";
	  public const string DASHBOARD = "Dashboard";
	  public const string METRICS = "Metrics";
	  public const string CASE_INSTANCE = "CaseInstance";
	  public const string PROPERTY = "Property";
	  public const string OPERATION_LOG_CATEGORY = "OperationLogCatgeory";
	}

}