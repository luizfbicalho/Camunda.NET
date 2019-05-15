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
namespace org.camunda.bpm.engine.test.api.authorization.util
{

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using Assert = org.junit.Assert;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AuthorizationTestBaseRule : TestWatcher
	{

	  protected internal ProcessEngineRule engineRule;

	  protected internal IList<User> users = new List<User>();
	  protected internal IList<Group> groups = new List<Group>();
	  protected internal IList<Authorization> authorizations = new List<Authorization>();

	  public AuthorizationTestBaseRule(ProcessEngineRule engineRule)
	  {
		this.engineRule = engineRule;
	  }

	  public virtual void enableAuthorization(string userId)
	  {
		engineRule.ProcessEngine.ProcessEngineConfiguration.AuthorizationEnabled = true;
		if (!string.ReferenceEquals(userId, null))
		{
		  engineRule.IdentityService.AuthenticatedUserId = userId;
		}
	  }

	  public virtual void disableAuthorization()
	  {
		engineRule.ProcessEngine.ProcessEngineConfiguration.AuthorizationEnabled = false;
		engineRule.IdentityService.clearAuthentication();
	  }

	  protected internal virtual void finished(Description description)
	  {
		engineRule.IdentityService.clearAuthentication();

		deleteManagedAuthorizations();

		base.finished(description);

		Assert.assertTrue("Users have been created but not deleted", users.Count == 0);
		Assert.assertTrue("Groups have been created but not deleted", groups.Count == 0);
	  }

	  public virtual void manageAuthorization(Authorization authorization)
	  {
		this.authorizations.Add(authorization);
	  }

	  protected internal virtual Authorization createAuthorization(int type, Resource resource, string resourceId)
	  {
		Authorization authorization = engineRule.AuthorizationService.createNewAuthorization(type);

		authorization.Resource = resource;
		if (!string.ReferenceEquals(resourceId, null))
		{
		  authorization.ResourceId = resourceId;
		}

		return authorization;
	  }

	  public virtual void createGrantAuthorization(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		Authorization authorization = createAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT, resource, resourceId);
		authorization.UserId = userId;
		foreach (Permission permission in permissions)
		{
		  authorization.addPermission(permission);
		}

		engineRule.AuthorizationService.saveAuthorization(authorization);
		manageAuthorization(authorization);
	  }

	  protected internal virtual void deleteManagedAuthorizations()
	  {
		foreach (Authorization authorization in authorizations)
		{
		  engineRule.AuthorizationService.deleteAuthorization(authorization.Id);
		}
	  }

	  public virtual void createUserAndGroup(string userId, string groupId)
	  {

		User user = engineRule.IdentityService.newUser(userId);
		engineRule.IdentityService.saveUser(user);
		users.Add(user);

		Group group = engineRule.IdentityService.newGroup(groupId);
		engineRule.IdentityService.saveGroup(group);
		groups.Add(group);
	  }

	  public virtual void deleteUsersAndGroups()
	  {
		foreach (User user in users)
		{
		  engineRule.IdentityService.deleteUser(user.Id);
		}
		users.Clear();

		foreach (Group group in groups)
		{
		  engineRule.IdentityService.deleteGroup(group.Id);
		}
		groups.Clear();
	  }
	}

}