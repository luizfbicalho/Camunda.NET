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
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using ResourceTypeUtil = org.camunda.bpm.engine.impl.util.ResourceTypeUtil;

	/// <summary>
	/// Default implementation of <seealso cref="PermissionProvider"/>
	/// 
	/// @author Yana.Vasileva
	/// @author Tobias Metzke
	/// 
	/// </summary>
	public class DefaultPermissionProvider : PermissionProvider
	{

	  public virtual Permission getPermissionForName(string name, int resourceType)
	  {
		return ResourceTypeUtil.getPermissionByNameAndResourceType(name, resourceType);
	  }

	  public virtual Permission[] getPermissionsForResource(int resourceType)
	  {
		return ResourceTypeUtil.getPermissionsByResourceType(resourceType);
	  }

	  public virtual string getNameForResource(int resourceType)
	  {
		Resource resourceByType = ResourceTypeUtil.getResourceByType(resourceType);
		return resourceByType == null ? null : resourceByType.resourceName();
	  }

	}

}