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
namespace org.camunda.bpm.model.xml.impl.type.attribute
{
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;

	/// <summary>
	/// <para>class for providing Boolean value attributes. Takes care of type conversion and
	/// the interaction with the underlying Xml model model.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BooleanAttribute : AttributeImpl<bool>
	{

	  public BooleanAttribute(ModelElementType owningElementType) : base(owningElementType)
	  {
	  }

	  protected internal override bool? convertXmlValueToModelValue(string rawValue)
	  {
		return ModelUtil.valueAsBoolean(rawValue);
	  }

	  protected internal virtual string convertModelValueToXmlValue(bool? modelValue)
	  {
		return ModelUtil.valueAsString(modelValue);
	  }

	}

}