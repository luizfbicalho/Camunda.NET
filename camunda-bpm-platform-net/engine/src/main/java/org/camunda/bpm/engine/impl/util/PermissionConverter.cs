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

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;

	/// <summary>
	/// <para>
	/// Converts between the String-Array based and the Integer-based representation
	/// of permissions.
	/// </para>
	/// 
	/// @author Daniel Meyer
	/// @author Tobias Metzke
	/// 
	/// </summary>
	public class PermissionConverter
	{

	  public static Permission[] getPermissionsForNames(string[] names, int resourceType, ProcessEngineConfiguration engineConfiguration)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.authorization.Permission[] permissions = new org.camunda.bpm.engine.authorization.Permission[names.length];
		Permission[] permissions = new Permission[names.Length];

		for (int i = 0; i < names.Length; i++)
		{
		  permissions[i] = ((ProcessEngineConfigurationImpl) engineConfiguration).PermissionProvider.getPermissionForName(names[i], resourceType);
		}

		return permissions;
	  }

	  public static string[] getNamesForPermissions(Authorization authorization, Permission[] permissions)
	  {

		int type = authorization.AuthorizationType;

		// special case all permissions are granted
		if ((type == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL || type == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT) && authorization.EveryPermissionGranted)
		{
		  return new string[] {Permissions.ALL.Name};
		}

		// special case all permissions are revoked
		if (type == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE && authorization.EveryPermissionRevoked)
		{
		  return new string[] {Permissions.ALL.Name};
		}

		IList<string> names = new List<string>();

		foreach (Permission permission in permissions)
		{
		  string name = permission.Name;
		  // filter NONE and ALL from permissions array
		  if (!name.Equals(Permissions.NONE.Name) && !name.Equals(Permissions.ALL.Name))
		  {
			names.Add(name);
		  }
		}

		return names.ToArray();
	  }

	}

}