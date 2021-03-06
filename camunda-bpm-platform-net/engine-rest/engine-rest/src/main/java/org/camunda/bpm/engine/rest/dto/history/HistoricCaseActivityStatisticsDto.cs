﻿/*
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
namespace org.camunda.bpm.engine.rest.dto.history
{
	using HistoricCaseActivityStatistics = org.camunda.bpm.engine.history.HistoricCaseActivityStatistics;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricCaseActivityStatisticsDto
	{

	  protected internal string id;
	  protected internal long available;
	  protected internal long enabled;
	  protected internal long disabled;
	  protected internal long active;
	  protected internal long completed;
	  protected internal long terminated;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual long Available
	  {
		  get
		  {
			return available;
		  }
	  }

	  public virtual long Enabled
	  {
		  get
		  {
			return enabled;
		  }
	  }

	  public virtual long Disabled
	  {
		  get
		  {
			return disabled;
		  }
	  }

	  public virtual long Active
	  {
		  get
		  {
			return active;
		  }
	  }

	  public virtual long Completed
	  {
		  get
		  {
			return completed;
		  }
	  }

	  public virtual long Terminated
	  {
		  get
		  {
			return terminated;
		  }
	  }

	  public static HistoricCaseActivityStatisticsDto fromHistoricCaseActivityStatistics(HistoricCaseActivityStatistics statistics)
	  {
		HistoricCaseActivityStatisticsDto result = new HistoricCaseActivityStatisticsDto();

		result.id = statistics.Id;
		result.available = statistics.Available;
		result.enabled = statistics.Enabled;
		result.disabled = statistics.Disabled;
		result.active = statistics.Active;
		result.completed = statistics.Completed;
		result.terminated = statistics.Terminated;

		return result;
	  }

	}

}