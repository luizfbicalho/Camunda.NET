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
namespace org.camunda.bpm.engine.impl.cfg.auth
{
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// <para>Manages (create/update/delete) default authorization when an entity is
	/// changed</para>
	/// 
	/// <para>Implementations should throw an exception when a specific resource's id is <code>*</code>, as
	/// <code>*</code> represents access to all resources/by all users.</para>
	/// 
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public interface ResourceAuthorizationProvider
	{

	  // Users /////////////////////////////////////////////

	  /// <summary>
	  /// <para>Invoked whenever a new user is created</para>
	  /// </summary>
	  /// <param name="user">
	  ///          a newly created user </param>
	  /// <returns> a list of authorizations to be automatically added when a new user
	  ///         is created. </returns>
	  AuthorizationEntity[] newUser(User user);

	  /// <summary>
	  /// <para>Invoked whenever a new group is created</para>
	  /// </summary>
	  /// <param name="group">
	  ///          a newly created <seealso cref="Group"/> </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="Group"/> is created. </returns>
	  AuthorizationEntity[] newGroup(Group group);

	  /// <summary>
	  /// <para>
	  /// Invoked whenever a new tenant is created
	  /// </para>
	  /// </summary>
	  /// <param name="tenant">
	  ///          a newly created <seealso cref="Tenant"/> </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="Tenant"/> is created. </returns>
	  AuthorizationEntity[] newTenant(Tenant tenant);

	  /// <summary>
	  /// <para>Invoked whenever a user is added to a group</para>
	  /// </summary>
	  /// <param name="userId">
	  ///          the id of the user who is added to a group a newly created
	  ///          <seealso cref="User"/> </param>
	  /// <param name="groupId">
	  ///          the id of the group to which the user is added </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="User"/> is created. </returns>
	  AuthorizationEntity[] groupMembershipCreated(string groupId, string userId);

	  /// <summary>
	  /// <para>Invoked whenever an user is added to a tenant.</para>
	  /// </summary>
	  /// <param name="tenant">
	  ///          the id of the tenant </param>
	  /// <param name="userId">
	  ///          the id of the user </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         membership is created. </returns>
	  AuthorizationEntity[] tenantMembershipCreated(Tenant tenant, User user);

	  /// <summary>
	  /// <para>Invoked whenever a group is added to a tenant.</para>
	  /// </summary>
	  /// <param name="tenant">
	  ///          the id of the tenant </param>
	  /// <param name="groupId">
	  ///          the id of the group </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         membership is created. </returns>
	  AuthorizationEntity[] tenantMembershipCreated(Tenant tenant, Group group);

	  // Filter ////////////////////////////////////////////////

	  /// <summary>
	  /// <para>Invoked whenever a new filter is created</para>
	  /// </summary>
	  /// <param name="filter"> the newly created filter </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="Filter"/> is created. </returns>
	  AuthorizationEntity[] newFilter(Filter filter);

	  // Deployment //////////////////////////////////////////////

	  /// <summary>
	  /// <para>Invoked whenever a new deployment is created</para>
	  /// </summary>
	  /// <param name="deployment"> the newly created deployment </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="Deployment"/> is created. </returns>
	  AuthorizationEntity[] newDeployment(Deployment deployment);

	  // Process Definition //////////////////////////////////////

	  /// <summary>
	  /// <para>Invoked whenever a new process definition is created</para>
	  /// </summary>
	  /// <param name="processDefinition"> the newly created process definition </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="ProcessDefinition"/> is created. </returns>
	  AuthorizationEntity[] newProcessDefinition(ProcessDefinition processDefinition);

	  // Process Instance ///////////////////////////////////////

	  /// <summary>
	  /// <para>Invoked whenever a new process instance is started</para>
	  /// </summary>
	  /// <param name="processInstance"> the newly started process instance </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="ProcessInstance"/> is started. </returns>
	  AuthorizationEntity[] newProcessInstance(ProcessInstance processInstance);

	  // Task /////////////////////////////////////////////////

	  /// <summary>
	  /// <para>Invoked whenever a new task is created</para>
	  /// </summary>
	  /// <param name="task"> the newly created task </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="Task"/> is created. </returns>
	  AuthorizationEntity[] newTask(Task task);

	  /// <summary>
	  /// <para>Invoked whenever an user has been assigned to a task.</para>
	  /// </summary>
	  /// <param name="task"> the task on which the assignee has been changed </param>
	  /// <param name="oldAssignee"> the old assignee of the task </param>
	  /// <param name="newAssignee"> the new assignee of the task
	  /// </param>
	  /// <returns> a list of authorizations to be automatically added when an
	  ///          assignee of a task changes. </returns>
	  AuthorizationEntity[] newTaskAssignee(Task task, string oldAssignee, string newAssignee);

	  /// <summary>
	  /// <para>Invoked whenever an user has been set as the owner of a task.</para>
	  /// </summary>
	  /// <param name="task"> the task on which the owner has been changed </param>
	  /// <param name="oldOwner"> the old owner of the task </param>
	  /// <param name="newOwner"> the new owner of the task
	  /// </param>
	  /// <returns> a list of authorizations to be automatically added when the
	  ///          owner of a task changes. </returns>
	  AuthorizationEntity[] newTaskOwner(Task task, string oldOwner, string newOwner);

	  /// <summary>
	  /// <para>Invoked whenever a new user identity link has been added to a task.</para>
	  /// </summary>
	  /// <param name="task"> the task on which a new identity link has been added </param>
	  /// <param name="userId"> the user for which the identity link has been created </param>
	  /// <param name="type"> the type of the identity link (e.g. <seealso cref="IdentityLinkType.CANDIDATE"/>)
	  /// </param>
	  /// <returns> a list of authorizations to be automatically added when
	  ///          a new user identity link has been added. </returns>
	  AuthorizationEntity[] newTaskUserIdentityLink(Task task, string userId, string type);

	  /// <summary>
	  /// <para>Invoked whenever a new group identity link has been added to a task.</para>
	  /// </summary>
	  /// <param name="task"> the task on which a new identity link has been added </param>
	  /// <param name="groupId"> the group for which the identity link has been created </param>
	  /// <param name="type"> the type of the identity link (e.g. <seealso cref="IdentityLinkType.CANDIDATE"/>)
	  /// </param>
	  /// <returns> a list of authorizations to be automatically added when
	  ///          a new group identity link has been added. </returns>
	  AuthorizationEntity[] newTaskGroupIdentityLink(Task task, string groupId, string type);

	  /// <summary>
	  /// <para>Invoked whenever a user identity link of a task has been deleted.</para>
	  /// </summary>
	  /// <param name="task"> the task on which the identity link has been deleted </param>
	  /// <param name="userId"> the user for which the identity link has been deleted </param>
	  /// <param name="type"> the type of the identity link (e.g. <seealso cref="IdentityLinkType.CANDIDATE"/>)
	  /// </param>
	  /// <returns> a list of authorizations to be automatically deleted when
	  ///          a user identity link has been deleted. </returns>
	  AuthorizationEntity[] deleteTaskUserIdentityLink(Task task, string userId, string type);

	  /// <summary>
	  /// <para>Invoked whenever a group identity link of a task has been deleted.</para>
	  /// </summary>
	  /// <param name="task"> the task on which the identity link has been deleted </param>
	  /// <param name="groupId"> the group for which the identity link has been deleted </param>
	  /// <param name="type"> the type of the identity link (e.g. <seealso cref="IdentityLinkType.CANDIDATE"/>)
	  /// </param>
	  /// <returns> a list of authorizations to be automatically deleted when
	  ///          a group identity link has been deleted. </returns>
	  AuthorizationEntity[] deleteTaskGroupIdentityLink(Task task, string groupId, string type);

	  /// <summary>
	  /// <para>Invoked whenever a new decision definition is created.</para>
	  /// </summary>
	  /// <param name="decisionDefinition"> the newly created decision definition </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="DecisionDefinition"/> is created. </returns>
	  AuthorizationEntity[] newDecisionDefinition(DecisionDefinition decisionDefinition);

	  /// <summary>
	  /// <para>Invoked whenever a new decision requirements definition is created.</para>
	  /// </summary>
	  /// <param name="decisionRequirementsDefinition"> the newly created decision requirements definition </param>
	  /// <returns> a list of authorizations to be automatically added when a new
	  ///         <seealso cref="DecisionRequirementsDefinition"/> is created. </returns>
	  AuthorizationEntity[] newDecisionRequirementsDefinition(DecisionRequirementsDefinition decisionRequirementsDefinition);

	}

}