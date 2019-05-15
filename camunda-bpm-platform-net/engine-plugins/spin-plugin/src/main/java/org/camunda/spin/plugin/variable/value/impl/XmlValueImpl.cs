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
namespace org.camunda.spin.plugin.variable.value.impl
{
	using SpinValueType = org.camunda.spin.plugin.variable.type.SpinValueType;
	using XmlValueType = org.camunda.spin.plugin.variable.type.XmlValueType;
	using DataFormat = org.camunda.spin.spi.DataFormat;
	using SpinXmlElement = org.camunda.spin.xml.SpinXmlElement;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class XmlValueImpl : SpinValueImpl, XmlValue
	{

	  private const long serialVersionUID = 1L;

	  public XmlValueImpl(SpinXmlElement value) : this(value, null, value.DataFormatName, true)
	  {
	  }

	  public XmlValueImpl(string value, string dataFormatName) : this(null, value, dataFormatName, false)
	  {
	  }

	  public XmlValueImpl(string value) : this(null, value, DataFormats.XML_DATAFORMAT_NAME, false)
	  {
	  }

	  public XmlValueImpl(SpinXmlElement value, string serializedValue, string dataFormatName, bool isDeserialized) : this(value, serializedValue, dataFormatName, isDeserialized, false)
	  {
	  }

	  public XmlValueImpl(SpinXmlElement value, string serializedValue, string dataFormatName, bool isDeserialized, bool isTransient) : base(value, serializedValue, dataFormatName, isDeserialized, org.camunda.spin.plugin.variable.type.SpinValueType_Fields.XML, isTransient)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public org.camunda.spin.spi.DataFormat<org.camunda.spin.xml.SpinXmlElement> getDataFormat()
	  public override DataFormat<SpinXmlElement> DataFormat
	  {
		  get
		  {
			return (DataFormat<SpinXmlElement>) base.DataFormat;
		  }
	  }

	  public override XmlValueType Type
	  {
		  get
		  {
			return (XmlValueType) base.Type;
		  }
	  }

	  public override SpinXmlElement Value
	  {
		  get
		  {
			return (SpinXmlElement) base.Value;
		  }
	  }

	}

}