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
namespace org.camunda.bpm.engine.rest.hal.identitylink
{

	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;

	public class HalIdentityLink : HalResource<HalIdentityLink>
	{

	  public static readonly HalRelation REL_GROUP = HalRelation.build("group", typeof(GroupRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.GroupRestService_Fields.PATH).path("{id}"));
	  public static readonly HalRelation REL_USER = HalRelation.build("user", typeof(UserRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.UserRestService_Fields.PATH).path("{id}"));
	  public static readonly HalRelation REL_TASK = HalRelation.build("task", typeof(TaskRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH).path("{id}"));

	  protected internal string type;
	  protected internal string userId;
	  protected internal string groupId;
	  protected internal string taskId;

	  public static HalIdentityLink fromIdentityLink(IdentityLink identityLink)
	  {
		HalIdentityLink halIdentityLink = new HalIdentityLink();

		halIdentityLink.type = identityLink.Type;
		halIdentityLink.userId = identityLink.UserId;
		halIdentityLink.groupId = identityLink.GroupId;
		halIdentityLink.taskId = identityLink.TaskId;

		halIdentityLink.linker.createLink(REL_USER, identityLink.UserId);
		halIdentityLink.linker.createLink(REL_GROUP, identityLink.GroupId);
		halIdentityLink.linker.createLink(REL_TASK, identityLink.TaskId);

		return halIdentityLink;
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
	  }

	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
	  }

	}

}