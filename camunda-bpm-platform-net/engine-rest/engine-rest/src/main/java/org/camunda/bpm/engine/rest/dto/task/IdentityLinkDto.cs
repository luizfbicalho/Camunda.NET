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
namespace org.camunda.bpm.engine.rest.dto.task
{
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;

	public class IdentityLinkDto
	{

	  protected internal string userId;
	  protected internal string groupId;
	  protected internal string type;

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			this.userId = value;
		  }
	  }


	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
		  set
		  {
			this.groupId = value;
		  }
	  }


	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public static IdentityLinkDto fromIdentityLink(IdentityLink identityLink)
	  {
		IdentityLinkDto dto = new IdentityLinkDto();
		dto.userId = identityLink.UserId;
		dto.groupId = identityLink.GroupId;
		dto.type = identityLink.Type;

		return dto;
	  }

	  public virtual void validate()
	  {
		if (!string.ReferenceEquals(userId, null) && !string.ReferenceEquals(groupId, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Identity Link requires userId or groupId, but not both.");
		}

		if (string.ReferenceEquals(userId, null) && string.ReferenceEquals(groupId, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Identity Link requires userId or groupId.");
		}
	  }

	}

}