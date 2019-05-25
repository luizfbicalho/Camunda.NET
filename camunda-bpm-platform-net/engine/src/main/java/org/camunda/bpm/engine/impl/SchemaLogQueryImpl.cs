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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;
	using SchemaLogQuery = org.camunda.bpm.engine.management.SchemaLogQuery;
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	[Serializable]
	public class SchemaLogQueryImpl : AbstractQuery<SchemaLogQuery, SchemaLogEntry>, SchemaLogQuery
	{

	  private const long serialVersionUID = 1L;
	  private static readonly QueryProperty TIMESTAMP_PROPERTY = new QueryPropertyImpl("TIMESTAMP_");

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string version_Conflict;

	  public SchemaLogQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual SchemaLogQuery version(string version)
	  {
		ensureNotNull("version", version);
		this.version_Conflict = version;
		return this;
	  }

	  public virtual SchemaLogQuery orderByTimestamp()
	  {
		orderBy(TIMESTAMP_PROPERTY);
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.SchemaLogManager.findSchemaLogEntryCountByQueryCriteria(this).Value;
	  }

	  public override IList<SchemaLogEntry> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.SchemaLogManager.findSchemaLogEntriesByQueryCriteria(this, page);
	  }
	}
}