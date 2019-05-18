using System;

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
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;

	public class NamedEnumAttribute<T> : AttributeImpl<T> where T : Enum<T>
	{

	  protected internal readonly Type<T> type;

	  public NamedEnumAttribute(ModelElementType owningElementType, Type<T> type) : base(owningElementType)
	  {
		this.type = type;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T convertXmlValueToModelValue(String rawValue)
	  protected internal override T convertXmlValueToModelValue(string rawValue)
	  {
		T[] enumConstants = type.EnumConstants;
		if (!string.ReferenceEquals(rawValue, null) && enumConstants != null)
		{
		  foreach (T enumConstant in enumConstants)
		  {
			if (rawValue.Equals(enumConstant.ToString()))
			{
			  return enumConstant;
			}
		  }
		}
		return default(T);
	  }

	  protected internal override string convertModelValueToXmlValue(T modelValue)
	  {
		return modelValue.ToString();
	  }

	}

}