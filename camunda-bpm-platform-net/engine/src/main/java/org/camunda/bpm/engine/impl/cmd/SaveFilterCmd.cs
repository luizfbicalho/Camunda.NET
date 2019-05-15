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
namespace org.camunda.bpm.engine.impl.cmd
{

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class SaveFilterCmd : Command<Filter>
	{

	  private const long serialVersionUID = 1L;

	  protected internal Filter filter;

	  public SaveFilterCmd(Filter filter)
	  {
		this.filter = filter;
	  }

	  public virtual Filter execute(CommandContext commandContext)
	  {
		EnsureUtil.ensureNotNull("filter", filter);

		string operation = string.ReferenceEquals(filter.Id, null) ? org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE : org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE;

		Filter savedFilter = commandContext.FilterManager.insertOrUpdateFilter(filter);

		commandContext.OperationLogManager.logFilterOperation(operation, filter.Id);

		return savedFilter;
	  }

	}

}