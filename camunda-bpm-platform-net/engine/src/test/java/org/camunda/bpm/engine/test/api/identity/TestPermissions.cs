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
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;

	public sealed class TestPermissions : Permission
	{
	  public static readonly TestPermissions NONE = new TestPermissions("NONE", InnerEnum.NONE, "NONE", 0);
	  public static readonly TestPermissions ALL = new TestPermissions("ALL", InnerEnum.ALL, "ALL", int.MaxValue);
	  public static readonly TestPermissions READ = new TestPermissions("READ", InnerEnum.READ, "READ", 2);
	  public static readonly TestPermissions UPDATE = new TestPermissions("UPDATE", InnerEnum.UPDATE, "UPDATE", 4);
	  public static readonly TestPermissions CREATE = new TestPermissions("CREATE", InnerEnum.CREATE, "CREATE", 4);
	  public static readonly TestPermissions DELETE = new TestPermissions("DELETE", InnerEnum.DELETE, "DELETE", 16);
	  public static readonly TestPermissions ACCESS = new TestPermissions("ACCESS", InnerEnum.ACCESS, "ACCESS", 32);
	  public static readonly TestPermissions RANDOM = new TestPermissions("RANDOM", InnerEnum.RANDOM, "RANDOM", 64);
	  public static readonly TestPermissions LONG_NAME = new TestPermissions("LONG_NAME", InnerEnum.LONG_NAME, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", 128);

	  private static readonly IList<TestPermissions> valueList = new List<TestPermissions>();

	  static TestPermissions()
	  {
		  valueList.Add(NONE);
		  valueList.Add(ALL);
		  valueList.Add(READ);
		  valueList.Add(UPDATE);
		  valueList.Add(CREATE);
		  valueList.Add(DELETE);
		  valueList.Add(ACCESS);
		  valueList.Add(RANDOM);
		  valueList.Add(LONG_NAME);
	  }

	  public enum InnerEnum
	  {
		  NONE,
		  ALL,
		  READ,
		  UPDATE,
		  CREATE,
		  DELETE,
		  ACCESS,
		  RANDOM,
		  LONG_NAME
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private TestPermissions(string name, InnerEnum innerEnum, string name, int value)
	  {
		this.name = name;
		this.value = value;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  internal string name;
	  internal int value;

	  public string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public int Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  public org.camunda.bpm.engine.authorization.Resource[] Types
	  {
		  get
		  {
			return new Resource[] {TestResource.RESOURCE1, TestResource.RESOURCE2};
		  }
	  }


		public static IList<TestPermissions> values()
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

		public static TestPermissions valueOf(string name)
		{
			foreach (TestPermissions enumInstance in TestPermissions.valueList)
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