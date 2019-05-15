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
namespace org.camunda.bpm.engine.rest.dto
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using QueryOperator = org.camunda.bpm.engine.impl.QueryOperator;


	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class ConditionQueryParameterDto
	{

	  public ConditionQueryParameterDto()
	  {

	  }

	  public const string EQUALS_OPERATOR_NAME = "eq";
	  public const string NOT_EQUALS_OPERATOR_NAME = "neq";
	  public const string GREATER_THAN_OPERATOR_NAME = "gt";
	  public const string GREATER_THAN_OR_EQUALS_OPERATOR_NAME = "gteq";
	  public const string LESS_THAN_OPERATOR_NAME = "lt";
	  public const string LESS_THAN_OR_EQUALS_OPERATOR_NAME = "lteq";
	  public const string LIKE_OPERATOR_NAME = "like";

	  protected internal static readonly IDictionary<string, QueryOperator> NAME_OPERATOR_MAP = new Dictionary<string, QueryOperator>();

	  static ConditionQueryParameterDto()
	  {
		NAME_OPERATOR_MAP[EQUALS_OPERATOR_NAME] = QueryOperator.EQUALS;
		NAME_OPERATOR_MAP[NOT_EQUALS_OPERATOR_NAME] = QueryOperator.NOT_EQUALS;
		NAME_OPERATOR_MAP[GREATER_THAN_OPERATOR_NAME] = QueryOperator.GREATER_THAN;
		NAME_OPERATOR_MAP[GREATER_THAN_OR_EQUALS_OPERATOR_NAME] = QueryOperator.GREATER_THAN_OR_EQUAL;
		NAME_OPERATOR_MAP[LESS_THAN_OPERATOR_NAME] = QueryOperator.LESS_THAN;
		NAME_OPERATOR_MAP[LESS_THAN_OR_EQUALS_OPERATOR_NAME] = QueryOperator.LESS_THAN_OR_EQUAL;
		NAME_OPERATOR_MAP[LIKE_OPERATOR_NAME] = QueryOperator.LIKE;
		OPERATOR_NAME_MAP[QueryOperator.EQUALS] = EQUALS_OPERATOR_NAME;
		OPERATOR_NAME_MAP[QueryOperator.NOT_EQUALS] = NOT_EQUALS_OPERATOR_NAME;
		OPERATOR_NAME_MAP[QueryOperator.GREATER_THAN] = GREATER_THAN_OPERATOR_NAME;
		OPERATOR_NAME_MAP[QueryOperator.GREATER_THAN_OR_EQUAL] = GREATER_THAN_OR_EQUALS_OPERATOR_NAME;
		OPERATOR_NAME_MAP[QueryOperator.LESS_THAN] = LESS_THAN_OPERATOR_NAME;
		OPERATOR_NAME_MAP[QueryOperator.LESS_THAN_OR_EQUAL] = LESS_THAN_OR_EQUALS_OPERATOR_NAME;
		OPERATOR_NAME_MAP[QueryOperator.LIKE] = LIKE_OPERATOR_NAME;
	  };

	  protected internal static readonly IDictionary<QueryOperator, string> OPERATOR_NAME_MAP = new Dictionary<QueryOperator, string>();


	  protected internal string @operator;
	  protected internal object value;

	  public virtual string Operator
	  {
		  get
		  {
			return @operator;
		  }
		  set
		  {
			this.@operator = value;
		  }
	  }
	  public virtual object resolveValue(ObjectMapper objectMapper)
	  {
		if (value is string && objectMapper != null)
		{
		  try
		  {
			return objectMapper.readValue("\"" + value + "\"", typeof(DateTime));
		  }
		  catch (Exception)
		  {
			// ignore the exception
		  }
		}

		return value;
	  }

	  public virtual object Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }


	  public static QueryOperator getQueryOperator(string name)
	  {
		return NAME_OPERATOR_MAP[name];
	  }
	}

}