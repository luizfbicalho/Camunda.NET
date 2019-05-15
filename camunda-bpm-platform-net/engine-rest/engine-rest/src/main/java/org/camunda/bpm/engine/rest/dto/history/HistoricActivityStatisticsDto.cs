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
namespace org.camunda.bpm.engine.rest.dto.history
{
	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricActivityStatisticsDto
	{

	  protected internal string id;
	  protected internal long instances;
	  protected internal long canceled;
	  protected internal long finished;
	  protected internal long completeScope;

	  public HistoricActivityStatisticsDto()
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual long Instances
	  {
		  get
		  {
			return instances;
		  }
	  }

	  public virtual long Canceled
	  {
		  get
		  {
			return canceled;
		  }
	  }

	  public virtual long Finished
	  {
		  get
		  {
			return finished;
		  }
	  }

	  public virtual long CompleteScope
	  {
		  get
		  {
			return completeScope;
		  }
	  }

	  public static HistoricActivityStatisticsDto fromHistoricActivityStatistics(HistoricActivityStatistics statistics)
	  {
		HistoricActivityStatisticsDto result = new HistoricActivityStatisticsDto();

		result.id = statistics.Id;

		result.instances = statistics.Instances;
		result.canceled = statistics.Canceled;
		result.finished = statistics.Finished;
		result.completeScope = statistics.CompleteScope;

		return result;
	  }

	}

}