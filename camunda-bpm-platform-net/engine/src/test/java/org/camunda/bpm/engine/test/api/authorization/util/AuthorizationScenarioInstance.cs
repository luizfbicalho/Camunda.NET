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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsInAnyOrder;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AuthorizationScenarioInstance
	{

	  protected internal AuthorizationScenario scenario;

	  protected internal IList<Authorization> createdAuthorizations = new List<Authorization>();
	  protected internal IList<Authorization> missingAuthorizations = new List<Authorization>();

	  public AuthorizationScenarioInstance(AuthorizationScenario scenario, AuthorizationService authorizationService, IDictionary<string, string> resourceBindings)
	  {
		this.scenario = scenario;
		init(authorizationService, resourceBindings);
	  }

	  public virtual void init(AuthorizationService authorizationService, IDictionary<string, string> resourceBindings)
	  {
		foreach (AuthorizationSpec authorizationSpec in scenario.GivenAuthorizations)
		{
		  Authorization authorization = authorizationSpec.instantiate(authorizationService, resourceBindings);
		  authorizationService.saveAuthorization(authorization);
		  createdAuthorizations.Add(authorization);
		}

		foreach (AuthorizationSpec authorizationSpec in scenario.MissingAuthorizations)
		{
		  Authorization authorization = authorizationSpec.instantiate(authorizationService, resourceBindings);
		  missingAuthorizations.Add(authorization);
		}
	  }

	  public virtual void tearDown(AuthorizationService authorizationService)
	  {
		ISet<string> activeAuthorizations = new HashSet<string>();
		foreach (Authorization activeAuthorization in authorizationService.createAuthorizationQuery().list())
		{
		  activeAuthorizations.Add(activeAuthorization.Id);
		}

		foreach (Authorization createdAuthorization in createdAuthorizations)
		{
		  if (activeAuthorizations.Contains(createdAuthorization.Id))
		  {
			authorizationService.deleteAuthorization(createdAuthorization.Id);
		  }
		}
	  }

	  public virtual void assertAuthorizationException(AuthorizationException e)
	  {
		if (missingAuthorizations.Count > 0 && e != null)
		{
		  string message = e.Message;
		  string assertionFailureMessage = describeScenarioFailure("Expected an authorization exception but the message was wrong: " + e.Message);

		  IList<MissingAuthorization> actualMissingAuthorizations = new List<MissingAuthorization>(e.MissingAuthorizations);
		  IList<MissingAuthorization> expectedMissingAuthorizations = MissingAuthorizationMatcher.asMissingAuthorizations(missingAuthorizations);

		  Assert.assertThat(actualMissingAuthorizations, containsInAnyOrder(MissingAuthorizationMatcher.asMatchers(expectedMissingAuthorizations)));

		  foreach (Authorization missingAuthorization in missingAuthorizations)
		  {
			Assert.assertTrue(assertionFailureMessage, message.Contains(missingAuthorization.UserId));
			Assert.assertEquals(missingAuthorization.UserId, e.UserId);

			Permission[] permissions = AuthorizationTestUtil.getPermissions(missingAuthorization);
			foreach (Permission permission in permissions)
			{
			  if (permission.Value != Permissions.NONE.Value)
			  {
				Assert.assertTrue(assertionFailureMessage, message.Contains(permission.Name));
				break;
			  }
			}

			if (!org.camunda.bpm.engine.authorization.Authorization_Fields.ANY.Equals(missingAuthorization.ResourceId))
			{
			  // missing ANY authorizations are not explicitly represented in the error message
			  Assert.assertTrue(assertionFailureMessage, message.Contains(missingAuthorization.ResourceId));
			}

			Resource resource = AuthorizationTestUtil.getResourceByType(missingAuthorization.ResourceType);
			Assert.assertTrue(assertionFailureMessage, message.Contains(resource.resourceName()));
		  }
		}
		else if (missingAuthorizations.Count == 0 && e == null)
		{
		  // nothing to do
		}
		else
		{
		  if (e != null)
		  {
			Assert.fail(describeScenarioFailure("Expected no authorization exception but got one: " + e.Message));
		  }
		  else
		  {
			Assert.fail(describeScenarioFailure("Expected failure due to missing authorizations but code under test was successful"));
		  }
		}
	  }

	  protected internal virtual string describeScenarioFailure(string message)
	  {
		return message + "\n"
			+ "\n"
			+ "Scenario: \n"
			+ scenario.ToString();
	  }
	}

}