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
//	import static junit.framework.TestCase.assertEquals;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ResourceTypeUtil = org.camunda.bpm.engine.impl.util.ResourceTypeUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AuthorizationTestUtil
	{

	  protected internal static IDictionary<int, Resource> resourcesByType = new Dictionary<int, Resource>();

	  static AuthorizationTestUtil()
	  {
		foreach (Resource resource in Resources.values())
		{
		  resourcesByType[resource.resourceType()] = resource;
		}
	  }

	  public static Resource getResourceByType(int type)
	  {
		return resourcesByType[type];
	  }

	  /// <summary>
	  /// Checks if the info has the expected parameters.
	  /// </summary>
	  /// <param name="expectedPermissionName"> to use </param>
	  /// <param name="expectedResourceName"> to use </param>
	  /// <param name="expectedResourceId"> to use </param>
	  /// <param name="info"> to check </param>
	  public static void assertExceptionInfo(string expectedPermissionName, string expectedResourceName, string expectedResourceId, MissingAuthorization info)
	  {
		assertEquals(expectedPermissionName, info.ViolatedPermissionName);
		assertEquals(expectedResourceName, info.ResourceType);
		assertEquals(expectedResourceId, info.ResourceId);
	  }

	  /// <returns> the set of permission for the given authorization </returns>
	  public static Permission[] getPermissions(Authorization authorization)
	  {
		int resourceType = authorization.ResourceType;
		Permission[] permissionsByResourceType = ResourceTypeUtil.getPermissionsByResourceType(resourceType);

		return authorization.getPermissions(permissionsByResourceType);
	  }
	}

}