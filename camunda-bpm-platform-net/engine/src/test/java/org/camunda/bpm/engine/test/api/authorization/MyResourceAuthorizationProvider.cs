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
namespace org.camunda.bpm.engine.test.api.authorization
{
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class MyResourceAuthorizationProvider : ResourceAuthorizationProvider
	{

	  // assignee
	  public static string OLD_ASSIGNEE;
	  public static string NEW_ASSIGNEE;

	  // owner
	  public static string OLD_OWNER;
	  public static string NEW_OWNER;

	  // add user identity link
	  public static string ADD_USER_IDENTITY_LINK_TYPE;
	  public static string ADD_USER_IDENTITY_LINK_USER;

	  // delete user identity link
	  public static string DELETE_USER_IDENTITY_LINK_TYPE = null;
	  public static string DELETE_USER_IDENTITY_LINK_USER = null;

	  // add group identity link
	  public static string ADD_GROUP_IDENTITY_LINK_TYPE;
	  public static string ADD_GROUP_IDENTITY_LINK_GROUP;

	  // delete group identity link
	  public static string DELETE_GROUP_IDENTITY_LINK_TYPE = null;
	  public static string DELETE_GROUP_IDENTITY_LINK_GROUP = null;

	  public virtual AuthorizationEntity[] newUser(User user)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newGroup(Group group)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newTenant(Tenant tenant)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] groupMembershipCreated(string groupId, string userId)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] tenantMembershipCreated(Tenant tenant, User user)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] tenantMembershipCreated(Tenant tenant, Group group)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newFilter(Filter filter)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newDeployment(Deployment deployment)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newProcessDefinition(ProcessDefinition processDefinition)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newProcessInstance(ProcessInstance processInstance)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newTask(Task task)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newTaskAssignee(Task task, string oldAssignee, string newAssignee)
	  {
		OLD_ASSIGNEE = oldAssignee;
		NEW_ASSIGNEE = newAssignee;
		return null;
	  }

	  public virtual AuthorizationEntity[] newTaskOwner(Task task, string oldOwner, string newOwner)
	  {
		OLD_OWNER = oldOwner;
		NEW_OWNER = newOwner;
		return null;
	  }

	  public virtual AuthorizationEntity[] newTaskUserIdentityLink(Task task, string userId, string type)
	  {
		ADD_USER_IDENTITY_LINK_TYPE = type;
		ADD_USER_IDENTITY_LINK_USER = userId;
		return null;
	  }

	  public virtual AuthorizationEntity[] newTaskGroupIdentityLink(Task task, string groupId, string type)
	  {
		ADD_GROUP_IDENTITY_LINK_TYPE = type;
		ADD_GROUP_IDENTITY_LINK_GROUP = groupId;
		return null;
	  }

	  public virtual AuthorizationEntity[] deleteTaskUserIdentityLink(Task task, string userId, string type)
	  {
		DELETE_USER_IDENTITY_LINK_TYPE = type;
		DELETE_USER_IDENTITY_LINK_USER = userId;
		return null;
	  }

	  public virtual AuthorizationEntity[] deleteTaskGroupIdentityLink(Task task, string groupId, string type)
	  {
		DELETE_GROUP_IDENTITY_LINK_TYPE = type;
		DELETE_GROUP_IDENTITY_LINK_GROUP = groupId;
		return null;
	  }

	  public static void clearProperties()
	  {
		OLD_ASSIGNEE = null;
		NEW_ASSIGNEE = null;
		OLD_OWNER = null;
		NEW_OWNER = null;
		ADD_USER_IDENTITY_LINK_TYPE = null;
		ADD_USER_IDENTITY_LINK_USER = null;
		ADD_GROUP_IDENTITY_LINK_TYPE = null;
		ADD_GROUP_IDENTITY_LINK_GROUP = null;
		DELETE_USER_IDENTITY_LINK_TYPE = null;
		DELETE_USER_IDENTITY_LINK_USER = null;
		DELETE_GROUP_IDENTITY_LINK_TYPE = null;
		DELETE_GROUP_IDENTITY_LINK_GROUP = null;
	  }

	  public virtual AuthorizationEntity[] newDecisionDefinition(DecisionDefinition decisionDefinition)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] newDecisionRequirementsDefinition(DecisionRequirementsDefinition decisionRequirementsDefinition)
	  {
		return null;
	  }

	}

}