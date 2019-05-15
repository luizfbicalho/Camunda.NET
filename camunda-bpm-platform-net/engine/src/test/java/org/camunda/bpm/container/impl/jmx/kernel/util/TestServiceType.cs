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
namespace org.camunda.bpm.container.impl.jmx.kernel.util
{
	using PlatformServiceContainer_ServiceType = org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType;

	public sealed class TestServiceType : ServiceType
	{

		public static readonly TestServiceType TYPE1 = new TestServiceType("TYPE1", InnerEnum.TYPE1, "test.type1");
		public static readonly TestServiceType TYPE2 = new TestServiceType("TYPE2", InnerEnum.TYPE2, "test.type2");

		private static readonly IList<TestServiceType> valueList = new List<TestServiceType>();

		static TestServiceType()
		{
			valueList.Add(TYPE1);
			valueList.Add(TYPE2);
		}

		public enum InnerEnum
		{
			TYPE1,
			TYPE2
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		protected internal string serviceRealm;

		private TestServiceType(string name, InnerEnum innerEnum, string serviceRealm)
		{
		  this.serviceRealm = serviceRealm;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public string TypeName
		{
			get
			{
			  return serviceRealm;
			}
		}


		public static IList<TestServiceType> values()
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

		public static TestServiceType valueOf(string name)
		{
			foreach (TestServiceType enumInstance in TestServiceType.valueList)
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