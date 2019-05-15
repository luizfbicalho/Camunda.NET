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

	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[Serializable]
	public class VariableOrderProperty : QueryOrderingProperty
	{

	  private new const long serialVersionUID = 1L;

	  public VariableOrderProperty(string name, ValueType valueType) : base(QueryOrderingProperty.RELATION_VARIABLE, typeToQueryProperty(valueType))
	  {
		this.relationConditions = new List<QueryEntityRelationCondition>();
		relationConditions.Add(new QueryEntityRelationCondition(VariableInstanceQueryProperty_Fields.VARIABLE_NAME, name));

		// works only for primitive types
		relationConditions.Add(new QueryEntityRelationCondition(VariableInstanceQueryProperty_Fields.VARIABLE_TYPE, valueType.Name));
	  }

	  public VariableOrderProperty()
	  {
	  }

	  public static VariableOrderProperty forProcessInstanceVariable(string variableName, ValueType valueType)
	  {
		VariableOrderProperty orderingProperty = new VariableOrderProperty(variableName, valueType);
		orderingProperty.relationConditions.Add(new QueryEntityRelationCondition(VariableInstanceQueryProperty_Fields.EXECUTION_ID, TaskQueryProperty_Fields.PROCESS_INSTANCE_ID));

		return orderingProperty;
	  }

	  public static VariableOrderProperty forExecutionVariable(string variableName, ValueType valueType)
	  {
		VariableOrderProperty orderingProperty = new VariableOrderProperty(variableName, valueType);
		orderingProperty.relationConditions.Add(new QueryEntityRelationCondition(VariableInstanceQueryProperty_Fields.EXECUTION_ID, TaskQueryProperty_Fields.EXECUTION_ID));

		return orderingProperty;
	  }

	  public static VariableOrderProperty forTaskVariable(string variableName, ValueType valueType)
	  {
		VariableOrderProperty orderingProperty = new VariableOrderProperty(variableName, valueType);
		orderingProperty.relationConditions.Add(new QueryEntityRelationCondition(VariableInstanceQueryProperty_Fields.TASK_ID, TaskQueryProperty_Fields.TASK_ID));

		return orderingProperty;
	  }

	  public static VariableOrderProperty forCaseInstanceVariable(string variableName, ValueType valueType)
	  {
		VariableOrderProperty orderingProperty = new VariableOrderProperty(variableName, valueType);
		orderingProperty.relationConditions.Add(new QueryEntityRelationCondition(VariableInstanceQueryProperty_Fields.CASE_EXECUTION_ID, TaskQueryProperty_Fields.CASE_INSTANCE_ID));

		return orderingProperty;
	  }

	  public static VariableOrderProperty forCaseExecutionVariable(string variableName, ValueType valueType)
	  {
		VariableOrderProperty orderingProperty = new VariableOrderProperty(variableName, valueType);
		orderingProperty.relationConditions.Add(new QueryEntityRelationCondition(VariableInstanceQueryProperty_Fields.CASE_EXECUTION_ID, TaskQueryProperty_Fields.CASE_EXECUTION_ID));

		return orderingProperty;
	  }

	  public static QueryProperty typeToQueryProperty(ValueType type)
	  {
		if (ValueType.STRING.Equals(type))
		{
		  return VariableInstanceQueryProperty_Fields.TEXT_AS_LOWER;
		}
		else if (ValueType.INTEGER.Equals(type))
		{
		  return VariableInstanceQueryProperty_Fields.LONG;
		}
		else if (ValueType.SHORT.Equals(type))
		{
		  return VariableInstanceQueryProperty_Fields.LONG;
		}
		else if (ValueType.DATE.Equals(type))
		{
		  return VariableInstanceQueryProperty_Fields.LONG;
		}
		else if (ValueType.BOOLEAN.Equals(type))
		{
		  return VariableInstanceQueryProperty_Fields.LONG;
		}
		else if (ValueType.LONG.Equals(type))
		{
		  return VariableInstanceQueryProperty_Fields.LONG;
		}
		else if (ValueType.DOUBLE.Equals(type))
		{
		  return VariableInstanceQueryProperty_Fields.DOUBLE;
		}
		else
		{
		  throw new ProcessEngineException("Cannot order by variables of type " + type.Name);
		}
	  }

	}

}