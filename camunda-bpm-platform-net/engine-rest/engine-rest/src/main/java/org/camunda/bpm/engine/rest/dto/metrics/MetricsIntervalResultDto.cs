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
namespace org.camunda.bpm.engine.rest.dto.metrics
{
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class MetricsIntervalResultDto
	{

	  protected internal DateTime timestamp;

	  protected internal string name;

	  protected internal string reporter;

	  protected internal long value;

	  public MetricsIntervalResultDto(MetricIntervalValue metric)
	  {
		this.timestamp = metric.Timestamp;
		this.name = metric.Name;
		this.reporter = metric.Reporter;
		this.value = metric.Value;
	  }

	  public MetricsIntervalResultDto(DateTime timestamp, string name, string reporter, long value)
	  {
		this.timestamp = timestamp;
		this.name = name;
		this.reporter = reporter;
		this.value = value;
	  }

	  public MetricsIntervalResultDto()
	  {
	  }

	  public virtual DateTime Timestamp
	  {
		  get
		  {
			return timestamp;
		  }
		  set
		  {
			this.timestamp = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string Reporter
	  {
		  get
		  {
			return reporter;
		  }
		  set
		  {
			this.reporter = value;
		  }
	  }


	  public virtual long Value
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


	}

}