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
namespace org.camunda.bpm.model.bpmn
{
	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public sealed class TransactionMethod
	{

	  public static readonly TransactionMethod Compensate = new TransactionMethod("Compensate", InnerEnum.Compensate, "##Compensate");
	  public static readonly TransactionMethod Image = new TransactionMethod("Image", InnerEnum.Image, "##Image");
	  public static readonly TransactionMethod Store = new TransactionMethod("Store", InnerEnum.Store, "##Store");

	  private static readonly IList<TransactionMethod> valueList = new List<TransactionMethod>();

	  static TransactionMethod()
	  {
		  valueList.Add(Compensate);
		  valueList.Add(Image);
		  valueList.Add(Store);
	  }

	  public enum InnerEnum
	  {
		  Compensate,
		  Image,
		  Store
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private readonly string name;

	  internal TransactionMethod(string name, InnerEnum innerEnum)
	  {
		this.name = name();

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  internal TransactionMethod(string name, InnerEnum innerEnum, string name)
	  {
		this.name = name;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  public override string ToString()
	  {
		return name;
	  }


		public static IList<TransactionMethod> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public static TransactionMethod valueOf(string name)
		{
			foreach (TransactionMethod enumInstance in TransactionMethod.valueList)
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