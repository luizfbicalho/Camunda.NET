using System;
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
namespace org.camunda.bpm.engine.impl.util
{

	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using TaskPermissions = org.camunda.bpm.engine.authorization.TaskPermissions;

	public class ResourceTypeUtil
	{

	  /// <summary>
	  /// A map containing all <seealso cref="Resources"/> as a key and
	  /// the respective <seealso cref="Permission"/> Enum class for this resource.<para>
	  /// NOTE: In case of new <seealso cref="Permission"/> Enum class, please adjust the map accordingly
	  /// </para>
	  /// </summary>
	  private static readonly IDictionary<int, Type> PERMISSION_ENUMS = new Dictionary<int, Type>();

	  static ResourceTypeUtil()
	  {
		PERMISSION_ENUMS[Resources.BATCH.resourceType()] = typeof(BatchPermissions);
		PERMISSION_ENUMS[Resources.PROCESS_DEFINITION.resourceType()] = typeof(ProcessDefinitionPermissions);
		PERMISSION_ENUMS[Resources.PROCESS_INSTANCE.resourceType()] = typeof(ProcessInstancePermissions);
		PERMISSION_ENUMS[Resources.TASK.resourceType()] = typeof(TaskPermissions);

		// the rest
		foreach (Resource resource in Resources.values())
		{
		  int resourceType = resource.resourceType();
		  if (!PERMISSION_ENUMS.ContainsKey(resourceType))
		  {
			PERMISSION_ENUMS[resourceType] = typeof(Permissions);
		  }
		}
	  }

	  /// <returns> <code>true</code> in case the resource with the provided resourceTypeId is contained by the specified list </returns>
	  public static bool resourceIsContainedInArray(int? resourceTypeId, Resource[] resources)
	  {
		foreach (Resource resource in resources)
		{
		  if (resourceTypeId.Value == resource.resourceType())
		  {
			return true;
		  }
		}
		return false;
	  }


	  /// <returns> See <seealso cref="ResourceTypeUtil#PERMISSION_ENUMS"/> </returns>
	  public static IDictionary<int, Type> PermissionEnums
	  {
		  get
		  {
			return PERMISSION_ENUMS;
		  }
	  }

	  /// <summary>
	  /// Retrieves the <seealso cref="Permission"/> array based on the predifined <seealso cref="ResourceTypeUtil#PERMISSION_ENUMS PERMISSION_ENUMS"/>
	  /// </summary>
	  public static Permission[] getPermissionsByResourceType(int givenResourceType)
	  {
		Type clazz = PERMISSION_ENUMS[givenResourceType];
		if (clazz == null)
		{
		  return Permissions.values();
		}
		return ((Permission[]) clazz.EnumConstants);
	  }

	  /// <summary>
	  /// Currently used only in the Rest API
	  /// Returns a <seealso cref="Permission"/> based on the specified <code>permissionName</code> and <code>resourceType</code> </summary>
	  /// <exception cref="BadUserRequestException"> in case the permission is not valid for the specified resource type </exception>
	  public static Permission getPermissionByNameAndResourceType(string permissionName, int resourceType)
	  {
		foreach (Permission permission in getPermissionsByResourceType(resourceType))
		{
		  if (permission.Name.Equals(permissionName))
		  {
			return permission;
		  }
		}
		throw new BadUserRequestException(string.Format("The permission '{0}' is not valid for '{1}' resource type.", permissionName, getResourceByType(resourceType)));
	  }

	  /// <summary>
	  /// Iterates over the <seealso cref="Resources"/> and 
	  /// returns either the resource with specified <code>resourceType</code> or <code>null</code>.
	  /// </summary>
	  public static Resource getResourceByType(int resourceType)
	  {
		foreach (Resource resource in Resources.values())
		{
		  if (resource.resourceType() == resourceType)
		  {
			return resource;
		  }
		}
		return null;
	  }
	}

}