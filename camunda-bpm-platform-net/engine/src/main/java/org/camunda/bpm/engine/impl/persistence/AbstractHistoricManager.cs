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
namespace org.camunda.bpm.engine.impl.persistence
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AbstractHistoricManager : AbstractManager
	{
		private bool InstanceFieldsInitialized = false;

		public AbstractHistoricManager()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			isHistoryEnabled = !historyLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE);
			isHistoryLevelFullEnabled = historyLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL);
		}


	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;

	  protected internal bool isHistoryEnabled;
	  protected internal bool isHistoryLevelFullEnabled;

	  protected internal virtual void checkHistoryEnabled()
	  {
		if (!isHistoryEnabled)
		{
		  throw LOG.disabledHistoryException();
		}
	  }

	  public virtual bool HistoryEnabled
	  {
		  get
		  {
			return isHistoryEnabled;
		  }
	  }

	  public virtual bool HistoryLevelFullEnabled
	  {
		  get
		  {
			return isHistoryLevelFullEnabled;
		  }
	  }
	}

}