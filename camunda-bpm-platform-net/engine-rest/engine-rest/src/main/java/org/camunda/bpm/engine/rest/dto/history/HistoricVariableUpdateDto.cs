using System;
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
namespace org.camunda.bpm.engine.rest.dto.history
{

	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;

	using JsonTypeName = com.fasterxml.jackson.annotation.JsonTypeName;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonTypeName("variableUpdate") public class HistoricVariableUpdateDto extends HistoricDetailDto
	public class HistoricVariableUpdateDto : HistoricDetailDto
	{

	  protected internal string variableName;
	  protected internal string variableInstanceId;
	  protected internal string variableType;
	  protected internal object value;
	  protected internal IDictionary<string, object> valueInfo;

	  protected internal int revision;
	  protected internal string errorMessage;

	  public virtual string VariableName
	  {
		  get
		  {
			return variableName;
		  }
	  }

	  public virtual string VariableInstanceId
	  {
		  get
		  {
			return variableInstanceId;
		  }
	  }

	  public virtual string VariableType
	  {
		  get
		  {
			return variableType;
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
	  }

	  public virtual IDictionary<string, object> ValueInfo
	  {
		  get
		  {
			return valueInfo;
		  }
	  }

	  public static HistoricVariableUpdateDto fromHistoricVariableUpdate(HistoricVariableUpdate historicVariableUpdate)
	  {

		HistoricVariableUpdateDto dto = new HistoricVariableUpdateDto();
		fromHistoricVariableUpdate(dto, historicVariableUpdate);
		return dto;
	  }

	  protected internal static void fromHistoricVariableUpdate(HistoricVariableUpdateDto dto, HistoricVariableUpdate historicVariableUpdate)
	  {
		dto.revision = historicVariableUpdate.Revision;
		dto.variableName = historicVariableUpdate.VariableName;
		dto.variableInstanceId = historicVariableUpdate.VariableInstanceId;

		if (string.ReferenceEquals(historicVariableUpdate.ErrorMessage, null))
		{
		  try
		  {
			VariableValueDto variableValueDto = VariableValueDto.fromTypedValue(historicVariableUpdate.TypedValue);
			dto.value = variableValueDto.Value;
			dto.variableType = variableValueDto.Type;
			dto.valueInfo = variableValueDto.ValueInfo;
		  }
		  catch (Exception e)
		  {
			dto.errorMessage = e.Message;
			dto.variableType = VariableValueDto.toRestApiTypeName(historicVariableUpdate.TypeName);
		  }
		}
		else
		{
		  dto.errorMessage = historicVariableUpdate.ErrorMessage;
		  dto.variableType = VariableValueDto.toRestApiTypeName(historicVariableUpdate.TypeName);
		}
	  }

	}

}