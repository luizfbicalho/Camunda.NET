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
namespace org.camunda.bpm.engine.test.api.filter
{

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using User = org.camunda.bpm.engine.identity.User;
	using FilterEntity = org.camunda.bpm.engine.impl.persistence.entity.FilterEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterAuthorizationsTest : PluggableProcessEngineTestCase
	{

	  protected internal User testUser;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal Authorization createAuthorization_Conflict;
	  protected internal Authorization updateAuthorization;
	  protected internal Authorization readAuthorization;
	  protected internal Authorization deleteAuthorization;

	  public override void setUp()
	  {
		testUser = createTestUser("test");

		createAuthorization_Conflict = createAuthorization_Conflict(Permissions.CREATE, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY);
		updateAuthorization = createAuthorization(Permissions.UPDATE, null);
		readAuthorization = createAuthorization(Permissions.READ, null);
		deleteAuthorization = createAuthorization(Permissions.DELETE, null);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = testUser.Id;
	  }

	  public override void tearDown()
	  {
		processEngineConfiguration.AuthorizationEnabled = false;
		foreach (Filter filter in filterService.createFilterQuery().list())
		{
		  filterService.deleteFilter(filter.Id);
		}
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }

	  public virtual void testCreateFilterNotPermitted()
	  {
		try
		{
		  filterService.newTaskFilter();
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}
	  }

	  public virtual void testCreateFilterPermitted()
	  {
		grantCreateFilter();
		Filter filter = filterService.newTaskFilter();
		assertNotNull(filter);
	  }

	  public virtual void testSaveFilterNotPermitted()
	  {
		Filter filter = new FilterEntity(EntityTypes.TASK);
		try
		{
		  filterService.saveFilter(filter);
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}
	  }

	  public virtual void testSaveFilterPermitted()
	  {
		Filter filter = (new FilterEntity(EntityTypes.TASK)).setName("testFilter");

		grantCreateFilter();

		filterService.saveFilter(filter);

		assertNotNull(filter.Id);
	  }

	  public virtual void testUpdateFilterNotPermitted()
	  {
		Filter filter = createTestFilter();

		filter.Name = "anotherName";

		try
		{
		  filterService.saveFilter(filter);
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}
	  }

	  public virtual void testUpdateFilterPermitted()
	  {
		Filter filter = createTestFilter();

		filter.Name = "anotherName";

		grantUpdateFilter(filter.Id);

		filter = filterService.saveFilter(filter);
		assertEquals("anotherName", filter.Name);
	  }

	  public virtual void testDeleteFilterNotPermitted()
	  {
		Filter filter = createTestFilter();

		try
		{
		  filterService.deleteFilter(filter.Id);
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}
	  }

	  public virtual void testDeleteFilterPermitted()
	  {
		Filter filter = createTestFilter();

		grantDeleteFilter(filter.Id);

		filterService.deleteFilter(filter.Id);

		long count = filterService.createFilterQuery().count();
		assertEquals(0, count);
	  }

	  public virtual void testReadFilterNotPermitted()
	  {
		Filter filter = createTestFilter();

		long count = filterService.createFilterQuery().count();
		assertEquals(0, count);

		Filter returnedFilter = filterService.createFilterQuery().filterId(filter.Id).singleResult();
		assertNull(returnedFilter);

		try
		{
		  filterService.getFilter(filter.Id);
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}

		try
		{
		  filterService.singleResult(filter.Id);
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}

		try
		{
		  filterService.list(filter.Id);
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}

		try
		{
		  filterService.listPage(filter.Id, 1, 2);
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}

		try
		{
		  filterService.count(filter.Id);
		  fail("Exception expected");
		}
		catch (AuthorizationException)
		{
		  // expected
		}
	  }

	  public virtual void testReadFilterPermitted()
	  {
		Filter filter = createTestFilter();

		grantReadFilter(filter.Id);

		long count = filterService.createFilterQuery().count();
		assertEquals(1, count);

		Filter returnedFilter = filterService.createFilterQuery().filterId(filter.Id).singleResult();
		assertNotNull(returnedFilter);

		returnedFilter = filterService.getFilter(filter.Id);
		assertNotNull(returnedFilter);

		// create test Task
		Task task = taskService.newTask("test");
		taskService.saveTask(task);

		Task result = filterService.singleResult(filter.Id);
		assertNotNull(result);
		assertEquals(task.Id, result.Id);

		IList<Task> resultList = filterService.list(filter.Id);
		assertNotNull(resultList);
		assertEquals(1, resultList.Count);
		assertEquals(task.Id, resultList[0].Id);

		resultList = filterService.listPage(filter.Id, 0, 2);
		assertNotNull(resultList);
		assertEquals(1, resultList.Count);
		assertEquals(task.Id, resultList[0].Id);

		count = filterService.count(filter.Id).Value;
		assertEquals(1, count);

		// remove Task
		taskService.deleteTask(task.Id, true);
	  }

	  public virtual void testReadFilterPermittedWithMultiple()
	  {
		Filter filter = createTestFilter();

		grantReadFilter(filter.Id);
		Authorization authorization = processEngine.AuthorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		authorization.addPermission(Permissions.READ);
		authorization.UserId = org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
		authorization.Resource = Resources.FILTER;
		authorization.ResourceId = org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
		processEngine.AuthorizationService.saveAuthorization(authorization);

		long count = filterService.createFilterQuery().count();
		assertEquals(1, count);

		Filter returnedFilter = filterService.createFilterQuery().filterId(filter.Id).singleResult();
		assertNotNull(returnedFilter);

		returnedFilter = filterService.getFilter(filter.Id);
		assertNotNull(returnedFilter);

		processEngine.AuthorizationService.deleteAuthorization(authorization.Id);
	  }

	  public virtual void testDefaultFilterAuthorization()
	  {
		// create two other users beside testUser
		User ownerUser = createTestUser("ownerUser");
		User anotherUser = createTestUser("anotherUser");

		// grant testUser create permission
		grantCreateFilter();

		// create a new filter with ownerUser as owner
		Filter filter = filterService.newTaskFilter("testFilter");
		filter.Owner = ownerUser.Id;
		filterService.saveFilter(filter);

		assertFilterPermission(Permissions.CREATE, testUser, null, true);
		assertFilterPermission(Permissions.CREATE, ownerUser, null, false);
		assertFilterPermission(Permissions.CREATE, anotherUser, null, false);

		assertFilterPermission(Permissions.UPDATE, testUser, filter.Id, false);
		assertFilterPermission(Permissions.UPDATE, ownerUser, filter.Id, true);
		assertFilterPermission(Permissions.UPDATE, anotherUser, filter.Id, false);

		assertFilterPermission(Permissions.READ, testUser, filter.Id, false);
		assertFilterPermission(Permissions.READ, ownerUser, filter.Id, true);
		assertFilterPermission(Permissions.READ, anotherUser, filter.Id, false);

		assertFilterPermission(Permissions.DELETE, testUser, filter.Id, false);
		assertFilterPermission(Permissions.DELETE, ownerUser, filter.Id, true);
		assertFilterPermission(Permissions.DELETE, anotherUser, filter.Id, false);
	  }

	  public virtual void testCreateFilterGenericOwnerId()
	  {
		grantCreateFilter();

		Filter filter = filterService.newTaskFilter("someName");
		filter.Owner = "*";

		try
		{
		  filterService.saveFilter(filter);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot create default authorization for filter owner *: " + "id cannot be *. * is a reserved identifier.", e.Message);
		}
	  }

	  /// <summary>
	  /// CAM-4889
	  /// </summary>
	  public virtual void FAILING_testUpdateFilterGenericOwnerId()
	  {
		grantCreateFilter();

		Filter filter = filterService.newTaskFilter("someName");
		filterService.saveFilter(filter);

		grantUpdateFilter(filter.Id);
		filter.Owner = "*";

		try
		{
		  filterService.saveFilter(filter);
		  fail("it should not be possible to save a filter with the generic owner id");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("foo", e.Message);
		}
	  }

	  protected internal virtual User createTestUser(string userId)
	  {
		User user = identityService.newUser(userId);
		identityService.saveUser(user);

		// give user all permission to manipulate authorisations
		Authorization authorization = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		authorization.UserId = user.Id;
		authorization.Resource = Resources.AUTHORIZATION;
		authorization.ResourceId = org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
		authorization.addPermission(Permissions.ALL);
		authorizationService.saveAuthorization(authorization);

		// give user all permission to manipulate users
		authorization = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		authorization.UserId = user.Id;
		authorization.Resource = Resources.USER;
		authorization.ResourceId = org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
		authorization.addPermission(Permissions.ALL);
		authorizationService.saveAuthorization(authorization);

		authorization = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		authorization.UserId = user.Id;
		authorization.Resource = Resources.TASK;
		authorization.ResourceId = org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
		authorization.addPermission(Permissions.ALL);
		authorizationService.saveAuthorization(authorization);

		return user;
	  }

	  protected internal virtual Filter createTestFilter()
	  {
		grantCreateFilter();
		Filter filter = filterService.newTaskFilter("testFilter");
		return filterService.saveFilter(filter);
	  }

	  protected internal virtual Authorization createAuthorization(Permission permission, string resourceId)
	  {
		Authorization authorization = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		authorization.UserId = testUser.Id;
		authorization.Resource = Resources.FILTER;
		authorization.addPermission(permission);
		if (!string.ReferenceEquals(resourceId, null))
		{
		  authorization.ResourceId = resourceId;
		}
		return authorization;
	  }

	  protected internal virtual void grantCreateFilter()
	  {
		grantFilterPermission(createAuthorization_Conflict, null);
		assertFilterPermission(Permissions.CREATE, testUser, null, true);
	  }

	  protected internal virtual void grantUpdateFilter(string filterId)
	  {
		grantFilterPermission(updateAuthorization, filterId);
		assertFilterPermission(Permissions.UPDATE, testUser, filterId, true);
	  }

	  protected internal virtual void grantReadFilter(string filterId)
	  {
		grantFilterPermission(readAuthorization, filterId);
		assertFilterPermission(Permissions.READ, testUser, filterId, true);
	  }

	  protected internal virtual void grantDeleteFilter(string filterId)
	  {
		grantFilterPermission(deleteAuthorization, filterId);
		assertFilterPermission(Permissions.DELETE, testUser, filterId, true);
	  }

	  protected internal virtual void grantFilterPermission(Authorization authorization, string filterId)
	  {
		if (!string.ReferenceEquals(filterId, null))
		{
		  authorization.ResourceId = filterId;
		}
		authorizationService.saveAuthorization(authorization);
	  }

	  protected internal virtual void assertFilterPermission(Permission permission, User user, string filterId, bool expected)
	  {
		bool result;
		if (!string.ReferenceEquals(filterId, null))
		{
		  result = authorizationService.isUserAuthorized(user.Id, null, permission, Resources.FILTER, filterId);
		}
		else
		{
		  result = authorizationService.isUserAuthorized(user.Id, null, permission, Resources.FILTER);
		}
		assertEquals(expected, result);
	  }

	}

}