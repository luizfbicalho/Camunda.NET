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
namespace org.camunda.bpm.engine.authorization
{
	/// <summary>
	/// Wrapper containing the missing authorization information. It contains the name of the violated permission,
	/// the type of the resouce and the Id of the resource.
	/// 
	/// @author Filip Hrisafov
	/// </summary>
	public class MissingAuthorization
	{

	  private string permissionName;
	  private string resourceType;
	  protected internal string resourceId;

	  public MissingAuthorization(string permissionName, string resourceType, string resourceId)
	  {
		this.permissionName = permissionName;
		this.resourceType = resourceType;
		this.resourceId = resourceId;
	  }

	  public virtual string ViolatedPermissionName
	  {
		  get
		  {
			return permissionName;
		  }
	  }

	  public virtual string ResourceType
	  {
		  get
		  {
			return resourceType;
		  }
	  }

	  public virtual string ResourceId
	  {
		  get
		  {
			return resourceId;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[permissionName=" + permissionName + ", resourceType=" + resourceType + ", resourceId=" + resourceId + "]";
	  }
	}

}