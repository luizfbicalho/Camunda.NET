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
namespace org.camunda.bpm.engine.impl.jobexecutor
{
	using DbEntityCache = org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityCache;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class JobExecutorContext
	{

	  protected internal IList<string> currentProcessorJobQueue = new LinkedList<string>();

	  /// <summary>
	  /// the currently executed job </summary>
	  protected internal JobEntity currentJob;

	  /// <summary>
	  /// reusable cache </summary>
	  protected internal DbEntityCache entityCache;

	  public virtual IList<string> CurrentProcessorJobQueue
	  {
		  get
		  {
			return currentProcessorJobQueue;
		  }
	  }

	  public virtual bool ExecutingExclusiveJob
	  {
		  get
		  {
			return currentJob == null ? false : currentJob.Exclusive;
		  }
	  }

	  public virtual JobEntity CurrentJob
	  {
		  set
		  {
			this.currentJob = value;
		  }
		  get
		  {
			return currentJob;
		  }
	  }


	  public virtual DbEntityCache EntityCache
	  {
		  get
		  {
			return entityCache;
		  }
		  set
		  {
			this.entityCache = value;
		  }
	  }


	}

}