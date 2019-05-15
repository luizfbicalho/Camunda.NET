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

	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Description = org.hamcrest.Description;
	using Matcher = org.hamcrest.Matcher;
	using TypeSafeDiagnosingMatcher = org.hamcrest.TypeSafeDiagnosingMatcher;

	/// <summary>
	/// @author Filip Hrisafov
	/// </summary>
	public class MissingAuthorizationMatcher : TypeSafeDiagnosingMatcher<MissingAuthorization>
	{

	  private MissingAuthorization missing;

	  private MissingAuthorizationMatcher(MissingAuthorization authorization)
	  {
		this.missing = authorization;
	  }

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static java.util.Collection<org.hamcrest.Matcher<? super org.camunda.bpm.engine.authorization.MissingAuthorization>> asMatchers(java.util.List<org.camunda.bpm.engine.authorization.MissingAuthorization> missingAuthorizations)
	  public static ICollection<Matcher> asMatchers(IList<MissingAuthorization> missingAuthorizations)
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Collection<org.hamcrest.Matcher<? super org.camunda.bpm.engine.authorization.MissingAuthorization>> matchers = new java.util.ArrayList<org.hamcrest.Matcher<? super org.camunda.bpm.engine.authorization.MissingAuthorization>>(missingAuthorizations.size());
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
		ICollection<Matcher> matchers = new List<Matcher>(missingAuthorizations.Count);
		foreach (MissingAuthorization authorization in missingAuthorizations)
		{
		  matchers.Add(new MissingAuthorizationMatcher(authorization));
		}
		return matchers;
	  }

	  protected internal static MissingAuthorization asMissingAuthorization(Authorization authorization)
	  {
		string permissionName = null;
		string resourceId = null;
		string resourceName = null;

		Permission[] permissions = AuthorizationTestUtil.getPermissions(authorization);
		foreach (Permission permission in permissions)
		{
		  if (permission.Value != Permissions.NONE.Value)
		  {
			permissionName = permission.Name;
			break;
		  }
		}

		if (!org.camunda.bpm.engine.authorization.Authorization_Fields.ANY.Equals(authorization.ResourceId))
		{
		  // missing ANY authorizations are not explicitly represented in the error message
		  resourceId = authorization.ResourceId;
		}

		Resource resource = AuthorizationTestUtil.getResourceByType(authorization.ResourceType);
		resourceName = resource.resourceName();
		return new MissingAuthorization(permissionName, resourceName, resourceId);
	  }

	  public static IList<MissingAuthorization> asMissingAuthorizations(IList<Authorization> authorizations)
	  {
		IList<MissingAuthorization> missingAuthorizations = new List<MissingAuthorization>();
		foreach (Authorization authorization in authorizations)
		{
		  missingAuthorizations.Add(asMissingAuthorization(authorization));
		}
		return missingAuthorizations;
	  }

	  protected internal override bool matchesSafely(MissingAuthorization item, Description mismatchDescription)
	  {
		if (StringUtils.Equals(missing.ResourceId, item.ResourceId) && StringUtils.Equals(missing.ResourceType, item.ResourceType) && StringUtils.Equals(missing.ViolatedPermissionName, item.ViolatedPermissionName))
		{
		  return true;
		}
		mismatchDescription.appendText("expected missing authorization: ").appendValue(missing).appendValue(" received: ").appendValue(item);
		return false;
	  }

	  public override void describeTo(Description description)
	  {
		description.appendValue(missing);
	  }
	}

}