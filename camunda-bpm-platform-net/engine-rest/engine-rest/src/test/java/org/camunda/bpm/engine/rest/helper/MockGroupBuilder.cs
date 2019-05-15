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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using Group = org.camunda.bpm.engine.identity.Group;

	public class MockGroupBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Renamed;

	  public virtual MockGroupBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockGroupBuilder name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MockGroupBuilder type(string type)
	  {
		this.type_Renamed = type;
		return this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public org.camunda.bpm.engine.identity.Group build()
	  public virtual Group build()
	  {
		Group group = mock(typeof(Group));
		when(group.Id).thenReturn(id_Renamed);
		when(group.Name).thenReturn(name_Renamed);
		when(group.Type).thenReturn(type_Renamed);
		return group;
	  }

	}

}