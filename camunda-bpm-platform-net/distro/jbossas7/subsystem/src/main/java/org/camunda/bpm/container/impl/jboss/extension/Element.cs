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
	/// An Element.
	/// 
	/// @author christian.lipphardt@camunda.com
	/// </summary>
	public sealed class Element
	{
	  /// <summary>
	  /// always first
	  /// </summary>
	  public static readonly Element UNKNOWN = new Element("UNKNOWN", InnerEnum.UNKNOWN, (string) null);

	  public static readonly Element PROCESS_ENGINES = new Element("PROCESS_ENGINES", InnerEnum.PROCESS_ENGINES, ModelConstants_Fields.PROCESS_ENGINES);
	  public static readonly Element PROCESS_ENGINE = new Element("PROCESS_ENGINE", InnerEnum.PROCESS_ENGINE, ModelConstants_Fields.PROCESS_ENGINE);
	  public static readonly Element CONFIGURATION = new Element("CONFIGURATION", InnerEnum.CONFIGURATION, ModelConstants_Fields.CONFIGURATION);
	  public static readonly Element DATASOURCE = new Element("DATASOURCE", InnerEnum.DATASOURCE, ModelConstants_Fields.DATASOURCE);
	  public static readonly Element HISTORY_LEVEL = new Element("HISTORY_LEVEL", InnerEnum.HISTORY_LEVEL, ModelConstants_Fields.HISTORY_LEVEL);
	  public static readonly Element JOB_EXECUTOR = new Element("JOB_EXECUTOR", InnerEnum.JOB_EXECUTOR, ModelConstants_Fields.JOB_EXECUTOR);
	  public static readonly Element JOB_AQUISITIONS = new Element("JOB_AQUISITIONS", InnerEnum.JOB_AQUISITIONS, ModelConstants_Fields.JOB_ACQUISITIONS);
	  public static readonly Element JOB_AQUISITION = new Element("JOB_AQUISITION", InnerEnum.JOB_AQUISITION, ModelConstants_Fields.JOB_ACQUISITION);

	  public static readonly Element PLUGINS = new Element("PLUGINS", InnerEnum.PLUGINS, ModelConstants_Fields.PLUGINS);
	  public static readonly Element PLUGIN = new Element("PLUGIN", InnerEnum.PLUGIN, ModelConstants_Fields.PLUGIN);
	  public static readonly Element PLUGIN_CLASS = new Element("PLUGIN_CLASS", InnerEnum.PLUGIN_CLASS, ModelConstants_Fields.PLUGIN_CLASS);

	  [System.Obsolete]
	  public static readonly Element ACQUISITION_STRATEGY = new Element("ACQUISITION_STRATEGY", InnerEnum.ACQUISITION_STRATEGY, ModelConstants_Fields.ACQUISITION_STRATEGY);

	  public static readonly Element PROPERTIES = new Element("PROPERTIES", InnerEnum.PROPERTIES, ModelConstants_Fields.PROPERTIES);
	  public static readonly Element PROPERTY = new Element("PROPERTY", InnerEnum.PROPERTY, ModelConstants_Fields.PROPERTY);
	  public static readonly Element THREAD_POOL_NAME = new Element("THREAD_POOL_NAME", InnerEnum.THREAD_POOL_NAME, ModelConstants_Fields.THREAD_POOL_NAME);

	  private static readonly IList<Element> valueList = new List<Element>();

	  public enum InnerEnum
	  {
		  UNKNOWN,
		  PROCESS_ENGINES,
		  PROCESS_ENGINE,
		  CONFIGURATION,
		  DATASOURCE,
		  HISTORY_LEVEL,
		  JOB_EXECUTOR,
		  JOB_AQUISITIONS,
		  JOB_AQUISITION,
		  PLUGINS,
		  PLUGIN,
		  PLUGIN_CLASS,
		  ACQUISITION_STRATEGY,
		  PROPERTIES,
		  PROPERTY,
		  THREAD_POOL_NAME
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private readonly string name;
	  private readonly org.jboss.@as.controller.AttributeDefinition definition;
	  private readonly IDictionary<string, org.jboss.@as.controller.AttributeDefinition> definitions;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: Element(final String name)
	  internal Element(string name, InnerEnum innerEnum, string name)
	  {
		this.name = name;
		this.definition = null;
		this.definitions = null;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: Element(final org.jboss.as.controller.AttributeDefinition definition)
	  internal Element(string name, InnerEnum innerEnum, org.jboss.@as.controller.AttributeDefinition definition)
	  {
		this.name = definition.XmlName;
		this.definition = definition;
		this.definitions = null;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: Element(final java.util.List<org.jboss.as.controller.AttributeDefinition> definitions)
	  internal Element(string name, InnerEnum innerEnum, IList<org.jboss.@as.controller.AttributeDefinition> definitions)
	  {
		this.definition = null;
		this.definitions = new Dictionary<string, AttributeDefinition>();
		string ourName = null;
		foreach (AttributeDefinition def in definitions)
		{
		  if (string.ReferenceEquals(ourName, null))
		  {
			ourName = def.XmlName;
		  }
		  else if (!ourName.Equals(def.XmlName))
		  {
			// TODO: throw correct exception
			// throw MESSAGES.attributeDefinitionsMustMatch(def.getXmlName(),
			// ourName);
		  }
		  if (this.definitions.put(def.Name, def) != null)
		  {
			// TODO: throw correct exception
			// throw MESSAGES.attributeDefinitionsNotUnique(def.getName());
		  }
		}
		this.name = ourName;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: Element(final java.util.Map<String, org.jboss.as.controller.AttributeDefinition> definitions)
	  internal Element(string name, InnerEnum innerEnum, IDictionary<string, org.jboss.@as.controller.AttributeDefinition> definitions)
	  {
		this.definition = null;
		this.definitions = new Dictionary<string, AttributeDefinition>();
		string ourName = null;
		foreach (KeyValuePair<string, AttributeDefinition> def in definitions.SetOfKeyValuePairs())
		{
		  string xmlName = def.Value.XmlName;
		  if (string.ReferenceEquals(ourName, null))
		  {
			ourName = xmlName;
		  }
		  else if (!ourName.Equals(xmlName))
		  {
			// TODO: throw correct exception
			// throw MESSAGES.attributeDefinitionsMustMatch(xmlName, ourName);
		  }
		  this.definitions.put(def.Key, def.Value);
		}
		this.name = ourName;

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

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.jboss.as.controller.AttributeDefinition getDefinition(final String name)
	  public org.jboss.@as.controller.AttributeDefinition getDefinition(string name)
	  {
		return definitions.get(name);
	  }

	  private static readonly IDictionary<string, Element> MAP;

	  static Element()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, Element> map = new java.util.HashMap<String, Element>();
		IDictionary<string, Element> map = new Dictionary<string, Element>();
		foreach (Element element in values())
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
		  valueList.Add(PROCESS_ENGINES);
		  valueList.Add(PROCESS_ENGINE);
		  valueList.Add(CONFIGURATION);
		  valueList.Add(DATASOURCE);
		  valueList.Add(HISTORY_LEVEL);
		  valueList.Add(JOB_EXECUTOR);
		  valueList.Add(JOB_AQUISITIONS);
		  valueList.Add(JOB_AQUISITION);
		  valueList.Add(PLUGINS);
		  valueList.Add(PLUGIN);
		  valueList.Add(PLUGIN_CLASS);
		  valueList.Add(ACQUISITION_STRATEGY);
		  valueList.Add(PROPERTIES);
		  valueList.Add(PROPERTY);
		  valueList.Add(THREAD_POOL_NAME);
	  }

	  public static Element forName(string localName)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Element element = MAP.get(localName);
		Element element = MAP.get(localName);
		return element == null ? UNKNOWN : element;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private static java.util.List<org.jboss.as.controller.AttributeDefinition> getAttributeDefinitions(final org.jboss.as.controller.AttributeDefinition... attributeDefinitions)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private static IList<org.jboss.@as.controller.AttributeDefinition> getAttributeDefinitions(params org.jboss.@as.controller.AttributeDefinition[] attributeDefinitions)
	  {
		return Arrays.asList(attributeDefinitions);
	  }

		public static IList<Element> values()
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

		public static Element valueOf(string name)
		{
			foreach (Element enumInstance in Element.valueList)
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