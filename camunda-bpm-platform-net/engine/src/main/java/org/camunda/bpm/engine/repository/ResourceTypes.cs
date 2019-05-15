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
namespace org.camunda.bpm.engine.repository
{
	public sealed class ResourceTypes : ResourceType
	{

	  public static readonly ResourceTypes REPOSITORY = new ResourceTypes("REPOSITORY", InnerEnum.REPOSITORY, "REPOSITORY", 1);

	  public static readonly ResourceTypes RUNTIME = new ResourceTypes("RUNTIME", InnerEnum.RUNTIME, "RUNTIME", 2);

	  public static readonly ResourceTypes HISTORY = new ResourceTypes("HISTORY", InnerEnum.HISTORY, "HISTORY", 3);

	  private static readonly IList<ResourceTypes> valueList = new List<ResourceTypes>();

	  static ResourceTypes()
	  {
		  valueList.Add(REPOSITORY);
		  valueList.Add(RUNTIME);
		  valueList.Add(HISTORY);
	  }

	  public enum InnerEnum
	  {
		  REPOSITORY,
		  RUNTIME,
		  HISTORY
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  // implmentation //////////////////////////

	  private string name;
	  private int? id;

	  private ResourceTypes(string name, InnerEnum innerEnum, string name, int? id)
	  {
		this.name = name;
		this.id = id;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  public override string ToString()
	  {
		return name;
	  }

	  public string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public int? Value
	  {
		  get
		  {
			return id;
		  }
	  }

	  public static ResourceType forName(string name)
	  {
		ResourceType type = valueOf(name);
		return type;
	  }


		public static IList<ResourceTypes> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public static ResourceTypes valueOf(string name)
		{
			foreach (ResourceTypes enumInstance in ResourceTypes.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}