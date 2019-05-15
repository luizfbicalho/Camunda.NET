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

	/// <summary>
	/// An Element.
	/// 
	/// @author christian.lipphardt@camunda.com
	/// </summary>
	public sealed class Namespace
	{
	  /// <summary>
	  /// always first
	  /// </summary>
	  public static readonly Namespace UNKNOWN = new Namespace("UNKNOWN", InnerEnum.UNKNOWN, (string) null);

	  public static readonly Namespace CAMUNDA_BPM_PLATFORM_1_1 = new Namespace("CAMUNDA_BPM_PLATFORM_1_1", InnerEnum.CAMUNDA_BPM_PLATFORM_1_1, "urn:org.camunda.bpm.jboss:1.1");

	  private static readonly IList<Namespace> valueList = new List<Namespace>();

	  public enum InnerEnum
	  {
		  UNKNOWN,
		  CAMUNDA_BPM_PLATFORM_1_1
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  /// <summary>
	  /// The current namespace version.
	  /// </summary>

	  public static readonly Namespace CURRENT = CAMUNDA_BPM_PLATFORM_1_1;

	  private readonly string name;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: Namespace(final String name)
	  internal Namespace(string name, InnerEnum innerEnum, string name)
	  {
		this.name = name;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  /// <summary>
	  /// Get the URI of this element. </summary>
	  /// <returns> the URI </returns>
	  public string UriString
	  {
		  get
		  {
			return name;
		  }
	  }

	  private static readonly IDictionary<string, Namespace> MAP;

	  static Namespace()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, Namespace> map = new java.util.HashMap<String, Namespace>();
		IDictionary<string, Namespace> map = new Dictionary<string, Namespace>();
		foreach (Namespace element in values())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = element.getUriString();
		  string name = element.UriString;
		  if (!string.ReferenceEquals(name, null))
		  {
			map[name] = element;
		  }
		}
		MAP = map;

		  valueList.Add(UNKNOWN);
		  valueList.Add(CAMUNDA_BPM_PLATFORM_1_1);
	  }

	  public static Namespace forUri(string uri)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Namespace element = MAP.get(uri);
		Namespace element = MAP.get(uri);
		return element == null ? UNKNOWN : element;
	  }


		public static IList<Namespace> values()
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

		public static Namespace valueOf(string name)
		{
			foreach (Namespace enumInstance in Namespace.valueList)
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