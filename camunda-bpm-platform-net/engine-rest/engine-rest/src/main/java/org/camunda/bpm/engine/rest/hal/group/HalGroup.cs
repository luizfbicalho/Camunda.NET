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
namespace org.camunda.bpm.engine.rest.hal.group
{

	using Group = org.camunda.bpm.engine.identity.Group;

	public class HalGroup : HalResource<HalGroup>, HalIdResource
	{

	  public static readonly HalRelation REL_SELF = HalRelation.build("self", typeof(GroupRestService), UriBuilder.fromPath(org.camunda.bpm.engine.rest.GroupRestService_Fields.PATH).path("{id}"));

	  protected internal string id;
	  protected internal string name;
	  protected internal string type;

	  public static HalGroup fromGroup(Group group)
	  {
		HalGroup halGroup = new HalGroup();

		halGroup.id = group.Id;
		halGroup.name = group.Name;
		halGroup.type = group.Type;

		halGroup.linker.createLink(REL_SELF, group.Id);

		return halGroup;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }

	}

}