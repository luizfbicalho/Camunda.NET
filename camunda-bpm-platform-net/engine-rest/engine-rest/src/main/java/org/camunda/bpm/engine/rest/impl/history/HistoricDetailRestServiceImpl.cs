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
namespace org.camunda.bpm.engine.rest.impl.history
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricDetailDto = org.camunda.bpm.engine.rest.dto.history.HistoricDetailDto;
	using HistoricDetailQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricDetailQueryDto;
	using HistoricDetailRestService = org.camunda.bpm.engine.rest.history.HistoricDetailRestService;
	using HistoricDetailResource = org.camunda.bpm.engine.rest.sub.history.HistoricDetailResource;
	using HistoricDetailResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricDetailResourceImpl;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricDetailRestServiceImpl : HistoricDetailRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricDetailRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricDetailResource historicDetail(string detailId)
	  {
		return new HistoricDetailResourceImpl(detailId, processEngine);
	  }

	  public virtual IList<HistoricDetailDto> getHistoricDetails(UriInfo uriInfo, int? firstResult, int? maxResults, bool deserializeObjectValues)
	  {
		HistoricDetailQueryDto queryDto = new HistoricDetailQueryDto(objectMapper, uriInfo.QueryParameters);
		HistoricDetailQuery query = queryDto.toQuery(processEngine);

		return executeHistoricDetailQuery(query, firstResult, maxResults, deserializeObjectValues);
	  }

	  public virtual IList<HistoricDetailDto> queryHistoricDetails(HistoricDetailQueryDto queryDto, int? firstResult, int? maxResults, bool deserializeObjectValues)
	  {
		HistoricDetailQuery query = queryDto.toQuery(processEngine);

		return executeHistoricDetailQuery(query, firstResult, maxResults, deserializeObjectValues);
	  }

	  public virtual CountResultDto getHistoricDetailsCount(UriInfo uriInfo)
	  {
		HistoricDetailQueryDto queryDto = new HistoricDetailQueryDto(objectMapper, uriInfo.QueryParameters);
		HistoricDetailQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  private IList<HistoricDetailDto> executeHistoricDetailQuery(HistoricDetailQuery query, int? firstResult, int? maxResults, bool deserializeObjectValues)
	  {

		query.disableBinaryFetching();
		if (!deserializeObjectValues)
		{
		  query.disableCustomObjectDeserialization();
		}

		IList<HistoricDetail> queryResult;
		if (firstResult != null || maxResults != null)
		{
		  queryResult = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  queryResult = query.list();
		}

		IList<HistoricDetailDto> result = new List<HistoricDetailDto>();
		foreach (HistoricDetail historicDetail in queryResult)
		{
		  HistoricDetailDto dto = HistoricDetailDto.fromHistoricDetail(historicDetail);
		  result.Add(dto);
		}

		return result;
	  }

	  private IList<HistoricDetail> executePaginatedQuery(HistoricDetailQuery query, int? firstResult, int? maxResults)
	  {
		if (firstResult == null)
		{
		  firstResult = 0;
		}
		if (maxResults == null)
		{
		  maxResults = int.MaxValue;
		}
		return query.listPage(firstResult, maxResults);
	  }

	}

}