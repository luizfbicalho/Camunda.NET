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
namespace org.camunda.bpm.engine.impl.cmd
{

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class DeleteMetricsCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal DateTime timestamp;
	  protected internal string reporter;

	  public DeleteMetricsCmd(DateTime timestamp, string reporter)
	  {
		this.timestamp = timestamp;
		this.reporter = reporter;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		writeUserOperationLog(commandContext);

		if (timestamp == null && string.ReferenceEquals(reporter, null))
		{
		  commandContext.MeterLogManager.deleteAll();
		}
		else
		{
		  commandContext.MeterLogManager.deleteByTimestampAndReporter(timestamp, reporter);
		}
		return null;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		if (timestamp != null)
		{
		  propertyChanges.Add(new PropertyChange("timestamp", null, timestamp));
		}
		if (!string.ReferenceEquals(reporter, null))
		{
		  propertyChanges.Add(new PropertyChange("reporter", null, reporter));
		}
		if (propertyChanges.Count == 0)
		{
		  propertyChanges.Add(PropertyChange.EMPTY_CHANGE);
		}
		commandContext.OperationLogManager.logMetricsOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, propertyChanges);
	  }

	}

}