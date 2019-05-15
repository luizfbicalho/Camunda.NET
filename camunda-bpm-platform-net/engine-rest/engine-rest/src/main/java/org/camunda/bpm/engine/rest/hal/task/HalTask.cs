using System;

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
namespace org.camunda.bpm.engine.rest.hal.task
{

	using HalIdentityLink = org.camunda.bpm.engine.rest.hal.identitylink.HalIdentityLink;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HalTask : HalResource<HalTask>
	{

	  public static HalRelation REL_SELF = HalRelation.build("self", typeof(TaskRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_ASSIGNEE = HalRelation.build("assignee", typeof(UserRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.UserRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_OWNER = HalRelation.build("owner", typeof(UserRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.UserRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_EXECUTION = HalRelation.build("execution", typeof(ExecutionRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.ExecutionRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_PARENT_TASK = HalRelation.build("parentTask", typeof(TaskRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_PROCESS_DEFINITION = HalRelation.build("processDefinition", typeof(ProcessDefinitionRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.ProcessDefinitionRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_PROCESS_INSTANCE = HalRelation.build("processInstance", typeof(ProcessInstanceRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.ProcessInstanceRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_CASE_INSTANCE = HalRelation.build("caseInstance", typeof(CaseInstanceRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.CaseInstanceRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_CASE_EXECUTION = HalRelation.build("caseExecution", typeof(CaseExecutionRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.CaseExecutionRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_CASE_DEFINITION = HalRelation.build("caseDefinition", typeof(CaseDefinitionRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.CaseDefinitionRestService_Fields.PATH).path("{id}"));
	  public static HalRelation REL_IDENTITY_LINKS = HalRelation.build("identityLink", typeof(IdentityRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH).path("{taskId}").path("identity-links"));

	  private string id;
	  private string name;
	  private string assignee;
	  private DateTime created;
	  private DateTime due;
	  private DateTime followUp;
	  private DelegationState delegationState;
	  private string description;
	  private string executionId;
	  private string owner;
	  private string parentTaskId;
	  private int priority;
	  private string processDefinitionId;
	  private string processInstanceId;
	  private string taskDefinitionKey;
	  private string caseExecutionId;
	  private string caseInstanceId;
	  private string caseDefinitionId;
	  private bool suspended;
	  private string formKey;
	  private string tenantId;

	  public static HalTask generate(Task task, ProcessEngine engine)
	  {
		return fromTask(task).embed(HalTask.REL_PROCESS_DEFINITION, engine).embed(HalTask.REL_CASE_DEFINITION, engine).embed(HalTask.REL_IDENTITY_LINKS, engine).embed(HalIdentityLink.REL_USER, engine).embed(HalIdentityLink.REL_GROUP, engine);
	  }

	  public static HalTask fromTask(Task task)
	  {
		HalTask dto = new HalTask();

		// task state
		dto.id = task.Id;
		dto.name = task.Name;
		dto.assignee = task.Assignee;
		dto.created = task.CreateTime;
		dto.due = task.DueDate;
		dto.followUp = task.FollowUpDate;
		dto.delegationState = task.DelegationState;
		dto.description = task.Description;
		dto.executionId = task.ExecutionId;
		dto.owner = task.Owner;
		dto.parentTaskId = task.ParentTaskId;
		dto.priority = task.Priority;
		dto.processDefinitionId = task.ProcessDefinitionId;
		dto.processInstanceId = task.ProcessInstanceId;
		dto.taskDefinitionKey = task.TaskDefinitionKey;
		dto.caseDefinitionId = task.CaseDefinitionId;
		dto.caseExecutionId = task.CaseExecutionId;
		dto.caseInstanceId = task.CaseInstanceId;
		dto.suspended = task.Suspended;
		dto.tenantId = task.TenantId;
		try
		{
		  dto.formKey = task.FormKey;
		}
		catch (BadUserRequestException)
		{
		  // ignore (initializeFormKeys was not called)
		}

		// links
		dto.linker.createLink(REL_SELF, task.Id);
		dto.linker.createLink(REL_ASSIGNEE, task.Assignee);
		dto.linker.createLink(REL_OWNER, task.Owner);
		dto.linker.createLink(REL_EXECUTION,task.ExecutionId);
		dto.linker.createLink(REL_PARENT_TASK, task.ParentTaskId);
		dto.linker.createLink(REL_PROCESS_DEFINITION, task.ProcessDefinitionId);
		dto.linker.createLink(REL_PROCESS_INSTANCE, task.ProcessInstanceId);
		dto.linker.createLink(REL_CASE_INSTANCE, task.CaseInstanceId);
		dto.linker.createLink(REL_CASE_EXECUTION, task.CaseExecutionId);
		dto.linker.createLink(REL_CASE_DEFINITION, task.CaseDefinitionId);
		dto.linker.createLink(REL_IDENTITY_LINKS, task.Id);

		return dto;
	  }


	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Assignee
	  {
		  get
		  {
			return assignee;
		  }
	  }

	  public virtual DateTime Created
	  {
		  get
		  {
			return created;
		  }
	  }

	  public virtual DateTime Due
	  {
		  get
		  {
			return due;
		  }
	  }

	  public virtual DelegationState DelegationState
	  {
		  get
		  {
			return delegationState;
		  }
	  }

	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual string Owner
	  {
		  get
		  {
			return owner;
		  }
	  }

	  public virtual string ParentTaskId
	  {
		  get
		  {
			return parentTaskId;
		  }
	  }

	  public virtual int Priority
	  {
		  get
		  {
			return priority;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string TaskDefinitionKey
	  {
		  get
		  {
			return taskDefinitionKey;
		  }
	  }

	  public virtual DateTime FollowUp
	  {
		  get
		  {
			return followUp;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public virtual bool Suspended
	  {
		  get
		  {
			return suspended;
		  }
	  }

	  public virtual string FormKey
	  {
		  get
		  {
			return formKey;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	}

}