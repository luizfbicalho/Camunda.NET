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
namespace org.camunda.spin.plugin.impl
{

	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using SpinJsonNode = org.camunda.spin.json.SpinJsonNode;
	using DataFormat = org.camunda.spin.spi.DataFormat;
	using SpinXmlElement = org.camunda.spin.xml.SpinXmlElement;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SpinVariableSerializers
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public static java.util.List<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>> createObjectValueSerializers(org.camunda.spin.DataFormats dataFormats)
	  public static IList<TypedValueSerializer<object>> createObjectValueSerializers(DataFormats dataFormats)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>> serializers = new java.util.ArrayList<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>>();
		IList<TypedValueSerializer<object>> serializers = new List<TypedValueSerializer<object>>();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<org.camunda.spin.spi.DataFormat<?>> availableDataFormats = dataFormats.getAllAvailableDataFormats();
		ISet<DataFormat<object>> availableDataFormats = dataFormats.AllAvailableDataFormats;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.spin.spi.DataFormat<?> dataFormat : availableDataFormats)
		foreach (DataFormat<object> dataFormat in availableDataFormats)
		{
		  serializers.Add(new SpinObjectValueSerializer("spin://" + dataFormat.Name, dataFormat));
		}

		return serializers;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public static java.util.List<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>> createSpinValueSerializers(org.camunda.spin.DataFormats dataFormats)
	  public static IList<TypedValueSerializer<object>> createSpinValueSerializers(DataFormats dataFormats)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>> serializers = new java.util.ArrayList<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>>();
		IList<TypedValueSerializer<object>> serializers = new List<TypedValueSerializer<object>>();

		if (dataFormats.getDataFormatByName(DataFormats.JSON_DATAFORMAT_NAME) != null)
		{
		  DataFormat<SpinJsonNode> jsonDataFormat = (DataFormat<SpinJsonNode>) dataFormats.getDataFormatByName(DataFormats.JSON_DATAFORMAT_NAME);
		  serializers.Add(new JsonValueSerializer(jsonDataFormat));
		}
		if (dataFormats.getDataFormatByName(DataFormats.XML_DATAFORMAT_NAME) != null)
		{
		  DataFormat<SpinXmlElement> xmlDataFormat = (DataFormat<SpinXmlElement>) dataFormats.getDataFormatByName(DataFormats.XML_DATAFORMAT_NAME);
		  serializers.Add(new XmlValueSerializer(xmlDataFormat));
		}

		return serializers;
	  }
	}

}