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


	/// <summary>
	/// Reads a list of <seealso cref="VariableQueryParameterDto"/>s from a single parameter. Expects a given format (see method comments).
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class VariableListConverter : JacksonAwareStringToTypeConverter<IList<VariableQueryParameterDto>>
	{

	  private const string EXPRESSION_DELIMITER = ",";
	  private const string ATTRIBUTE_DELIMITER = "_";

	  /// <summary>
	  /// Expects a query parameter of multiple variable expressions formatted as KEY_OPERATOR_VALUE, e.g. aVariable_eq_aValue.
	  /// Multiple values are expected to be comma-separated.
	  /// </summary>
	  public override IList<VariableQueryParameterDto> convertQueryParameterToType(string value)
	  {
		string[] expressions = value.Split(EXPRESSION_DELIMITER, true);

		IList<VariableQueryParameterDto> queryVariables = new List<VariableQueryParameterDto>();

		foreach (string expression in expressions)
		{
		  string[] valueTriple = expression.Split(ATTRIBUTE_DELIMITER, true);
		  if (valueTriple.Length != 3)
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "variable query parameter has to have format KEY_OPERATOR_VALUE.");
		  }

		  VariableQueryParameterDto queryVariable = new VariableQueryParameterDto();
		  queryVariable.Name = valueTriple[0];
		  queryVariable.Operator = valueTriple[1];
		  queryVariable.Value = valueTriple[2];

		  queryVariables.Add(queryVariable);
		}

		return queryVariables;
	  }
	}

}