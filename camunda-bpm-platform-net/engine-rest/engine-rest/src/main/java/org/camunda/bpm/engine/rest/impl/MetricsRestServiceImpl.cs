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
namespace org.camunda.bpm.engine.rest.impl
{
	using MetricsResource = org.camunda.bpm.engine.rest.sub.metrics.MetricsResource;
	using MetricsResourceImpl = org.camunda.bpm.engine.rest.sub.metrics.MetricsResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using MetricsIntervalResultDto = org.camunda.bpm.engine.rest.dto.metrics.MetricsIntervalResultDto;
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using IntegerConverter = org.camunda.bpm.engine.rest.dto.converter.IntegerConverter;
	using LongConverter = org.camunda.bpm.engine.rest.dto.converter.LongConverter;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MetricsRestServiceImpl : AbstractRestProcessEngineAware, MetricsRestService
	{

	  public const string QUERY_PARAM_NAME = "name";
	  public const string QUERY_PARAM_REPORTER = "reporter";
	  public const string QUERY_PARAM_START_DATE = "startDate";
	  public const string QUERY_PARAM_END_DATE = "endDate";
	  public const string QUERY_PARAM_FIRST_RESULT = "firstResult";
	  public const string QUERY_PARAM_MAX_RESULTS = "maxResults";
	  public const string QUERY_PARAM_INTERVAL = "interval";
	  public const string QUERY_PARAM_AGG_BY_REPORTER = "aggregateByReporter";

	  public MetricsRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual MetricsResource getMetrics(string name)
	  {
		return new MetricsResourceImpl(name, processEngine, objectMapper);
	  }

	  public virtual IList<MetricsIntervalResultDto> interval(UriInfo uriInfo)
	  {
		MultivaluedMap<string, string> queryParameters = uriInfo.QueryParameters;
		MetricsQuery query = processEngine.ManagementService.createMetricsQuery().name(queryParameters.getFirst(QUERY_PARAM_NAME)).reporter(queryParameters.getFirst(QUERY_PARAM_REPORTER));

		applyQueryParams(query, queryParameters);

		IList<MetricIntervalValue> metrics;
		LongConverter longConverter = new LongConverter();
		longConverter.ObjectMapper = objectMapper;
		if (queryParameters.getFirst(QUERY_PARAM_INTERVAL) != null)
		{
		  long interval = longConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_INTERVAL)).Value;
		  metrics = query.interval(interval);
		}
		else
		{
		  metrics = query.interval();
		}

		return convertToDtos(metrics);
	  }

	  protected internal virtual void applyQueryParams(MetricsQuery query, MultivaluedMap<string, string> queryParameters)
	  {

		DateConverter dateConverter = new DateConverter();
		dateConverter.ObjectMapper = objectMapper;

		if (queryParameters.getFirst(QUERY_PARAM_START_DATE) != null)
		{
		  DateTime startDate = dateConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_START_DATE));
		  query.startDate(startDate);
		}

		if (queryParameters.getFirst(QUERY_PARAM_END_DATE) != null)
		{
		  DateTime endDate = dateConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_END_DATE));
		  query.endDate(endDate);
		}

		IntegerConverter intConverter = new IntegerConverter();
		intConverter.ObjectMapper = objectMapper;

		if (queryParameters.getFirst(QUERY_PARAM_FIRST_RESULT) != null)
		{
		  int firstResult = intConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_FIRST_RESULT)).Value;
		  query.offset(firstResult);
		}

		if (queryParameters.getFirst(QUERY_PARAM_MAX_RESULTS) != null)
		{
		  int maxResults = intConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_MAX_RESULTS)).Value;
		  query.limit(maxResults);
		}

		if (queryParameters.getFirst(QUERY_PARAM_AGG_BY_REPORTER) != null)
		{
		  query.aggregateByReporter();
		}
	  }

	  protected internal virtual IList<MetricsIntervalResultDto> convertToDtos(IList<MetricIntervalValue> metrics)
	  {
		IList<MetricsIntervalResultDto> intervalMetrics = new List<MetricsIntervalResultDto>();
		foreach (MetricIntervalValue m in metrics)
		{
		  intervalMetrics.Add(new MetricsIntervalResultDto(m));
		}
		return intervalMetrics;
	  }
	}

}