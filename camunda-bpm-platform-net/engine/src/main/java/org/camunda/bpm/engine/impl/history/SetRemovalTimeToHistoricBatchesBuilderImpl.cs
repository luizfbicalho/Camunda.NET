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
namespace org.camunda.bpm.engine.impl.history
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using SetRemovalTimeSelectModeForHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricBatchesBuilder;
	using SetRemovalTimeToHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricBatchesBuilder;
	using SetRemovalTimeToHistoricBatchesCmd = org.camunda.bpm.engine.impl.cmd.batch.removaltime.SetRemovalTimeToHistoricBatchesCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNull;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class SetRemovalTimeToHistoricBatchesBuilderImpl : SetRemovalTimeSelectModeForHistoricBatchesBuilder
	{

	  protected internal HistoricBatchQuery query;
	  protected internal IList<string> ids;
	  protected internal Mode mode;
	  protected internal DateTime removalTime;

	  protected internal CommandExecutor commandExecutor;

	  public SetRemovalTimeToHistoricBatchesBuilderImpl(CommandExecutor commandExecutor)
	  {
		mode = null;
		this.commandExecutor = commandExecutor;
	  }

	  public virtual SetRemovalTimeToHistoricBatchesBuilder byQuery(HistoricBatchQuery query)
	  {
		this.query = query;
		return this;
	  }

	  public virtual SetRemovalTimeToHistoricBatchesBuilder byIds(params string[] ids)
	  {
		this.ids = ids != null ? Arrays.asList(ids) : null;
		return this;
	  }

	  public virtual SetRemovalTimeToHistoricBatchesBuilder absoluteRemovalTime(DateTime removalTime)
	  {
		ensureNull(typeof(BadUserRequestException), "The removal time modes are mutually exclusive","mode", mode);

		this.mode = Mode.ABSOLUTE_REMOVAL_TIME;
		this.removalTime = removalTime;
		return this;
	  }

	  public virtual SetRemovalTimeToHistoricBatchesBuilder calculatedRemovalTime()
	  {
		ensureNull(typeof(BadUserRequestException), "The removal time modes are mutually exclusive","mode", mode);

		this.mode = Mode.CALCULATED_REMOVAL_TIME;
		return this;
	  }

	  public virtual SetRemovalTimeToHistoricBatchesBuilder clearedRemovalTime()
	  {
		ensureNull(typeof(BadUserRequestException), "The removal time modes are mutually exclusive","mode", mode);

		mode = Mode.CLEARED_REMOVAL_TIME;
		return this;
	  }

	  public virtual Batch executeAsync()
	  {
		return commandExecutor.execute(new SetRemovalTimeToHistoricBatchesCmd(this));
	  }

	  public virtual HistoricBatchQuery Query
	  {
		  get
		  {
			return query;
		  }
	  }

	  public virtual IList<string> Ids
	  {
		  get
		  {
			return ids;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
	  }

	  public virtual Mode getMode()
	  {
		return mode;
	  }

	  public enum Mode
	  {
		CALCULATED_REMOVAL_TIME,
		ABSOLUTE_REMOVAL_TIME,
		CLEARED_REMOVAL_TIME
	  }

	}

}