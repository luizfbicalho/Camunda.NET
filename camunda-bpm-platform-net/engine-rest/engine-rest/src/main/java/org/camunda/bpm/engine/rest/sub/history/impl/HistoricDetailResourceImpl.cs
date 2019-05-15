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

	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using Query = org.camunda.bpm.engine.query.Query;
	using HistoricDetailDto = org.camunda.bpm.engine.rest.dto.history.HistoricDetailDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class HistoricDetailResourceImpl : AbstractResourceProvider<HistoricDetailQuery, HistoricDetail, HistoricDetailDto>, HistoricDetailResource
	{

	  public HistoricDetailResourceImpl(string detailId, ProcessEngine engine) : base(detailId, engine)
	  {
	  }

	  protected internal virtual HistoricDetailQuery baseQuery()
	  {
		return engine.HistoryService.createHistoricDetailQuery().detailId(Id);
	  }

	  protected internal override Query<HistoricDetailQuery, HistoricDetail> baseQueryForBinaryVariable()
	  {
		return baseQuery().disableCustomObjectDeserialization();
	  }

	  protected internal override Query<HistoricDetailQuery, HistoricDetail> baseQueryForVariable(bool deserializeObjectValue)
	  {
		HistoricDetailQuery query = baseQuery().disableBinaryFetching();

		if (!deserializeObjectValue)
		{
		  query.disableCustomObjectDeserialization();
		}
		return query;
	  }

	  protected internal override TypedValue transformQueryResultIntoTypedValue(HistoricDetail queryResult)
	  {
		if (!(queryResult is HistoricVariableUpdate))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Historic detail with Id '" + Id + "' is not a variable update.");
		}
		HistoricVariableUpdate update = (HistoricVariableUpdate) queryResult;
		return update.TypedValue;
	  }

	  protected internal override HistoricDetailDto transformToDto(HistoricDetail queryResult)
	  {
		return HistoricDetailDto.fromHistoricDetail(queryResult);
	  }

	  protected internal override string ResourceNameForErrorMessage
	  {
		  get
		  {
			return "Historic detail";
		  }
	  }

	}

}