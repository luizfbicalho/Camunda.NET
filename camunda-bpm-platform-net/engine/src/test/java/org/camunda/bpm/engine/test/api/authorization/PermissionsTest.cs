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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;


	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using ResourceTypeUtil = org.camunda.bpm.engine.impl.util.ResourceTypeUtil;
	using Test = org.junit.Test;

	public class PermissionsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNewPermissionsIntegrityToOld()
	  public virtual void testNewPermissionsIntegrityToOld()
	  {
		foreach (Permissions permission in Permissions.values())
		{
		  string permissionName = permission.Name;
		  foreach (Resource resource in permission.Types)
		  {
			Type clazz = ResourceTypeUtil.PermissionEnums[resource.resourceType()];
			if (clazz != null && !clazz.Equals(typeof(Permissions)))
			{
			  Permission resolvedPermission = null;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (Enum<?> enumCandidate : clazz.getEnumConstants())
			  foreach (Enum<object> enumCandidate in clazz.EnumConstants)
			  {
				if (enumCandidate.ToString().Equals(permissionName))
				{
				  resolvedPermission = (Permission) enumCandidate;
				  break;
				}
			  }
			  assertThat(resolvedPermission).overridingErrorMessage("Permission %s for resource %s not found in new enum %s", permission, resource, clazz.Name).NotNull;

			  assertThat(resolvedPermission.Value).isEqualTo(permission.Value);
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPermissionsValues()
	  public virtual void testPermissionsValues()
	  {
		verifyValuesAreUniqueAndPowerOfTwo(Permissions.values(), typeof(Permissions).Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestOfPermissionsEnumValues()
	  public virtual void testRestOfPermissionsEnumValues()
	  {
		foreach (Type permissionsClass in ResourceTypeUtil.PermissionEnums.Values)
		{
		  if (!permissionsClass.Equals(typeof(Permissions)))
		  {
			verifyValuesAreUniqueAndPowerOfTwo((Permission[])permissionsClass.EnumConstants, permissionsClass.Name);
		  }
		}
	  }

	  private void verifyValuesAreUniqueAndPowerOfTwo(Permission[] permissions, string className)
	  {
		ISet<int> values = new HashSet<int>();
		foreach (Permission permission in permissions)
		{
		  int value = permission.Value;
		  // value is unique
		  assertThat(values.Add(value)).overridingErrorMessage("The value '%s' of '%s' permission is not unique for '%s' permission enum. Another permission already has this value.", value, permission, className).True;
		  if (value != int.MaxValue && value != 0)
		  {
			// value is power of 2
			assertThat(isPowerOfTwo(value)).overridingErrorMessage("The value '%s' of '%s' permission is invalid for '%s' permission enum. The values must be power of 2.", value, permission, className).True;
		  }
		}
	  }

	  private bool isPowerOfTwo(int value)
	  {
		return value > 1 && (value & (value - 1)) == 0;
	  }
	}

}