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
namespace org.camunda.bpm.engine.impl.cmd
{
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// Read the already configured historyLevel from DB and map to given list of total levels.
	/// </summary>
	public class DetermineHistoryLevelCmd : Command<HistoryLevel>
	{

	  private readonly IList<HistoryLevel> historyLevels;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public DetermineHistoryLevelCmd(final java.util.List<org.camunda.bpm.engine.impl.history.HistoryLevel> historyLevels)
	  public DetermineHistoryLevelCmd(IList<HistoryLevel> historyLevels)
	  {
		this.historyLevels = historyLevels;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.impl.history.HistoryLevel execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual HistoryLevel execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<int> databaseHistoryLevel = org.camunda.bpm.engine.impl.HistoryLevelSetupCommand.databaseHistoryLevel(commandContext);
		int? databaseHistoryLevel = HistoryLevelSetupCommand.databaseHistoryLevel(commandContext);

		HistoryLevel result = null;

		if (databaseHistoryLevel != null)
		{
		  foreach (HistoryLevel historyLevel in historyLevels)
		  {
			if (historyLevel.Id == databaseHistoryLevel)
			{
			  result = historyLevel;
			  break;
			}
		  }

		  if (result != null)
		  {
			return result;
		  }
		  else
		  {
			// if a custom non-null value is not registered, throw an exception.
			throw new ProcessEngineException(string.Format("The configured history level with id='{0}' is not registered in this config.", databaseHistoryLevel));
		  }
		}
		else
		{
		  return null;
		}
	  }


	}

}