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
namespace org.camunda.bpm.engine.rest.dto
{

	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;

	/// <summary>
	/// * <para>Dto for <seealso cref="MissingAuthorization"/></para>
	/// @author Filip Hrisafov
	/// 
	/// </summary>
	public class MissingAuthorizationDto
	{

	  private string permissionName;
	  private string resourceName;
	  protected internal string resourceId;

	  // transformer /////////////////////////////

	  public static MissingAuthorizationDto fromInfo(MissingAuthorization info)
	  {
		MissingAuthorizationDto dto = new MissingAuthorizationDto();

		dto.PermissionName = info.ViolatedPermissionName;
		dto.ResourceId = info.ResourceId;
		dto.ResourceName = info.ResourceType;

		return dto;
	  }

	  public static IList<MissingAuthorizationDto> fromInfo(ICollection<MissingAuthorization> infos)
	  {
		IList<MissingAuthorizationDto> dtos = new List<MissingAuthorizationDto>();
		foreach (MissingAuthorization info in infos)
		{
		  dtos.Add(fromInfo(info));
		}
		return dtos;
	  }

	  // getter / setters ////////////////////////
	  public virtual string PermissionName
	  {
		  get
		  {
			return permissionName;
		  }
		  set
		  {
			this.permissionName = value;
		  }
	  }


	  public virtual string ResourceName
	  {
		  get
		  {
			return resourceName;
		  }
		  set
		  {
			this.resourceName = value;
		  }
	  }


	  public virtual string ResourceId
	  {
		  get
		  {
			return resourceId;
		  }
		  set
		  {
			this.resourceId = value;
		  }
	  }


	}

}