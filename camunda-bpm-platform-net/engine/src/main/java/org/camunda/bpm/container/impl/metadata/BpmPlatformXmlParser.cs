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
namespace org.camunda.bpm.container.impl.metadata
{
	using ProcessesXmlParse = org.camunda.bpm.application.impl.metadata.ProcessesXmlParse;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using Parser = org.camunda.bpm.engine.impl.util.xml.Parser;


	/// <summary>
	/// <para>A SAX Parser for the bpm-platform.xml file</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BpmPlatformXmlParser : Parser
	{

	  /// <summary>
	  /// The bpm platform namespace
	  /// </summary>
	  public const string BPM_PLATFORM_NS = "http://www.camunda.org/schema/1.0/BpmPlatform";

	  /// <summary>
	  /// The location of the XSD file in the classpath.
	  /// </summary>
	  public const string BPM_PLATFORM_XSD = "BpmPlatform.xsd";

	  /// <summary>
	  /// create an configure the <seealso cref="ProcessesXmlParse"/> object.
	  /// </summary>
	  public override BpmPlatformXmlParse createParse()
	  {
		BpmPlatformXmlParse parse = new BpmPlatformXmlParse(this);
		parse.SchemaResource = ReflectUtil.getResourceUrlAsString(BPM_PLATFORM_XSD);
		return parse;
	  }

	}

}