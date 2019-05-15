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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricActivityStatisticsImpl : HistoricActivityStatistics
	{

	  protected internal string id;
	  protected internal long instances;
	  protected internal long finished;
	  protected internal long canceled;
	  protected internal long completeScope;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual long Instances
	  {
		  get
		  {
			return instances;
		  }
		  set
		  {
			this.instances = value;
		  }
	  }


	  public virtual long Finished
	  {
		  get
		  {
			return finished;
		  }
		  set
		  {
			this.finished = value;
		  }
	  }


	  public virtual long Canceled
	  {
		  get
		  {
			return canceled;
		  }
		  set
		  {
			this.canceled = value;
		  }
	  }


	  public virtual long CompleteScope
	  {
		  get
		  {
			return completeScope;
		  }
		  set
		  {
			this.completeScope = value;
		  }
	  }


	}

}