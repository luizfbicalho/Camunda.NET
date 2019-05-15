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
namespace org.camunda.bpm.engine.rest.dto.converter
{
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


	public class ConditionListConverter : JacksonAwareStringToTypeConverter<IList<ConditionQueryParameterDto>>
	{

	  private const string EXPRESSION_DELIMITER = ",";
	  private const string ATTRIBUTE_DELIMITER = "_";

	  public override IList<ConditionQueryParameterDto> convertQueryParameterToType(string value)
	  {
		string[] expressions = value.Split(EXPRESSION_DELIMITER, true);

		IList<ConditionQueryParameterDto> queryConditions = new List<ConditionQueryParameterDto>();

		foreach (string expression in expressions)
		{
		  string[] valueTuple = expression.Split(ATTRIBUTE_DELIMITER, true);
		  if (valueTuple.Length != 2)
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "condition query parameter has to have format OPERATOR_VALUE.");
		  }

		  ConditionQueryParameterDto queryCondition = new ConditionQueryParameterDto();
		  queryCondition.Operator = valueTuple[0];
		  queryCondition.Value = valueTuple[1];

		  queryConditions.Add(queryCondition);
		}

		return queryConditions;
	  }

	}

}