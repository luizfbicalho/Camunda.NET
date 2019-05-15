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
namespace org.camunda.bpm.engine.test.api.identity
{
	using Resource = org.camunda.bpm.engine.authorization.Resource;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public sealed class TestResource : Resource
	{
	  public static readonly TestResource RESOURCE1 = new TestResource("RESOURCE1", InnerEnum.RESOURCE1, "resource1", 100);
	  public static readonly TestResource RESOURCE2 = new TestResource("RESOURCE2", InnerEnum.RESOURCE2, "resource2", 101);

	  private static readonly IList<TestResource> valueList = new List<TestResource>();

	  static TestResource()
	  {
		  valueList.Add(RESOURCE1);
		  valueList.Add(RESOURCE2);
	  }

	  public enum InnerEnum
	  {
		  RESOURCE1,
		  RESOURCE2
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  protected internal int id;
	  protected internal string name;

	  internal TestResource(string name, InnerEnum innerEnum, string name, int id)
	  {
		this.name = name;
		this.id = id;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  public string resourceName()
	  {
		return name;
	  }

	  public int resourceType()
	  {
		return id;
	  }


		public static IList<TestResource> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static TestResource valueOf(string name)
		{
			foreach (TestResource enumInstance in TestResource.valueList)
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