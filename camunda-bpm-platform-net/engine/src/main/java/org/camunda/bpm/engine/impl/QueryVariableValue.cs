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
namespace org.camunda.bpm.engine.impl
{

	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// Represents a variable value used in queries.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class QueryVariableValue
	{
	  protected internal const long serialVersionUID = 1L;
	  protected internal string name;
	  protected internal TypedValue value;
	  protected internal QueryOperator @operator;
	  protected internal bool local;

	  protected internal AbstractQueryVariableValueCondition valueCondition;

	  protected internal bool variableNameIgnoreCase;
	  protected internal bool variableValueIgnoreCase;

	  public QueryVariableValue(string name, object value, QueryOperator @operator, bool local)
	  {
		this.name = name;
		this.value = Variables.untypedValue(value);
		this.@operator = @operator;
		this.local = local;
	  }

	  public virtual void initialize(VariableSerializers serializers)
	  {
		if (value.Type != null && value.Type.Abstract)
		{
		  valueCondition = new CompositeQueryVariableValueCondition(this);
		}
		else
		{
		  valueCondition = new SingleQueryVariableValueCondition(this);
		}

		valueCondition.initializeValue(serializers);
	  }

	  public virtual IList<SingleQueryVariableValueCondition> ValueConditions
	  {
		  get
		  {
			return valueCondition.DisjunctiveConditions;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual QueryOperator Operator
	  {
		  get
		  {
			if (@operator != null)
			{
			  return @operator;
			}
			return QueryOperator.EQUALS;
		  }
	  }

	  public virtual string OperatorName
	  {
		  get
		  {
			return Operator.ToString();
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			return value.Value;
		  }
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual bool Local
	  {
		  get
		  {
			return local;
		  }
	  }


	  public virtual bool VariableNameIgnoreCase
	  {
		  get
		  {
			return variableNameIgnoreCase;
		  }
		  set
		  {
			this.variableNameIgnoreCase = value;
		  }
	  }


	  public virtual bool VariableValueIgnoreCase
	  {
		  get
		  {
			return variableValueIgnoreCase;
		  }
		  set
		  {
			this.variableValueIgnoreCase = value;
		  }
	  }

	}

}