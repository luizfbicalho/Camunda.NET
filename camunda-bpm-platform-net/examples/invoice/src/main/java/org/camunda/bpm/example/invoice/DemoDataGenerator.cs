using System.Collections.Generic;

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
namespace org.camunda.bpm.example.invoice
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ACCESS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.APPLICATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.FILTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;


	using AuthorizationService = org.camunda.bpm.engine.AuthorizationService;
	using FilterService = org.camunda.bpm.engine.FilterService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using IdentityServiceImpl = org.camunda.bpm.engine.impl.IdentityServiceImpl;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// Creates demo credentials to be used in the invoice showcase.
	/// 
	/// @author drobisch
	/// </summary>
	public class DemoDataGenerator
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private static readonly Logger LOGGER = Logger.getLogger(typeof(DemoDataGenerator).FullName);

		public virtual void createUsers(ProcessEngine engine)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.IdentityServiceImpl identityService = (org.camunda.bpm.engine.impl.IdentityServiceImpl) engine.getIdentityService();
		  IdentityServiceImpl identityService = (IdentityServiceImpl) engine.IdentityService;

		  if (identityService.ReadOnly)
		  {
			LOGGER.info("Identity service provider is Read Only, not creating any demo users.");
			return;
		  }

		  User singleResult = identityService.createUserQuery().userId("demo").singleResult();
		  if (singleResult != null)
		  {
			return;
		  }

		  LOGGER.info("Generating demo data for invoice showcase");

		  User user = identityService.newUser("demo");
		  user.FirstName = "Demo";
		  user.LastName = "Demo";
		  user.Password = "demo";
		  user.Email = "demo@camunda.org";
		  identityService.saveUser(user, true);

		  User user2 = identityService.newUser("john");
		  user2.FirstName = "John";
		  user2.LastName = "Doe";
		  user2.Password = "john";
		  user2.Email = "john@camunda.org";
		  identityService.saveUser(user2, true);

		  User user3 = identityService.newUser("mary");
		  user3.FirstName = "Mary";
		  user3.LastName = "Anne";
		  user3.Password = "mary";
		  user3.Email = "mary@camunda.org";
		  identityService.saveUser(user3, true);

		  User user4 = identityService.newUser("peter");
		  user4.FirstName = "Peter";
		  user4.LastName = "Meter";
		  user4.Password = "peter";
		  user4.Email = "peter@camunda.org";
		  identityService.saveUser(user4, true);

		  Group salesGroup = identityService.newGroup("sales");
		  salesGroup.Name = "Sales";
		  salesGroup.Type = "WORKFLOW";
		  identityService.saveGroup(salesGroup);

		  Group accountingGroup = identityService.newGroup("accounting");
		  accountingGroup.Name = "Accounting";
		  accountingGroup.Type = "WORKFLOW";
		  identityService.saveGroup(accountingGroup);

		  Group managementGroup = identityService.newGroup("management");
		  managementGroup.Name = "Management";
		  managementGroup.Type = "WORKFLOW";
		  identityService.saveGroup(managementGroup);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.AuthorizationService authorizationService = engine.getAuthorizationService();
		  AuthorizationService authorizationService = engine.AuthorizationService;

		  // create group
		  if (identityService.createGroupQuery().groupId(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN).count() == 0)
		  {
			Group camundaAdminGroup = identityService.newGroup(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN);
			camundaAdminGroup.Name = "camunda BPM Administrators";
			camundaAdminGroup.Type = org.camunda.bpm.engine.authorization.Groups_Fields.GROUP_TYPE_SYSTEM;
			identityService.saveGroup(camundaAdminGroup);
		  }

		  // create ADMIN authorizations on all built-in resources
		  foreach (Resource resource in Resources.values())
		  {
			if (authorizationService.createAuthorizationQuery().groupIdIn(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN).resourceType(resource).resourceId(ANY).count() == 0)
			{
			  AuthorizationEntity userAdminAuth = new AuthorizationEntity(AUTH_TYPE_GRANT);
			  userAdminAuth.GroupId = org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN;
			  userAdminAuth.setResource(resource);
			  userAdminAuth.ResourceId = ANY;
			  userAdminAuth.addPermission(ALL);
			  authorizationService.saveAuthorization(userAdminAuth);
			}
		  }

		  identityService.createMembership("demo", "sales");
		  identityService.createMembership("demo", "accounting");
		  identityService.createMembership("demo", "management");
		  identityService.createMembership("demo", "camunda-admin");

		  identityService.createMembership("john", "sales");
		  identityService.createMembership("mary", "accounting");
		  identityService.createMembership("peter", "management");


		  // authorize groups for tasklist only:

		  Authorization salesTasklistAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  salesTasklistAuth.GroupId = "sales";
		  salesTasklistAuth.addPermission(ACCESS);
		  salesTasklistAuth.ResourceId = "tasklist";
		  salesTasklistAuth.Resource = APPLICATION;
		  authorizationService.saveAuthorization(salesTasklistAuth);

		  Authorization salesReadProcessDefinition = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  salesReadProcessDefinition.GroupId = "sales";
		  salesReadProcessDefinition.addPermission(Permissions.READ);
		  salesReadProcessDefinition.addPermission(Permissions.READ_HISTORY);
		  salesReadProcessDefinition.Resource = Resources.PROCESS_DEFINITION;
		  // restrict to invoice process definition only
		  salesReadProcessDefinition.ResourceId = "invoice";
		  authorizationService.saveAuthorization(salesReadProcessDefinition);

		  Authorization accountingTasklistAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  accountingTasklistAuth.GroupId = "accounting";
		  accountingTasklistAuth.addPermission(ACCESS);
		  accountingTasklistAuth.ResourceId = "tasklist";
		  accountingTasklistAuth.Resource = APPLICATION;
		  authorizationService.saveAuthorization(accountingTasklistAuth);

		  Authorization accountingReadProcessDefinition = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  accountingReadProcessDefinition.GroupId = "accounting";
		  accountingReadProcessDefinition.addPermission(Permissions.READ);
		  accountingReadProcessDefinition.addPermission(Permissions.READ_HISTORY);
		  accountingReadProcessDefinition.Resource = Resources.PROCESS_DEFINITION;
		  // restrict to invoice process definition only
		  accountingReadProcessDefinition.ResourceId = "invoice";
		  authorizationService.saveAuthorization(accountingReadProcessDefinition);

		  Authorization managementTasklistAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  managementTasklistAuth.GroupId = "management";
		  managementTasklistAuth.addPermission(ACCESS);
		  managementTasklistAuth.ResourceId = "tasklist";
		  managementTasklistAuth.Resource = APPLICATION;
		  authorizationService.saveAuthorization(managementTasklistAuth);

		  Authorization managementReadProcessDefinition = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  managementReadProcessDefinition.GroupId = "management";
		  managementReadProcessDefinition.addPermission(Permissions.READ);
		  managementReadProcessDefinition.addPermission(Permissions.READ_HISTORY);
		  managementReadProcessDefinition.Resource = Resources.PROCESS_DEFINITION;
		  // restrict to invoice process definition only
		  managementReadProcessDefinition.ResourceId = "invoice";
		  authorizationService.saveAuthorization(managementReadProcessDefinition);

		  Authorization salesDemoAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  salesDemoAuth.GroupId = "sales";
		  salesDemoAuth.Resource = USER;
		  salesDemoAuth.ResourceId = "demo";
		  salesDemoAuth.addPermission(READ);
		  authorizationService.saveAuthorization(salesDemoAuth);

		  Authorization salesJohnAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  salesJohnAuth.GroupId = "sales";
		  salesJohnAuth.Resource = USER;
		  salesJohnAuth.ResourceId = "john";
		  salesJohnAuth.addPermission(READ);
		  authorizationService.saveAuthorization(salesJohnAuth);

		  Authorization manDemoAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  manDemoAuth.GroupId = "management";
		  manDemoAuth.Resource = USER;
		  manDemoAuth.ResourceId = "demo";
		  manDemoAuth.addPermission(READ);
		  authorizationService.saveAuthorization(manDemoAuth);

		  Authorization manPeterAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  manPeterAuth.GroupId = "management";
		  manPeterAuth.Resource = USER;
		  manPeterAuth.ResourceId = "peter";
		  manPeterAuth.addPermission(READ);
		  authorizationService.saveAuthorization(manPeterAuth);

		  Authorization accDemoAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  accDemoAuth.GroupId = "accounting";
		  accDemoAuth.Resource = USER;
		  accDemoAuth.ResourceId = "demo";
		  accDemoAuth.addPermission(READ);
		  authorizationService.saveAuthorization(accDemoAuth);

		  Authorization accMaryAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  accMaryAuth.GroupId = "accounting";
		  accMaryAuth.Resource = USER;
		  accMaryAuth.ResourceId = "mary";
		  accMaryAuth.addPermission(READ);
		  authorizationService.saveAuthorization(accMaryAuth);

		  Authorization taskMaryAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		  taskMaryAuth.UserId = "mary";
		  taskMaryAuth.Resource = TASK;
		  taskMaryAuth.ResourceId = ANY;
		  taskMaryAuth.addPermission(READ);
		  taskMaryAuth.addPermission(UPDATE);
		  authorizationService.saveAuthorization(taskMaryAuth);

		  // create default filters

		  FilterService filterService = engine.FilterService;

		  IDictionary<string, object> filterProperties = new Dictionary<string, object>();
		  filterProperties["description"] = "Tasks assigned to me";
		  filterProperties["priority"] = -10;
		  addVariables(filterProperties);
		  TaskService taskService = engine.TaskService;
		  TaskQuery query = taskService.createTaskQuery().taskAssigneeExpression("${currentUser()}");
		  Filter myTasksFilter = filterService.newTaskFilter().setName("My Tasks").setProperties(filterProperties).setOwner("demo").setQuery(query);
		  filterService.saveFilter(myTasksFilter);

		  filterProperties.Clear();
		  filterProperties["description"] = "Tasks assigned to my Groups";
		  filterProperties["priority"] = -5;
		  addVariables(filterProperties);
		  query = taskService.createTaskQuery().taskCandidateGroupInExpression("${currentUserGroups()}").taskUnassigned();
		  Filter groupTasksFilter = filterService.newTaskFilter().setName("My Group Tasks").setProperties(filterProperties).setOwner("demo").setQuery(query);
		  filterService.saveFilter(groupTasksFilter);

		  // global read authorizations for these filters

		  Authorization globalMyTaskFilterRead = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL);
		  globalMyTaskFilterRead.Resource = FILTER;
		  globalMyTaskFilterRead.ResourceId = myTasksFilter.Id;
		  globalMyTaskFilterRead.addPermission(READ);
		  authorizationService.saveAuthorization(globalMyTaskFilterRead);

		  Authorization globalGroupFilterRead = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL);
		  globalGroupFilterRead.Resource = FILTER;
		  globalGroupFilterRead.ResourceId = groupTasksFilter.Id;
		  globalGroupFilterRead.addPermission(READ);
		  authorizationService.saveAuthorization(globalGroupFilterRead);

		  // management filter

		  filterProperties.Clear();
		  filterProperties["description"] = "Tasks for Group Accounting";
		  filterProperties["priority"] = -3;
		  addVariables(filterProperties);
		  query = taskService.createTaskQuery().taskCandidateGroupIn(Arrays.asList("accounting")).taskUnassigned();
		  Filter candidateGroupTasksFilter = filterService.newTaskFilter().setName("Accounting").setProperties(filterProperties).setOwner("demo").setQuery(query);
		  filterService.saveFilter(candidateGroupTasksFilter);

		  Authorization managementGroupFilterRead = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		  managementGroupFilterRead.Resource = FILTER;
		  managementGroupFilterRead.ResourceId = candidateGroupTasksFilter.Id;
		  managementGroupFilterRead.addPermission(READ);
		  managementGroupFilterRead.GroupId = "accounting";
		  authorizationService.saveAuthorization(managementGroupFilterRead);

		  // john's tasks

		  filterProperties.Clear();
		  filterProperties["description"] = "Tasks assigned to John";
		  filterProperties["priority"] = -1;
		  addVariables(filterProperties);
		  query = taskService.createTaskQuery().taskAssignee("john");
		  Filter johnsTasksFilter = filterService.newTaskFilter().setName("John's Tasks").setProperties(filterProperties).setOwner("demo").setQuery(query);
		  filterService.saveFilter(johnsTasksFilter);

		  // mary's tasks

		  filterProperties.Clear();
		  filterProperties["description"] = "Tasks assigned to Mary";
		  filterProperties["priority"] = -1;
		  addVariables(filterProperties);
		  query = taskService.createTaskQuery().taskAssignee("mary");
		  Filter marysTasksFilter = filterService.newTaskFilter().setName("Mary's Tasks").setProperties(filterProperties).setOwner("demo").setQuery(query);
		  filterService.saveFilter(marysTasksFilter);

		  // peter's tasks

		  filterProperties.Clear();
		  filterProperties["description"] = "Tasks assigned to Peter";
		  filterProperties["priority"] = -1;
		  addVariables(filterProperties);
		  query = taskService.createTaskQuery().taskAssignee("peter");
		  Filter petersTasksFilter = filterService.newTaskFilter().setName("Peter's Tasks").setProperties(filterProperties).setOwner("demo").setQuery(query);
		  filterService.saveFilter(petersTasksFilter);

		  // all tasks

		  filterProperties.Clear();
		  filterProperties["description"] = "All Tasks - Not recommended to be used in production :)";
		  filterProperties["priority"] = 10;
		  addVariables(filterProperties);
		  query = taskService.createTaskQuery();
		  Filter allTasksFilter = filterService.newTaskFilter().setName("All Tasks").setProperties(filterProperties).setOwner("demo").setQuery(query);
		  filterService.saveFilter(allTasksFilter);

		}

		protected internal virtual void addVariables(IDictionary<string, object> filterProperties)
		{
		  IList<object> variables = new List<object>();

		  addVariable(variables, "amount", "Invoice Amount");
		  addVariable(variables, "invoiceNumber", "Invoice Number");
		  addVariable(variables, "creditor", "Creditor");
		  addVariable(variables, "approver", "Approver");

		  filterProperties["variables"] = variables;
		}

		protected internal virtual void addVariable(IList<object> variables, string name, string label)
		{
		  IDictionary<string, string> variable = new Dictionary<string, string>();
		  variable["name"] = name;
		  variable["label"] = label;
		  variables.Add(variable);
		}
	}

}