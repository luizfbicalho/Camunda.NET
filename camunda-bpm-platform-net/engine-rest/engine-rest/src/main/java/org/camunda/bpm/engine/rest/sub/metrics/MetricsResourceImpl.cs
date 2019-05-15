using System;

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
namespace org.camunda.bpm.engine.rest.sub.metrics
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using MetricsResultDto = org.camunda.bpm.engine.rest.dto.metrics.MetricsResultDto;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MetricsResourceImpl : MetricsResource
	{

	  protected internal string metricsName;
	  protected internal ProcessEngine processEngine;
	  protected internal ObjectMapper objectMapper;

	  public MetricsResourceImpl(string metricsName, ProcessEngine processEngine, ObjectMapper objectMapper)
	  {
		this.metricsName = metricsName;
		this.processEngine = processEngine;
		this.objectMapper = objectMapper;
	  }

	  public virtual MetricsResultDto sum(UriInfo uriInfo)
	  {
		MetricsQuery query = processEngine.ManagementService.createMetricsQuery().name(metricsName);

		applyQueryParams(query, uriInfo);

		return new MetricsResultDto(query.sum());
	  }

	  protected internal virtual void applyQueryParams(MetricsQuery query, UriInfo uriInfo)
	  {
		MultivaluedMap<string, string> queryParameters = uriInfo.QueryParameters;

		DateConverter dateConverter = new DateConverter();
		dateConverter.ObjectMapper = objectMapper;

		if (queryParameters.getFirst("startDate") != null)
		{
		  DateTime startDate = dateConverter.convertQueryParameterToType(queryParameters.getFirst("startDate"));
		  query.startDate(startDate);
		}

		if (queryParameters.getFirst("endDate") != null)
		{
		  DateTime endDate = dateConverter.convertQueryParameterToType(queryParameters.getFirst("endDate"));
		  query.endDate(endDate);
		}
	  }

	}

}