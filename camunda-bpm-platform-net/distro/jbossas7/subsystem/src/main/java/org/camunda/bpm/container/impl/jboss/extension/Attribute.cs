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
namespace org.camunda.bpm.container.impl.jboss.extension
{

	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;

	/// <summary>
	/// An attribute.
	/// 
	/// @author christian.lipphardt@camunda.com
	/// </summary>
	public sealed class Attribute
	{
	  /// <summary>
	  /// always first
	  /// </summary>
	  public static readonly Attribute UNKNOWN = new Attribute("UNKNOWN", InnerEnum.UNKNOWN, (string) null);

	  public static readonly Attribute NAME = new Attribute("NAME", InnerEnum.NAME, ModelConstants_Fields.NAME);
	  public static readonly Attribute DEFAULT = new Attribute("DEFAULT", InnerEnum.DEFAULT, ModelConstants_Fields.DEFAULT);

	  private static readonly IList<Attribute> valueList = new List<Attribute>();

	  public enum InnerEnum
	  {
		  UNKNOWN,
		  NAME,
		  DEFAULT
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private readonly string name;
	  private readonly org.jboss.@as.controller.AttributeDefinition definition;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: Attribute(final String name)
	  internal Attribute(string name, InnerEnum innerEnum, string name)
	  {
		this.name = name;
		this.definition = null;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: Attribute(final org.jboss.as.controller.AttributeDefinition definition)
	  internal Attribute(string name, InnerEnum innerEnum, org.jboss.@as.controller.AttributeDefinition definition)
	  {
		this.name = definition.XmlName;
		this.definition = definition;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  /// <summary>
	  /// Get the local name of this element.
	  /// </summary>
	  /// <returns> the local name </returns>
	  public string LocalName
	  {
		  get
		  {
			return name;
		  }
	  }

	  public org.jboss.@as.controller.AttributeDefinition Definition
	  {
		  get
		  {
			return definition;
		  }
	  }

	  private static readonly IDictionary<string, Attribute> MAP;

	  static Attribute()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, Attribute> map = new java.util.HashMap<String, Attribute>();
		IDictionary<string, Attribute> map = new Dictionary<string, Attribute>();
		foreach (Attribute element in values())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = element.getLocalName();
		  string name = element.LocalName;
		  if (!string.ReferenceEquals(name, null))
		  {
			map[name] = element;
		  }
		}
		MAP = map;

		  valueList.Add(UNKNOWN);
		  valueList.Add(NAME);
		  valueList.Add(DEFAULT);
	  }

	  public static Attribute forName(string localName)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Attribute element = MAP.get(localName);
		Attribute element = MAP.get(localName);
		return element == null ? UNKNOWN : element;
	  }


		public static IList<Attribute> values()
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

		public static Attribute valueOf(string name)
		{
			foreach (Attribute enumInstance in Attribute.valueList)
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