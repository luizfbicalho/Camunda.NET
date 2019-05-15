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
namespace org.camunda.bpm.engine.rest.dto.history
{
	using HistoricFormField = org.camunda.bpm.engine.history.HistoricFormField;

	using JsonTypeName = com.fasterxml.jackson.annotation.JsonTypeName;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonTypeName("formField") public class HistoricFormFieldDto extends HistoricDetailDto
	public class HistoricFormFieldDto : HistoricDetailDto
	{

	  protected internal string fieldId;
	  protected internal object fieldValue;

	  public virtual string FieldId
	  {
		  get
		  {
			return fieldId;
		  }
	  }

	  public virtual object FieldValue
	  {
		  get
		  {
			return fieldValue;
		  }
	  }

	  public static HistoricFormFieldDto fromHistoricFormField(HistoricFormField historicFormField)
	  {

		HistoricFormFieldDto dto = new HistoricFormFieldDto();

		dto.fieldId = historicFormField.FieldId;
		dto.fieldValue = historicFormField.FieldValue;

		return dto;
	  }

	}

}