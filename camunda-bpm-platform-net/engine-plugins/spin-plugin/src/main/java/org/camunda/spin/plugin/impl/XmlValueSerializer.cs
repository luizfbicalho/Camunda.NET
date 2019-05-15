﻿/*
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
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using SpinValues = org.camunda.spin.plugin.variable.SpinValues;
	using SpinValueType = org.camunda.spin.plugin.variable.type.SpinValueType;
	using XmlValueType = org.camunda.spin.plugin.variable.type.XmlValueType;
	using SpinValue = org.camunda.spin.plugin.variable.value.SpinValue;
	using XmlValueImpl = org.camunda.spin.plugin.variable.value.impl.XmlValueImpl;
	using DataFormat = org.camunda.spin.spi.DataFormat;
	using SpinXmlElement = org.camunda.spin.xml.SpinXmlElement;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class XmlValueSerializer : SpinValueSerializer
	{

	  public XmlValueSerializer(DataFormat<SpinXmlElement> dataFormat) : base(org.camunda.spin.plugin.variable.type.SpinValueType_Fields.XML, dataFormat, org.camunda.spin.plugin.variable.type.XmlValueType_Fields.TYPE_NAME)
	  {
	  }

	  public XmlValueSerializer() : this(DataFormats.xml())
	  {
	  }

	  public virtual SpinValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return SpinValues.xmlValue((SpinXmlElement) untypedValue.Value).create();
	  }

	  protected internal override SpinValue createDeserializedValue(object deserializedObject, string serializedStringValue, ValueFields valueFields)
	  {
		SpinXmlElement value = (SpinXmlElement) deserializedObject;
		return new XmlValueImpl(value, serializedStringValue, value.DataFormatName, true);
	  }

	  protected internal override SpinValue createSerializedValue(string serializedStringValue, ValueFields valueFields)
	  {
		return new XmlValueImpl(serializedStringValue, serializationDataFormat);
	  }

	}

}