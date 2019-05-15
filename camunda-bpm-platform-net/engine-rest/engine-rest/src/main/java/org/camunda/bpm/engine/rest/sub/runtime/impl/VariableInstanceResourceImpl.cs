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
namespace org.camunda.bpm.engine.rest.sub.runtime.impl
{
	using Query = org.camunda.bpm.engine.query.Query;
	using VariableInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceDto;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class VariableInstanceResourceImpl : AbstractResourceProvider<VariableInstanceQuery, VariableInstance, VariableInstanceDto>, VariableInstanceResource
	{

	  public VariableInstanceResourceImpl(string variableId, ProcessEngine engine) : base(variableId, engine)
	  {
	  }

	  protected internal virtual VariableInstanceQuery baseQuery()
	  {
		return Engine.RuntimeService.createVariableInstanceQuery().variableId(Id);
	  }

	  protected internal override Query<VariableInstanceQuery, VariableInstance> baseQueryForBinaryVariable()
	  {
		return baseQuery().disableCustomObjectDeserialization();
	  }

	  protected internal override Query<VariableInstanceQuery, VariableInstance> baseQueryForVariable(bool deserializeObjectValue)
	  {
		VariableInstanceQuery baseQuery = baseQuery();

		// do not fetch byte arrays
		baseQuery.disableBinaryFetching();

		if (!deserializeObjectValue)
		{
		  baseQuery.disableCustomObjectDeserialization();
		}
		return baseQuery;
	  }

	  protected internal override TypedValue transformQueryResultIntoTypedValue(VariableInstance queryResult)
	  {
		return queryResult.TypedValue;
	  }

	  protected internal override VariableInstanceDto transformToDto(VariableInstance queryResult)
	  {
		return VariableInstanceDto.fromVariableInstance(queryResult);
	  }

	  protected internal override string ResourceNameForErrorMessage
	  {
		  get
		  {
			return "Variable instance";
		  }
	  }

	}

}