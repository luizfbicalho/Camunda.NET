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
namespace org.camunda.bpm.engine.rest.sub.history.impl
{

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using Query = org.camunda.bpm.engine.query.Query;
	using HistoricVariableInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class HistoricVariableInstanceResourceImpl : AbstractResourceProvider<HistoricVariableInstanceQuery, HistoricVariableInstance, HistoricVariableInstanceDto>, HistoricVariableInstanceResource
	{

	  public HistoricVariableInstanceResourceImpl(string variableId, ProcessEngine engine) : base(variableId, engine)
	  {
	  }

	  protected internal virtual HistoricVariableInstanceQuery baseQuery()
	  {
		return Engine.HistoryService.createHistoricVariableInstanceQuery().variableId(Id);
	  }

	  protected internal override Query<HistoricVariableInstanceQuery, HistoricVariableInstance> baseQueryForBinaryVariable()
	  {
		return baseQuery().disableCustomObjectDeserialization();
	  }

	  protected internal override Query<HistoricVariableInstanceQuery, HistoricVariableInstance> baseQueryForVariable(bool deserializeObjectValue)
	  {
		HistoricVariableInstanceQuery query = baseQuery().disableBinaryFetching();

		if (!deserializeObjectValue)
		{
		  query.disableCustomObjectDeserialization();
		}
		return query;
	  }

	  protected internal override TypedValue transformQueryResultIntoTypedValue(HistoricVariableInstance queryResult)
	  {
		return queryResult.TypedValue;
	  }

	  protected internal override HistoricVariableInstanceDto transformToDto(HistoricVariableInstance queryResult)
	  {
		return HistoricVariableInstanceDto.fromHistoricVariableInstance(queryResult);
	  }

	  protected internal override string ResourceNameForErrorMessage
	  {
		  get
		  {
			return "Historic variable instance";
		  }
	  }

	  public virtual Response deleteVariableInstance()
	  {
		try
		{
		  Engine.HistoryService.deleteHistoricVariableInstance(id);
		}
		catch (NotFoundException nfe)
		{ // rewrite status code from bad request (400) to not found (404)
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, nfe, nfe.Message);
		}
		// return no content (204) since resource is deleted
		return Response.noContent().build();
	  }

	}

}